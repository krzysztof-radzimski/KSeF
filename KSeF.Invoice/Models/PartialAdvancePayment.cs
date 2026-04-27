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

/// <summary>
/// Dane wcześniejszej zaliczki częściowej (ZaliczkaCzesciowa)
/// Element dla faktur dokumentujących otrzymanie więcej niż jednej płatności,
/// o której mowa w art. 106b ust. 1 pkt 4 ustawy o VAT.
/// Maksymalnie 31 wpisów w elemencie InvoiceData.
/// </summary>
[XmlType("ZaliczkaCzesciowa")]
public class PartialAdvancePayment
{
    /// <summary>
    /// Data otrzymania zaliczki częściowej w formacie ISO (P_6Z)
    /// Właściwość pomocnicza do serializacji XML (używa PaymentDate)
    /// </summary>
    [XmlElement("P_6Z", Order = 1)]
    public string PaymentDateString
    {
        get => PaymentDate.ToString("yyyy-MM-dd");
        set => PaymentDate = string.IsNullOrEmpty(value) ? default : DateOnly.Parse(value);
    }

    /// <summary>
    /// Data otrzymania zaliczki częściowej (pole P_6Z w schemacie FA(3))
    /// </summary>
    [XmlIgnore]
    public DateOnly PaymentDate { get; set; }

    /// <summary>
    /// Kwota zaliczki brutto (P_15Z)
    /// Wartość całkowita (brutto) wcześniejszej zaliczki częściowej
    /// </summary>
    [XmlElement("P_15Z", Order = 2)]
    public decimal Amount { get; set; }

    /// <summary>
    /// Kurs waluty dla zaliczki (KursWalutyZW)
    /// Opcjonalny kurs waluty stosowany przy przeliczeniu zaliczki
    /// </summary>
    [XmlElement("KursWalutyZW", Order = 3)]
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Kontroluje serializację kursu waluty (tylko gdy jest ustawiony)
    /// </summary>
    public bool ShouldSerializeExchangeRate() => ExchangeRate.HasValue;
}
