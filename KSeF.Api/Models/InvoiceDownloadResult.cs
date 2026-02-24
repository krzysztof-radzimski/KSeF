namespace KSeF.Api.Models;

/// <summary>
/// Wynik pobrania faktury z KSeF
/// </summary>
public class InvoiceDownloadResult
{
    /// <summary>
    /// Czy operacja zakończyła się sukcesem
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Numer KSeF faktury
    /// </summary>
    public string? KsefNumber { get; set; }

    /// <summary>
    /// XML faktury
    /// </summary>
    public string? InvoiceXml { get; set; }

    /// <summary>
    /// Zdeserializowana faktura (jeśli deserializacja się powiodła)
    /// </summary>
    public Invoice.Models.Invoice? Invoice { get; set; }

    /// <summary>
    /// Wiadomości o błędach
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Tworzy wynik sukcesu
    /// </summary>
    public static InvoiceDownloadResult Ok(string ksefNumber, string invoiceXml, Invoice.Models.Invoice? invoice = null) => new()
    {
        Success = true,
        KsefNumber = ksefNumber,
        InvoiceXml = invoiceXml,
        Invoice = invoice
    };

    /// <summary>
    /// Tworzy wynik błędu
    /// </summary>
    public static InvoiceDownloadResult Fail(params string[] errors) => new()
    {
        Success = false,
        Errors = [.. errors]
    };
}
