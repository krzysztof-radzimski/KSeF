/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca typ złożony klucz-wartość (TKluczWartosc).
    Wykorzystywana do przekazywania dodatkowych informacji na fakturze
    w postaci par klucz-wartość. Może być powiązana z konkretną
    pozycją faktury poprzez numer wiersza.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
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
