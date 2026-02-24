using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Stawka podatku VAT (TStawkaPodatku)
/// </summary>
public enum VatRate
{
    /// <summary>
    /// Stawka 23%
    /// </summary>
    [Description("Stawka 23%")]
    [XmlEnum("23")]
    Rate23,

    /// <summary>
    /// Stawka 22%
    /// </summary>
    [Description("Stawka 22%")]
    [XmlEnum("22")]
    Rate22,

    /// <summary>
    /// Stawka 8%
    /// </summary>
    [Description("Stawka 8%")]
    [XmlEnum("8")]
    Rate8,

    /// <summary>
    /// Stawka 7%
    /// </summary>
    [Description("Stawka 7%")]
    [XmlEnum("7")]
    Rate7,

    /// <summary>
    /// Stawka 5%
    /// </summary>
    [Description("Stawka 5%")]
    [XmlEnum("5")]
    Rate5,

    /// <summary>
    /// Stawka 4%
    /// </summary>
    [Description("Stawka 4%")]
    [XmlEnum("4")]
    Rate4,

    /// <summary>
    /// Stawka 3%
    /// </summary>
    [Description("Stawka 3%")]
    [XmlEnum("3")]
    Rate3,

    /// <summary>
    /// Stawka 0% w przypadku sprzedaży towarów i świadczenia usług na terytorium kraju
    /// (z wyłączeniem WDT i eksportu)
    /// </summary>
    [Description("Stawka 0% - sprzedaż krajowa")]
    [XmlEnum("0 KR")]
    Rate0Domestic,

    /// <summary>
    /// Stawka 0% w przypadku wewnątrzwspólnotowej dostawy towarów (WDT)
    /// </summary>
    [Description("Stawka 0% - WDT (wewnątrzwspólnotowa dostawa towarów)")]
    [XmlEnum("0 WDT")]
    Rate0IntraCommunitySupply,

    /// <summary>
    /// Stawka 0% w przypadku eksportu towarów
    /// </summary>
    [Description("Stawka 0% - eksport towarów")]
    [XmlEnum("0 EX")]
    Rate0Export,

    /// <summary>
    /// Zwolnione od podatku
    /// </summary>
    [Description("Zwolnione od podatku")]
    [XmlEnum("zw")]
    Exempt,

    /// <summary>
    /// Odwrotne obciążenie
    /// </summary>
    [Description("Odwrotne obciążenie")]
    [XmlEnum("oo")]
    ReverseCharge,

    /// <summary>
    /// Niepodlegające opodatkowaniu - dostawy towarów oraz świadczenia usług poza terytorium kraju,
    /// z wyłączeniem transakcji, o których mowa w art. 100 ust. 1 pkt 4 ustawy oraz OSS
    /// </summary>
    [Description("Niepodlegające opodatkowaniu - dostawy poza terytorium kraju (typ I)")]
    [XmlEnum("np I")]
    NotSubjectToTaxI,

    /// <summary>
    /// Niepodlegające opodatkowaniu na terytorium kraju, świadczenie usług o których mowa
    /// w art. 100 ust. 1 pkt 4 ustawy
    /// </summary>
    [Description("Niepodlegające opodatkowaniu - świadczenie usług art. 100 ust. 1 pkt 4 (typ II)")]
    [XmlEnum("np II")]
    NotSubjectToTaxII
}
