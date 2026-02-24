using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models;

/// <summary>
/// Pozycja faktury - pojedynczy wiersz faktury (FaWiersz)
/// Zawiera dane dotyczące sprzedawanego towaru lub usługi
/// Odpowiednik elementu FaWiersz w schemacie FA(3)
/// </summary>
[XmlType("FaWiersz")]
public class InvoiceLineItem
{
    #region Identyfikacja pozycji

    /// <summary>
    /// Numer wiersza faktury (NrWiersza)
    /// Kolejny numer pozycji na fakturze, zaczynając od 1
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("NrWiersza")]
    public int LineNumber { get; set; }

    #endregion

    #region Dane produktu/usługi (P_7)

    /// <summary>
    /// Nazwa (rodzaj) towaru lub usługi (P_7)
    /// Zgodnie z art. 106e ust. 1 pkt 7 ustawy o VAT
    /// Maksymalnie 512 znaków
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("P_7")]
    public string ProductName { get; set; } = string.Empty;

    #endregion

    #region Jednostka i ilość (P_8A, P_8B)

    /// <summary>
    /// Jednostka miary towaru lub usługi (P_8A)
    /// Np. szt., kg, m, godz., usł.
    /// Maksymalnie 256 znaków
    /// Pole opcjonalne - stosowane gdy dotyczy
    /// </summary>
    [XmlElement("P_8A")]
    public string? Unit { get; set; }

    /// <summary>
    /// Ilość (liczba) dostarczonych towarów lub zakres wykonanych usług (P_8B)
    /// Zgodnie z art. 106e ust. 1 pkt 8 ustawy o VAT
    /// Wartość z dokładnością do 6 miejsc po przecinku
    /// Pole opcjonalne - stosowane gdy dotyczy
    /// </summary>
    [XmlElement("P_8B")]
    public decimal? Quantity { get; set; }

    #endregion

    #region Ceny jednostkowe (P_9A, P_9B)

    /// <summary>
    /// Cena jednostkowa towaru lub usługi bez kwoty podatku (cena jednostkowa netto) (P_9A)
    /// Zgodnie z art. 106e ust. 1 pkt 9 ustawy o VAT
    /// Wyrażona w walucie faktury
    /// Wartość z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - stosowane alternatywnie z P_9B
    /// </summary>
    [XmlElement("P_9A")]
    public decimal? UnitNetPrice { get; set; }

    /// <summary>
    /// Cena jednostkowa towaru lub usługi wraz z kwotą podatku (cena jednostkowa brutto) (P_9B)
    /// Stosowana w przypadku zastosowania art. 106e ust. 7 i 8 ustawy o VAT (metoda "w stu")
    /// Wyrażona w walucie faktury
    /// Wartość z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - stosowane alternatywnie z P_9A
    /// </summary>
    [XmlElement("P_9B")]
    public decimal? UnitGrossPrice { get; set; }

    #endregion

    #region Rabat (P_10)

    /// <summary>
    /// Kwoty wszelkich opustów lub obniżek cen, w tym w formie rabatu z tytułu
    /// wcześniejszej zapłaty (P_10)
    /// Zgodnie z art. 106e ust. 1 pkt 10 ustawy o VAT
    /// Wartość ujemna lub dodatnia wyrażona w walucie faktury
    /// Wartość z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - stosowane gdy udzielono rabatu
    /// </summary>
    [XmlElement("P_10")]
    public decimal? Discount { get; set; }

    #endregion

    #region Wartości (P_11, P_11A, P_11Vat)

    /// <summary>
    /// Wartość sprzedaży netto (P_11)
    /// Wartość dostarczonych towarów lub wykonanych usług, objętych transakcją,
    /// bez kwoty podatku (wartość sprzedaży netto)
    /// Zgodnie z art. 106e ust. 1 pkt 11 ustawy o VAT
    /// Wyrażona w walucie faktury
    /// Wartość z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - stosowane alternatywnie z P_11Vat
    /// </summary>
    [XmlElement("P_11")]
    public decimal? NetAmount { get; set; }

    /// <summary>
    /// Kwota podatku VAT dla pozycji (P_11A)
    /// Kwota podatku od wartości netto pozycji
    /// Wyrażona w walucie faktury
    /// Wartość z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("P_11A")]
    public decimal? VatAmount { get; set; }

    /// <summary>
    /// Wartość sprzedaży brutto (P_11Vat)
    /// Wartość dostarczonych towarów lub wykonanych usług wraz z kwotą podatku
    /// Stosowana gdy faktura dokumentuje czynności, dla których podstawą opodatkowania
    /// jest kwota brutto (metoda "w stu")
    /// Wyrażona w walucie faktury
    /// Wartość z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - stosowane alternatywnie z P_11
    /// </summary>
    [XmlElement("P_11Vat")]
    public decimal? GrossAmount { get; set; }

    #endregion

    #region Stawka VAT (P_12)

    /// <summary>
    /// Stawka podatku VAT (P_12)
    /// Stawka podatku od wartości dodanej dla danej pozycji
    /// Zgodnie z art. 106e ust. 1 pkt 12 ustawy o VAT
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("P_12")]
    public VatRate VatRate { get; set; }

    #endregion

    #region Data sprzedaży pozycji (P_6A)

    /// <summary>
    /// Data dokonania lub zakończenia dostawy towarów lub wykonania usługi
    /// dla danej pozycji faktury (P_6A)
    /// Stosowana gdy różne pozycje mają różne daty sprzedaży
    /// Format: YYYY-MM-DD
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("P_6A")]
    public DateOnly? SaleDate { get; set; }

    #endregion

    #region Kody produktów (GTIN, PKWiU, CN)

    /// <summary>
    /// Globalny numer jednostki handlowej (GTIN)
    /// Kod GTIN/EAN produktu - 8, 12, 13 lub 14 cyfr
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("GTIN")]
    public string? GtinCode { get; set; }

    /// <summary>
    /// Symbol Polskiej Klasyfikacji Wyrobów i Usług (PKWiU)
    /// Format: XX.XX.XX.X
    /// Pole opcjonalne - wymagane dla niektórych towarów i usług
    /// </summary>
    [XmlElement("PKWiU")]
    public string? PkwiuCode { get; set; }

    /// <summary>
    /// Kod Nomenklatury Scalonej (CN)
    /// Kod taryfy celnej UE - 8 cyfr
    /// Pole opcjonalne - wymagane dla niektórych towarów w obrocie międzynarodowym
    /// </summary>
    [XmlElement("CN")]
    public string? CnCode { get; set; }

    #endregion

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy pozycja ma określoną cenę jednostkową netto
    /// </summary>
    [XmlIgnore]
    public bool HasUnitNetPrice => UnitNetPrice.HasValue;

    /// <summary>
    /// Sprawdza czy pozycja ma określoną cenę jednostkową brutto (metoda "w stu")
    /// </summary>
    [XmlIgnore]
    public bool HasUnitGrossPrice => UnitGrossPrice.HasValue;

    /// <summary>
    /// Sprawdza czy pozycja ma określony rabat
    /// </summary>
    [XmlIgnore]
    public bool HasDiscount => Discount.HasValue && Discount.Value != 0;

    /// <summary>
    /// Sprawdza czy pozycja ma indywidualną datę sprzedaży
    /// </summary>
    [XmlIgnore]
    public bool HasSaleDate => SaleDate.HasValue;

    /// <summary>
    /// Sprawdza czy pozycja ma przypisany kod GTIN
    /// </summary>
    [XmlIgnore]
    public bool HasGtinCode => !string.IsNullOrEmpty(GtinCode);

    /// <summary>
    /// Sprawdza czy pozycja ma przypisany kod PKWiU
    /// </summary>
    [XmlIgnore]
    public bool HasPkwiuCode => !string.IsNullOrEmpty(PkwiuCode);

    /// <summary>
    /// Sprawdza czy pozycja ma przypisany kod CN
    /// </summary>
    [XmlIgnore]
    public bool HasCnCode => !string.IsNullOrEmpty(CnCode);

    /// <summary>
    /// Sprawdza czy pozycja używa metody kalkulacji netto (P_11)
    /// </summary>
    [XmlIgnore]
    public bool UsesNetCalculation => NetAmount.HasValue;

    /// <summary>
    /// Sprawdza czy pozycja używa metody kalkulacji brutto - "w stu" (P_11Vat)
    /// </summary>
    [XmlIgnore]
    public bool UsesGrossCalculation => GrossAmount.HasValue;

    #endregion
}
