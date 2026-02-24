using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Corrections;

/// <summary>
/// Dane faktury korygowanej (DaneFaKorygowanej)
/// Zawiera informacje o fakturze pierwotnej, która jest korygowana
/// </summary>
public class CorrectedInvoiceData
{
    /// <summary>
    /// Numer faktury korygowanej (NrFaKorygowanej)
    /// Numer faktury pierwotnej, która jest przedmiotem korekty
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("NrFaKorygowanej")]
    public string CorrectedInvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Data wystawienia faktury korygowanej (DataWystFaKorygowanej)
    /// Data wystawienia faktury pierwotnej
    /// Format: YYYY-MM-DD
    /// </summary>
    [XmlElement("DataWystFaKorygowanej")]
    public DateOnly CorrectedInvoiceIssueDate { get; set; }

    /// <summary>
    /// Numer KSeF faktury korygowanej (NrKSeF)
    /// Numer identyfikujący fakturę korygowaną w systemie KSeF
    /// Używane gdy faktura korygowana została wystawiona w KSeF
    /// </summary>
    [XmlElement("NrKSeF")]
    public string? CorrectedInvoiceKSeFNumber { get; set; }

    /// <summary>
    /// Numer KSeF faktury korygującej tę fakturę (NrKSeFN)
    /// Stosowane w przypadku kolejnych korekt faktury
    /// </summary>
    [XmlElement("NrKSeFN")]
    public string? PreviousCorrectionKSeFNumber { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy faktura korygowana posiada numer KSeF
    /// </summary>
    [XmlIgnore]
    public bool HasKSeFNumber => !string.IsNullOrEmpty(CorrectedInvoiceKSeFNumber);

    /// <summary>
    /// Sprawdza czy istnieje wcześniejsza korekta tej faktury
    /// </summary>
    [XmlIgnore]
    public bool HasPreviousCorrection => !string.IsNullOrEmpty(PreviousCorrectionKSeFNumber);

    #endregion
}
