/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca informacje o płatnościach na fakturze
    (Platnosc). Obsługuje status zapłaty (całościowa lub częściowa),
    terminy płatności, formy płatności (standardowe i inne), rachunki
    bankowe (w tym faktoringowe), warunki skonta, link do płatności
    bezgotówkowej oraz identyfikator płatności KSeF.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Informacje o płatnościach na fakturze (Platnosc)
/// Struktura zgodna z XSD FA(3): choice zapłaty, terminy, forma płatności, rachunki bankowe
/// </summary>
public class Payment
{
    #region Choice 1: Status zapłaty (Zaplacono/ZaplataCzesciowa)

    /// <summary>
    /// Znacznik zapłacenia (Zaplacono): 1 - zapłacono w całości
    /// Używane w parze z DataZaplaty. Wzajemnie wykluczające z ZaplataCzesciowa.
    /// </summary>
    [XmlElement("Zaplacono")]
    public int? PaidInFull { get; set; }

    /// <summary>
    /// Data zapłaty (DataZaplaty) - proxy string dla serializacji XML
    /// Wymagane gdy Zaplacono=1
    /// </summary>
    [XmlElement("DataZaplaty")]
    public string? PaymentDateString
    {
        get => PaymentDate?.ToString("yyyy-MM-dd");
        set => PaymentDate = string.IsNullOrEmpty(value) ? null : DateOnly.Parse(value);
    }

    /// <summary>
    /// Data zapłaty (DataZaplaty)
    /// </summary>
    [XmlIgnore]
    public DateOnly? PaymentDate { get; set; }

    /// <summary>
    /// Znacznik zapłaty częściowej (ZnacznikZaplatyCzesciowej):
    /// 1 - zapłacono w części, 2 - zapłacono w całości w kilku częściach
    /// Wzajemnie wykluczające z Zaplacono.
    /// </summary>
    [XmlElement("ZnacznikZaplatyCzesciowej")]
    public int? PartialPaymentMarker { get; set; }

    /// <summary>
    /// Lista zapłat częściowych (ZaplataCzesciowa), max 100
    /// Wymagane gdy ZnacznikZaplatyCzesciowej jest ustawiony
    /// </summary>
    [XmlElement("ZaplataCzesciowa")]
    public List<PartialPayment>? PartialPayments { get; set; }

    #endregion

    #region Terminy płatności

    /// <summary>
    /// Lista terminów płatności (TerminPlatnosci), max 100
    /// </summary>
    [XmlElement("TerminPlatnosci")]
    public List<PaymentTerm>? PaymentTerms { get; set; }

    #endregion

    #region Choice 2: Forma płatności (FormaPlatnosci lub PlatnoscInna+OpisPlatnosci)

    /// <summary>
    /// Forma płatności (FormaPlatnosci) - standardowa (wartość 1-7)
    /// Null jeśli nie określono lub wybrano PlatnoscInna.
    /// </summary>
    [XmlIgnore]
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// Proxy string do serializacji XML FormaPlatnosci jako integer
    /// </summary>
    [XmlElement("FormaPlatnosci")]
    public string? PaymentMethodString
    {
        get => PaymentMethod.HasValue ? ((int)PaymentMethod.Value).ToString() : null;
        set => PaymentMethod = string.IsNullOrEmpty(value) ? null : (PaymentMethod)int.Parse(value);
    }

    /// <summary>
    /// Znacznik innej formy płatności (PlatnoscInna): 1 - inna forma płatności
    /// Null jeśli wybrano standardową FormaPlatnosci
    /// </summary>
    [XmlElement("PlatnoscInna")]
    public int? OtherPaymentMarker { get; set; }

    /// <summary>
    /// Opis innej formy płatności (OpisPlatnosci)
    /// Wymagane gdy PlatnoscInna=1. Maksymalnie 256 znaków.
    /// </summary>
    [XmlElement("OpisPlatnosci")]
    public string? OtherPaymentDescription { get; set; }

    #endregion

    #region Rachunki bankowe

    /// <summary>
    /// Lista rachunków bankowych do płatności (RachunekBankowy), max 100
    /// </summary>
    [XmlElement("RachunekBankowy")]
    public List<BankAccount>? BankAccounts { get; set; }

    /// <summary>
    /// Lista rachunków bankowych faktora (RachunekBankowyFaktora), max 20
    /// Rachunek, na który nabywca dokonuje zapłaty przy faktoringu
    /// </summary>
    [XmlElement("RachunekBankowyFaktora")]
    public List<BankAccount>? FactoringBankAccounts { get; set; }

    #endregion

    #region Skonto

    /// <summary>
    /// Informacja o skoncie (Skonto)
    /// </summary>
    [XmlElement("Skonto")]
    public DiscountTerms? Discount { get; set; }

    #endregion

    #region Link do płatności i IPKSeF

    /// <summary>
    /// Link do płatności bezgotówkowej (LinkDoPlatnosci)
    /// URL z parametrem IPKSeF, max 512 znaków
    /// </summary>
    [XmlElement("LinkDoPlatnosci")]
    public string? PaymentLink { get; set; }

    /// <summary>
    /// Identyfikator płatności KSeF (IPKSeF)
    /// Format: 3 cyfry + 10 znaków alfanumerycznych, max 13 znaków
    /// </summary>
    [XmlElement("IPKSeF")]
    public string? KSeFPaymentId { get; set; }

    #endregion

    #region ShouldSerialize

    public bool ShouldSerializePaidInFull() => PaidInFull.HasValue;

    public bool ShouldSerializePaymentDateString() => PaymentDate.HasValue;

    public bool ShouldSerializePartialPaymentMarker() => PartialPaymentMarker.HasValue;

    public bool ShouldSerializePaymentMethodString() => PaymentMethod.HasValue;

    public bool ShouldSerializeOtherPaymentMarker() => OtherPaymentMarker.HasValue;

    public bool ShouldSerializeOtherPaymentDescription() => !string.IsNullOrEmpty(OtherPaymentDescription);

    public bool ShouldSerializePaymentLink() => !string.IsNullOrEmpty(PaymentLink);

    public bool ShouldSerializeKSeFPaymentId() => !string.IsNullOrEmpty(KSeFPaymentId);

    #endregion

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy określono terminy płatności
    /// </summary>
    [XmlIgnore]
    public bool HasPaymentTerms => PaymentTerms != null && PaymentTerms.Count > 0;

    /// <summary>
    /// Sprawdza czy określono formę płatności (standardową lub inną)
    /// </summary>
    [XmlIgnore]
    public bool HasPaymentMethod => PaymentMethod.HasValue || OtherPaymentMarker.HasValue;

    /// <summary>
    /// Sprawdza czy określono rachunki bankowe
    /// </summary>
    [XmlIgnore]
    public bool HasBankAccounts => BankAccounts != null && BankAccounts.Count > 0;

    /// <summary>
    /// Sprawdza czy określono rachunki faktoringowe
    /// </summary>
    [XmlIgnore]
    public bool HasFactoringBankAccounts => FactoringBankAccounts != null && FactoringBankAccounts.Count > 0;

    /// <summary>
    /// Sprawdza czy określono warunki skonta
    /// </summary>
    [XmlIgnore]
    public bool HasDiscount => Discount != null;

    #endregion
}
