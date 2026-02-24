namespace KSeF.Api.Models;

/// <summary>
/// Kryteria wyszukiwania faktur w KSeF
/// </summary>
public class InvoiceQueryCriteria
{
    /// <summary>
    /// Data od (włącznie)
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Data do (włącznie)
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// NIP kontrahenta (sprzedawcy lub nabywcy)
    /// </summary>
    public string? CounterpartyNip { get; set; }

    /// <summary>
    /// Numer faktury
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Numer KSeF faktury
    /// </summary>
    public string? KsefNumber { get; set; }

    /// <summary>
    /// Rodzaj faktur: Purchase (zakupowe), Sales (sprzedażowe)
    /// </summary>
    public InvoiceDirection Direction { get; set; } = InvoiceDirection.Purchase;

    /// <summary>
    /// Numer strony wyników (od 0)
    /// </summary>
    public int PageOffset { get; set; }

    /// <summary>
    /// Rozmiar strony wyników (domyślnie 100)
    /// </summary>
    public int PageSize { get; set; } = 100;
}
