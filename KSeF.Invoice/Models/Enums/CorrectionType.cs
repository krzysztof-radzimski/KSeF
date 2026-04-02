/*=====================================================================================

    KSeF.Invoice
    Enum definiujący typ skutku korekty w ewidencji dla podatku
    od towarów i usług (TTypKorekty). Określa, w jakiej dacie
    korekta wywołuje skutek: w dacie ujęcia faktury pierwotnej,
    w dacie wystawienia faktury korygującej lub w innej dacie.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Typ skutku korekty w ewidencji dla podatku od towarów i usług (TTypKorekty)
/// </summary>
public enum CorrectionType
{
    /// <summary>
    /// Korekta skutkująca w dacie ujęcia faktury pierwotnej
    /// </summary>
    [Description("Korekta skutkująca w dacie ujęcia faktury pierwotnej")]
    [XmlEnum("1")]
    OriginalInvoiceDate = 1,

    /// <summary>
    /// Korekta skutkująca w dacie wystawienia faktury korygującej
    /// </summary>
    [Description("Korekta skutkująca w dacie wystawienia faktury korygującej")]
    [XmlEnum("2")]
    CorrectionInvoiceDate = 2,

    /// <summary>
    /// Korekta skutkująca w dacie innej, w tym gdy dla różnych pozycji faktury korygującej daty te są różne
    /// </summary>
    [Description("Korekta skutkująca w innej dacie")]
    [XmlEnum("3")]
    OtherDate = 3
}
