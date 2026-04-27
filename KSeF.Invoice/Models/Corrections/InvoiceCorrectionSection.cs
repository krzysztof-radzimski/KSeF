/*=====================================================================================

    KSeF.Invoice
    Value object grupujący wszystkie pola sekcji korekty faktury.
    Nie serializuje się bezpośrednio do XML — pola są eksponowane
    jako proxy properties w InvoiceData z atrybutami [XmlElement].

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Corrections;

/// <summary>
/// Value object grupujący wszystkie pola sekcji korekty faktury
/// Nie jest serializowany bezpośrednio do XML - pola eksponowane jako proxy properties w InvoiceData
/// </summary>
public class InvoiceCorrectionSection
{
    /// <summary>
    /// Przyczyna korekty (P_15) - opis słowny
    /// Element FA(3) "P_15", maksymalnie 3500 znaków
    /// </summary>
    public string? CorrectionReason { get; set; }

    /// <summary>
    /// Typ korekty (TypKorekty) - kod liczbowy 1-3
    /// Element FA(3) "TypKorekty"
    /// 1 = korekta rozliczeniowa, 2 = korekta treści, 3 = zmiana formy rozliczeń
    /// </summary>
    public int? CorrectionType { get; set; }

    /// <summary>
    /// Dane faktury korygowanej (DaneFaKorygowanej)
    /// Element FA(3) "DaneFaKorygowanej" - zawiera numer i datę faktury pierwotnej
    /// </summary>
    public CorrectedInvoiceData? CorrectedInvoiceData { get; set; }

    /// <summary>
    /// Okres faktury korygowanej (OkresFaKorygowanej)
    /// Element FA(3) "OkresFaKorygowanej" - data od/do okresu sprzedaży faktury pierwotnej
    /// </summary>
    public SalePeriod? CorrectedInvoicePeriod { get; set; }

    /// <summary>
    /// Numer faktury korygowanej zmieniony (NrFaKorygowany)
    /// Element FA(3) "NrFaKorygowany" - numer faktury pierwotnej po zmianie numeracji
    /// Maksymalnie 256 znaków
    /// </summary>
    public string? CorrectedInvoiceNumberAmended { get; set; }

    /// <summary>
    /// Kwota przed korektą (P_15ZK)
    /// Element FA(3) "P_15ZK" - kwota brutto z faktury pierwotnej
    /// </summary>
    public decimal? AmountBeforeCorrection { get; set; }

    /// <summary>
    /// Kurs waluty przed korektą (KursWalutyZK)
    /// Element FA(3) "KursWalutyZK" - kurs stosowany w fakturze pierwotnej
    /// </summary>
    public decimal? ExchangeRateBeforeCorrection { get; set; }

    /// <summary>
    /// Dane sprzedawcy z faktury korygowanej (Podmiot1K)
    /// Element FA(3) "Podmiot1K" - dane sprzedawcy z faktury pierwotnej
    /// </summary>
    public CorrectedSellerData? CorrectedSeller { get; set; }

    /// <summary>
    /// Dane nabywców z faktury korygowanej (Podmiot2K)
    /// Element FA(3) "Podmiot2K" - lista nabywców z faktury pierwotnej, maksymalnie 101 podmiotów
    /// </summary>
    public List<CorrectedBuyerData>? CorrectedBuyers { get; set; }

    /// <summary>
    /// Sprawdza czy sekcja korekty zawiera jakiekolwiek dane
    /// </summary>
    public bool HasAnyData =>
        !string.IsNullOrEmpty(CorrectionReason) ||
        CorrectionType.HasValue ||
        CorrectedInvoiceData != null ||
        CorrectedInvoicePeriod != null ||
        !string.IsNullOrEmpty(CorrectedInvoiceNumberAmended) ||
        AmountBeforeCorrection.HasValue ||
        ExchangeRateBeforeCorrection.HasValue ||
        CorrectedSeller != null ||
        (CorrectedBuyers != null && CorrectedBuyers.Count > 0);
}
