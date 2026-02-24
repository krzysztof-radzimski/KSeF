using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Rodzaj transportu (TRodzajTransportu)
/// </summary>
public enum TransportType
{
    /// <summary>
    /// Transport morski
    /// </summary>
    [Description("Transport morski")]
    [XmlEnum("1")]
    Sea = 1,

    /// <summary>
    /// Transport kolejowy
    /// </summary>
    [Description("Transport kolejowy")]
    [XmlEnum("2")]
    Rail = 2,

    /// <summary>
    /// Transport drogowy
    /// </summary>
    [Description("Transport drogowy")]
    [XmlEnum("3")]
    Road = 3,

    /// <summary>
    /// Transport lotniczy
    /// </summary>
    [Description("Transport lotniczy")]
    [XmlEnum("4")]
    Air = 4,

    /// <summary>
    /// Przesyłka pocztowa
    /// </summary>
    [Description("Przesyłka pocztowa")]
    [XmlEnum("5")]
    Post = 5,

    /// <summary>
    /// Stałe instalacje przesyłowe (np. rurociągi)
    /// </summary>
    [Description("Stałe instalacje przesyłowe")]
    [XmlEnum("7")]
    Pipeline = 7,

    /// <summary>
    /// Żegluga śródlądowa
    /// </summary>
    [Description("Żegluga śródlądowa")]
    [XmlEnum("8")]
    InlandWaterway = 8
}
