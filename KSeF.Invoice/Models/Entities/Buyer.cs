using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Entities;

/// <summary>
/// Podmiot będący nabywcą (Podmiot2)
/// Odpowiednik elementu Podmiot2 w schemacie KSeF
/// </summary>
[XmlRoot("Podmiot2")]
public class Buyer
{
    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP) nabywcy
    /// Format: 10 cyfr bez kresek
    /// Używany dla podmiotów polskich
    /// </summary>
    [XmlElement("NIP")]
    public string? TaxId { get; set; }

    /// <summary>
    /// Kod kraju UE (prefiks VAT) dla nabywców z UE
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
    /// Imię i nazwisko lub pełna nazwa firmy nabywcy
    /// Opcjonalne dla przypadków określonych w art. 106e ust. 5 pkt 3 ustawy
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("Nazwa")]
    public string? Name { get; set; }

    /// <summary>
    /// Adres nabywcy (adres podstawowy)
    /// </summary>
    [XmlElement("Adres")]
    public Address? Address { get; set; }

    /// <summary>
    /// Adres korespondencyjny nabywcy (opcjonalny)
    /// </summary>
    [XmlElement("AdresKoresp")]
    public Address? CorrespondenceAddress { get; set; }

    /// <summary>
    /// Dane kontaktowe nabywcy (email, telefon)
    /// </summary>
    [XmlElement("DaneKontaktowe")]
    public ContactData? ContactData { get; set; }

    /// <summary>
    /// Numer klienta nadany przez sprzedawcę (opcjonalny)
    /// Wewnętrzny identyfikator nabywcy w systemie sprzedawcy
    /// </summary>
    [XmlElement("NrKlienta")]
    public string? CustomerNumber { get; set; }

    /// <summary>
    /// Znacznik wskazujący, że nabywca jest jednostką samorządu terytorialnego (JST)
    /// Wartość 1 oznacza, że nabywca jest JST
    /// </summary>
    [XmlElement("IDNabywcy")]
    public int? IsLocalGovernmentUnit { get; set; }

    /// <summary>
    /// Znacznik wskazujący, że nabywca jest grupą VAT
    /// Wartość 1 oznacza, że nabywca jest grupą VAT
    /// </summary>
    [XmlElement("GrupaVAT")]
    public int? IsVatGroup { get; set; }

    /// <summary>
    /// Sprawdza czy nabywca ma NIP polski
    /// </summary>
    [XmlIgnore]
    public bool HasPolishTaxId => !string.IsNullOrEmpty(TaxId);

    /// <summary>
    /// Sprawdza czy nabywca ma VAT UE
    /// </summary>
    [XmlIgnore]
    public bool HasEuVatId => EuCountryCode.HasValue && !string.IsNullOrEmpty(EuVatId);

    /// <summary>
    /// Sprawdza czy nabywca ma inny identyfikator (zagraniczny spoza UE)
    /// </summary>
    [XmlIgnore]
    public bool HasOtherId => !string.IsNullOrEmpty(OtherIdCountryCode) && !string.IsNullOrEmpty(OtherId);

    /// <summary>
    /// Sprawdza czy nabywca nie posiada żadnego identyfikatora
    /// </summary>
    [XmlIgnore]
    public bool HasNoIdentifier => NoIdentifier == 1;

    /// <summary>
    /// Sprawdza czy adres korespondencyjny jest zdefiniowany
    /// </summary>
    [XmlIgnore]
    public bool HasCorrespondenceAddress => CorrespondenceAddress != null;

    /// <summary>
    /// Sprawdza czy dane kontaktowe są zdefiniowane
    /// </summary>
    [XmlIgnore]
    public bool HasContactData => ContactData != null;

    /// <summary>
    /// Sprawdza czy nabywca jest jednostką samorządu terytorialnego
    /// </summary>
    [XmlIgnore]
    public bool IsJST => IsLocalGovernmentUnit == 1;

    /// <summary>
    /// Sprawdza czy nabywca jest grupą VAT
    /// </summary>
    [XmlIgnore]
    public bool IsVATGroup => IsVatGroup == 1;
}
