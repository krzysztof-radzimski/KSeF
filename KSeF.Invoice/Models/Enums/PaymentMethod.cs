/*=====================================================================================

    KSeF.Invoice
    Enum definiujący formy płatności (TFormaPlatnosci) dostępne
    na fakturach w systemie KSeF. Zawiera metody: gotówka, karta
    płatnicza, bon, czek, kredyt, przelew bankowy oraz płatność
    mobilna.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Forma płatności (TFormaPlatnosci)
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Gotówka
    /// </summary>
    [Description("Gotówka")]
    [XmlEnum("1")]
    Cash = 1,

    /// <summary>
    /// Karta płatnicza
    /// </summary>
    [Description("Karta płatnicza")]
    [XmlEnum("2")]
    Card = 2,

    /// <summary>
    /// Bon
    /// </summary>
    [Description("Bon")]
    [XmlEnum("3")]
    Voucher = 3,

    /// <summary>
    /// Czek
    /// </summary>
    [Description("Czek")]
    [XmlEnum("4")]
    Check = 4,

    /// <summary>
    /// Kredyt
    /// </summary>
    [Description("Kredyt")]
    [XmlEnum("5")]
    Credit = 5,

    /// <summary>
    /// Przelew bankowy
    /// </summary>
    [Description("Przelew bankowy")]
    [XmlEnum("6")]
    BankTransfer = 6,

    /// <summary>
    /// Płatność mobilna
    /// </summary>
    [Description("Płatność mobilna")]
    [XmlEnum("7")]
    MobilePayment = 7
}
