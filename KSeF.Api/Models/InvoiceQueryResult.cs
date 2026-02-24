namespace KSeF.Api.Models;

/// <summary>
/// Wynik zapytania o faktury z KSeF
/// </summary>
public class InvoiceQueryResult
{
    /// <summary>
    /// Czy operacja zakończyła się sukcesem
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Lista metadanych znalezionych faktur
    /// </summary>
    public List<InvoiceMetadata> Invoices { get; set; } = [];

    /// <summary>
    /// Łączna liczba znalezionych faktur
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Aktualny numer strony
    /// </summary>
    public int PageOffset { get; set; }

    /// <summary>
    /// Rozmiar strony
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Czy istnieją kolejne strony wyników
    /// </summary>
    public bool HasMore => (PageOffset + 1) * PageSize < TotalCount;

    /// <summary>
    /// Wiadomości o błędach
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Tworzy wynik sukcesu
    /// </summary>
    public static InvoiceQueryResult Ok(List<InvoiceMetadata> invoices, int totalCount, int pageOffset, int pageSize) => new()
    {
        Success = true,
        Invoices = invoices,
        TotalCount = totalCount,
        PageOffset = pageOffset,
        PageSize = pageSize
    };

    /// <summary>
    /// Tworzy wynik błędu
    /// </summary>
    public static InvoiceQueryResult Fail(params string[] errors) => new()
    {
        Success = false,
        Errors = [.. errors]
    };
}
