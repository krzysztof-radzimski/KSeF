/*=====================================================================================

    KSeF.Invoice
    Dane sprzedawcy z faktury korygowanej (Podmiot1K).
    Używane gdy korekcie podlegają dane sprzedawcy — zawiera pełne dane
    sprzedawcy występujące na fakturze korygowanej.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Corrections;

/// <summary>
/// Dane sprzedawcy z faktury korygowanej (Podmiot1K)
/// Zawiera dane sprzedawcy takie jak były na fakturze korygowanej
/// Zgodne ze schematem KSeF FA(3)
/// </summary>
public class CorrectedSellerData
{
    /// <summary>
    /// Prefiks podatnika (PrefiksPodatnika)
    /// Dwuznakowy kod kraju EU, np. PL, DE
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("PrefiksPodatnika", Order = 1)]
    public EUCountryCode? TaxpayerPrefix { get; set; }

    /// <summary>
    /// Określa czy serializować PrefiksPodatnika (tylko gdy ma wartość)
    /// </summary>
    public bool ShouldSerializeTaxpayerPrefix() => TaxpayerPrefix.HasValue;

    /// <summary>
    /// Dane identyfikacyjne sprzedawcy (DaneIdentyfikacyjne)
    /// Zawiera NIP i nazwę sprzedawcy typu TPodmiot1
    /// </summary>
    [XmlElement("DaneIdentyfikacyjne", Order = 2)]
    public SellerIdentification IdentificationData { get; set; } = new();

    /// <summary>
    /// Adres sprzedawcy (Adres)
    /// Zawiera kod kraju, adres wiersz 1 i wiersz 2
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("Adres", Order = 3)]
    public Address Address { get; set; } = new();
}
