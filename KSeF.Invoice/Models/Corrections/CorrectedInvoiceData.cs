/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca dane faktury korygowanej (DaneFaKorygowanej).
    Zawiera informacje o fakturze pierwotnej będącej przedmiotem
    korekty: numer faktury, datę wystawienia oraz informację
    czy faktura pierwotna była wystawiona w KSeF (xsd:choice).

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Corrections;

public class CorrectedInvoiceData
{
    [XmlElement("DataWystFaKorygowanej", Order = 1)]
    public string CorrectedInvoiceIssueDateString
    {
        get => CorrectedInvoiceIssueDate.ToString("yyyy-MM-dd");
        set => CorrectedInvoiceIssueDate = string.IsNullOrEmpty(value) ? default : DateOnly.Parse(value);
    }

    [XmlIgnore]
    public DateOnly CorrectedInvoiceIssueDate { get; set; }

    [XmlElement("NrFaKorygowanej", Order = 2)]
    public string CorrectedInvoiceNumber { get; set; } = string.Empty;

    // TWybor1 — emituje "1" jako znacznik, że faktura pierwotna była w KSeF
    [XmlElement("NrKSeF", Order = 3)]
    public string? IsIssuedInKSeFMarker
    {
        get => IsIssuedInKSeF ? "1" : null;
        set => IsIssuedInKSeF = value == "1";
    }

    [XmlIgnore]
    public bool IsIssuedInKSeF { get; set; }

    [XmlElement("NrKSeFFaKorygowanej", Order = 4)]
    public string? CorrectedInvoiceKSeFNumber { get; set; }

    // TWybor1 — emituje "1" jako znacznik, że faktura pierwotna była poza KSeF
    [XmlElement("NrKSeFN", Order = 5)]
    public string? IsIssuedOutsideKSeFMarker
    {
        get => IsIssuedOutsideKSeF ? "1" : null;
        set => IsIssuedOutsideKSeF = value == "1";
    }

    [XmlIgnore]
    public bool IsIssuedOutsideKSeF { get; set; }

    public bool ShouldSerializeIsIssuedInKSeFMarker() => IsIssuedInKSeF;

    public bool ShouldSerializeCorrectedInvoiceKSeFNumber() =>
        IsIssuedInKSeF && !string.IsNullOrEmpty(CorrectedInvoiceKSeFNumber);

    public bool ShouldSerializeIsIssuedOutsideKSeFMarker() => IsIssuedOutsideKSeF;
}
