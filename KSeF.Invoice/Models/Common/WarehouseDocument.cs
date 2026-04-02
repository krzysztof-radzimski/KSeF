/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca dokument magazynowy WZ (wydanie zewnętrzne)
    powiązany z fakturą. Zawiera numer dokumentu magazynowego,
    który potwierdza wydanie towaru z magazynu.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
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
