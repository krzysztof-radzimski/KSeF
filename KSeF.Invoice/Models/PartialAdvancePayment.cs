/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca dane wcześniejszej zaliczki częściowej
    (ZaliczkaCzesciowa) zgodnie ze schematem FA(3).
    Dla faktur dokumentujących otrzymanie więcej niż jednej płatności,
    o której mowa w art. 106b ust. 1 pkt 4 ustawy o VAT.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;

namespace KSeF.Invoice.Models;

[XmlType("ZaliczkaCzesciowa")]
public class PartialAdvancePayment
{
    [XmlElement("P_6Z", Order = 1)]
    public string PaymentDateString
    {
        get => PaymentDate.ToString("yyyy-MM-dd");
        set => PaymentDate = string.IsNullOrEmpty(value) ? default : DateOnly.Parse(value);
    }

    [XmlIgnore]
    public DateOnly PaymentDate { get; set; }

    [XmlElement("P_15Z", Order = 2)]
    public decimal Amount { get; set; }

    [XmlElement("KursWalutyZW", Order = 3)]
    public decimal? ExchangeRate { get; set; }

    public bool ShouldSerializeExchangeRate() => ExchangeRate.HasValue;
}
