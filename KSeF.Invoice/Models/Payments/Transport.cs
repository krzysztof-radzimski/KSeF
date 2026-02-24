using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Informacje o transporcie towarów (Transport)
/// Dane dotyczące transportu związanego z dostawą towarów
/// </summary>
public class Transport
{
    /// <summary>
    /// Rodzaj transportu (RodzajTransportu)
    /// Określa środek transportu użyty do dostawy
    /// </summary>
    [XmlElement("RodzajTransportu")]
    public TransportType? TransportType { get; set; }

    /// <summary>
    /// Nazwa przewoźnika (Przewoznik)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("Przewoznik")]
    public string? CarrierName { get; set; }

    /// <summary>
    /// NIP przewoźnika (NrPrzewoznika)
    /// Numer identyfikacji podatkowej przewoźnika
    /// </summary>
    [XmlElement("NrPrzewoznika")]
    public string? CarrierTaxId { get; set; }

    /// <summary>
    /// Inne dane przewoźnika (OpisPrzew662)
    /// Opis pozwalający na identyfikację przewoźnika
    /// (np. gdy przewoźnik nie posiada NIP)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("OpisPrzewoznika")]
    public string? CarrierDescription { get; set; }

    /// <summary>
    /// Data rozpoczęcia transportu (DataGodzRozpsss)
    /// </summary>
    [XmlElement("DataGodzRozpsss")]
    public DateTime? TransportStartDateTime { get; set; }

    /// <summary>
    /// Data zakończenia transportu lub dostarczenia (DataGodzZakonczenia)
    /// </summary>
    [XmlElement("DataGodzZakonczenia")]
    public DateTime? TransportEndDateTime { get; set; }

    /// <summary>
    /// Miejsce wysyłki/rozpoczęcia transportu (WysijkiZ)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("WysijkiZ")]
    public string? ShipmentFrom { get; set; }

    /// <summary>
    /// Miejsce dostarczenia/zakończenia transportu (WysijkiDo)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("WysijkiDo")]
    public string? ShipmentTo { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy określono rodzaj transportu
    /// </summary>
    [XmlIgnore]
    public bool HasTransportType => TransportType.HasValue;

    /// <summary>
    /// Sprawdza czy określono przewoźnika
    /// </summary>
    [XmlIgnore]
    public bool HasCarrier => !string.IsNullOrEmpty(CarrierName) ||
                               !string.IsNullOrEmpty(CarrierTaxId) ||
                               !string.IsNullOrEmpty(CarrierDescription);

    #endregion
}
