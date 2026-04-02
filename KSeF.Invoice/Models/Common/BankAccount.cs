/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca rachunek bankowy na fakturze (TRachunekBankowy).
    Zawiera numer rachunku (IBAN lub krajowy), kod SWIFT, nazwę banku,
    typ rachunku własnego banku/SKOK oraz opcjonalny opis rachunku.
    Wykorzystywana w sekcji płatności faktury.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Informacje o rachunku bankowym (TRachunekBankowy)
/// </summary>
public class BankAccount
{
    /// <summary>
    /// Pełny numer rachunku bankowego (IBAN lub krajowy)
    /// Od 10 do 34 znaków
    /// </summary>
    [XmlElement("NrRB")]
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Kod SWIFT banku (opcjonalnie)
    /// Format: 8 lub 11 znaków alfanumerycznych
    /// </summary>
    [XmlElement("SWIFT")]
    public string? SwiftCode { get; set; }

    /// <summary>
    /// Typ rachunku własnego banku (opcjonalnie)
    /// Stosowane gdy rachunek jest rachunkiem własnym banku lub SKOK
    /// </summary>
    [XmlElement("RachunekWlasnyBanku")]
    public BankAccountType? BankOwnAccountType { get; set; }

    /// <summary>
    /// Nazwa banku (opcjonalnie)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("NazwaBanku")]
    public string? BankName { get; set; }

    /// <summary>
    /// Opis rachunku (opcjonalnie)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("OpisRachunku")]
    public string? AccountDescription { get; set; }
}
