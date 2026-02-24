namespace KSeF.Api.Models;

/// <summary>
/// Wynik zapytania o faktury w sesji
/// </summary>
public class SessionInvoicesResult
{
    /// <summary>
    /// Czy operacja zakończyła się sukcesem
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Numer referencyjny sesji
    /// </summary>
    public string? SessionReference { get; set; }

    /// <summary>
    /// Lista statusów faktur w sesji
    /// </summary>
    public List<InvoiceStatusResult> InvoiceStatuses { get; set; } = [];

    /// <summary>
    /// Liczba faktur przetworzonych poprawnie
    /// </summary>
    public int ProcessedCount => InvoiceStatuses.Count(s => s.Status == InvoiceProcessingStatus.Processed);

    /// <summary>
    /// Liczba faktur odrzuconych
    /// </summary>
    public int RejectedCount => InvoiceStatuses.Count(s => s.Status == InvoiceProcessingStatus.Rejected);

    /// <summary>
    /// Lista błędów
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Tworzy wynik sukcesu
    /// </summary>
    public static SessionInvoicesResult Ok(string sessionReference, List<InvoiceStatusResult> statuses) => new()
    {
        Success = true,
        SessionReference = sessionReference,
        InvoiceStatuses = statuses
    };

    /// <summary>
    /// Tworzy wynik błędu
    /// </summary>
    public static SessionInvoicesResult Fail(params string[] errors) => new()
    {
        Success = false,
        Errors = [.. errors]
    };
}
