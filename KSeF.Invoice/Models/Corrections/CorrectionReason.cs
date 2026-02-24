using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Corrections;

/// <summary>
/// Przyczyna korekty faktury (PrzyczynaKorekty)
/// Zawiera informacje o powodzie wystawienia faktury korygującej
/// </summary>
public class CorrectionReason
{
    /// <summary>
    /// Przyczyna korekty (Przyczyna)
    /// Tekstowy opis powodu korekty faktury
    /// Wymagane dla faktur korygujących zgodnie z art. 106j ust. 2 ustawy o VAT
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("Przyczyna")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Typ skutku korekty w ewidencji (TypKorekty)
    /// Określa, w jakim okresie korekta powinna być ujęta w ewidencji VAT
    /// </summary>
    [XmlElement("TypKorekty")]
    public CorrectionType? CorrectionType { get; set; }

    /// <summary>
    /// Data ujęcia korekty (DataUjeciaKorekty)
    /// Data, w której korekta powinna być ujęta w ewidencji
    /// Stosowane gdy typ korekty to OtherDate (3)
    /// </summary>
    [XmlElement("DataUjeciaKorekty")]
    public DateOnly? CorrectionRecordDate { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy określono typ korekty
    /// </summary>
    [XmlIgnore]
    public bool HasCorrectionType => CorrectionType.HasValue;

    /// <summary>
    /// Sprawdza czy określono datę ujęcia korekty
    /// </summary>
    [XmlIgnore]
    public bool HasCorrectionRecordDate => CorrectionRecordDate.HasValue;

    #endregion
}
