/*=====================================================================================

    KSeF.Api
    Model wyniku operacji wysłania faktury do KSeF. Zawiera
    informacje o sukcesie operacji, numer referencyjny, numer
    KSeF nadany fakturze, referencję sesji, znacznik czasu
    przetworzenia, listę błędów oraz XML wysłanej faktury.
    Udostępnia metody fabryczne Ok() i Fail().

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

namespace KSeF.Api.Models;

/// <summary>
/// Wynik wysłania faktury do KSeF
/// </summary>
public class InvoiceSendResult
{
    /// <summary>
    /// Czy operacja zakończyła się sukcesem
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Numer referencyjny faktury w KSeF
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Numer KSeF nadany fakturze (np. "1234567890-20250101-ABCDEF-01")
    /// </summary>
    public string? KsefNumber { get; set; }

    /// <summary>
    /// Numer sesji w której wysłano fakturę
    /// </summary>
    public string? SessionReference { get; set; }

    /// <summary>
    /// Data i czas przetworzenia faktury w KSeF
    /// </summary>
    public DateTime? ProcessingTimestamp { get; set; }

    /// <summary>
    /// Wiadomości o błędach (jeśli wystąpiły)
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// XML faktury który został wysłany
    /// </summary>
    public string? InvoiceXml { get; set; }

    /// <summary>
    /// Tworzy wynik sukcesu
    /// </summary>
    public static InvoiceSendResult Ok(string referenceNumber, string? ksefNumber = null, string? sessionReference = null) => new()
    {
        Success = true,
        ReferenceNumber = referenceNumber,
        KsefNumber = ksefNumber,
        SessionReference = sessionReference,
        ProcessingTimestamp = DateTime.UtcNow
    };

    /// <summary>
    /// Tworzy wynik błędu
    /// </summary>
    public static InvoiceSendResult Fail(params string[] errors) => new()
    {
        Success = false,
        Errors = [.. errors]
    };
}
