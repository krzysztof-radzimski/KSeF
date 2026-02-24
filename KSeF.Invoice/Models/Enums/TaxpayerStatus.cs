using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Status podatnika (TStatusInfoPodatnika)
/// </summary>
public enum TaxpayerStatus
{
    /// <summary>
    /// Podatnik znajdujący się w stanie likwidacji
    /// </summary>
    [Description("Podatnik w stanie likwidacji")]
    [XmlEnum("1")]
    InLiquidation = 1,

    /// <summary>
    /// Podatnik, który jest w trakcie postępowania restrukturyzacyjnego
    /// </summary>
    [Description("Podatnik w trakcie postępowania restrukturyzacyjnego")]
    [XmlEnum("2")]
    InRestructuring = 2,

    /// <summary>
    /// Podatnik znajdujący się w stanie upadłości
    /// </summary>
    [Description("Podatnik w stanie upadłości")]
    [XmlEnum("3")]
    InBankruptcy = 3,

    /// <summary>
    /// Przedsiębiorstwo w spadku
    /// </summary>
    [Description("Przedsiębiorstwo w spadku")]
    [XmlEnum("4")]
    InheritedEnterprise = 4
}
