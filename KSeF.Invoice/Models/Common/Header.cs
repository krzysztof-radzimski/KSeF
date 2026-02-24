using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Nagłówek faktury (TNaglowek)
/// </summary>
public class InvoiceHeader
{
    /// <summary>
    /// Kod formularza - zawsze "FA"
    /// </summary>
    [XmlElement("KodFormularza")]
    public FormCodeElement FormCode { get; set; } = new FormCodeElement();

    /// <summary>
    /// Wariant formularza - zawsze 3 dla FA(3)
    /// </summary>
    [XmlElement("WariantFormularza")]
    public byte FormVariant { get; set; } = 3;

    /// <summary>
    /// Data i czas wytworzenia faktury
    /// Format: YYYY-MM-DDTHH:MM:SSZ (UTC)
    /// </summary>
    [XmlElement("DataWytworzeniaFa")]
    public DateTime CreationDateTime { get; set; }

    /// <summary>
    /// Nazwa systemu teleinformatycznego, z którego korzysta podatnik (opcjonalnie)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("SystemInfo")]
    public string? SystemInfo { get; set; }
}

/// <summary>
/// Element kodu formularza z atrybutami
/// </summary>
public class FormCodeElement
{
    /// <summary>
    /// Wartość kodu formularza
    /// </summary>
    [XmlText]
    public string Value { get; set; } = "FA";

    /// <summary>
    /// Kod systemowy formularza
    /// </summary>
    [XmlAttribute("kodSystemowy")]
    public string SystemCode { get; set; } = "FA (3)";

    /// <summary>
    /// Wersja schematu
    /// </summary>
    [XmlAttribute("wersjaSchemy")]
    public string SchemaVersion { get; set; } = "1-0E";
}
