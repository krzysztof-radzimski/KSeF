/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca dane zapłaty częściowej (ZaplataCzesciowa).
    Zawiera kwotę zapłaty częściowej, datę zapłaty oraz opcjonalną
    formę płatności (standardową lub inną z opisem). Wykorzystywana
    w sekcji płatności faktury gdy nastąpiła zapłata w częściach.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Dane zapłaty częściowej (ZaplataCzesciowa)
/// Zawiera kwotę, datę i opcjonalnie formę płatności
/// </summary>
public class PartialPayment
{
    /// <summary>
    /// Kwota zapłaty częściowej (KwotaZaplatyCzesciowej)
    /// </summary>
    [XmlElement("KwotaZaplatyCzesciowej")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Data zapłaty częściowej (DataZaplatyCzesciowej) - proxy string
    /// </summary>
    [XmlElement("DataZaplatyCzesciowej")]
    public string? DateString
    {
        get => Date?.ToString("yyyy-MM-dd");
        set => Date = string.IsNullOrEmpty(value) ? null : DateOnly.Parse(value);
    }

    /// <summary>
    /// Data zapłaty częściowej
    /// </summary>
    [XmlIgnore]
    public DateOnly? Date { get; set; }

    #region Choice: Forma płatności (FormaPlatnosci lub PlatnoscInna+OpisPlatnosci)

    /// <summary>
    /// Forma płatności (FormaPlatnosci) - standardowa (wartość 1-7)
    /// </summary>
    [XmlIgnore]
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// Proxy string do serializacji XML FormaPlatnosci jako integer
    /// </summary>
    [XmlElement("FormaPlatnosci")]
    public string? PaymentMethodString
    {
        get => PaymentMethod.HasValue ? ((int)PaymentMethod.Value).ToString() : null;
        set => PaymentMethod = string.IsNullOrEmpty(value) ? null : (PaymentMethod)int.Parse(value);
    }

    /// <summary>
    /// Znacznik innej formy płatności (PlatnoscInna): 1 - inna forma płatności
    /// </summary>
    [XmlElement("PlatnoscInna")]
    public int? OtherPaymentMarker { get; set; }

    /// <summary>
    /// Opis innej formy płatności (OpisPlatnosci), max 256 znaków
    /// </summary>
    [XmlElement("OpisPlatnosci")]
    public string? OtherPaymentDescription { get; set; }

    #endregion

    #region ShouldSerialize

    public bool ShouldSerializeDateString() => Date.HasValue;

    public bool ShouldSerializePaymentMethodString() => PaymentMethod.HasValue;

    public bool ShouldSerializeOtherPaymentMarker() => OtherPaymentMarker.HasValue;

    public bool ShouldSerializeOtherPaymentDescription() => !string.IsNullOrEmpty(OtherPaymentDescription);

    #endregion
}
