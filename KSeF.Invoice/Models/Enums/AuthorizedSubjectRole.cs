/*=====================================================================================

    KSeF.Invoice
    Enum definiujący role podmiotów upoważnionych
    (TRolaPodmiotuUpowaznionego) na fakturze w systemie KSeF.
    Zawiera role: organ egzekucyjny (art. 106c pkt 1), komornik
    sądowy (art. 106c pkt 2) oraz przedstawiciel podatkowy
    (art. 18a-18d ustawy o VAT).

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Rola podmiotu upoważnionego (TRolaPodmiotuUpowaznionego)
/// </summary>
public enum AuthorizedSubjectRole
{
    /// <summary>
    /// Organ egzekucyjny - w przypadku, o którym mowa w art. 106c pkt 1 ustawy
    /// </summary>
    [Description("Organ egzekucyjny")]
    [XmlEnum("1")]
    EnforcementAuthority = 1,

    /// <summary>
    /// Komornik sądowy - w przypadku, o którym mowa w art. 106c pkt 2 ustawy
    /// </summary>
    [Description("Komornik sądowy")]
    [XmlEnum("2")]
    Bailiff = 2,

    /// <summary>
    /// Przedstawiciel podatkowy - w przypadku gdy na fakturze występują dane przedstawiciela podatkowego,
    /// o którym mowa w art. 18a - 18d ustawy
    /// </summary>
    [Description("Przedstawiciel podatkowy")]
    [XmlEnum("3")]
    TaxRepresentative = 3
}
