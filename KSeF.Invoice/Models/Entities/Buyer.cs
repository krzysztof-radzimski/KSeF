/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca nabywcę na fakturze (Podmiot2) zgodnie
    ze schematem KSeF FA(3). Zawiera dane identyfikacyjne, adresy
    (podstawowy i korespondencyjny), dane kontaktowe, numer klienta
    oraz obowiązkowe znaczniki JST (jednostka samorządu terytorialnego)
    i GV (członek grupy VAT). Udostępnia wygodne akcesory do danych
    identyfikacyjnych nabywcy.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Entities;

/// <summary>
/// Podmiot będący nabywcą (Podmiot2)
/// Odpowiednik elementu Podmiot2 w schemacie KSeF FA(3)
/// Kolejność elementów: NrEORI?, DaneIdentyfikacyjne, Adres?, AdresKoresp?, DaneKontaktowe*, NrKlienta?, IDNabywcy?, JST, GV
/// </summary>
[XmlRoot("Podmiot2")]
public class Buyer
{
    /// <summary>
    /// Numer EORI nabywcy towarów (opcjonalny)
    /// </summary>
    [XmlElement("NrEORI", Order = 0)]
    public string? EoriNumber { get; set; }

    /// <summary>
    /// Dane identyfikacyjne nabywcy (DaneIdentyfikacyjne)
    /// Zawiera identyfikator nabywcy (NIP, KodUE+NrVatUE, KodKraju+NrID lub BrakID) i opcjonalnie Nazwę
    /// Typ TPodmiot2 w schemacie
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("DaneIdentyfikacyjne", Order = 1)]
    public BuyerIdentification Identification { get; set; } = new BuyerIdentification();

    /// <summary>
    /// Adres nabywcy (adres podstawowy)
    /// </summary>
    [XmlElement("Adres", Order = 2)]
    public Address? Address { get; set; }

    /// <summary>
    /// Adres korespondencyjny nabywcy (opcjonalny)
    /// </summary>
    [XmlElement("AdresKoresp", Order = 3)]
    public Address? CorrespondenceAddress { get; set; }

    /// <summary>
    /// Dane kontaktowe nabywcy (email, telefon)
    /// </summary>
    [XmlElement("DaneKontaktowe", Order = 4)]
    public ContactData? ContactData { get; set; }

    /// <summary>
    /// Numer klienta nadany przez sprzedawcę (opcjonalny)
    /// </summary>
    [XmlElement("NrKlienta", Order = 5)]
    public string? CustomerNumber { get; set; }

    /// <summary>
    /// Unikalny klucz powiązania danych nabywcy na fakturach korygujących (IDNabywcy)
    /// Używany gdy dane nabywcy na fakturze korygującej zmieniły się
    /// Maksymalnie 32 znaki
    /// </summary>
    [XmlElement("IDNabywcy", Order = 6)]
    public string? BuyerIdentityKey { get; set; }

    /// <summary>
    /// Znacznik jednostki podrzędnej JST (JST)
    /// Wartość 1 - faktura dotyczy jednostki podrzędnej JST
    /// Wartość 2 - faktura nie dotyczy jednostki podrzędnej JST
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("JST", Order = 7)]
    public int Jst { get; set; } = 2;

    /// <summary>
    /// Znacznik członka grupy VAT (GV)
    /// Wartość 1 - faktura dotyczy członka grupy VAT (wymaga wypełnienia Podmiot3 z rolą 10)
    /// Wartość 2 - faktura nie dotyczy członka grupy VAT
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("GV", Order = 8)]
    public int Gv { get; set; } = 2;

    public bool ShouldSerializeEoriNumber() => !string.IsNullOrEmpty(EoriNumber);
    public bool ShouldSerializeAddress() => Address != null;
    public bool ShouldSerializeCorrespondenceAddress() => CorrespondenceAddress != null;
    public bool ShouldSerializeContactData() => ContactData != null;
    public bool ShouldSerializeCustomerNumber() => !string.IsNullOrEmpty(CustomerNumber);
    public bool ShouldSerializeBuyerIdentityKey() => !string.IsNullOrEmpty(BuyerIdentityKey);

    // Convenience accessors delegating to Identification

    /// <summary>
    /// NIP nabywcy - convenience accessor do Identification.Nip
    /// </summary>
    [XmlIgnore]
    public string? TaxId
    {
        get => Identification.Nip;
        set => Identification.Nip = value;
    }

    /// <summary>
    /// Kod kraju UE - convenience accessor do Identification.EUCountryCode
    /// </summary>
    [XmlIgnore]
    public EUCountryCode? EuCountryCode
    {
        get => Identification.EUCountryCode;
        set => Identification.EUCountryCode = value;
    }

    /// <summary>
    /// Numer VAT UE - convenience accessor do Identification.VatNumberEU
    /// </summary>
    [XmlIgnore]
    public string? EuVatId
    {
        get => Identification.VatNumberEU;
        set => Identification.VatNumberEU = value;
    }

    /// <summary>
    /// Kod kraju identyfikatora spoza UE - convenience accessor do Identification.CountryCode
    /// </summary>
    [XmlIgnore]
    public string? OtherIdCountryCode
    {
        get => Identification.CountryCode;
        set => Identification.CountryCode = value;
    }

    /// <summary>
    /// Identyfikator podatkowy inny - convenience accessor do Identification.OtherTaxId
    /// </summary>
    [XmlIgnore]
    public string? OtherId
    {
        get => Identification.OtherTaxId;
        set => Identification.OtherTaxId = value;
    }

    /// <summary>
    /// Brak identyfikatora - convenience accessor do Identification.NoIdentifier
    /// </summary>
    [XmlIgnore]
    public int? NoIdentifier
    {
        get => Identification.NoIdentifier;
        set => Identification.NoIdentifier = value;
    }

    /// <summary>
    /// Nazwa nabywcy - convenience accessor do Identification.Name
    /// </summary>
    [XmlIgnore]
    public string? Name
    {
        get => Identification.Name;
        set => Identification.Name = value;
    }

    [XmlIgnore]
    public bool HasPolishTaxId => !string.IsNullOrEmpty(TaxId);

    [XmlIgnore]
    public bool HasEuVatId => EuCountryCode.HasValue && !string.IsNullOrEmpty(EuVatId);

    [XmlIgnore]
    public bool HasOtherId => !string.IsNullOrEmpty(OtherIdCountryCode) && !string.IsNullOrEmpty(OtherId);

    [XmlIgnore]
    public bool HasNoIdentifier => NoIdentifier == 1;

    [XmlIgnore]
    public bool HasCorrespondenceAddress => CorrespondenceAddress != null;

    [XmlIgnore]
    public bool HasContactData => ContactData != null;

    [XmlIgnore]
    public bool IsJST => Jst == 1;

    [XmlIgnore]
    public bool IsVatGroupMember => Gv == 1;
}
