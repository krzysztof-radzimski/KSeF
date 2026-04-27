/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca referencję do wcześniejszej faktury zaliczkowej
    (FakturaZaliczkowa) zgodnie ze schematem FA(3).
    XSD wymaga xsd:choice: albo (NrKSeFZN + NrFaZaliczkowej) — faktura poza KSeF,
    albo NrKSeFFaZaliczkowej — faktura wystawiona w KSeF.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;

namespace KSeF.Invoice.Models;

/// <summary>
/// Referencja do wcześniejszej faktury zaliczkowej (FakturaZaliczkowa)
/// Schemat FA(3) wymaga xsd:choice: faktura wystawiona w KSeF (NrKSeFFaZaliczkowej)
/// LUB faktura poza KSeF (NrKSeFZN + NrFaZaliczkowej).
/// </summary>
[XmlType("FakturaZaliczkowa")]
public class AdvanceInvoiceReference
{
    /// <summary>
    /// Znacznik czy faktura zaliczkowa została wystawiona poza KSeF
    /// </summary>
    [XmlIgnore]
    public bool IsIssuedOutsideKSeF { get; set; }

    /// <summary>
    /// Znacznik faktury zaliczkowej poza KSeF (NrKSeFZN)
    /// Element pomocniczy - zawsze wartość "1" gdy faktura wystawiona poza KSeF
    /// </summary>
    [XmlElement("NrKSeFZN", Order = 1)]
    public string? IssuedOutsideKSeFMarker
    {
        get => IsIssuedOutsideKSeF ? "1" : null;
        set => IsIssuedOutsideKSeF = value == "1";
    }

    /// <summary>
    /// Kontroluje serializację znacznika NrKSeFZN
    /// </summary>
    public bool ShouldSerializeIssuedOutsideKSeFMarker() => IsIssuedOutsideKSeF;

    /// <summary>
    /// Numer faktury zaliczkowej wystawionej poza KSeF (NrFaZaliczkowej)
    /// Wymagany gdy IsIssuedOutsideKSeF = true
    /// </summary>
    [XmlElement("NrFaZaliczkowej", Order = 2)]
    public string? AdvanceInvoiceNumber { get; set; }

    /// <summary>
    /// Kontroluje serializację numeru faktury poza KSeF
    /// </summary>
    public bool ShouldSerializeAdvanceInvoiceNumber() =>
        IsIssuedOutsideKSeF && !string.IsNullOrEmpty(AdvanceInvoiceNumber);

    /// <summary>
    /// Numer faktury zaliczkowej wystawionej w KSeF (NrKSeFFaZaliczkowej)
    /// Wymagany gdy faktura została wystawiona przez KSeF (IsIssuedOutsideKSeF = false)
    /// </summary>
    [XmlElement("NrKSeFFaZaliczkowej", Order = 3)]
    public string? KSeFNumber { get; set; }

    /// <summary>
    /// Kontroluje serializację numeru KSeF
    /// </summary>
    public bool ShouldSerializeKSeFNumber() =>
        !IsIssuedOutsideKSeF && !string.IsNullOrEmpty(KSeFNumber);
}
