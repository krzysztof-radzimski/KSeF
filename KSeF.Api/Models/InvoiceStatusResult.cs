namespace KSeF.Api.Models;

/// <summary>
/// Wynik sprawdzenia statusu faktury w KSeF
/// </summary>
public class InvoiceStatusResult
{
    /// <summary>
    /// Czy operacja zakończyła się sukcesem
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Numer referencyjny faktury
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Numer KSeF faktury (nadawany po przetworzeniu)
    /// </summary>
    public string? KsefNumber { get; set; }

    /// <summary>
    /// Status przetwarzania faktury
    /// </summary>
    public InvoiceProcessingStatus Status { get; set; }

    /// <summary>
    /// Data przetworzenia
    /// </summary>
    public DateTime? ProcessingTimestamp { get; set; }

    /// <summary>
    /// Lista błędów
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Tworzy wynik sukcesu
    /// </summary>
    public static InvoiceStatusResult Ok(string referenceNumber, InvoiceProcessingStatus status, string? ksefNumber = null) => new()
    {
        Success = true,
        ReferenceNumber = referenceNumber,
        Status = status,
        KsefNumber = ksefNumber
    };

    /// <summary>
    /// Tworzy wynik błędu
    /// </summary>
    public static InvoiceStatusResult Fail(params string[] errors) => new()
    {
        Success = false,
        Errors = [.. errors]
    };
}

/// <summary>
/// Status przetwarzania faktury w KSeF
/// </summary>
public enum InvoiceProcessingStatus
{
    /// <summary>
    /// Nieznany status
    /// </summary>
    Unknown,

    /// <summary>
    /// Oczekuje na przetworzenie
    /// </summary>
    Pending,

    /// <summary>
    /// W trakcie przetwarzania
    /// </summary>
    Processing,

    /// <summary>
    /// Przetworzona poprawnie
    /// </summary>
    Processed,

    /// <summary>
    /// Odrzucona (błąd walidacji)
    /// </summary>
    Rejected,

    /// <summary>
    /// Błąd przetwarzania
    /// </summary>
    Error
}
