/*=====================================================================================

    KSeF.Api
    Model metadanych faktury pobranej z KSeF. Zawiera numer
    KSeF, numer referencyjny, numer faktury wystawcy, NIP
    sprzedawcy i nabywcy, kwotę brutto, datę wystawienia,
    datę przesłania do KSeF oraz rodzaj faktury.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

namespace KSeF.Api.Models;

/// <summary>
/// Metadane faktury z KSeF
/// </summary>
public class InvoiceMetadata
{
    /// <summary>
    /// Numer KSeF faktury
    /// </summary>
    public string KsefNumber { get; set; } = string.Empty;

    /// <summary>
    /// Numer referencyjny
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Numer faktury nadany przez wystawcę
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// NIP sprzedawcy
    /// </summary>
    public string? SellerNip { get; set; }

    /// <summary>
    /// NIP nabywcy
    /// </summary>
    public string? BuyerNip { get; set; }

    /// <summary>
    /// Kwota brutto faktury
    /// </summary>
    public decimal? GrossAmount { get; set; }

    /// <summary>
    /// Data wystawienia faktury
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Data przesłania do KSeF
    /// </summary>
    public DateTime? AcquisitionTimestamp { get; set; }

    /// <summary>
    /// Rodzaj faktury (VAT, KOR, ZAL, itd.)
    /// </summary>
    public string? InvoiceType { get; set; }
}
