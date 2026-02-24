using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Rola podmiotu trzeciego (TRolaPodmiotu3)
/// </summary>
public enum SubjectRole
{
    /// <summary>
    /// Faktor - w przypadku gdy na fakturze występują dane faktora
    /// </summary>
    [Description("Faktor")]
    [XmlEnum("1")]
    Factor = 1,

    /// <summary>
    /// Odbiorca - w przypadku gdy na fakturze występują dane jednostek wewnętrznych, oddziałów,
    /// wyodrębnionych w ramach nabywcy, które same nie stanowią nabywcy w rozumieniu ustawy
    /// </summary>
    [Description("Odbiorca - jednostka wewnętrzna nabywcy")]
    [XmlEnum("2")]
    Recipient = 2,

    /// <summary>
    /// Podmiot pierwotny - w przypadku gdy na fakturze występują dane podmiotu będącego w stosunku
    /// do podatnika podmiotem przejętym lub przekształconym, który dokonywał dostawy lub świadczył usługę
    /// </summary>
    [Description("Podmiot pierwotny (przejęty/przekształcony)")]
    [XmlEnum("3")]
    OriginalEntity = 3,

    /// <summary>
    /// Dodatkowy nabywca - w przypadku gdy na fakturze występują dane kolejnych
    /// (innych niż wymieniony w części Podmiot2) nabywców
    /// </summary>
    [Description("Dodatkowy nabywca")]
    [XmlEnum("4")]
    AdditionalBuyer = 4,

    /// <summary>
    /// Wystawca faktury - w przypadku gdy na fakturze występują dane podmiotu wystawiającego fakturę
    /// w imieniu podatnika. Nie dotyczy przypadku, gdy wystawcą faktury jest nabywca
    /// </summary>
    [Description("Wystawca faktury (w imieniu podatnika)")]
    [XmlEnum("5")]
    InvoiceIssuer = 5,

    /// <summary>
    /// Dokonujący płatności - w przypadku gdy na fakturze występują dane podmiotu regulującego
    /// zobowiązanie w miejsce nabywcy
    /// </summary>
    [Description("Dokonujący płatności (w miejsce nabywcy)")]
    [XmlEnum("6")]
    Payer = 6,

    /// <summary>
    /// Jednostka samorządu terytorialnego - wystawca
    /// </summary>
    [Description("Jednostka samorządu terytorialnego - wystawca")]
    [XmlEnum("7")]
    LocalGovernmentIssuer = 7,

    /// <summary>
    /// Jednostka samorządu terytorialnego - odbiorca
    /// </summary>
    [Description("Jednostka samorządu terytorialnego - odbiorca")]
    [XmlEnum("8")]
    LocalGovernmentRecipient = 8,

    /// <summary>
    /// Członek grupy VAT - wystawca
    /// </summary>
    [Description("Członek grupy VAT - wystawca")]
    [XmlEnum("9")]
    VatGroupMemberIssuer = 9,

    /// <summary>
    /// Członek grupy VAT - odbiorca
    /// </summary>
    [Description("Członek grupy VAT - odbiorca")]
    [XmlEnum("10")]
    VatGroupMemberRecipient = 10,

    /// <summary>
    /// Pracownik
    /// </summary>
    [Description("Pracownik")]
    [XmlEnum("11")]
    Employee = 11
}
