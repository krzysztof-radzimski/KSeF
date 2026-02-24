using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Client.Core.Interfaces.Services;
using KSeF.Client.Core.Models.Sessions;
using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KSeF.Api.Services;

/// <summary>
/// Implementacja serwisu wysyłania faktur sprzedażowych do KSeF.
/// Obsługuje: VAT, KOR, ZAL, ROZ, UPR, KOR_ZAL, KOR_ROZ.
/// </summary>
public class KsefInvoiceSendService : IKsefInvoiceSendService
{
    private readonly IKSeFClient _ksefClient;
    private readonly ICryptographyService _cryptographyService;
    private readonly IKsefSessionService _sessionService;
    private readonly IKsefInvoiceService _invoiceService;
    private readonly KsefApiOptions _options;
    private readonly ILogger<KsefInvoiceSendService> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję serwisu wysyłania faktur do KSeF.
    /// </summary>
    /// <param name="ksefClient">Klient KSeF do wykonywania operacji API.</param>
    /// <param name="cryptographyService">Serwis kryptograficzny do szyfrowania faktur.</param>
    /// <param name="sessionService">Serwis zarządzania sesjami KSeF.</param>
    /// <param name="invoiceService">Serwis do walidacji i serializacji faktur.</param>
    /// <param name="options">Opcje konfiguracyjne KSeF API.</param>
    /// <param name="logger">Logger do rejestrowania operacji.</param>
    public KsefInvoiceSendService(
        IKSeFClient ksefClient,
        ICryptographyService cryptographyService,
        IKsefSessionService sessionService,
        IKsefInvoiceService invoiceService,
        IOptions<KsefApiOptions> options,
        ILogger<KsefInvoiceSendService> logger)
    {
        _ksefClient = ksefClient;
        _cryptographyService = cryptographyService;
        _sessionService = sessionService;
        _invoiceService = invoiceService;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<InvoiceSendResult> SendInvoiceAsync(
        Invoice.Models.Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        SessionInfo? session = null;
        try
        {
            session = await _sessionService.OpenSessionAsync(cancellationToken);
            var result = await SendInvoiceAsync(invoice, session, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wysyłania faktury do KSeF");
            return InvoiceSendResult.Fail(ex.Message);
        }
        finally
        {
            if (session != null)
            {
                try
                {
                    await _sessionService.CloseSessionAsync(session, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Błąd podczas zamykania sesji KSeF");
                }
            }
        }
    }

    /// <inheritdoc />
    public async Task<InvoiceSendResult> SendInvoiceAsync(
        Invoice.Models.Invoice invoice,
        SessionInfo sessionInfo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Walidacja faktury
            var validationResult = _invoiceService.Validate(invoice);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => $"{e.FieldName}: {e.Message}")
                    .ToList();
                _logger.LogWarning("Faktura nie przeszła walidacji: {Errors}",
                    string.Join("; ", errors));
                return InvoiceSendResult.Fail([.. errors]);
            }

            // 2. Serializacja do XML i bajtów
            var invoiceXml = _invoiceService.ToXml(invoice);
            var invoiceBytes = _invoiceService.ToBytes(invoice);
            _logger.LogDebug("Faktura {InvoiceNumber} zserializowana ({Length} bajtów)",
                invoice.InvoiceData?.InvoiceNumber, invoiceBytes.Length);

            // 3. Szyfrowanie AES i przygotowanie metadanych
            var encryptionData = _cryptographyService.GetEncryptionData();
            var encryptedInvoice = _cryptographyService.EncryptBytesWithAES256(
                invoiceBytes, encryptionData.CipherKey, encryptionData.CipherIv);
            var fileMetadata = _cryptographyService.GetMetaData(invoiceBytes);
            var encryptedMetadata = _cryptographyService.GetMetaData(encryptedInvoice);

            // 4. Budowanie żądania i wysłanie
            var sendRequest = new SendInvoiceRequest
            {
                EncryptedInvoiceContent = Convert.ToBase64String(encryptedInvoice),
                InvoiceHash = fileMetadata.HashSHA,
                InvoiceSize = fileMetadata.FileSize,
                EncryptedInvoiceHash = encryptedMetadata.HashSHA,
                EncryptedInvoiceSize = encryptedMetadata.FileSize
            };

            var response = await _ksefClient.SendOnlineSessionInvoiceAsync(
                sendRequest,
                sessionInfo.SessionReference,
                sessionInfo.AccessToken,
                cancellationToken);

            var result = InvoiceSendResult.Ok(
                referenceNumber: response.ReferenceNumber,
                sessionReference: sessionInfo.SessionReference);
            result.InvoiceXml = invoiceXml;

            _logger.LogInformation(
                "Faktura {InvoiceNumber} wysłana do KSeF. Ref: {RefNumber}",
                invoice.InvoiceData?.InvoiceNumber,
                response.ReferenceNumber);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wysyłania faktury {InvoiceNumber}",
                invoice.InvoiceData?.InvoiceNumber);
            return InvoiceSendResult.Fail(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<List<InvoiceSendResult>> SendInvoicesAsync(
        IEnumerable<Invoice.Models.Invoice> invoices,
        CancellationToken cancellationToken = default)
    {
        var results = new List<InvoiceSendResult>();
        SessionInfo? session = null;

        try
        {
            session = await _sessionService.OpenSessionAsync(cancellationToken);

            foreach (var invoice in invoices)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await SendInvoiceAsync(invoice, session, cancellationToken);
                results.Add(result);
            }

            return results;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Wysyłanie faktur anulowane. Wysłano {Count} faktur.",
                results.Count);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wysyłania pakietu faktur");
            results.Add(InvoiceSendResult.Fail(ex.Message));
            return results;
        }
        finally
        {
            if (session != null)
            {
                try
                {
                    await _sessionService.CloseSessionAsync(session, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Błąd podczas zamykania sesji KSeF");
                }
            }
        }
    }

    /// <inheritdoc />
    public Task<InvoiceSendResult> SendVatInvoiceAsync(
        Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default)
    {
        ValidateInvoiceType(invoice, InvoiceType.VAT);
        return SendInvoiceAsync(invoice, cancellationToken);
    }

    /// <inheritdoc />
    public Task<InvoiceSendResult> SendCorrectionInvoiceAsync(
        Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default)
    {
        ValidateInvoiceType(invoice, InvoiceType.KOR);
        return SendInvoiceAsync(invoice, cancellationToken);
    }

    /// <inheritdoc />
    public Task<InvoiceSendResult> SendAdvancePaymentInvoiceAsync(
        Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default)
    {
        ValidateInvoiceType(invoice, InvoiceType.ZAL);
        return SendInvoiceAsync(invoice, cancellationToken);
    }

    /// <inheritdoc />
    public Task<InvoiceSendResult> SendSettlementInvoiceAsync(
        Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default)
    {
        ValidateInvoiceType(invoice, InvoiceType.ROZ);
        return SendInvoiceAsync(invoice, cancellationToken);
    }

    /// <inheritdoc />
    public Task<InvoiceSendResult> SendSimplifiedInvoiceAsync(
        Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default)
    {
        ValidateInvoiceType(invoice, InvoiceType.UPR);
        return SendInvoiceAsync(invoice, cancellationToken);
    }

    /// <inheritdoc />
    public Task<InvoiceSendResult> SendAdvancePaymentCorrectionAsync(
        Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default)
    {
        ValidateInvoiceType(invoice, InvoiceType.KOR_ZAL);
        return SendInvoiceAsync(invoice, cancellationToken);
    }

    /// <inheritdoc />
    public Task<InvoiceSendResult> SendSettlementCorrectionAsync(
        Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default)
    {
        ValidateInvoiceType(invoice, InvoiceType.KOR_ROZ);
        return SendInvoiceAsync(invoice, cancellationToken);
    }

    private static void ValidateInvoiceType(Invoice.Models.Invoice invoice, InvoiceType expectedType)
    {
        if (invoice.InvoiceData?.InvoiceType != expectedType)
        {
            throw new ArgumentException(
                $"Oczekiwano faktury typu {expectedType}, " +
                $"ale otrzymano {invoice.InvoiceData?.InvoiceType}",
                nameof(invoice));
        }
    }
}
