using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Entities;

/// <summary>
/// Podmiot będący sprzedawcą / wystawcą faktury (Podmiot1)
/// Odpowiednik elementu Podmiot1 w schemacie KSeF
/// </summary>
[XmlRoot("Podmiot1")]
public class Seller
{
    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP) sprzedawcy
    /// Format: 10 cyfr bez kresek
    /// </summary>
    [XmlElement("NIP")]
    public string TaxId { get; set; } = string.Empty;

    /// <summary>
    /// Imię i nazwisko lub pełna nazwa firmy sprzedawcy
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("Nazwa")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Adres sprzedawcy (adres podstawowy)
    /// </summary>
    [XmlElement("Adres")]
    public Address? Address { get; set; }

    /// <summary>
    /// Adres korespondencyjny sprzedawcy (opcjonalny)
    /// </summary>
    [XmlElement("AdresKoresp")]
    public Address? CorrespondenceAddress { get; set; }

    /// <summary>
    /// Dane kontaktowe sprzedawcy (email, telefon)
    /// </summary>
    [XmlElement("DaneKontaktowe")]
    public ContactData? ContactData { get; set; }

    /// <summary>
    /// Numer EORI (Economic Operators Registration and Identification)
    /// Numer rejestracyjny i identyfikacyjny przedsiębiorców w UE
    /// Używany w transakcjach celnych
    /// </summary>
    [XmlElement("NrEORI")]
    public string? EoriNumber { get; set; }

    /// <summary>
    /// Informacja o statusie podatnika (likwidacja, restrukturyzacja, upadłość, przedsiębiorstwo w spadku)
    /// </summary>
    [XmlElement("StatusInfoPodatnika")]
    public TaxpayerStatus? StatusInfo { get; set; }

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
    /// Sprawdza czy numer EORI jest zdefiniowany
    /// </summary>
    [XmlIgnore]
    public bool HasEoriNumber => !string.IsNullOrEmpty(EoriNumber);

    /// <summary>
    /// Sprawdza czy informacja o statusie podatnika jest zdefiniowana
    /// </summary>
    [XmlIgnore]
    public bool HasStatusInfo => StatusInfo.HasValue;
}
