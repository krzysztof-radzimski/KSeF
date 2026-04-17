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

public class CorrectedSellerData
{
    [XmlElement("PrefiksPodatnika", Order = 1)]
    public EUCountryCode? TaxpayerPrefix { get; set; }

    public bool ShouldSerializeTaxpayerPrefix() => TaxpayerPrefix.HasValue;

    [XmlElement("DaneIdentyfikacyjne", Order = 2)]
    public SellerIdentification IdentificationData { get; set; } = new();

    [XmlElement("Adres", Order = 3)]
    public Address Address { get; set; } = new();
}
