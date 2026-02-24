using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Typ złożony klucz-wartość (TKluczWartosc)
/// Wykorzystywany do przekazywania dodatkowych informacji na fakturze
/// </summary>
public class KeyValue
{
    /// <summary>
    /// Numer wiersza podany w polu NrWierszaFa lub NrWierszaZam,
    /// jeśli informacja odnosi się wyłącznie do danej pozycji faktury (opcjonalnie)
    /// </summary>
    [XmlElement("NrWiersza")]
    public int? RowNumber { get; set; }

    /// <summary>
    /// Klucz (nazwa parametru)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("Klucz")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Wartość parametru
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("Wartosc")]
    public string Value { get; set; } = string.Empty;
}
