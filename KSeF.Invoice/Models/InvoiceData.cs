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
    /// Data wystawienia faktury (P_1)
    /// Format: YYYY-MM-DD
    /// Pole obowiązkowe zgodnie z art. 106e ust. 1 pkt 1 ustawy o VAT
    /// </summary>
    [XmlElement("P_1", Order = 1)]
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
    /// Data dokonania lub zakończenia dostawy towarów lub wykonania usługi (P_6)
    /// Data sprzedaży - jeżeli taka data jest określona i różni się od daty wystawienia faktury
    /// Format: YYYY-MM-DD
    /// Pole opcjonalne zgodnie z art. 106e ust. 1 pkt 6 ustawy o VAT
    /// Alternatywa dla OkresFa
    /// </summary>
    [XmlElement("P_6", Order = 5)]
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
    /// Suma wartości objętych stawką 4% (P_13_11)
    /// </summary>
    [XmlElement("P_13_11", Order = 28)]
    public decimal? NetAmount4 { get; set; }

    /// <summary>
    /// Kwota podatku VAT ze stawką 4% (P_14_11)
    /// </summary>
    [XmlElement("P_14_11", Order = 29)]
    public decimal? VatAmount4 { get; set; }

    /// <summary>
    /// Kwota podatku VAT ze stawką 4% przeliczona na PLN (P_14_11W)
    /// </summary>
    [XmlElement("P_14_11W", Order = 30)]
    public decimal? VatAmount4InPLN { get; set; }

    /// <summary>
    /// Kwota należności ogółem - wartość brutto faktury (P_15)
    /// Łączna kwota do zapłaty
    /// Pole obowiązkowe zgodnie z art. 106e ust. 1 pkt 15 ustawy o VAT
    /// </summary>
    [XmlElement("P_15", Order = 31)]
    public decimal TotalAmount { get; set; }

    #endregion

    #region Adnotacje

    /// <summary>
    /// Adnotacje faktury (Adnotacje)
    /// Specjalne oznaczenia i procedury podatkowe
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("Adnotacje", Order = 32)]
    public InvoiceAnnotations Annotations { get; set; } = new InvoiceAnnotations();

    #endregion

    #region Rodzaj faktury

    /// <summary>
    /// Rodzaj faktury (RodzajFaktury)
    /// Określa typ faktury: VAT (podstawowa), KOR (korygująca),
    /// ZAL (zaliczkowa), ROZ (rozliczeniowa), UPR (uproszczona)
    /// </summary>
    [XmlElement("RodzajFaktury", Order = 33)]
    public InvoiceType InvoiceType { get; set; } = InvoiceType.VAT;

    #endregion

    #region Dane korekty (dla faktur korygujących)

    /// <summary>
    /// Przyczyna korekty (PrzyczynaKorekty)
    /// Opis przyczyny dokonania korekty faktury
    /// Wymagane dla faktur korygujących
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("PrzyczynaKorekty", Order = 34)]
    public string? CorrectionReason { get; set; }

    /// <summary>
    /// Typ korekty (TypKorekty)
    /// Określa typ korekty: 1 - korekta wartości, 2 - korekta danych, 3 - oba typy
    /// </summary>
    [XmlElement("TypKorekty", Order = 35)]
    public int? CorrectionType { get; set; }

    /// <summary>
    /// Dane faktury korygowanej (DaneFaKorygowanej)
    /// Informacje o fakturze pierwotnej będącej przedmiotem korekty
    /// Wymagane dla faktur korygujących
    /// </summary>
    [XmlElement("DaneFaKorygowanej", Order = 36)]
    public CorrectedInvoiceData? CorrectedInvoiceData { get; set; }

    /// <summary>
    /// Numer KSeF faktury korygującej poprzednią korektę (NrKSeFN)
    /// Stosowane w przypadku kolejnych korekt faktury
    /// </summary>
    [XmlElement("NrKSeFN", Order = 37)]
    public string? PreviousCorrectionKSeFNumber { get; set; }

    /// <summary>
    /// Okres, którego dotyczy korekta (OkresFaKorygowanej)
    /// Data "od" - "do" dla faktur z okresem, które są korygowane
    /// </summary>
    [XmlElement("OkresFaKorygowanej", Order = 38)]
    public SalePeriod? CorrectedInvoicePeriod { get; set; }

    #endregion

    #region Dane zaliczki (dla faktur zaliczkowych i rozliczeniowych)

    /// <summary>
    /// Lista wcześniejszych faktur zaliczkowych (ZalszczkaCzesciowa)
    /// Dane dotyczące wcześniejszych faktur zaliczkowych przy fakturze końcowej/rozliczeniowej
    /// </summary>
    [XmlElement("ZaliczkaCalosciowa", Order = 39)]
    public List<AdvancePaymentData>? AdvancePayments { get; set; }

    #endregion

    #region Pozycje faktury (FaWiersz)

    /// <summary>
    /// Lista pozycji faktury (FaWiersz)
    /// Zawiera szczegółowe dane o towarach i usługach
    /// Może zawierać do 10000 pozycji
    /// </summary>
    [XmlElement("FaWiersz", Order = 40)]
    public List<InvoiceLineItem>? LineItems { get; set; }

    #endregion

    #region Płatności (Platnosc)

    /// <summary>
    /// Informacje o płatnościach (Platnosc)
    /// Zawiera terminy płatności, formy płatności i rachunki bankowe
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("Platnosc", Order = 41)]
    public Payment? Payment { get; set; }

    #endregion

    #region Warunki transakcji (WarunkiTransakcji)

    /// <summary>
    /// Warunki transakcji (WarunkiTransakcji)
    /// Zawiera informacje o warunkach dostawy i transportu
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("WarunkiTransakcji", Order = 42)]
    public TransactionTerms? TransactionTerms { get; set; }

    #endregion

    #region Dodatkowe informacje

    /// <summary>
    /// Dodatkowy opis faktury (DodatkowyOpis)
    /// Dodatkowe informacje tekstowe w formie klucz-wartość
    /// Pole opcjonalne - do 100 wpisów
    /// </summary>
    [XmlElement("DodatkowyOpis", Order = 43)]
    public List<KeyValue>? AdditionalDescription { get; set; }

    /// <summary>
    /// Numer faktury zaliczkowej dla faktury rozliczeniowej (NrFaZal662)
    /// Numer faktury zaliczkowej, której dotyczy faktura rozliczeniowa
    /// </summary>
    [XmlElement("NrFaZaliczkowej", Order = 44)]
    public List<string>? AdvanceInvoiceNumbers { get; set; }

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
        (VatAmount23 ?? 0) + (VatAmount8 ?? 0) + (VatAmount5 ?? 0) + (VatAmount4 ?? 0) +
        (VatAmountTaxi ?? 0) + (VatAmountOSS ?? 0) + (MarginVatAmount ?? 0);

    #endregion
}

/// <summary>
/// Dane zaliczki częściowej lub całościowej
/// Informacje o wcześniejszych fakturach zaliczkowych
/// </summary>
[XmlType("ZaliczkaCalosciowa")]
public class AdvancePaymentData
{
    /// <summary>
    /// Numer faktury zaliczkowej
    /// </summary>
    [XmlElement("NrKSeFFaZaliczkowej")]
    public string? KSeFNumber { get; set; }

    /// <summary>
    /// Numer faktury zaliczkowej (bez KSeF)
    /// </summary>
    [XmlElement("NrFaZaliczkowej")]
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Data wystawienia faktury zaliczkowej
    /// </summary>
    [XmlElement("DataFaZaliczkowej")]
    public DateOnly? IssueDate { get; set; }

    /// <summary>
    /// Kwota zaliczki netto
    /// </summary>
    [XmlElement("KwotaZaliczki")]
    public decimal? Amount { get; set; }

    /// <summary>
    /// Kwota podatku VAT od zaliczki
    /// </summary>
    [XmlElement("KwotaVATZaliczki")]
    public decimal? VatAmount { get; set; }
}
