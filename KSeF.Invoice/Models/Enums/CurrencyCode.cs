using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Słownik kodów walut (TKodWaluty) - najczęściej używane waluty
/// Pełna lista zawiera ponad 150 walut zgodnie ze schematem XSD
/// </summary>
public enum CurrencyCode
{
    /// <summary>
    /// Polski złoty
    /// </summary>
    [Description("Polski złoty")]
    [XmlEnum("PLN")]
    PLN,

    /// <summary>
    /// Euro
    /// </summary>
    [Description("Euro")]
    [XmlEnum("EUR")]
    EUR,

    /// <summary>
    /// Dolar amerykański
    /// </summary>
    [Description("Dolar amerykański")]
    [XmlEnum("USD")]
    USD,

    /// <summary>
    /// Funt szterling
    /// </summary>
    [Description("Funt szterling")]
    [XmlEnum("GBP")]
    GBP,

    /// <summary>
    /// Frank szwajcarski
    /// </summary>
    [Description("Frank szwajcarski")]
    [XmlEnum("CHF")]
    CHF,

    /// <summary>
    /// Korona czeska
    /// </summary>
    [Description("Korona czeska")]
    [XmlEnum("CZK")]
    CZK,

    /// <summary>
    /// Korona duńska
    /// </summary>
    [Description("Korona duńska")]
    [XmlEnum("DKK")]
    DKK,

    /// <summary>
    /// Korona szwedzka
    /// </summary>
    [Description("Korona szwedzka")]
    [XmlEnum("SEK")]
    SEK,

    /// <summary>
    /// Korona norweska
    /// </summary>
    [Description("Korona norweska")]
    [XmlEnum("NOK")]
    NOK,

    /// <summary>
    /// Forint węgierski
    /// </summary>
    [Description("Forint węgierski")]
    [XmlEnum("HUF")]
    HUF,

    /// <summary>
    /// Lew bułgarski
    /// </summary>
    [Description("Lew bułgarski")]
    [XmlEnum("BGN")]
    BGN,

    /// <summary>
    /// Lej rumuński
    /// </summary>
    [Description("Lej rumuński")]
    [XmlEnum("RON")]
    RON,

    /// <summary>
    /// Hrywna ukraińska
    /// </summary>
    [Description("Hrywna ukraińska")]
    [XmlEnum("UAH")]
    UAH,

    /// <summary>
    /// Rubel rosyjski
    /// </summary>
    [Description("Rubel rosyjski")]
    [XmlEnum("RUB")]
    RUB,

    /// <summary>
    /// Jen japoński
    /// </summary>
    [Description("Jen japoński")]
    [XmlEnum("JPY")]
    JPY,

    /// <summary>
    /// Yuan renminbi chiński
    /// </summary>
    [Description("Yuan renminbi chiński")]
    [XmlEnum("CNY")]
    CNY,

    /// <summary>
    /// Dolar kanadyjski
    /// </summary>
    [Description("Dolar kanadyjski")]
    [XmlEnum("CAD")]
    CAD,

    /// <summary>
    /// Dolar australijski
    /// </summary>
    [Description("Dolar australijski")]
    [XmlEnum("AUD")]
    AUD,

    /// <summary>
    /// Dolar nowozelandzki
    /// </summary>
    [Description("Dolar nowozelandzki")]
    [XmlEnum("NZD")]
    NZD,

    /// <summary>
    /// Lira turecka
    /// </summary>
    [Description("Lira turecka")]
    [XmlEnum("TRY")]
    TRY,

    /// <summary>
    /// Real brazylijski
    /// </summary>
    [Description("Real brazylijski")]
    [XmlEnum("BRL")]
    BRL,

    /// <summary>
    /// Peso meksykańskie
    /// </summary>
    [Description("Peso meksykańskie")]
    [XmlEnum("MXN")]
    MXN,

    /// <summary>
    /// Rand południowoafrykański
    /// </summary>
    [Description("Rand południowoafrykański")]
    [XmlEnum("ZAR")]
    ZAR,

    /// <summary>
    /// Won południowokoreański
    /// </summary>
    [Description("Won południowokoreański")]
    [XmlEnum("KRW")]
    KRW,

    /// <summary>
    /// Rupia indyjska
    /// </summary>
    [Description("Rupia indyjska")]
    [XmlEnum("INR")]
    INR,

    /// <summary>
    /// Szekel izraelski
    /// </summary>
    [Description("Szekel izraelski")]
    [XmlEnum("ILS")]
    ILS,

    /// <summary>
    /// Dolar Singapuru
    /// </summary>
    [Description("Dolar Singapuru")]
    [XmlEnum("SGD")]
    SGD,

    /// <summary>
    /// Dolar Hongkongu
    /// </summary>
    [Description("Dolar Hongkongu")]
    [XmlEnum("HKD")]
    HKD,

    /// <summary>
    /// Dirham ZEA
    /// </summary>
    [Description("Dirham ZEA")]
    [XmlEnum("AED")]
    AED,

    /// <summary>
    /// Rial saudyjski
    /// </summary>
    [Description("Rial saudyjski")]
    [XmlEnum("SAR")]
    SAR
}
