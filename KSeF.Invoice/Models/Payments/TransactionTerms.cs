using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Warunki transakcji (WarunkiTransakcji)
/// Informacje o warunkach realizacji transakcji handlowej
/// </summary>
public class TransactionTerms
{
    /// <summary>
    /// Informacje o transporcie (Transport)
    /// Dane dotyczące transportu towarów
    /// </summary>
    [XmlElement("Transport")]
    public Transport? Transport { get; set; }

    /// <summary>
    /// Warunki dostawy (WarunkiDostawy)
    /// Określenie warunków dostawy np. według Incoterms
    /// </summary>
    [XmlElement("WarunkiDostawy")]
    public DeliveryConditions? DeliveryConditions { get; set; }

    /// <summary>
    /// Numer zamówienia lub umowy (NrZamowienia)
    /// Numer dokumentu, do którego odnosi się faktura
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("NrZamowienia")]
    public string? OrderNumber { get; set; }

    /// <summary>
    /// Data zamówienia lub umowy (DataZamowienia)
    /// </summary>
    [XmlElement("DataZamowienia")]
    public DateOnly? OrderDate { get; set; }

    /// <summary>
    /// Numer specyfikacji do faktury (NrSpecyfikacji)
    /// Numer dokumentu zawierającego szczegółową specyfikację towaru/usługi
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("NrSpecyfikacji")]
    public string? SpecificationNumber { get; set; }

    /// <summary>
    /// Numer partii produkcyjnej towaru (NrPartiiTowaru)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("NrPartiiTowaru")]
    public string? BatchNumber { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy określono informacje o transporcie
    /// </summary>
    [XmlIgnore]
    public bool HasTransport => Transport != null;

    /// <summary>
    /// Sprawdza czy określono warunki dostawy
    /// </summary>
    [XmlIgnore]
    public bool HasDeliveryConditions => DeliveryConditions != null;

    /// <summary>
    /// Sprawdza czy określono numer zamówienia
    /// </summary>
    [XmlIgnore]
    public bool HasOrderNumber => !string.IsNullOrEmpty(OrderNumber);

    /// <summary>
    /// Sprawdza czy określono numer specyfikacji
    /// </summary>
    [XmlIgnore]
    public bool HasSpecificationNumber => !string.IsNullOrEmpty(SpecificationNumber);

    /// <summary>
    /// Sprawdza czy określono numer partii
    /// </summary>
    [XmlIgnore]
    public bool HasBatchNumber => !string.IsNullOrEmpty(BatchNumber);

    #endregion
}
