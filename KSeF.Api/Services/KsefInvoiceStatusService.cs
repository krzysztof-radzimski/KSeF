using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Client.Core.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KSeF.Api.Services;

/// <summary>
/// Implementacja serwisu sprawdzania statusów faktur w KSeF.
///
/// UWAGA: KSeF API 2.0 nie udostępnia dedykowanego endpointu do oznaczania faktur
/// jako zaksięgowane. Serwis pozwala na sprawdzanie statusu przetwarzania faktur
/// i sesji, co jest wystarczające do śledzenia stanu faktur.
/// </summary>
public class KsefInvoiceStatusService : IKsefInvoiceStatusService
{
    private readonly IKSeFClient _ksefClient;
    private readonly KsefApiOptions _options;
    private readonly ILogger<KsefInvoiceStatusService> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję serwisu sprawdzania statusów faktur w KSeF.
    /// </summary>
    /// <param name="ksefClient">Klient KSeF do wykonywania operacji API.</param>
    /// <param name="options">Opcje konfiguracyjne KSeF API.</param>
    /// <param name="logger">Logger do rejestrowania operacji.</param>
    public KsefInvoiceStatusService(
        IKSeFClient ksefClient,
        IOptions<KsefApiOptions> options,
        ILogger<KsefInvoiceStatusService> logger)
    {
        _ksefClient = ksefClient;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<InvoiceStatusResult> GetInvoiceStatusAsync(
        string referenceNumber,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sprawdzanie statusu faktury: {RefNumber}", referenceNumber);

            var response = await _ksefClient.GetSessionInvoiceAsync(
                referenceNumber,
                referenceNumber,
                accessToken,
                cancellationToken);

            var status = MapProcessingStatus(response.Status?.Code);

            return InvoiceStatusResult.Ok(
                referenceNumber,
                status,
                response.KsefNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd sprawdzania statusu faktury {RefNumber}", referenceNumber);
            return InvoiceStatusResult.Fail(ex.Message);
        }
    }

    /// <inheritdoc />
    public Task<InvoiceStatusResult> GetInvoiceStatusAsync(
        string referenceNumber,
        SessionInfo sessionInfo,
        CancellationToken cancellationToken = default)
    {
        return GetInvoiceStatusAsync(referenceNumber, sessionInfo.AccessToken, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SessionInvoicesResult> GetSessionInvoicesStatusAsync(
        string sessionReference,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sprawdzanie statusu faktur w sesji: {SessionRef}", sessionReference);

            var response = await _ksefClient.GetSessionInvoicesAsync(
                sessionReference,
                accessToken,
                cancellationToken: cancellationToken);

            var statuses = new List<InvoiceStatusResult>();

            if (response?.Invoices != null)
            {
                foreach (var inv in response.Invoices)
                {
                    var status = MapProcessingStatus(inv.Status?.Code);
                    statuses.Add(InvoiceStatusResult.Ok(
                        inv.ReferenceNumber ?? string.Empty,
                        status,
                        inv.KsefNumber));
                }
            }

            _logger.LogInformation(
                "Sesja {SessionRef}: {Processed} przetworzonych, {Rejected} odrzuconych",
                sessionReference,
                statuses.Count(s => s.Status == InvoiceProcessingStatus.Processed),
                statuses.Count(s => s.Status == InvoiceProcessingStatus.Rejected));

            return SessionInvoicesResult.Ok(sessionReference, statuses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd sprawdzania statusu sesji {SessionRef}", sessionReference);
            return SessionInvoicesResult.Fail(ex.Message);
        }
    }

    private static InvoiceProcessingStatus MapProcessingStatus(int? processingCode)
    {
        return processingCode switch
        {
            100 => InvoiceProcessingStatus.Pending,
            200 => InvoiceProcessingStatus.Processing,
            300 => InvoiceProcessingStatus.Processed,
            400 => InvoiceProcessingStatus.Rejected,
            500 => InvoiceProcessingStatus.Error,
            _ => InvoiceProcessingStatus.Unknown
        };
    }
}
