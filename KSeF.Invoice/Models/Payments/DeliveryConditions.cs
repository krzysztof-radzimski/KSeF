using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Warunki dostawy (WarunkiDostawy)
/// Określa warunki dostarczenia towarów zgodnie z Incoterms lub innymi umowami
/// </summary>
public class DeliveryConditions
{
    /// <summary>
    /// Kod warunków dostawy (KodWarunkow)
    /// Kod zgodny z Incoterms (np. EXW, FCA, CPT, CIP, DAP, DPU, DDP, FAS, FOB, CFR, CIF)
    /// Maksymalnie 3 znaki
    /// </summary>
    [XmlElement("KodWarunkow")]
    public string? ConditionCode { get; set; }

    /// <summary>
    /// Opis warunków dostawy (OpisWarunkow)
    /// Tekstowy opis warunków dostawy
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("OpisWarunkow")]
    public string? ConditionDescription { get; set; }

    /// <summary>
    /// Miejsce dostawy dla warunków Incoterms (MiejsceDostawy)
    /// Miejsce, w którym następuje przejście ryzyka
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("MiejsceDostawy")]
    public string? DeliveryPlace { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy określono kod warunków dostawy
    /// </summary>
    [XmlIgnore]
    public bool HasConditionCode => !string.IsNullOrEmpty(ConditionCode);

    /// <summary>
    /// Sprawdza czy określono opis warunków dostawy
    /// </summary>
    [XmlIgnore]
    public bool HasConditionDescription => !string.IsNullOrEmpty(ConditionDescription);

    #endregion
}
