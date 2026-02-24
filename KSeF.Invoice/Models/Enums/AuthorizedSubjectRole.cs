using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Rola podmiotu upoważnionego (TRolaPodmiotuUpowaznionego)
/// </summary>
public enum AuthorizedSubjectRole
{
    /// <summary>
    /// Organ egzekucyjny - w przypadku, o którym mowa w art. 106c pkt 1 ustawy
    /// </summary>
    [Description("Organ egzekucyjny")]
    [XmlEnum("1")]
    EnforcementAuthority = 1,

    /// <summary>
    /// Komornik sądowy - w przypadku, o którym mowa w art. 106c pkt 2 ustawy
    /// </summary>
    [Description("Komornik sądowy")]
    [XmlEnum("2")]
    Bailiff = 2,

    /// <summary>
    /// Przedstawiciel podatkowy - w przypadku gdy na fakturze występują dane przedstawiciela podatkowego,
    /// o którym mowa w art. 18a - 18d ustawy
    /// </summary>
    [Description("Przedstawiciel podatkowy")]
    [XmlEnum("3")]
    TaxRepresentative = 3
}
