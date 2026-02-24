using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Identyfikator podatkowy podmiotu
/// </summary>
public class TaxIdentifier
{
    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP)
    /// Format: 10 cyfr bez kresek
    /// </summary>
    [XmlElement("NIP")]
    public string? Nip { get; set; }

    /// <summary>
    /// Numer PESEL (dla osób fizycznych)
    /// </summary>
    [XmlElement("PESEL")]
    public string? Pesel { get; set; }

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
    /// Identyfikator wewnętrzny z NIP
    /// Format: NIP-XXXXX (gdzie XXXXX to 5 cyfr)
    /// </summary>
    [XmlElement("IDWew")]
    public string? InternalId { get; set; }

    /// <summary>
    /// Podmiot nie posiada identyfikatora podatkowego lub identyfikator nie występuje na fakturze
    /// Wartość 1 oznacza brak identyfikatora
    /// </summary>
    [XmlElement("BrakID")]
    public int? NoIdentifier { get; set; }

    /// <summary>
    /// Sprawdza czy identyfikator jest typu NIP
    /// </summary>
    [XmlIgnore]
    public bool IsNip => !string.IsNullOrEmpty(Nip);

    /// <summary>
    /// Sprawdza czy identyfikator jest typu PESEL
    /// </summary>
    [XmlIgnore]
    public bool IsPesel => !string.IsNullOrEmpty(Pesel);

    /// <summary>
    /// Sprawdza czy identyfikator jest typu VAT UE
    /// </summary>
    [XmlIgnore]
    public bool IsVatEU => EUCountryCode.HasValue && !string.IsNullOrEmpty(VatNumberEU);

    /// <summary>
    /// Sprawdza czy identyfikator jest innym typem (zagraniczny)
    /// </summary>
    [XmlIgnore]
    public bool IsOther => !string.IsNullOrEmpty(OtherTaxId);

    /// <summary>
    /// Sprawdza czy podmiot nie posiada identyfikatora
    /// </summary>
    [XmlIgnore]
    public bool HasNoIdentifier => NoIdentifier == 1;
}
