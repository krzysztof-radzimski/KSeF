using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Dane identyfikacyjne podatnika/sprzedawcy (TPodmiot1)
/// </summary>
public class SellerIdentification
{
    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP) podatnika
    /// Format: 10 cyfr bez kresek
    /// </summary>
    [XmlElement("NIP")]
    public string Nip { get; set; } = string.Empty;

    /// <summary>
    /// Imię i nazwisko lub pełna nazwa firmy
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("Nazwa")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Dane identyfikacyjne nabywcy (TPodmiot2)
/// </summary>
public class BuyerIdentification
{
    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP) nabywcy
    /// </summary>
    [XmlElement("NIP")]
    public string? Nip { get; set; }

    /// <summary>
    /// Kod kraju UE (prefiks VAT) dla nabywców z UE
    /// </summary>
    [XmlElement("KodUE")]
    public EUCountryCode? EUCountryCode { get; set; }

    /// <summary>
    /// Numer Identyfikacyjny VAT kontrahenta UE
    /// </summary>
    [XmlElement("NrVatUE")]
    public string? VatNumberEU { get; set; }

    /// <summary>
    /// Kod kraju nadania identyfikatora podatkowego (dla podmiotów spoza UE)
    /// </summary>
    [XmlElement("KodKraju")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Identyfikator podatkowy inny (dla podmiotów zagranicznych)
    /// </summary>
    [XmlElement("NrID")]
    public string? OtherTaxId { get; set; }

    /// <summary>
    /// Podmiot nie posiada identyfikatora podatkowego
    /// Wartość 1 oznacza brak identyfikatora
    /// </summary>
    [XmlElement("BrakID")]
    public int? NoIdentifier { get; set; }

    /// <summary>
    /// Imię i nazwisko lub pełna nazwa firmy nabywcy
    /// Opcjonalne dla przypadków określonych w art. 106e ust. 5 pkt 3 ustawy
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("Nazwa")]
    public string? Name { get; set; }
}

/// <summary>
/// Dane identyfikacyjne podmiotu trzeciego (TPodmiot3)
/// </summary>
public class ThirdPartyIdentification
{
    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP)
    /// </summary>
    [XmlElement("NIP")]
    public string? Nip { get; set; }

    /// <summary>
    /// Identyfikator wewnętrzny z NIP
    /// Format: NIP-XXXXX
    /// </summary>
    [XmlElement("IDWew")]
    public string? InternalId { get; set; }

    /// <summary>
    /// Kod kraju UE (prefiks VAT)
    /// </summary>
    [XmlElement("KodUE")]
    public EUCountryCode? EUCountryCode { get; set; }

    /// <summary>
    /// Numer Identyfikacyjny VAT kontrahenta UE
    /// </summary>
    [XmlElement("NrVatUE")]
    public string? VatNumberEU { get; set; }

    /// <summary>
    /// Kod kraju nadania identyfikatora podatkowego
    /// </summary>
    [XmlElement("KodKraju")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Identyfikator podatkowy inny
    /// </summary>
    [XmlElement("NrID")]
    public string? OtherTaxId { get; set; }

    /// <summary>
    /// Podmiot nie posiada identyfikatora podatkowego
    /// </summary>
    [XmlElement("BrakID")]
    public int? NoIdentifier { get; set; }

    /// <summary>
    /// Imię i nazwisko lub pełna nazwa firmy
    /// </summary>
    [XmlElement("Nazwa")]
    public string? Name { get; set; }
}
