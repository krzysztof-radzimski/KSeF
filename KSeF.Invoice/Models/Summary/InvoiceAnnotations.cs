/*=====================================================================================

    KSeF.Invoice
    Klasy reprezentujące adnotacje faktury i powiązane struktury.
    InvoiceAnnotations (Adnotacje) - specjalne oznaczenia i procedury
    podatkowe: metoda kasowa, samofakturowanie, odwrotne obciążenie,
    split payment, zwolnienia z VAT, nowe środki transportu,
    procedura uproszczona oraz procedury marży. Zawiera również
    klasy pomocnicze: VatExemption, NewTransportMeans, MarginProcedures
    oraz enumy AnnotationValue i MarginProcedureType.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Summary;

/// <summary>
/// Adnotacje faktury - specjalne oznaczenia i procedury stosowane na fakturze
/// Odpowiednik elementu Adnotacje w schemacie FA(3)
/// Wszystkie pola są obowiązkowe w schemacie
/// </summary>
[XmlType("Adnotacje")]
public class InvoiceAnnotations
{
    /// <summary>
    /// Metoda kasowa (P_16)
    /// Wartość 1 - tak, wartość 2 - nie
    /// </summary>
    [XmlElement("P_16")]
    public AnnotationValue CashMethod { get; set; } = AnnotationValue.No;

    /// <summary>
    /// Samofakturowanie (P_17)
    /// Wartość 1 - tak, wartość 2 - nie
    /// </summary>
    [XmlElement("P_17")]
    public AnnotationValue SelfBilling { get; set; } = AnnotationValue.No;

    /// <summary>
    /// Odwrotne obciążenie (P_18)
    /// Wartość 1 - tak, wartość 2 - nie
    /// </summary>
    [XmlElement("P_18")]
    public AnnotationValue ReverseCharge { get; set; } = AnnotationValue.No;

    /// <summary>
    /// Mechanizm podzielonej płatności - split payment (P_18A)
    /// Wartość 1 - tak, wartość 2 - nie
    /// </summary>
    [XmlElement("P_18A")]
    public AnnotationValue SplitPayment { get; set; } = AnnotationValue.No;

    /// <summary>
    /// Informacja o zwolnieniu z VAT (Zwolnienie)
    /// Pole obowiązkowe - domyślnie brak zwolnienia (P_19N=1)
    /// </summary>
    [XmlElement("Zwolnienie")]
    public VatExemption Exemption { get; set; } = new VatExemption();

    /// <summary>
    /// Informacja o nowych środkach transportu (NoweSrodkiTransportu)
    /// Pole obowiązkowe - domyślnie brak (P_22N=1)
    /// </summary>
    [XmlElement("NoweSrodkiTransportu")]
    public NewTransportMeans NewTransportMeans { get; set; } = new NewTransportMeans();

    /// <summary>
    /// Procedura uproszczona (P_23)
    /// Wartość 1 - tak, wartość 2 - nie
    /// </summary>
    [XmlElement("P_23")]
    public AnnotationValue SimplifiedProcedure { get; set; } = AnnotationValue.No;

    /// <summary>
    /// Procedury marży (PMarzy)
    /// Pole obowiązkowe - domyślnie brak procedury marży (P_PMarzyN=1)
    /// </summary>
    [XmlElement("PMarzy")]
    public MarginProcedures MarginProcedures { get; set; } = new MarginProcedures();

    #region Właściwości pomocnicze

    [XmlIgnore]
    public bool IsCashMethod => CashMethod == AnnotationValue.Yes;

    [XmlIgnore]
    public bool IsSelfBilling => SelfBilling == AnnotationValue.Yes;

    [XmlIgnore]
    public bool IsReverseCharge => ReverseCharge == AnnotationValue.Yes;

    [XmlIgnore]
    public bool IsSplitPayment => SplitPayment == AnnotationValue.Yes;

    [XmlIgnore]
    public bool HasExemption => Exemption != null && Exemption.IsExempt;

    [XmlIgnore]
    public bool HasNewTransportMeans => NewTransportMeans != null && NewTransportMeans.HasTransportMeans;

    #endregion
}

/// <summary>
/// Wartość adnotacji (P_16, P_17, P_18, P_18A, P_23)
/// </summary>
public enum AnnotationValue
{
    /// <summary>
    /// Wartość 1 - Tak
    /// </summary>
    [XmlEnum("1")]
    Yes = 1,

    /// <summary>
    /// Wartość 2 - Nie
    /// </summary>
    [XmlEnum("2")]
    No = 2
}

/// <summary>
/// Informacja o zwolnieniu z VAT (Zwolnienie)
/// Schemat: choice(P_19 + choice(P_19A|P_19B|P_19C) | P_19N)
/// </summary>
[XmlType("Zwolnienie")]
public class VatExemption
{
    /// <summary>
    /// Znacznik zwolnienia z VAT (P_19) - wartość 1 = zwolniony
    /// Serializowany tylko gdy IsExempt = true
    /// </summary>
    [XmlElement("P_19")]
    public int P_19 { get; set; }

    public bool ShouldSerializeP_19() => IsExempt;

    /// <summary>
    /// Przepis ustawy (P_19A) - podstawa zwolnienia z ustawy krajowej
    /// </summary>
    [XmlElement("P_19A")]
    public string? DirectiveBasis { get; set; }

    public bool ShouldSerializeDirectiveBasis() => IsExempt && !string.IsNullOrEmpty(DirectiveBasis);

    /// <summary>
    /// Przepis dyrektywy (P_19B) - podstawa zwolnienia z dyrektywy UE
    /// </summary>
    [XmlElement("P_19B")]
    public string? EuDirectiveBasis { get; set; }

    public bool ShouldSerializeEuDirectiveBasis() => IsExempt && !string.IsNullOrEmpty(EuDirectiveBasis);

    /// <summary>
    /// Inna podstawa prawna (P_19C)
    /// </summary>
    [XmlElement("P_19C")]
    public string? OtherLegalBasis { get; set; }

    public bool ShouldSerializeOtherLegalBasis() => IsExempt && !string.IsNullOrEmpty(OtherLegalBasis);

    /// <summary>
    /// Brak zwolnienia (P_19N) - wartość 1 = brak zwolnienia
    /// Serializowany tylko gdy IsExempt = false
    /// </summary>
    [XmlElement("P_19N")]
    public int P_19N { get; set; } = 1;

    public bool ShouldSerializeP_19N() => !IsExempt;

    /// <summary>
    /// Czy faktura zawiera zwolnienie z VAT
    /// </summary>
    [XmlIgnore]
    public bool IsExempt { get; set; }

    /// <summary>
    /// Convenience: ustawia zwolnienie z podaną podstawą prawną ustawy
    /// </summary>
    public void SetExemption(string legalBasis)
    {
        IsExempt = true;
        P_19 = 1;
        DirectiveBasis = legalBasis;
    }
}

/// <summary>
/// Informacja o nowych środkach transportu (NoweSrodkiTransportu)
/// Schemat: choice(P_22+P_42_5+NowySrodekTransportu* | P_22N)
/// </summary>
[XmlType("NoweSrodkiTransportu")]
public class NewTransportMeans
{
    /// <summary>
    /// Znacznik WDT nowych środków transportu (P_22) - wartość 1 = tak
    /// </summary>
    [XmlElement("P_22")]
    public int P_22 { get; set; }

    public bool ShouldSerializeP_22() => HasTransportMeans;

    /// <summary>
    /// Obowiązek z art. 42 ust. 5 (P_42_5)
    /// Wartość 1 = tak, 2 = nie
    /// </summary>
    [XmlElement("P_42_5")]
    public int P_42_5 { get; set; } = 2;

    public bool ShouldSerializeP_42_5() => HasTransportMeans;

    /// <summary>
    /// Brak WDT nowych środków transportu (P_22N) - wartość 1 = brak
    /// </summary>
    [XmlElement("P_22N")]
    public int P_22N { get; set; } = 1;

    public bool ShouldSerializeP_22N() => !HasTransportMeans;

    /// <summary>
    /// Czy faktura dotyczy nowych środków transportu
    /// </summary>
    [XmlIgnore]
    public bool HasTransportMeans { get; set; }
}

/// <summary>
/// Procedury marży (PMarzy)
/// Schemat: choice(P_PMarzy + choice(P_PMarzy_2|P_PMarzy_3_1|P_PMarzy_3_2|P_PMarzy_3_3) | P_PMarzyN)
/// </summary>
[XmlType("PMarzy")]
public class MarginProcedures
{
    /// <summary>
    /// Znacznik procedury marży (P_PMarzy) - wartość 1 = tak
    /// </summary>
    [XmlElement("P_PMarzy")]
    public int P_PMarzy { get; set; }

    public bool ShouldSerializeP_PMarzy() => HasMarginProcedure;

    /// <summary>
    /// Procedura marży biur podróży (P_PMarzy_2) - wartość 1 = tak
    /// </summary>
    [XmlElement("P_PMarzy_2")]
    public int P_PMarzy_2 { get; set; }

    public bool ShouldSerializeP_PMarzy_2() => HasMarginProcedure && MarginType == MarginProcedureType.TravelAgency;

    /// <summary>
    /// Procedura marży - towary używane (P_PMarzy_3_1) - wartość 1 = tak
    /// </summary>
    [XmlElement("P_PMarzy_3_1")]
    public int P_PMarzy_3_1 { get; set; }

    public bool ShouldSerializeP_PMarzy_3_1() => HasMarginProcedure && MarginType == MarginProcedureType.UsedGoods;

    /// <summary>
    /// Procedura marży - dzieła sztuki (P_PMarzy_3_2) - wartość 1 = tak
    /// </summary>
    [XmlElement("P_PMarzy_3_2")]
    public int P_PMarzy_3_2 { get; set; }

    public bool ShouldSerializeP_PMarzy_3_2() => HasMarginProcedure && MarginType == MarginProcedureType.ArtWorks;

    /// <summary>
    /// Procedura marży - przedmioty kolekcjonerskie i antyki (P_PMarzy_3_3) - wartość 1 = tak
    /// </summary>
    [XmlElement("P_PMarzy_3_3")]
    public int P_PMarzy_3_3 { get; set; }

    public bool ShouldSerializeP_PMarzy_3_3() => HasMarginProcedure && MarginType == MarginProcedureType.Collectibles;

    /// <summary>
    /// Brak procedury marży (P_PMarzyN) - wartość 1 = brak
    /// </summary>
    [XmlElement("P_PMarzyN")]
    public int P_PMarzyN { get; set; } = 1;

    public bool ShouldSerializeP_PMarzyN() => !HasMarginProcedure;

    /// <summary>
    /// Czy stosowana jest procedura marży
    /// </summary>
    [XmlIgnore]
    public bool HasMarginProcedure { get; set; }

    /// <summary>
    /// Typ procedury marży
    /// </summary>
    [XmlIgnore]
    public MarginProcedureType MarginType { get; set; }
}

/// <summary>
/// Typ procedury marży
/// </summary>
public enum MarginProcedureType
{
    None = 0,
    TravelAgency = 1,
    UsedGoods = 2,
    ArtWorks = 3,
    Collectibles = 4
}
