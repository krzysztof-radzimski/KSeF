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

/// <summary>
/// Dane faktury korygowanej (DaneFaKorygowanej)
/// Zawiera informacje o fakturze pierwotnej: datę wystawienia, numer oraz miejsce wystawienia (KSeF / poza KSeF)
/// Element FA(3) "DaneFaKorygowanej"
/// </summary>
public class CorrectedInvoiceData
{
    /// <summary>
    /// Data wystawienia faktury korygowanej (DataWystFaKorygowanej) - proxy string dla serializacji XML
    /// Format: YYYY-MM-DD
    /// Element FA(3) "DataWystFaKorygowanej"
    /// </summary>
    [XmlElement("DataWystFaKorygowanej", Order = 1)]
    public string CorrectedInvoiceIssueDateString
    {
        get => CorrectedInvoiceIssueDate.ToString("yyyy-MM-dd");
        set => CorrectedInvoiceIssueDate = string.IsNullOrEmpty(value) ? default : DateOnly.Parse(value);
    }

    /// <summary>
    /// Data wystawienia faktury korygowanej (DataWystFaKorygowanej)
    /// Data wystawienia faktury pierwotnej będącej przedmiotem korekty
    /// </summary>
    [XmlIgnore]
    public DateOnly CorrectedInvoiceIssueDate { get; set; }

    /// <summary>
    /// Numer faktury korygowanej (NrFaKorygowanej)
    /// Numer faktury pierwotnej będącej przedmiotem korekty
    /// Element FA(3) "NrFaKorygowanej", maksymalnie 256 znaków
    /// </summary>
    [XmlElement("NrFaKorygowanej", Order = 2)]
    public string CorrectedInvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Znacznik TWybor1 dla faktury wystawionej w KSeF (NrKSeF) - proxy dla serializacji XML
    /// Emituje "1" gdy faktura pierwotna była w KSeF, null gdy była poza KSeF
    /// Element FA(3) "NrKSeF" (pusty element znacznikujący wybór)
    /// </summary>
    [XmlElement("NrKSeF", Order = 3)]
    public string? IsIssuedInKSeFMarker
    {
        get => IsIssuedInKSeF ? "1" : null;
        set => IsIssuedInKSeF = value == "1";
    }

    /// <summary>
    /// Czy faktura pierwotna była wystawiona w KSeF (steruje znacznikiem NrKSeF)
    /// </summary>
    [XmlIgnore]
    public bool IsIssuedInKSeF { get; set; }

    /// <summary>
    /// Numer KSeF faktury korygowanej (NrKSeFFaKorygowanej)
    /// Numer KSeF faktury pierwotnej - wymagany gdy IsIssuedInKSeF=true
    /// Element FA(3) "NrKSeFFaKorygowanej"
    /// </summary>
    [XmlElement("NrKSeFFaKorygowanej", Order = 4)]
    public string? CorrectedInvoiceKSeFNumber { get; set; }

    /// <summary>
    /// Znacznik TWybor1 dla faktury wystawionej poza KSeF (NrKSeFN) - proxy dla serializacji XML
    /// Emituje "1" gdy faktura pierwotna była poza KSeF, null gdy była w KSeF
    /// Element FA(3) "NrKSeFN" (pusty element znacznikujący wybór)
    /// </summary>
    [XmlElement("NrKSeFN", Order = 5)]
    public string? IsIssuedOutsideKSeFMarker
    {
        get => IsIssuedOutsideKSeF ? "1" : null;
        set => IsIssuedOutsideKSeF = value == "1";
    }

    /// <summary>
    /// Czy faktura pierwotna była wystawiona poza KSeF (steruje znacznikiem NrKSeFN)
    /// </summary>
    [XmlIgnore]
    public bool IsIssuedOutsideKSeF { get; set; }

    public bool ShouldSerializeIsIssuedInKSeFMarker() => IsIssuedInKSeF;

    public bool ShouldSerializeCorrectedInvoiceKSeFNumber() =>
        IsIssuedInKSeF && !string.IsNullOrEmpty(CorrectedInvoiceKSeFNumber);

    public bool ShouldSerializeIsIssuedOutsideKSeFMarker() => IsIssuedOutsideKSeF;
}
