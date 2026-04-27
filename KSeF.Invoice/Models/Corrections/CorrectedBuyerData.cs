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

/// <summary>
/// Dane nabywcy z faktury korygowanej (Podmiot2K)
/// Element listy z danymi identyfikacyjnymi, adresem i identyfikatorem nabywcy
/// Maksymalnie 101 wpisów na fakturę korygującą
/// </summary>
public class CorrectedBuyerData
{
    /// <summary>
    /// Dane identyfikacyjne nabywcy (DaneIdentyfikacyjne)
    /// Identyfikacja przez NIP, VAT UE, identyfikator zagraniczny lub brak ID
    /// Typ: TPodmiot2
    /// </summary>
    [XmlElement("DaneIdentyfikacyjne", Order = 1)]
    public BuyerIdentification IdentificationData { get; set; } = new();

    /// <summary>
    /// Adres nabywcy (Adres)
    /// Opcjonalny
    /// Typ: TAdres
    /// </summary>
    [XmlElement("Adres", Order = 2)]
    public Address? Address { get; set; }

    /// <summary>
    /// Kontroluje serializację właściwości Address
    /// </summary>
    public bool ShouldSerializeAddress() => Address != null;

    /// <summary>
    /// Identyfikator nabywcy (IDNabywcy)
    /// Opcjonalny, maksymalnie 32 znaki
    /// </summary>
    [XmlElement("IDNabywcy", Order = 3)]
    public string? BuyerIdentifier { get; set; }

    /// <summary>
    /// Kontroluje serializację właściwości BuyerIdentifier
    /// </summary>
    public bool ShouldSerializeBuyerIdentifier() => !string.IsNullOrEmpty(BuyerIdentifier);
}
