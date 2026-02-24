using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Okres, którego dotyczy faktura (OkresFa)
/// Używane gdy faktura dotyczy okresu rozliczeniowego, a nie konkretnej daty sprzedaży
/// </summary>
public class SalePeriod
{
    /// <summary>
    /// Data początkowa okresu rozliczeniowego
    /// Format: YYYY-MM-DD
    /// Wymagane pole elementu OkresFa
    /// </summary>
    [XmlElement("OkresOd")]
    public DateOnly PeriodFrom { get; set; }

    /// <summary>
    /// Data końcowa okresu rozliczeniowego
    /// Format: YYYY-MM-DD
    /// Wymagane pole elementu OkresFa
    /// </summary>
    [XmlElement("OkresDo")]
    public DateOnly PeriodTo { get; set; }
}
