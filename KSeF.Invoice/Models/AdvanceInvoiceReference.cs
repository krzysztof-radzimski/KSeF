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

[XmlType("FakturaZaliczkowa")]
public class AdvanceInvoiceReference
{
    [XmlIgnore]
    public bool IsIssuedOutsideKSeF { get; set; }

    [XmlElement("NrKSeFZN", Order = 1)]
    public string? IssuedOutsideKSeFMarker
    {
        get => IsIssuedOutsideKSeF ? "1" : null;
        set => IsIssuedOutsideKSeF = value == "1";
    }

    public bool ShouldSerializeIssuedOutsideKSeFMarker() => IsIssuedOutsideKSeF;

    [XmlElement("NrFaZaliczkowej", Order = 2)]
    public string? AdvanceInvoiceNumber { get; set; }

    public bool ShouldSerializeAdvanceInvoiceNumber() =>
        IsIssuedOutsideKSeF && !string.IsNullOrEmpty(AdvanceInvoiceNumber);

    [XmlElement("NrKSeFFaZaliczkowej", Order = 3)]
    public string? KSeFNumber { get; set; }

    public bool ShouldSerializeKSeFNumber() =>
        !IsIssuedOutsideKSeF && !string.IsNullOrEmpty(KSeFNumber);
}
