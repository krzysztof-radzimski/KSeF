/*=====================================================================================

    KSeF.Invoice
    Enum definiujący symbol wzoru formularza (TKodFormularza)
    w systemie KSeF. Obecnie zawiera jedną wartość FA oznaczającą
    fakturę VAT, zgodnie ze schematem FA(3) Krajowego Systemu
    e-Faktur.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

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
