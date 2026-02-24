using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Attachments;

/// <summary>
/// Stopka faktury (Stopka)
/// Zawiera pozostałe dane na fakturze - informacje dodatkowe i dane rejestrowe
/// Zgodna ze schematem KSeF FA(3)
/// </summary>
[XmlType("Stopka")]
public class InvoiceFooter
{
    /// <summary>
    /// Lista informacji dodatkowych (Informacje)
    /// Pozostałe dane na fakturze - maksymalnie 3 wpisy
    /// </summary>
    [XmlElement("Informacje")]
    public List<FooterInfo>? AdditionalInfo { get; set; }

    /// <summary>
    /// Lista danych rejestrowych (Rejestry)
    /// Numery podmiotu w innych rejestrach i bazach danych
    /// Maksymalnie 100 wpisów
    /// </summary>
    [XmlElement("Rejestry")]
    public List<RegistryData>? Registries { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy stopka zawiera dodatkowe informacje
    /// </summary>
    [XmlIgnore]
    public bool HasAdditionalInfo => AdditionalInfo != null && AdditionalInfo.Count > 0;

    /// <summary>
    /// Sprawdza czy stopka zawiera dane rejestrowe
    /// </summary>
    [XmlIgnore]
    public bool HasRegistries => Registries != null && Registries.Count > 0;

    #endregion
}

/// <summary>
/// Informacje dodatkowe w stopce faktury (Informacje)
/// </summary>
[XmlType("Informacje")]
public class FooterInfo
{
    /// <summary>
    /// Tekst stopki faktury (StopkaFaktury)
    /// Dodatkowe informacje tekstowe
    /// </summary>
    [XmlElement("StopkaFaktury")]
    public string? FooterText { get; set; }

    /// <summary>
    /// Sprawdza czy zawiera tekst
    /// </summary>
    [XmlIgnore]
    public bool HasText => !string.IsNullOrEmpty(FooterText);
}

/// <summary>
/// Dane rejestrowe podmiotu (Rejestry)
/// Numery podmiotu lub grupy podmiotów w innych rejestrach i bazach danych
/// </summary>
[XmlType("Rejestry")]
public class RegistryData
{
    /// <summary>
    /// Pełna nazwa podmiotu (PelnaNazwa)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("PelnaNazwa")]
    public string? FullName { get; set; }

    /// <summary>
    /// Numer KRS (KRS)
    /// Numer w Krajowym Rejestrze Sądowym
    /// Format: 10 cyfr
    /// </summary>
    [XmlElement("KRS")]
    public string? KRS { get; set; }

    /// <summary>
    /// Numer REGON (REGON)
    /// Numer w rejestrze REGON
    /// Format: 9 lub 14 cyfr
    /// </summary>
    [XmlElement("REGON")]
    public string? REGON { get; set; }

    /// <summary>
    /// Numer BDO (BDO)
    /// Numer rejestrowy w Bazie danych o produktach i opakowaniach
    /// oraz o gospodarce odpadami
    /// Maksymalnie 9 znaków
    /// </summary>
    [XmlElement("BDO")]
    public string? BDO { get; set; }

    /// <summary>
    /// Sprawdza czy podano numer KRS
    /// </summary>
    [XmlIgnore]
    public bool HasKRS => !string.IsNullOrEmpty(KRS);

    /// <summary>
    /// Sprawdza czy podano numer REGON
    /// </summary>
    [XmlIgnore]
    public bool HasREGON => !string.IsNullOrEmpty(REGON);

    /// <summary>
    /// Sprawdza czy podano numer BDO
    /// </summary>
    [XmlIgnore]
    public bool HasBDO => !string.IsNullOrEmpty(BDO);
}
