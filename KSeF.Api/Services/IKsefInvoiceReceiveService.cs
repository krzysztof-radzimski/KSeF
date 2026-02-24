using KSeF.Api.Models;

namespace KSeF.Api.Services;

/// <summary>
/// Serwis pobierania faktur zakupowych z KSeF
/// </summary>
public interface IKsefInvoiceReceiveService
{
    /// <summary>
    /// Pobiera fakturę z KSeF po numerze KSeF
    /// </summary>
    /// <param name="ksefNumber">Numer KSeF faktury</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Pobrana faktura</returns>
    Task<InvoiceDownloadResult> GetInvoiceAsync(string ksefNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera fakturę z KSeF w ramach istniejącej sesji
    /// </summary>
    /// <param name="ksefNumber">Numer KSeF faktury</param>
    /// <param name="sessionInfo">Istniejąca sesja</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Pobrana faktura</returns>
    Task<InvoiceDownloadResult> GetInvoiceAsync(string ksefNumber, SessionInfo sessionInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wyszukuje faktury zakupowe w KSeF według kryteriów
    /// </summary>
    /// <param name="criteria">Kryteria wyszukiwania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista metadanych faktur</returns>
    Task<InvoiceQueryResult> QueryPurchaseInvoicesAsync(InvoiceQueryCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wyszukuje faktury zakupowe w ramach istniejącej sesji
    /// </summary>
    /// <param name="criteria">Kryteria wyszukiwania</param>
    /// <param name="sessionInfo">Istniejąca sesja</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista metadanych faktur</returns>
    Task<InvoiceQueryResult> QueryPurchaseInvoicesAsync(InvoiceQueryCriteria criteria, SessionInfo sessionInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera wszystkie nowe faktury zakupowe od podanej daty.
    /// Iteruje po stronach wyników automatycznie.
    /// </summary>
    /// <param name="fromDate">Data od której pobierać faktury</param>
    /// <param name="toDate">Data do (opcjonalnie, domyślnie teraz)</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista pobranych faktur z XML</returns>
    Task<List<InvoiceDownloadResult>> DownloadPurchaseInvoicesAsync(
        DateTime fromDate,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera UPO (Urzędowe Poświadczenie Odbioru) dla faktury
    /// </summary>
    /// <param name="ksefNumber">Numer KSeF faktury</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>XML dokumentu UPO</returns>
    Task<KsefOperationResult> GetUpoAsync(string ksefNumber, CancellationToken cancellationToken = default);
}
