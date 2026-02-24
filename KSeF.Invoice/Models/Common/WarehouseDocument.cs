using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Dokument magazynowy powiązany z fakturą (WZ)
/// Reprezentuje element listy dokumentów WZ (wydanie zewnętrzne)
/// </summary>
public class WarehouseDocument
{
    /// <summary>
    /// Numer dokumentu magazynowego WZ (wydanie zewnętrzne)
    /// Element tekstowy zawierający identyfikator dokumentu magazynowego
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlText]
    public string DocumentNumber { get; set; } = string.Empty;
}
