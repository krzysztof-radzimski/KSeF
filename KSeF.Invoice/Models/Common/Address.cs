using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Informacje opisujące adres (TAdres)
/// </summary>
public class Address
{
    /// <summary>
    /// Kod kraju (ISO 3166-1 alpha-2)
    /// </summary>
    [XmlElement("KodKraju")]
    public string CountryCode { get; set; } = "PL";

    /// <summary>
    /// Adres - linia 1 (ulica, numer domu/lokalu lub miejscowość)
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("AdresL1")]
    public string AddressLine1 { get; set; } = string.Empty;

    /// <summary>
    /// Adres - linia 2 (kod pocztowy, miejscowość - opcjonalnie)
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("AdresL2")]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Globalny Numer Lokalizacyjny (GLN)
    /// Maksymalnie 13 znaków
    /// </summary>
    [XmlElement("GLN")]
    public string? Gln { get; set; }
}

/// <summary>
/// Informacje opisujące adres polski (TAdresPolski) - rozszerzony format
/// </summary>
public class PolishAddress
{
    /// <summary>
    /// Kod kraju - zawsze PL dla adresu polskiego
    /// </summary>
    [XmlElement("KodKraju")]
    public string CountryCode { get; set; } = "PL";

    /// <summary>
    /// Województwo
    /// </summary>
    [XmlElement("Wojewodztwo")]
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// Powiat
    /// </summary>
    [XmlElement("Powiat")]
    public string County { get; set; } = string.Empty;

    /// <summary>
    /// Gmina
    /// </summary>
    [XmlElement("Gmina")]
    public string Municipality { get; set; } = string.Empty;

    /// <summary>
    /// Nazwa ulicy (opcjonalnie - może nie występować dla małych miejscowości)
    /// </summary>
    [XmlElement("Ulica")]
    public string? Street { get; set; }

    /// <summary>
    /// Numer budynku
    /// </summary>
    [XmlElement("NrDomu")]
    public string BuildingNumber { get; set; } = string.Empty;

    /// <summary>
    /// Numer lokalu (opcjonalnie)
    /// </summary>
    [XmlElement("NrLokalu")]
    public string? ApartmentNumber { get; set; }

    /// <summary>
    /// Nazwa miejscowości
    /// </summary>
    [XmlElement("Miejscowosc")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Kod pocztowy (format XX-XXX)
    /// </summary>
    [XmlElement("KodPocztowy")]
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Nazwa urzędu pocztowego (opcjonalnie w niektórych wariantach)
    /// </summary>
    [XmlElement("Poczta")]
    public string? PostOffice { get; set; }
}

/// <summary>
/// Informacje opisujące adres zagraniczny (TAdresZagraniczny)
/// </summary>
public class ForeignAddress
{
    /// <summary>
    /// Kod kraju (ISO 3166-1 alpha-2) - inny niż PL
    /// </summary>
    [XmlElement("KodKraju")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Kod pocztowy (opcjonalnie)
    /// </summary>
    [XmlElement("KodPocztowy")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Nazwa miejscowości
    /// </summary>
    [XmlElement("Miejscowosc")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Nazwa ulicy (opcjonalnie)
    /// </summary>
    [XmlElement("Ulica")]
    public string? Street { get; set; }

    /// <summary>
    /// Numer budynku (opcjonalnie)
    /// </summary>
    [XmlElement("NrDomu")]
    public string? BuildingNumber { get; set; }

    /// <summary>
    /// Numer lokalu (opcjonalnie)
    /// </summary>
    [XmlElement("NrLokalu")]
    public string? ApartmentNumber { get; set; }
}
