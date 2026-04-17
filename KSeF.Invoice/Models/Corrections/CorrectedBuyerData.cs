/*=====================================================================================

    KSeF.Invoice
    Dane nabywcy z faktury korygowanej (Podmiot2K).
    Używane gdy korekcie podlegają dane nabywcy lub dodatkowego nabywcy —
    zawiera pełne dane tego podmiotu występujące na fakturze korygowanej.
    Lista (maxOccurs=101), bo faktura może mieć wielu nabywców (Podmiot2 + kilku Podmiot3).

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;

namespace KSeF.Invoice.Models.Corrections;

public class CorrectedBuyerData
{
    [XmlElement("DaneIdentyfikacyjne", Order = 1)]
    public BuyerIdentification IdentificationData { get; set; } = new();

    [XmlElement("Adres", Order = 2)]
    public Address? Address { get; set; }

    public bool ShouldSerializeAddress() => Address != null;

    [XmlElement("IDNabywcy", Order = 3)]
    public string? BuyerIdentifier { get; set; }

    public bool ShouldSerializeBuyerIdentifier() => !string.IsNullOrEmpty(BuyerIdentifier);
}
