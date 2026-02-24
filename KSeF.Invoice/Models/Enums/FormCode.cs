using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Symbol wzoru formularza (TKodFormularza)
/// </summary>
public enum FormCode
{
    /// <summary>
    /// Faktura VAT
    /// </summary>
    [Description("Faktura VAT")]
    [XmlEnum("FA")]
    FA
}
