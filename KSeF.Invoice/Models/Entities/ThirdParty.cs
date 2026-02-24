using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Entities;

/// <summary>
/// Podmiot trzeci występujący na fakturze (Podmiot3)
/// Odpowiednik elementu Podmiot3 w schemacie KSeF
/// Może to być m.in. faktor, odbiorca, podmiot pierwotny, dodatkowy nabywca, wystawca faktury, płatnik
/// </summary>
[XmlRoot("Podmiot3")]
public class ThirdParty
{
    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP)
    /// Format: 10 cyfr bez kresek
    /// </summary>
    [XmlElement("NIP")]
    public string? TaxId { get; set; }

    /// <summary>
    /// Identyfikator wewnętrzny z NIP
    /// Format: NIP-XXXXX (gdzie XXXXX to 5 cyfr)
    /// Używany np. dla jednostek organizacyjnych JST
    /// </summary>
    [XmlElement("IDWew")]
    public string? InternalId { get; set; }

    /// <summary>
    /// Kod kraju UE (prefiks VAT) dla podmiotów z UE
    /// </summary>
    [XmlElement("KodUE")]
    public EUCountryCode? EuCountryCode { get; set; }

    /// <summary>
    /// Numer Identyfikacyjny VAT kontrahenta UE
    /// Używany razem z EuCountryCode dla kontrahentów z UE
    /// </summary>
    [XmlElement("NrVatUE")]
    public string? EuVatId { get; set; }

    /// <summary>
    /// Kod kraju nadania identyfikatora podatkowego (dla podmiotów spoza UE)
    /// </summary>
    [XmlElement("KodKraju")]
    public string? OtherIdCountryCode { get; set; }

    /// <summary>
    /// Identyfikator podatkowy inny (dla podmiotów zagranicznych spoza UE)
    /// </summary>
    [XmlElement("NrID")]
    public string? OtherId { get; set; }

    /// <summary>
    /// Podmiot nie posiada identyfikatora podatkowego
    /// Wartość 1 oznacza brak identyfikatora
    /// </summary>
    [XmlElement("BrakID")]
    public int? NoIdentifier { get; set; }

    /// <summary>
    /// Imię i nazwisko lub pełna nazwa firmy podmiotu trzeciego
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("Nazwa")]
    public string? Name { get; set; }

    /// <summary>
    /// Adres podmiotu trzeciego
    /// </summary>
    [XmlElement("Adres")]
    public Address? Address { get; set; }

    /// <summary>
    /// Rola podmiotu trzeciego na fakturze
    /// Określa w jakiej roli występuje podmiot (faktor, odbiorca, podmiot pierwotny itp.)
    /// </summary>
    [XmlElement("Rola")]
    public SubjectRole Role { get; set; }

    /// <summary>
    /// Opis roli w przypadku wyboru roli "Inny"
    /// Dodatkowy opis wyjaśniający rolę podmiotu
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("OpisRoli")]
    public string? RoleDescription { get; set; }

    /// <summary>
    /// Udział procentowy podmiotu (dla roli AdditionalBuyer)
    /// Używany gdy faktura dotyczy kilku nabywców z określonymi udziałami
    /// Wartość od 0.01 do 100.00
    /// </summary>
    [XmlElement("Udzial")]
    public decimal? SharePercentage { get; set; }

    /// <summary>
    /// Sprawdza czy podmiot ma NIP polski
    /// </summary>
    [XmlIgnore]
    public bool HasPolishTaxId => !string.IsNullOrEmpty(TaxId);

    /// <summary>
    /// Sprawdza czy podmiot ma identyfikator wewnętrzny
    /// </summary>
    [XmlIgnore]
    public bool HasInternalId => !string.IsNullOrEmpty(InternalId);

    /// <summary>
    /// Sprawdza czy podmiot ma VAT UE
    /// </summary>
    [XmlIgnore]
    public bool HasEuVatId => EuCountryCode.HasValue && !string.IsNullOrEmpty(EuVatId);

    /// <summary>
    /// Sprawdza czy podmiot ma inny identyfikator (zagraniczny spoza UE)
    /// </summary>
    [XmlIgnore]
    public bool HasOtherId => !string.IsNullOrEmpty(OtherIdCountryCode) && !string.IsNullOrEmpty(OtherId);

    /// <summary>
    /// Sprawdza czy podmiot nie posiada żadnego identyfikatora
    /// </summary>
    [XmlIgnore]
    public bool HasNoIdentifier => NoIdentifier == 1;

    /// <summary>
    /// Sprawdza czy określono udział procentowy
    /// </summary>
    [XmlIgnore]
    public bool HasSharePercentage => SharePercentage.HasValue;
}
