/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca opis terminu płatności (TerminOpis).
    Opisuje termin płatności w formie: ilość jednostek czasu
    od zdarzenia początkowego, np. "14 dni od daty wystawienia
    faktury". Zawiera pola: Ilość, Jednostka, ZdarzeniePoczatkowe.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Opis terminu płatności (TerminOpis)
/// Complex type z XSD: Ilosc + Jednostka + ZdarzeniePoczatkowe
/// Np. "14 dni od daty wystawienia faktury"
/// </summary>
public class PaymentTermDescription
{
    /// <summary>
    /// Ilość jednostek (Ilosc)
    /// Np. 14 (dla "14 dni")
    /// </summary>
    [XmlElement("Ilosc")]
    public int Quantity { get; set; }

    /// <summary>
    /// Jednostka czasu (Jednostka)
    /// Np. "dni", "miesięcy". Maksymalnie 50 znaków.
    /// </summary>
    [XmlElement("Jednostka")]
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Zdarzenie początkowe (ZdarzeniePoczatkowe)
    /// Np. "od daty wystawienia faktury". Maksymalnie 256 znaków.
    /// </summary>
    [XmlElement("ZdarzeniePoczatkowe")]
    public string StartingEvent { get; set; } = string.Empty;
}
