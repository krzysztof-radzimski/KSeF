using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Entities;

/// <summary>
/// Podmiot trzeci występujący na fakturze (Podmiot3)
/// Odpowiednik elementu Podmiot3 w schemacie KSeF FA(3)
/// Kolejność: IDNabywcy?, NrEORI?, DaneIdentyfikacyjne, Adres?, AdresKoresp?, DaneKontaktowe*, Rola|RolaInna+OpisRoli, Udzial?, NrKlienta?
/// </summary>
[XmlRoot("Podmiot3")]
public class ThirdParty
{
    /// <summary>
    /// Numer EORI podmiotu trzeciego (opcjonalny)
    /// </summary>
    [XmlElement("NrEORI", Order = 0)]
    public string? EoriNumber { get; set; }

    /// <summary>
    /// Dane identyfikacyjne podmiotu trzeciego (DaneIdentyfikacyjne)
    /// Typ TPodmiot3 w schemacie
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("DaneIdentyfikacyjne", Order = 1)]
    public ThirdPartyIdentification Identification { get; set; } = new ThirdPartyIdentification();

    /// <summary>
    /// Adres podmiotu trzeciego
    /// </summary>
    [XmlElement("Adres", Order = 2)]
    public Address? Address { get; set; }

    /// <summary>
    /// Rola podmiotu trzeciego na fakturze
    /// </summary>
    [XmlElement("Rola", Order = 3)]
    public SubjectRole Role { get; set; }

    /// <summary>
    /// Opis roli w przypadku wyboru roli "Inny"
    /// </summary>
    [XmlElement("OpisRoli", Order = 4)]
    public string? RoleDescription { get; set; }

    /// <summary>
    /// Udział procentowy podmiotu (dla roli AdditionalBuyer)
    /// </summary>
    [XmlElement("Udzial", Order = 5)]
    public decimal? SharePercentage { get; set; }

    // Convenience accessors delegating to Identification

    [XmlIgnore]
    public string? TaxId
    {
        get => Identification.Nip;
        set => Identification.Nip = value;
    }

    [XmlIgnore]
    public string? InternalId
    {
        get => Identification.InternalId;
        set => Identification.InternalId = value;
    }

    [XmlIgnore]
    public EUCountryCode? EuCountryCode
    {
        get => Identification.EUCountryCode;
        set => Identification.EUCountryCode = value;
    }

    [XmlIgnore]
    public string? EuVatId
    {
        get => Identification.VatNumberEU;
        set => Identification.VatNumberEU = value;
    }

    [XmlIgnore]
    public string? OtherIdCountryCode
    {
        get => Identification.CountryCode;
        set => Identification.CountryCode = value;
    }

    [XmlIgnore]
    public string? OtherId
    {
        get => Identification.OtherTaxId;
        set => Identification.OtherTaxId = value;
    }

    [XmlIgnore]
    public int? NoIdentifier
    {
        get => Identification.NoIdentifier;
        set => Identification.NoIdentifier = value;
    }

    [XmlIgnore]
    public string? Name
    {
        get => Identification.Name;
        set => Identification.Name = value;
    }

    [XmlIgnore]
    public bool HasPolishTaxId => !string.IsNullOrEmpty(TaxId);

    [XmlIgnore]
    public bool HasInternalId => !string.IsNullOrEmpty(InternalId);

    [XmlIgnore]
    public bool HasEuVatId => EuCountryCode.HasValue && !string.IsNullOrEmpty(EuVatId);

    [XmlIgnore]
    public bool HasOtherId => !string.IsNullOrEmpty(OtherIdCountryCode) && !string.IsNullOrEmpty(OtherId);

    [XmlIgnore]
    public bool HasNoIdentifier => NoIdentifier == 1;

    [XmlIgnore]
    public bool HasSharePercentage => SharePercentage.HasValue;
}
