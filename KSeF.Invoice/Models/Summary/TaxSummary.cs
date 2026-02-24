using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Summary;

/// <summary>
/// Podsumowanie podatkowe faktury - sumy wartości w podziale na stawki VAT
/// Odpowiednik elementów P_13_* i P_14_* oraz P_15 w schemacie FA(3)
/// Zawiera sumy netto, VAT oraz wartość brutto faktury
/// </summary>
[XmlType("Podsumowanie")]
public class TaxSummary
{
    #region Stawka 23% (P_13_1, P_14_1)

    /// <summary>
    /// Suma wartości sprzedaży netto ze stawką podstawową 23% (P_13_1)
    /// Suma wartości netto dla wszystkich pozycji ze stawką VAT 23%
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują pozycje ze stawką 23%
    /// </summary>
    [XmlElement("P_13_1")]
    public decimal? NetAmount23 { get; set; }

    /// <summary>
    /// Suma kwot podatku VAT od wartości netto ze stawką 23% (P_14_1)
    /// Suma podatku VAT dla wszystkich pozycji ze stawką VAT 23%
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują pozycje ze stawką 23%
    /// </summary>
    [XmlElement("P_14_1")]
    public decimal? VatAmount23 { get; set; }

    #endregion

    #region Stawka 8% (P_13_2, P_14_2)

    /// <summary>
    /// Suma wartości sprzedaży netto ze stawką obniżoną 8% (P_13_2)
    /// Suma wartości netto dla wszystkich pozycji ze stawką VAT 8%
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują pozycje ze stawką 8%
    /// </summary>
    [XmlElement("P_13_2")]
    public decimal? NetAmount8 { get; set; }

    /// <summary>
    /// Suma kwot podatku VAT od wartości netto ze stawką 8% (P_14_2)
    /// Suma podatku VAT dla wszystkich pozycji ze stawką VAT 8%
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują pozycje ze stawką 8%
    /// </summary>
    [XmlElement("P_14_2")]
    public decimal? VatAmount8 { get; set; }

    #endregion

    #region Stawka 5% (P_13_3, P_14_3)

    /// <summary>
    /// Suma wartości sprzedaży netto ze stawką obniżoną 5% (P_13_3)
    /// Suma wartości netto dla wszystkich pozycji ze stawką VAT 5%
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują pozycje ze stawką 5%
    /// </summary>
    [XmlElement("P_13_3")]
    public decimal? NetAmount5 { get; set; }

    /// <summary>
    /// Suma kwot podatku VAT od wartości netto ze stawką 5% (P_14_3)
    /// Suma podatku VAT dla wszystkich pozycji ze stawką VAT 5%
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują pozycje ze stawką 5%
    /// </summary>
    [XmlElement("P_14_3")]
    public decimal? VatAmount5 { get; set; }

    #endregion

    #region Stawka 0% krajowa, WDT, eksport (P_13_6_1, P_13_6_2, P_13_6_3)

    /// <summary>
    /// Suma wartości sprzedaży netto ze stawką 0% krajową (P_13_6_1)
    /// Suma wartości netto dla wszystkich pozycji ze stawką VAT 0%
    /// (sprzedaż krajowa objęta stawką 0%)
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują pozycje ze stawką 0%
    /// </summary>
    [XmlElement("P_13_6_1")]
    public decimal? NetAmount0 { get; set; }

    /// <summary>
    /// Suma wartości wewnątrzwspólnotowej dostawy towarów (WDT) (P_13_6_2)
    /// Suma wartości netto dla wszystkich pozycji dotyczących WDT
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują transakcje WDT
    /// </summary>
    [XmlElement("P_13_6_2")]
    public decimal? NetAmountWdt { get; set; }

    /// <summary>
    /// Suma wartości eksportu towarów (P_13_6_3)
    /// Suma wartości netto dla wszystkich pozycji dotyczących eksportu poza UE
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują transakcje eksportowe
    /// </summary>
    [XmlElement("P_13_6_3")]
    public decimal? NetAmountExport { get; set; }

    #endregion

    #region Zwolnienie z VAT (P_13_7)

    /// <summary>
    /// Suma wartości sprzedaży zwolnionej z VAT (P_13_7)
    /// Suma wartości netto dla wszystkich pozycji zwolnionych z podatku VAT
    /// zgodnie z art. 43 ust. 1 lub art. 82 ustawy o VAT
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole opcjonalne - wypełniane gdy występują pozycje zwolnione z VAT
    /// </summary>
    [XmlElement("P_13_7")]
    public decimal? ExemptAmount { get; set; }

    #endregion

    #region Suma ogółem (P_15)

    /// <summary>
    /// Kwota należności ogółem (P_15)
    /// Suma wartości brutto faktury - łączna kwota do zapłaty
    /// Zgodnie z art. 106e ust. 1 pkt 15 ustawy o VAT
    /// Wyrażona w walucie faktury z dokładnością do 2 miejsc po przecinku
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("P_15")]
    public decimal TotalAmount { get; set; }

    #endregion

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy występują pozycje ze stawką 23%
    /// </summary>
    [XmlIgnore]
    public bool Has23Rate => NetAmount23.HasValue && NetAmount23.Value != 0;

    /// <summary>
    /// Sprawdza czy występują pozycje ze stawką 8%
    /// </summary>
    [XmlIgnore]
    public bool Has8Rate => NetAmount8.HasValue && NetAmount8.Value != 0;

    /// <summary>
    /// Sprawdza czy występują pozycje ze stawką 5%
    /// </summary>
    [XmlIgnore]
    public bool Has5Rate => NetAmount5.HasValue && NetAmount5.Value != 0;

    /// <summary>
    /// Sprawdza czy występują pozycje ze stawką 0%
    /// </summary>
    [XmlIgnore]
    public bool Has0Rate => NetAmount0.HasValue && NetAmount0.Value != 0;

    /// <summary>
    /// Sprawdza czy występują transakcje WDT
    /// </summary>
    [XmlIgnore]
    public bool HasWdt => NetAmountWdt.HasValue && NetAmountWdt.Value != 0;

    /// <summary>
    /// Sprawdza czy występują transakcje eksportowe
    /// </summary>
    [XmlIgnore]
    public bool HasExport => NetAmountExport.HasValue && NetAmountExport.Value != 0;

    /// <summary>
    /// Sprawdza czy występują pozycje zwolnione z VAT
    /// </summary>
    [XmlIgnore]
    public bool HasExempt => ExemptAmount.HasValue && ExemptAmount.Value != 0;

    /// <summary>
    /// Oblicza łączną wartość netto faktury
    /// Suma wszystkich wartości netto ze wszystkich stawek VAT
    /// </summary>
    [XmlIgnore]
    public decimal TotalNetAmount =>
        (NetAmount23 ?? 0) + (NetAmount8 ?? 0) + (NetAmount5 ?? 0) +
        (NetAmount0 ?? 0) + (NetAmountWdt ?? 0) + (NetAmountExport ?? 0) +
        (ExemptAmount ?? 0);

    /// <summary>
    /// Oblicza łączną wartość podatku VAT faktury
    /// Suma wszystkich kwot VAT ze wszystkich stawek
    /// </summary>
    [XmlIgnore]
    public decimal TotalVatAmount =>
        (VatAmount23 ?? 0) + (VatAmount8 ?? 0) + (VatAmount5 ?? 0);

    #endregion
}
