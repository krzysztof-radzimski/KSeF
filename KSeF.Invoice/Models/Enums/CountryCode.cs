using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Kody krajów członkowskich Unii Europejskiej (TKodyKrajowUE)
/// </summary>
public enum EUCountryCode
{
    /// <summary>
    /// Austria
    /// </summary>
    [Description("Austria")]
    [XmlEnum("AT")]
    AT,

    /// <summary>
    /// Belgia
    /// </summary>
    [Description("Belgia")]
    [XmlEnum("BE")]
    BE,

    /// <summary>
    /// Bułgaria
    /// </summary>
    [Description("Bułgaria")]
    [XmlEnum("BG")]
    BG,

    /// <summary>
    /// Cypr
    /// </summary>
    [Description("Cypr")]
    [XmlEnum("CY")]
    CY,

    /// <summary>
    /// Czechy
    /// </summary>
    [Description("Czechy")]
    [XmlEnum("CZ")]
    CZ,

    /// <summary>
    /// Dania
    /// </summary>
    [Description("Dania")]
    [XmlEnum("DK")]
    DK,

    /// <summary>
    /// Estonia
    /// </summary>
    [Description("Estonia")]
    [XmlEnum("EE")]
    EE,

    /// <summary>
    /// Finlandia
    /// </summary>
    [Description("Finlandia")]
    [XmlEnum("FI")]
    FI,

    /// <summary>
    /// Francja
    /// </summary>
    [Description("Francja")]
    [XmlEnum("FR")]
    FR,

    /// <summary>
    /// Niemcy
    /// </summary>
    [Description("Niemcy")]
    [XmlEnum("DE")]
    DE,

    /// <summary>
    /// Grecja
    /// </summary>
    [Description("Grecja")]
    [XmlEnum("EL")]
    EL,

    /// <summary>
    /// Chorwacja
    /// </summary>
    [Description("Chorwacja")]
    [XmlEnum("HR")]
    HR,

    /// <summary>
    /// Węgry
    /// </summary>
    [Description("Węgry")]
    [XmlEnum("HU")]
    HU,

    /// <summary>
    /// Irlandia
    /// </summary>
    [Description("Irlandia")]
    [XmlEnum("IE")]
    IE,

    /// <summary>
    /// Włochy
    /// </summary>
    [Description("Włochy")]
    [XmlEnum("IT")]
    IT,

    /// <summary>
    /// Łotwa
    /// </summary>
    [Description("Łotwa")]
    [XmlEnum("LV")]
    LV,

    /// <summary>
    /// Litwa
    /// </summary>
    [Description("Litwa")]
    [XmlEnum("LT")]
    LT,

    /// <summary>
    /// Luksemburg
    /// </summary>
    [Description("Luksemburg")]
    [XmlEnum("LU")]
    LU,

    /// <summary>
    /// Malta
    /// </summary>
    [Description("Malta")]
    [XmlEnum("MT")]
    MT,

    /// <summary>
    /// Holandia
    /// </summary>
    [Description("Holandia")]
    [XmlEnum("NL")]
    NL,

    /// <summary>
    /// Polska
    /// </summary>
    [Description("Polska")]
    [XmlEnum("PL")]
    PL,

    /// <summary>
    /// Portugalia
    /// </summary>
    [Description("Portugalia")]
    [XmlEnum("PT")]
    PT,

    /// <summary>
    /// Rumunia
    /// </summary>
    [Description("Rumunia")]
    [XmlEnum("RO")]
    RO,

    /// <summary>
    /// Słowacja
    /// </summary>
    [Description("Słowacja")]
    [XmlEnum("SK")]
    SK,

    /// <summary>
    /// Słowenia
    /// </summary>
    [Description("Słowenia")]
    [XmlEnum("SI")]
    SI,

    /// <summary>
    /// Hiszpania
    /// </summary>
    [Description("Hiszpania")]
    [XmlEnum("ES")]
    ES,

    /// <summary>
    /// Szwecja
    /// </summary>
    [Description("Szwecja")]
    [XmlEnum("SE")]
    SE,

    /// <summary>
    /// Irlandia Północna
    /// </summary>
    [Description("Irlandia Północna")]
    [XmlEnum("XI")]
    XI
}
