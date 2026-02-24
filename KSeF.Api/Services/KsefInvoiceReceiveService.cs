using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Client.Core.Models.Invoices;
using KSeF.Invoice;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using InvoiceSubjectType = KSeF.Client.Core.Models.Invoices.InvoiceSubjectType;
using DateType = KSeF.Client.Core.Models.Invoices.DateType;

namespace KSeF.Api.Services;

/// <summary>
/// Implementacja serwisu pobierania faktur zakupowych z KSeF
/// </summary>
public class KsefInvoiceReceiveService : IKsefInvoiceReceiveService
{
    private readonly IKSeFClient _ksefClient;
    private readonly IKsefSessionService _sessionService;
    private readonly IKsefInvoiceService _invoiceService;
    private readonly KsefApiOptions _options;
    private readonly ILogger<KsefInvoiceReceiveService> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję serwisu pobierania faktur zakupowych z KSeF.
    /// </summary>
    /// <param name="ksefClient">Klient KSeF do wykonywania operacji API.</param>
    /// <param name="sessionService">Serwis zarządzania sesjami KSeF.</param>
    /// <param name="invoiceService">Serwis do deserializacji faktur z XML.</param>
    /// <param name="options">Opcje konfiguracyjne KSeF API.</param>
    /// <param name="logger">Logger do rejestrowania operacji.</param>
    public KsefInvoiceReceiveService(
        IKSeFClient ksefClient,
        IKsefSessionService sessionService,
        IKsefInvoiceService invoiceService,
        IOptions<KsefApiOptions> options,
        ILogger<KsefInvoiceReceiveService> logger)
    {
        _ksefClient = ksefClient;
        _sessionService = sessionService;
        _invoiceService = invoiceService;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<InvoiceDownloadResult> GetInvoiceAsync(
        string ksefNumber,
        CancellationToken cancellationToken = default)
    {
        SessionInfo? session = null;
        try
        {
            session = await _sessionService.OpenSessionAsync(cancellationToken);
            return await GetInvoiceAsync(ksefNumber, session, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania faktury {KsefNumber}", ksefNumber);
            return InvoiceDownloadResult.Fail(ex.Message);
        }
        finally
        {
            if (session != null)
            {
                try { await _sessionService.CloseSessionAsync(session, cancellationToken); }
                catch (Exception ex) { _logger.LogWarning(ex, "Błąd zamykania sesji"); }
            }
        }
    }

    /// <inheritdoc />
    public async Task<InvoiceDownloadResult> GetInvoiceAsync(
        string ksefNumber,
        SessionInfo sessionInfo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Pobieranie faktury z KSeF: {KsefNumber}", ksefNumber);

            var invoiceXml = await _ksefClient.GetInvoiceAsync(
                ksefNumber,
                sessionInfo.AccessToken,
                cancellationToken);

            // Próba deserializacji pobranego XML do modelu
            Invoice.Models.Invoice? invoice = null;
            try
            {
                invoice = _invoiceService.FromXml(invoiceXml);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Nie udało się zdeserializować faktury {KsefNumber} - " +
                    "XML zostanie zwrócony bez modelu", ksefNumber);
            }

            _logger.LogInformation("Faktura {KsefNumber} pobrana pomyślnie", ksefNumber);
            return InvoiceDownloadResult.Ok(ksefNumber, invoiceXml, invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania faktury {KsefNumber}", ksefNumber);
            return InvoiceDownloadResult.Fail(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<InvoiceQueryResult> QueryPurchaseInvoicesAsync(
        InvoiceQueryCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        SessionInfo? session = null;
        try
        {
            session = await _sessionService.OpenSessionAsync(cancellationToken);
            return await QueryPurchaseInvoicesAsync(criteria, session, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wyszukiwania faktur zakupowych");
            return InvoiceQueryResult.Fail(ex.Message);
        }
        finally
        {
            if (session != null)
            {
                try { await _sessionService.CloseSessionAsync(session, cancellationToken); }
                catch (Exception ex) { _logger.LogWarning(ex, "Błąd zamykania sesji"); }
            }
        }
    }

    /// <inheritdoc />
    public async Task<InvoiceQueryResult> QueryPurchaseInvoicesAsync(
        InvoiceQueryCriteria criteria,
        SessionInfo sessionInfo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Wyszukiwanie faktur zakupowych: od {DateFrom} do {DateTo}, NIP: {Nip}",
                criteria.DateFrom, criteria.DateTo, criteria.CounterpartyNip);

            criteria.Direction = InvoiceDirection.Purchase;

            var filters = new InvoiceQueryFilters
            {
                SubjectType = InvoiceSubjectType.Subject2, // Faktury zakupowe - jesteśmy nabywcą (podmiot2)
                InvoiceNumber = criteria.InvoiceNumber,
                KsefNumber = criteria.KsefNumber,
                SellerNip = criteria.CounterpartyNip
            };

            if (criteria.DateFrom.HasValue)
            {
                filters.DateRange = new DateRange
                {
                    DateType = DateType.Invoicing,
                    From = new DateTimeOffset(criteria.DateFrom.Value, TimeSpan.Zero),
                    To = criteria.DateTo.HasValue
                        ? new DateTimeOffset(criteria.DateTo.Value, TimeSpan.Zero)
                        : null
                };
            }

            var response = await _ksefClient.QueryInvoiceMetadataAsync(
                filters,
                sessionInfo.AccessToken,
                criteria.PageOffset,
                criteria.PageSize,
                cancellationToken: cancellationToken);

            var invoices = new List<InvoiceMetadata>();
            if (response?.Invoices != null)
            {
                foreach (var inv in response.Invoices)
                {
                    invoices.Add(new InvoiceMetadata
                    {
                        KsefNumber = inv.KsefNumber ?? string.Empty,
                        InvoiceNumber = inv.InvoiceNumber,
                        SellerNip = inv.Seller?.Nip,
                        BuyerNip = inv.Buyer?.Identifier?.Value,
                        GrossAmount = inv.GrossAmount,
                        IssueDate = inv.IssueDate.DateTime,
                        AcquisitionTimestamp = inv.AcquisitionDate.DateTime,
                        InvoiceType = inv.InvoiceType.ToString()
                    });
                }
            }

            _logger.LogInformation("Znaleziono {Count} faktur zakupowych", invoices.Count);
            return InvoiceQueryResult.Ok(
                invoices, invoices.Count,
                criteria.PageOffset, criteria.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wyszukiwania faktur zakupowych");
            return InvoiceQueryResult.Fail(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<List<InvoiceDownloadResult>> DownloadPurchaseInvoicesAsync(
        DateTime fromDate,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<InvoiceDownloadResult>();
        SessionInfo? session = null;

        try
        {
            session = await _sessionService.OpenSessionAsync(cancellationToken);

            var criteria = new InvoiceQueryCriteria
            {
                DateFrom = fromDate,
                DateTo = toDate ?? DateTime.UtcNow,
                Direction = InvoiceDirection.Purchase,
                PageOffset = 0,
                PageSize = 100
            };

            var hasMore = true;
            while (hasMore)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queryResult = await QueryPurchaseInvoicesAsync(
                    criteria, session, cancellationToken);

                if (!queryResult.Success)
                {
                    _logger.LogWarning(
                        "Błąd wyszukiwania faktur na stronie {Page}: {Errors}",
                        criteria.PageOffset, string.Join("; ", queryResult.Errors));
                    break;
                }

                foreach (var metadata in queryResult.Invoices)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!string.IsNullOrEmpty(metadata.KsefNumber))
                    {
                        var downloadResult = await GetInvoiceAsync(
                            metadata.KsefNumber, session, cancellationToken);
                        results.Add(downloadResult);
                    }
                }

                hasMore = queryResult.HasMore;
                criteria.PageOffset++;
            }

            _logger.LogInformation("Pobrano {Count} faktur zakupowych z KSeF", results.Count);
            return results;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Pobieranie faktur anulowane. Pobrano {Count} faktur.",
                results.Count);
            throw;
        }
        finally
        {
            if (session != null)
            {
                try { await _sessionService.CloseSessionAsync(session, cancellationToken); }
                catch (Exception ex) { _logger.LogWarning(ex, "Błąd zamykania sesji"); }
            }
        }
    }

    /// <inheritdoc />
    public async Task<KsefOperationResult> GetUpoAsync(
        string ksefNumber,
        CancellationToken cancellationToken = default)
    {
        SessionInfo? session = null;
        try
        {
            session = await _sessionService.OpenSessionAsync(cancellationToken);

            await _ksefClient.GetSessionInvoiceUpoByKsefNumberAsync(
                ksefNumber,
                session.SessionReference,
                session.AccessToken,
                cancellationToken);

            return KsefOperationResult.Ok($"UPO pobrane dla faktury {ksefNumber}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd pobierania UPO dla {KsefNumber}", ksefNumber);
            return KsefOperationResult.Fail(ex.Message);
        }
        finally
        {
            if (session != null)
            {
                try { await _sessionService.CloseSessionAsync(session, cancellationToken); }
                catch (Exception ex) { _logger.LogWarning(ex, "Błąd zamykania sesji"); }
            }
        }
    }
}
