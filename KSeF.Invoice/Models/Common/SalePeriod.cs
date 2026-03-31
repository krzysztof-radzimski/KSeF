using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Okres, którego dotyczy faktura (OkresFa)
/// Używane gdy faktura dotyczy okresu rozliczeniowego, a nie konkretnej daty sprzedaży
/// </summary>
public class SalePeriod
{
    /// <summary>
    /// Data początkowa okresu rozliczeniowego - proxy string dla serializacji XML
    /// </summary>
    [XmlElement("OkresOd")]
    public string PeriodFromString
    {
        get => PeriodFrom.ToString("yyyy-MM-dd");
        set => PeriodFrom = string.IsNullOrEmpty(value) ? default : DateOnly.Parse(value);
    }

    /// <summary>
    /// Data początkowa okresu rozliczeniowego
    /// </summary>
    [XmlIgnore]
    public DateOnly PeriodFrom { get; set; }

    /// <summary>
    /// Data końcowa okresu rozliczeniowego - proxy string dla serializacji XML
    /// </summary>
    [XmlElement("OkresDo")]
    public string PeriodToString
    {
        get => PeriodTo.ToString("yyyy-MM-dd");
        set => PeriodTo = string.IsNullOrEmpty(value) ? default : DateOnly.Parse(value);
    }

    /// <summary>
    /// Data końcowa okresu rozliczeniowego
    /// </summary>
    [XmlIgnore]
    public DateOnly PeriodTo { get; set; }
}
