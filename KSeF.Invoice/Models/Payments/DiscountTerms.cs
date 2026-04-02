using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Informacja o skoncie (Skonto)
/// Zawiera warunki i wysokość skonta
/// </summary>
public class DiscountTerms
{
    /// <summary>
    /// Warunki skonta (WarunkiSkonta)
    /// Warunki, które nabywca powinien spełnić, aby skorzystać ze skonta.
    /// Maksymalnie 256 znaków.
    /// </summary>
    [XmlElement("WarunkiSkonta")]
    public string Conditions { get; set; } = string.Empty;

    /// <summary>
    /// Wysokość skonta (WysokoscSkonta)
    /// Maksymalnie 256 znaków.
    /// </summary>
    [XmlElement("WysokoscSkonta")]
    public string Amount { get; set; } = string.Empty;
}
