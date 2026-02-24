using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Informacje o płatnościach na fakturze (Platnosc)
/// Zawiera terminy płatności, formy płatności i rachunki bankowe
/// </summary>
public class Payment
{
    /// <summary>
    /// Lista terminów płatności (TerminPlatnosci)
    /// Może zawierać wiele terminów płatności dla płatności w ratach
    /// </summary>
    [XmlElement("TerminPlatnosci")]
    public List<PaymentTerm>? PaymentTerms { get; set; }

    /// <summary>
    /// Lista form płatności (FormaPlatnosci)
    /// Określa jedną lub więcej metod płatności za fakturę
    /// </summary>
    [XmlElement("FormaPlatnosci")]
    public List<PaymentMethodInfo>? PaymentMethods { get; set; }

    /// <summary>
    /// Lista rachunków bankowych do płatności (RachunekBankowy)
    /// Rachunki bankowe, na które należy dokonać płatności
    /// </summary>
    [XmlElement("RachunekBankowy")]
    public List<BankAccount>? BankAccounts { get; set; }

    /// <summary>
    /// Rachunek bankowy faktoringowy (RachunekBankowyFakt662)
    /// Rachunek, na który nabywca dokonuje zapłaty przy faktoringu
    /// zgodnie z art. 66 § 2 Ordynacji podatkowej
    /// </summary>
    [XmlElement("RachunekBankowyFakt662")]
    public BankAccount? FactoringBankAccount { get; set; }

    /// <summary>
    /// Opis uzgodnionych warunków płatności (Skonto)
    /// Np. opis warunków skonta lub innych szczegółowych ustaleń płatniczych
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("Skonto")]
    public string? DiscountTerms { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy określono terminy płatności
    /// </summary>
    [XmlIgnore]
    public bool HasPaymentTerms => PaymentTerms != null && PaymentTerms.Count > 0;

    /// <summary>
    /// Sprawdza czy określono formy płatności
    /// </summary>
    [XmlIgnore]
    public bool HasPaymentMethods => PaymentMethods != null && PaymentMethods.Count > 0;

    /// <summary>
    /// Sprawdza czy określono rachunki bankowe
    /// </summary>
    [XmlIgnore]
    public bool HasBankAccounts => BankAccounts != null && BankAccounts.Count > 0;

    /// <summary>
    /// Sprawdza czy określono rachunek faktoringowy
    /// </summary>
    [XmlIgnore]
    public bool HasFactoringBankAccount => FactoringBankAccount != null;

    /// <summary>
    /// Sprawdza czy określono warunki skonta
    /// </summary>
    [XmlIgnore]
    public bool HasDiscountTerms => !string.IsNullOrEmpty(DiscountTerms);

    #endregion
}
