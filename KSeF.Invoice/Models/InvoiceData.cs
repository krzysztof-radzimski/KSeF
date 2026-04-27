/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca dane merytoryczne faktury (Fa) zgodnie
    z art. 106a-106q ustawy o VAT. Zawiera wszystkie dane transakcji:
    walutę, datę wystawienia, numer faktury, podsumowania podatkowe
    w podziale na stawki VAT, adnotacje, rodzaj faktury, dane korekt,
    dane zaliczek, pozycje faktury, płatności, warunki transakcji
    oraz dodatkowe opisy.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Payments;
using KSeF.Invoice.Models.Summary;

namespace KSeF.Invoice.Models;

/// <summary>
/// Dane merytoryczne faktury (Fa)
/// Zawiera wszystkie dane dotyczące transakcji zgodnie z art. 106a - 106q ustawy o VAT
/// Pola dotyczące wartości wypełniane są w walucie faktury
/// </summary>
[XmlType("Fa")]
public class InvoiceData
{
    #region Podstawowe dane faktury

    /// <summary>
    /// Kod waluty, w której wystawiona jest faktura (KodWaluty)
    /// Domyślnie PLN - polski złoty
    /// Zgodny z normą ISO 4217
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("KodWaluty", Order = 0)]
    public CurrencyCode CurrencyCode { get; set; } = CurrencyCode.PLN;

    /// <summary>
    /// Data wystawienia faktury (P_1) - proxy string dla serializacji XML
    /// Format: YYYY-MM-DD
    /// </summary>
    [XmlElement("P_1", Order = 1)]
    public string IssueDateString
    {
        get => IssueDate.ToString("yyyy-MM-dd");
        set => IssueDate = string.IsNullOrEmpty(value) ? default : DateOnly.Parse(value);
    }

    /// <summary>
    /// Data wystawienia faktury (P_1)
    /// Pole obowiązkowe zgodnie z art. 106e ust. 1 pkt 1 ustawy o VAT
    /// </summary>
    [XmlIgnore]
    public DateOnly IssueDate { get; set; }

    /// <summary>
    /// Miejsce wystawienia faktury (P_1M)
    /// Pole opcjonalne - miejscowość, w której wystawiono fakturę
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("P_1M", Order = 2)]
    public string? IssuePlace { get; set; }

    /// <summary>
    /// Numer faktury (P_2)
    /// Kolejny numer nadany w ramach jednej lub więcej serii, który w sposób
    /// jednoznaczny identyfikuje fakturę
    /// Pole obowiązkowe zgodnie z art. 106e ust. 1 pkt 2 ustawy o VAT
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("P_2", Order = 3)]
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Lista numerów dokumentów magazynowych WZ (WZ)
    /// Numery dokumentów potwierdzających wydanie towarów z magazynu
    /// Element opcjonalny - może zawierać do 1000 dokumentów WZ
    /// </summary>
    [XmlElement("WZ", Order = 4)]
    public List<string>? WarehouseDocuments { get; set; }

    /// <summary>
    /// Data sprzedaży (P_6) - proxy string dla serializacji XML
    /// Format: YYYY-MM-DD
    /// </summary>
    [XmlElement("P_6", Order = 5)]
    public string? SaleDateString
    {
        get => SaleDate?.ToString("yyyy-MM-dd");
        set => SaleDate = string.IsNullOrEmpty(value) ? null : DateOnly.Parse(value);
    }

    /// <summary>
    /// Data dokonania lub zakończenia dostawy towarów lub wykonania usługi (P_6)
    /// Pole opcjonalne zgodnie z art. 106e ust. 1 pkt 6 ustawy o VAT
    /// Alternatywa dla OkresFa
    /// </summary>
    [XmlIgnore]
    public DateOnly? SaleDate { get; set; }

    /// <summary>
    /// Okres rozliczeniowy, którego dotyczy faktura (OkresFa)
    /// Używane zamiast konkretnej daty sprzedaży gdy faktura dotyczy
    /// świadczeń ciągłych lub okresowych (np. najem, media, abonamenty)
    /// Element alternatywny dla P_6
    /// </summary>
    [XmlElement("OkresFa", Order = 6)]
    public SalePeriod? SalePeriod { get; set; }

    #endregion

    #region Podsumowania podatkowe (P_13_*, P_14_*, P_15)

    /// <summary>
    /// Suma wartości sprzedaży netto ze stawką 23% (P_13_1)
    /// </summary>
    [XmlElement("P_13_1", Order = 7)]
    public decimal? NetAmount23 { get; set; }

    /// <summary>
    /// Kwota podatku VAT od wartości netto ze stawką 23% (P_14_1)
    /// </summary>
    [XmlElement("P_14_1", Order = 8)]
    public decimal? VatAmount23 { get; set; }

    /// <summary>
    /// Kwota podatku VAT od wartości netto ze stawką 23% przeliczona na PLN (P_14_1W)
    /// Dla faktur w walucie obcej - przeliczenie zgodnie z przepisami
    /// </summary>
    [XmlElement("P_14_1W", Order = 9)]
    public decimal? VatAmount23InPLN { get; set; }

    /// <summary>
    /// Suma wartości sprzedaży netto ze stawką 8% (P_13_2)
    /// </summary>
    [XmlElement("P_13_2", Order = 10)]
    public decimal? NetAmount8 { get; set; }

    /// <summary>
    /// Kwota podatku VAT od wartości netto ze stawką 8% (P_14_2)
    /// </summary>
    [XmlElement("P_14_2", Order = 11)]
    public decimal? VatAmount8 { get; set; }

    /// <summary>
    /// Kwota podatku VAT od wartości netto ze stawką 8% przeliczona na PLN (P_14_2W)
    /// </summary>
    [XmlElement("P_14_2W", Order = 12)]
    public decimal? VatAmount8InPLN { get; set; }

    /// <summary>
    /// Suma wartości sprzedaży netto ze stawką 5% (P_13_3)
    /// </summary>
    [XmlElement("P_13_3", Order = 13)]
    public decimal? NetAmount5 { get; set; }

    /// <summary>
    /// Kwota podatku VAT od wartości netto ze stawką 5% (P_14_3)
    /// </summary>
    [XmlElement("P_14_3", Order = 14)]
    public decimal? VatAmount5 { get; set; }

    /// <summary>
    /// Kwota podatku VAT od wartości netto ze stawką 5% przeliczona na PLN (P_14_3W)
    /// </summary>
    [XmlElement("P_14_3W", Order = 15)]
    public decimal? VatAmount5InPLN { get; set; }

    /// <summary>
    /// Suma wartości sprzedaży netto - ryczałt dla taksówek (P_13_4)
    /// </summary>
    [XmlElement("P_13_4", Order = 16)]
    public decimal? NetAmountTaxi { get; set; }

    /// <summary>
    /// Kwota podatku VAT - ryczałt dla taksówek (P_14_4)
    /// </summary>
    [XmlElement("P_14_4", Order = 17)]
    public decimal? VatAmountTaxi { get; set; }

    /// <summary>
    /// Kwota podatku VAT - ryczałt dla taksówek przeliczona na PLN (P_14_4W)
    /// </summary>
    [XmlElement("P_14_4W", Order = 18)]
    public decimal? VatAmountTaxiInPLN { get; set; }

    /// <summary>
    /// Suma wartości sprzedaży netto - procedura OSS/IOSS (P_13_5)
    /// </summary>
    [XmlElement("P_13_5", Order = 19)]
    public decimal? NetAmountOSS { get; set; }

    /// <summary>
    /// Kwota podatku VAT - procedura OSS/IOSS (P_14_5)
    /// </summary>
    [XmlElement("P_14_5", Order = 20)]
    public decimal? VatAmountOSS { get; set; }

    /// <summary>
    /// Suma wartości sprzedaży netto ze stawką 0% krajową (P_13_6_1)
    /// </summary>
    [XmlElement("P_13_6_1", Order = 21)]
    public decimal? NetAmount0 { get; set; }

    /// <summary>
    /// Suma wartości WDT (P_13_6_2)
    /// </summary>
    [XmlElement("P_13_6_2", Order = 22)]
    public decimal? NetAmountWdt { get; set; }

    /// <summary>
    /// Suma wartości eksportu (P_13_6_3)
    /// </summary>
    [XmlElement("P_13_6_3", Order = 23)]
    public decimal? NetAmountExport { get; set; }

    /// <summary>
    /// Suma wartości sprzedaży zwolnionej z VAT (P_13_7)
    /// </summary>
    [XmlElement("P_13_7", Order = 24)]
    public decimal? ExemptAmount { get; set; }

    /// <summary>
    /// Suma wartości procedur marży (P_13_8)
    /// </summary>
    [XmlElement("P_13_8", Order = 25)]
    public decimal? MarginAmount { get; set; }

    /// <summary>
    /// Suma wartości - procedura marży VAT (P_13_9)
    /// </summary>
    [XmlElement("P_13_9", Order = 26)]
    public decimal? MarginVatAmount { get; set; }

    /// <summary>
    /// Suma wartości niepodlegających opodatkowaniu (P_13_10)
    /// </summary>
    [XmlElement("P_13_10", Order = 27)]
    public decimal? NotTaxableAmount { get; set; }

    /// <summary>
    /// Suma wartości sprzedaży w procedurze marży (P_13_11)
    /// </summary>
    [XmlElement("P_13_11", Order = 28)]
    public decimal? NetAmount4 { get; set; }

    /// <summary>
    /// Kwota należności ogółem - wartość brutto faktury (P_15)
    /// Łączna kwota do zapłaty
    /// Pole obowiązkowe zgodnie z art. 106e ust. 1 pkt 15 ustawy o VAT
    /// </summary>
    [XmlElement("P_15", Order = 29)]
    public decimal TotalAmount { get; set; }

    #endregion

    #region Adnotacje

    /// <summary>
    /// Adnotacje faktury (Adnotacje)
    /// Specjalne oznaczenia i procedury podatkowe
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("Adnotacje", Order = 30)]
    public InvoiceAnnotations Annotations { get; set; } = new InvoiceAnnotations();

    #endregion

    #region Rodzaj faktury

    /// <summary>
    /// Rodzaj faktury (RodzajFaktury)
    /// Określa typ faktury: VAT (podstawowa), KOR (korygująca),
    /// ZAL (zaliczkowa), ROZ (rozliczeniowa), UPR (uproszczona)
    /// </summary>
    [XmlElement("RodzajFaktury", Order = 31)]
    public InvoiceType InvoiceType { get; set; } = InvoiceType.VAT;

    #endregion

    #region Dane korekty (dla faktur korygujących)

    private InvoiceCorrectionSection _correction = new();

    [XmlIgnore]
    public InvoiceCorrectionSection Correction
    {
        get => _correction;
        set => _correction = value ?? new();
    }

    [XmlElement("PrzyczynaKorekty", Order = 32)]
    public string? CorrectionReason
    {
        get => _correction.CorrectionReason;
        set => _correction.CorrectionReason = value;
    }

    [XmlElement("TypKorekty", Order = 33)]
    public int? CorrectionType
    {
        get => _correction.CorrectionType;
        set => _correction.CorrectionType = value;
    }

    [XmlElement("DaneFaKorygowanej", Order = 34)]
    public CorrectedInvoiceData? CorrectedInvoiceData
    {
        get => _correction.CorrectedInvoiceData;
        set => _correction.CorrectedInvoiceData = value;
    }

    [XmlElement("OkresFaKorygowanej", Order = 36)]
    public SalePeriod? CorrectedInvoicePeriod
    {
        get => _correction.CorrectedInvoicePeriod;
        set => _correction.CorrectedInvoicePeriod = value;
    }

    /// <summary>
    /// Numer faktury korygowanej zmieniony (NrFaKorygowany)
    /// Delegat do InvoiceCorrectionSection - numer faktury pierwotnej po zmianie numeracji
    /// Element FA(3) "NrFaKorygowany", maksymalnie 256 znaków
    /// </summary>
    [XmlElement("NrFaKorygowany", Order = 37)]
    public string? CorrectedInvoiceNumberAmended
    {
        get => _correction.CorrectedInvoiceNumberAmended;
        set => _correction.CorrectedInvoiceNumberAmended = value;
    }

    [XmlElement("Podmiot1K", Order = 38)]
    public CorrectedSellerData? CorrectedSeller
    {
        get => _correction.CorrectedSeller;
        set => _correction.CorrectedSeller = value;
    }

    [XmlElement("Podmiot2K", Order = 39)]
    public List<CorrectedBuyerData>? CorrectedBuyers
    {
        get => _correction.CorrectedBuyers;
        set => _correction.CorrectedBuyers = value;
    }

    [XmlElement("P_15ZK", Order = 40)]
    public decimal? AmountBeforeCorrection
    {
        get => _correction.AmountBeforeCorrection;
        set => _correction.AmountBeforeCorrection = value;
    }

    [XmlElement("KursWalutyZK", Order = 41)]
    public decimal? ExchangeRateBeforeCorrection
    {
        get => _correction.ExchangeRateBeforeCorrection;
        set => _correction.ExchangeRateBeforeCorrection = value;
    }

    #endregion

    #region Zaliczki częściowe (ZaliczkaCzesciowa)

    /// <summary>
    /// Lista wcześniejszych zaliczek częściowych (ZaliczkaCzesciowa)
    /// Dane dla faktur dokumentujących otrzymanie więcej niż jednej płatności
    /// Może zawierać do 31 pozycji
    /// </summary>
    [XmlElement("ZaliczkaCzesciowa", Order = 42)]
    public List<PartialAdvancePayment>? PartialAdvancePayments { get; set; }

    #endregion

    #region Znaczniki FP, TP

    /// <summary>
    /// Znacznik faktury, o której mowa w art. 109 ust. 3d ustawy (FP)
    /// </summary>
    [XmlIgnore]
    public bool IsArticle109_3dInvoice { get; set; }

    [XmlElement("FP", Order = 43)]
    public string? Article109_3dMarker
    {
        get => IsArticle109_3dInvoice ? "1" : null;
        set => IsArticle109_3dInvoice = value == "1";
    }

    public bool ShouldSerializeArticle109_3dMarker() => IsArticle109_3dInvoice;

    /// <summary>
    /// Znacznik istniejących powiązań między nabywcą a dokonującym dostawy (TP)
    /// </summary>
    [XmlIgnore]
    public bool HasRelatedPartyTransaction { get; set; }

    [XmlElement("TP", Order = 44)]
    public string? RelatedPartyMarker
    {
        get => HasRelatedPartyTransaction ? "1" : null;
        set => HasRelatedPartyTransaction = value == "1";
    }

    public bool ShouldSerializeRelatedPartyMarker() => HasRelatedPartyTransaction;

    #endregion

    #region Dodatkowe informacje

    /// <summary>
    /// Dodatkowy opis faktury (DodatkowyOpis)
    /// Dodatkowe informacje tekstowe w formie klucz-wartość
    /// Pole opcjonalne - do 100 wpisów
    /// </summary>
    [XmlElement("DodatkowyOpis", Order = 45)]
    public List<KeyValue>? AdditionalDescription { get; set; }

    #endregion

    #region Faktury zaliczkowe (FakturaZaliczkowa)

    /// <summary>
    /// Referencje do wcześniejszych faktur zaliczkowych (FakturaZaliczkowa).
    /// XSD wymaga xsd:choice: albo (NrKSeFZN + NrFaZaliczkowej) — faktura poza KSeF,
    /// albo NrKSeFFaZaliczkowej — faktura wystawiona w KSeF.
    /// Maksymalnie 100 pozycji.
    /// </summary>
    [XmlElement("FakturaZaliczkowa", Order = 46)]
    public List<AdvanceInvoiceReference>? AdvanceInvoiceReferences { get; set; }

    public bool ShouldSerializeAdvanceInvoiceReferences() =>
        AdvanceInvoiceReferences != null && AdvanceInvoiceReferences.Count > 0;

    #endregion

    #region Znacznik ZwrotAkcyzy

    /// <summary>
    /// Znacznik dla rolników ubiegających się o zwrot podatku akcyzowego (ZwrotAkcyzy)
    /// </summary>
    [XmlIgnore]
    public bool IsExciseRefundEligible { get; set; }

    [XmlElement("ZwrotAkcyzy", Order = 47)]
    public string? ExciseRefundMarker
    {
        get => IsExciseRefundEligible ? "1" : null;
        set => IsExciseRefundEligible = value == "1";
    }

    public bool ShouldSerializeExciseRefundMarker() => IsExciseRefundEligible;

    #endregion

    #region Pozycje faktury (FaWiersz)

    /// <summary>
    /// Lista pozycji faktury (FaWiersz)
    /// Zawiera szczegółowe dane o towarach i usługach
    /// Może zawierać do 10000 pozycji
    /// </summary>
    [XmlElement("FaWiersz", Order = 48)]
    public List<InvoiceLineItem>? LineItems { get; set; }

    #endregion

    #region Płatności (Platnosc)

    /// <summary>
    /// Informacje o płatnościach (Platnosc)
    /// Zawiera terminy płatności, formy płatności i rachunki bankowe
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("Platnosc", Order = 49)]
    public Payment? Payment { get; set; }

    #endregion

    #region Warunki transakcji (WarunkiTransakcji)

    /// <summary>
    /// Warunki transakcji (WarunkiTransakcji)
    /// Zawiera informacje o warunkach dostawy i transportu
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("WarunkiTransakcji", Order = 50)]
    public TransactionTerms? TransactionTerms { get; set; }

    #endregion

    #region ShouldSerialize - prevents xsi:nil="true" for optional elements

    public bool ShouldSerializeIssuePlace() => !string.IsNullOrEmpty(IssuePlace);
    public bool ShouldSerializeSaleDateString() => SaleDate.HasValue;
    public bool ShouldSerializeSalePeriod() => SalePeriod != null;
    public bool ShouldSerializeWarehouseDocuments() => WarehouseDocuments != null && WarehouseDocuments.Count > 0;
    public bool ShouldSerializeNetAmount23() => NetAmount23.HasValue;
    public bool ShouldSerializeVatAmount23() => VatAmount23.HasValue;
    public bool ShouldSerializeVatAmount23InPLN() => VatAmount23InPLN.HasValue;
    public bool ShouldSerializeNetAmount8() => NetAmount8.HasValue;
    public bool ShouldSerializeVatAmount8() => VatAmount8.HasValue;
    public bool ShouldSerializeVatAmount8InPLN() => VatAmount8InPLN.HasValue;
    public bool ShouldSerializeNetAmount5() => NetAmount5.HasValue;
    public bool ShouldSerializeVatAmount5() => VatAmount5.HasValue;
    public bool ShouldSerializeVatAmount5InPLN() => VatAmount5InPLN.HasValue;
    public bool ShouldSerializeNetAmountTaxi() => NetAmountTaxi.HasValue;
    public bool ShouldSerializeVatAmountTaxi() => VatAmountTaxi.HasValue;
    public bool ShouldSerializeVatAmountTaxiInPLN() => VatAmountTaxiInPLN.HasValue;
    public bool ShouldSerializeNetAmountOSS() => NetAmountOSS.HasValue;
    public bool ShouldSerializeVatAmountOSS() => VatAmountOSS.HasValue;
    public bool ShouldSerializeNetAmount0() => NetAmount0.HasValue;
    public bool ShouldSerializeNetAmountWdt() => NetAmountWdt.HasValue;
    public bool ShouldSerializeNetAmountExport() => NetAmountExport.HasValue;
    public bool ShouldSerializeExemptAmount() => ExemptAmount.HasValue;
    public bool ShouldSerializeMarginAmount() => MarginAmount.HasValue;
    public bool ShouldSerializeMarginVatAmount() => MarginVatAmount.HasValue;
    public bool ShouldSerializeNotTaxableAmount() => NotTaxableAmount.HasValue;
    public bool ShouldSerializeNetAmount4() => NetAmount4.HasValue;
    public bool ShouldSerializeCorrectionReason() => !string.IsNullOrEmpty(CorrectionReason);
    public bool ShouldSerializeCorrectionType() => CorrectionType.HasValue;
    public bool ShouldSerializeCorrectedInvoiceData() => CorrectedInvoiceData != null;
    public bool ShouldSerializeCorrectedInvoicePeriod() => CorrectedInvoicePeriod != null;
    public bool ShouldSerializeCorrectedInvoiceNumberAmended() => !string.IsNullOrEmpty(CorrectedInvoiceNumberAmended);
    public bool ShouldSerializeCorrectedSeller() => CorrectedSeller != null;
    public bool ShouldSerializeCorrectedBuyers() => CorrectedBuyers != null && CorrectedBuyers.Count > 0;
    public bool ShouldSerializeAmountBeforeCorrection() => AmountBeforeCorrection.HasValue;
    public bool ShouldSerializeExchangeRateBeforeCorrection() => ExchangeRateBeforeCorrection.HasValue;
    public bool ShouldSerializePartialAdvancePayments() => PartialAdvancePayments != null && PartialAdvancePayments.Count > 0;
    public bool ShouldSerializeLineItems() => LineItems != null && LineItems.Count > 0;
    public bool ShouldSerializePayment() => Payment != null;
    public bool ShouldSerializeTransactionTerms() => TransactionTerms != null;
    public bool ShouldSerializeAdditionalDescription() => AdditionalDescription != null && AdditionalDescription.Count > 0;

    #endregion

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy faktura ma określoną datę sprzedaży
    /// </summary>
    [XmlIgnore]
    public bool HasSaleDate => SaleDate.HasValue;

    /// <summary>
    /// Sprawdza czy faktura dotyczy okresu rozliczeniowego
    /// </summary>
    [XmlIgnore]
    public bool HasSalePeriod => SalePeriod != null;

    /// <summary>
    /// Sprawdza czy faktura ma przypisane dokumenty magazynowe WZ
    /// </summary>
    [XmlIgnore]
    public bool HasWarehouseDocuments => WarehouseDocuments != null && WarehouseDocuments.Count > 0;

    /// <summary>
    /// Sprawdza czy określono miejsce wystawienia faktury
    /// </summary>
    [XmlIgnore]
    public bool HasIssuePlace => !string.IsNullOrEmpty(IssuePlace);

    /// <summary>
    /// Sprawdza czy faktura ma pozycje
    /// </summary>
    [XmlIgnore]
    public bool HasLineItems => LineItems != null && LineItems.Count > 0;

    /// <summary>
    /// Sprawdza czy faktura ma dane o płatnościach
    /// </summary>
    [XmlIgnore]
    public bool HasPayment => Payment != null;

    /// <summary>
    /// Sprawdza czy faktura jest korektą
    /// </summary>
    [XmlIgnore]
    public bool IsCorrection => InvoiceType == InvoiceType.KOR ||
                                 InvoiceType == InvoiceType.KOR_ZAL ||
                                 InvoiceType == InvoiceType.KOR_ROZ;

    /// <summary>
    /// Oblicza łączną wartość netto faktury
    /// </summary>
    [XmlIgnore]
    public decimal TotalNetAmount =>
        (NetAmount23 ?? 0) + (NetAmount8 ?? 0) + (NetAmount5 ?? 0) + (NetAmount4 ?? 0) +
        (NetAmount0 ?? 0) + (NetAmountWdt ?? 0) + (NetAmountExport ?? 0) +
        (ExemptAmount ?? 0) + (NetAmountTaxi ?? 0) + (NetAmountOSS ?? 0) +
        (MarginAmount ?? 0) + (NotTaxableAmount ?? 0);

    /// <summary>
    /// Oblicza łączną wartość podatku VAT faktury
    /// </summary>
    [XmlIgnore]
    public decimal TotalVatAmount =>
        (VatAmount23 ?? 0) + (VatAmount8 ?? 0) + (VatAmount5 ?? 0) +
        (VatAmountTaxi ?? 0) + (VatAmountOSS ?? 0) + (MarginVatAmount ?? 0);

    #endregion
}

