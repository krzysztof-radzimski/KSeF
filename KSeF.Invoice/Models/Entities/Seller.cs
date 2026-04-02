using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Entities;

/// <summary>
/// Podmiot będący sprzedawcą / wystawcą faktury (Podmiot1)
/// Odpowiednik elementu Podmiot1 w schemacie KSeF FA(3)
/// Kolejność elementów: PrefiksPodatnika?, NrEORI?, DaneIdentyfikacyjne, Adres, AdresKoresp?, DaneKontaktowe*, StatusInfoPodatnika?
/// </summary>
[XmlRoot("Podmiot1")]
public class Seller
{
    /// <summary>
    /// Kod (prefiks) podatnika VAT UE - stała wartość "PL"
    /// Opcjonalny - używany dla przypadków określonych w art. 97 ust. 10 pkt 2 i 3 ustawy
    /// </summary>
    [XmlElement("PrefiksPodatnika", Order = 0)]
    public string? TaxPrefix { get; set; }

    /// <summary>
    /// Numer EORI (Economic Operators Registration and Identification)
    /// Numer rejestracyjny i identyfikacyjny przedsiębiorców w UE
    /// Używany w transakcjach celnych
    /// </summary>
    [XmlElement("NrEORI", Order = 1)]
    public string? EoriNumber { get; set; }

    /// <summary>
    /// Dane identyfikacyjne podatnika (DaneIdentyfikacyjne)
    /// Zawiera NIP i Nazwę sprzedawcy - typ TPodmiot1 w schemacie
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("DaneIdentyfikacyjne", Order = 2)]
    public SellerIdentification Identification { get; set; } = new SellerIdentification();

    /// <summary>
    /// Adres sprzedawcy (adres podstawowy)
    /// </summary>
    [XmlElement("Adres", Order = 3)]
    public Address? Address { get; set; }

    /// <summary>
    /// Adres korespondencyjny sprzedawcy (opcjonalny)
    /// </summary>
    [XmlElement("AdresKoresp", Order = 4)]
    public Address? CorrespondenceAddress { get; set; }

    /// <summary>
    /// Dane kontaktowe sprzedawcy (email, telefon)
    /// </summary>
    [XmlElement("DaneKontaktowe", Order = 5)]
    public ContactData? ContactData { get; set; }

    /// <summary>
    /// Informacja o statusie podatnika (likwidacja, restrukturyzacja, upadłość, przedsiębiorstwo w spadku)
    /// </summary>
    [XmlElement("StatusInfoPodatnika", Order = 6)]
    public TaxpayerStatus? StatusInfo { get; set; }

    public bool ShouldSerializeTaxPrefix() => !string.IsNullOrEmpty(TaxPrefix);
    public bool ShouldSerializeEoriNumber() => !string.IsNullOrEmpty(EoriNumber);
    public bool ShouldSerializeAddress() => Address != null;
    public bool ShouldSerializeCorrespondenceAddress() => CorrespondenceAddress != null;
    public bool ShouldSerializeContactData() => ContactData != null;
    public bool ShouldSerializeStatusInfo() => StatusInfo.HasValue;

    /// <summary>
    /// NIP sprzedawcy - convenience accessor do Identification.Nip
    /// Format: 10 cyfr bez kresek
    /// </summary>
    [XmlIgnore]
    public string? TaxId
    {
        get => Identification.Nip;
        set => Identification.Nip = value;
    }

    /// <summary>
    /// Nazwa sprzedawcy - convenience accessor do Identification.Name
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlIgnore]
    public string Name
    {
        get => Identification.Name;
        set => Identification.Name = value;
    }

    [XmlIgnore]
    public bool HasCorrespondenceAddress => CorrespondenceAddress != null;

    [XmlIgnore]
    public bool HasContactData => ContactData != null;

    [XmlIgnore]
    public bool HasEoriNumber => !string.IsNullOrEmpty(EoriNumber);

    [XmlIgnore]
    public bool HasStatusInfo => StatusInfo.HasValue;
}
