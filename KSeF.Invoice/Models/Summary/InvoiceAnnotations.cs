using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Summary;

/// <summary>
/// Adnotacje faktury - specjalne oznaczenia i procedury stosowane na fakturze
/// Odpowiednik elementu Adnotacje w schemacie FA(3)
/// Zawiera informacje o szczególnych procedurach podatkowych i transakcyjnych
/// </summary>
[XmlType("Adnotacje")]
public class InvoiceAnnotations
{
    #region Metoda kasowa (P_16)

    /// <summary>
    /// Metoda kasowa (P_16)
    /// Wartość 1 - faktura wystawiona przez podatnika stosującego metodę kasową
    /// zgodnie z art. 21 ustawy o VAT
    /// Wartość 2 - faktura nie dotyczy metody kasowej
    /// Pole obowiązkowe
    /// </summary>
    /// <remarks>
    /// Metoda kasowa polega na rozliczaniu VAT w momencie otrzymania zapłaty,
    /// a nie w momencie wystawienia faktury. Dotyczy głównie małych podatników.
    /// </remarks>
    [XmlElement("P_16")]
    public AnnotationValue CashMethod { get; set; } = AnnotationValue.No;

    #endregion

    #region Samofakturowanie (P_17)

    /// <summary>
    /// Samofakturowanie (P_17)
    /// Wartość 1 - faktura wystawiona przez nabywcę w imieniu sprzedawcy (self-billing)
    /// zgodnie z art. 106d ust. 1 ustawy o VAT
    /// Wartość 2 - faktura nie jest wystawiona w trybie samofakturowania
    /// Pole obowiązkowe
    /// </summary>
    /// <remarks>
    /// Samofakturowanie to procedura, w której nabywca wystawia fakturę w imieniu
    /// i na rzecz sprzedawcy, na podstawie uprzedniego porozumienia stron.
    /// </remarks>
    [XmlElement("P_17")]
    public AnnotationValue SelfBilling { get; set; } = AnnotationValue.No;

    #endregion

    #region Odwrotne obciążenie (P_18)

    /// <summary>
    /// Odwrotne obciążenie (P_18)
    /// Wartość 1 - transakcja objęta mechanizmem odwrotnego obciążenia
    /// zgodnie z art. 17 ust. 1 pkt 7 lub 8 ustawy o VAT
    /// Wartość 2 - transakcja nie jest objęta odwrotnym obciążeniem
    /// Pole obowiązkowe
    /// </summary>
    /// <remarks>
    /// Odwrotne obciążenie (reverse charge) oznacza, że VAT rozlicza nabywca,
    /// a nie sprzedawca. Dotyczy m.in. usług budowlanych i niektórych towarów.
    /// </remarks>
    [XmlElement("P_18")]
    public AnnotationValue ReverseCharge { get; set; } = AnnotationValue.No;

    #endregion

    #region Mechanizm podzielonej płatności (P_18A)

    /// <summary>
    /// Mechanizm podzielonej płatności - split payment (P_18A)
    /// Wartość 1 - transakcja objęta obowiązkowym mechanizmem podzielonej płatności
    /// zgodnie z art. 108a ust. 1a ustawy o VAT
    /// Wartość 2 - transakcja nie podlega obowiązkowemu MPP
    /// Pole obowiązkowe
    /// </summary>
    /// <remarks>
    /// Split payment (MPP) polega na rozdzieleniu płatności - kwota netto trafia
    /// na rachunek bieżący, a kwota VAT na specjalny rachunek VAT.
    /// Obowiązkowe dla transakcji powyżej 15 000 zł dotyczących towarów/usług
    /// z załącznika nr 15 do ustawy o VAT.
    /// </remarks>
    [XmlElement("P_18A")]
    public AnnotationValue SplitPayment { get; set; } = AnnotationValue.No;

    #endregion

    #region Zwolnienie z VAT (Zwolnienie)

    /// <summary>
    /// Informacja o podstawie zwolnienia z VAT (Zwolnienie)
    /// Wskazuje podstawę prawną zwolnienia z podatku VAT
    /// Element opcjonalny - wypełniany gdy faktura zawiera pozycje zwolnione z VAT
    /// </summary>
    /// <remarks>
    /// Wymagane gdy na fakturze występują pozycje ze stawką ZW (zwolniona).
    /// Określa przepis ustawy lub dyrektywy, na podstawie którego zastosowano zwolnienie.
    /// </remarks>
    [XmlElement("Zwolnienie")]
    public VatExemption? Exemption { get; set; }

    #endregion

    #region Nowe środki transportu (NoweSrodkiTransportu)

    /// <summary>
    /// Informacja o nowych środkach transportu (NoweSrodkiTransportu)
    /// Dotyczy wewnątrzwspólnotowej dostawy nowych środków transportu
    /// zgodnie z art. 106e ust. 3 ustawy o VAT
    /// Element opcjonalny - wypełniany gdy faktura dotyczy nowego środka transportu
    /// </summary>
    /// <remarks>
    /// Nowy środek transportu to pojazd mechaniczny, statek lub samolot,
    /// który spełnia określone warunki dotyczące przebiegu/godzin użytkowania
    /// i okresu od pierwszej rejestracji.
    /// </remarks>
    [XmlElement("NoweSrodkiTransportu")]
    public NewTransportMeans? NewTransportMeans { get; set; }

    #endregion

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy faktura dotyczy metody kasowej
    /// </summary>
    [XmlIgnore]
    public bool IsCashMethod => CashMethod == AnnotationValue.Yes;

    /// <summary>
    /// Sprawdza czy faktura jest wystawiona w trybie samofakturowania
    /// </summary>
    [XmlIgnore]
    public bool IsSelfBilling => SelfBilling == AnnotationValue.Yes;

    /// <summary>
    /// Sprawdza czy transakcja jest objęta odwrotnym obciążeniem
    /// </summary>
    [XmlIgnore]
    public bool IsReverseCharge => ReverseCharge == AnnotationValue.Yes;

    /// <summary>
    /// Sprawdza czy transakcja podlega obowiązkowemu split payment
    /// </summary>
    [XmlIgnore]
    public bool IsSplitPayment => SplitPayment == AnnotationValue.Yes;

    /// <summary>
    /// Sprawdza czy faktura zawiera informację o zwolnieniu z VAT
    /// </summary>
    [XmlIgnore]
    public bool HasExemption => Exemption != null;

    /// <summary>
    /// Sprawdza czy faktura dotyczy nowego środka transportu
    /// </summary>
    [XmlIgnore]
    public bool HasNewTransportMeans => NewTransportMeans != null;

    #endregion
}

/// <summary>
/// Wartość adnotacji (P_16, P_17, P_18, P_18A)
/// Określa czy dana adnotacja ma zastosowanie do faktury
/// </summary>
public enum AnnotationValue
{
    /// <summary>
    /// Wartość 1 - Tak, adnotacja ma zastosowanie
    /// </summary>
    [XmlEnum("1")]
    Yes = 1,

    /// <summary>
    /// Wartość 2 - Nie, adnotacja nie ma zastosowania
    /// </summary>
    [XmlEnum("2")]
    No = 2
}

/// <summary>
/// Informacja o podstawie zwolnienia z VAT
/// Odpowiednik elementu Zwolnienie w schemacie FA(3)
/// </summary>
[XmlType("Zwolnienie")]
public class VatExemption
{
    /// <summary>
    /// Przyczyna zwolnienia z VAT (P_19)
    /// Opis przepisu ustawy albo aktu wydanego na podstawie ustawy,
    /// zgodnie z którym podatnik stosuje zwolnienie od podatku
    /// Zgodnie z art. 106e ust. 1 pkt 19 ustawy o VAT
    /// Maksymalnie 256 znaków
    /// Pole obowiązkowe w ramach elementu Zwolnienie
    /// </summary>
    /// <remarks>
    /// Przykładowe wartości:
    /// - "art. 43 ust. 1 pkt 2 ustawy o VAT" - towary używane
    /// - "art. 43 ust. 1 pkt 10 ustawy o VAT" - nieruchomości
    /// - "art. 43 ust. 1 pkt 29 ustawy o VAT" - usługi szkoleniowe
    /// </remarks>
    [XmlElement("P_19")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Wskazanie przepisu dyrektywy jako podstawy zwolnienia (P_19A)
    /// Przepis dyrektywy 2006/112/WE, który zwalnia od podatku taką dostawę
    /// towarów lub takie świadczenie usług
    /// Pole opcjonalne - stosowane gdy podstawą zwolnienia jest dyrektywa
    /// </summary>
    /// <remarks>
    /// Wypełniane gdy podstawą zwolnienia jest bezpośrednio przepis dyrektywy VAT,
    /// a nie przepis krajowy implementujący dyrektywę.
    /// </remarks>
    [XmlElement("P_19A")]
    public string? DirectiveBasis { get; set; }

    /// <summary>
    /// Wskazanie innej podstawy prawnej zwolnienia (P_19B)
    /// Inna podstawa prawna wskazująca na to, że dostawa towarów
    /// lub świadczenie usług korzysta ze zwolnienia
    /// Pole opcjonalne - stosowane gdy podstawą zwolnienia jest inny akt prawny
    /// </summary>
    /// <remarks>
    /// Stosowane gdy zwolnienie wynika z aktów innych niż ustawa o VAT
    /// lub dyrektywa VAT, np. umów międzynarodowych.
    /// </remarks>
    [XmlElement("P_19B")]
    public string? OtherLegalBasis { get; set; }

    /// <summary>
    /// Wskazanie przepisu ustawy lub aktu krajowego jako podstawy zwolnienia (P_19C)
    /// Przepis ustawy albo aktu wydanego na podstawie ustawy uprawniający
    /// do stosowania zwolnienia
    /// Pole opcjonalne - alternatywa dla P_19
    /// </summary>
    [XmlElement("P_19C")]
    public string? NationalLegalBasis { get; set; }
}

/// <summary>
/// Informacja o nowym środku transportu
/// Odpowiednik elementu NoweSrodkiTransportu w schemacie FA(3)
/// Dane wymagane przy wewnątrzwspólnotowej dostawie nowych środków transportu
/// </summary>
[XmlType("NoweSrodkiTransportu")]
public class NewTransportMeans
{
    /// <summary>
    /// Znacznik nowego środka transportu (P_22)
    /// Wartość true oznacza, że faktura dokumentuje dostawę nowego środka transportu
    /// Pole obowiązkowe w ramach elementu NoweSrodkiTransportu
    /// </summary>
    [XmlElement("P_22")]
    public bool IsNewTransportMeans { get; set; }

    /// <summary>
    /// Data dopuszczenia nowego środka transportu do użytku (P_42_5)
    /// Data pierwszej rejestracji lub pierwszego dopuszczenia do ruchu
    /// Format: YYYY-MM-DD
    /// Pole opcjonalne - wymagane dla pojazdów lądowych
    /// </summary>
    /// <remarks>
    /// Dotyczy pojazdów lądowych napędzanych silnikiem o pojemności skokowej
    /// większej niż 48 cm³ lub o mocy większej niż 7,2 kW.
    /// </remarks>
    [XmlElement("P_42_5")]
    public DateOnly? FirstRegistrationDate { get; set; }

    /// <summary>
    /// Przebieg pojazdu (P_42_6)
    /// Liczba przejechanych kilometrów przez pojazd lądowy
    /// Wyrażona jako liczba całkowita
    /// Pole opcjonalne - wymagane dla pojazdów lądowych
    /// </summary>
    [XmlElement("P_42_6")]
    public int? Mileage { get; set; }

    /// <summary>
    /// Liczba godzin używania statku (P_42_7)
    /// Liczba godzin, przez które jednostka pływająca była używana
    /// Wyrażona jako liczba całkowita
    /// Pole opcjonalne - wymagane dla jednostek pływających
    /// </summary>
    /// <remarks>
    /// Dotyczy jednostek pływających o długości większej niż 7,5 m
    /// (z wyjątkiem jednostek do żeglugi przybrzeżnej do celów handlowych).
    /// </remarks>
    [XmlElement("P_42_7")]
    public int? BoatOperatingHours { get; set; }

    /// <summary>
    /// Liczba godzin używania statku powietrznego (P_42_8)
    /// Liczba godzin, przez które statek powietrzny był używany
    /// Wyrażona jako liczba całkowita
    /// Pole opcjonalne - wymagane dla statków powietrznych
    /// </summary>
    /// <remarks>
    /// Dotyczy statków powietrznych o masie startowej większej niż 1550 kg
    /// (z wyjątkiem używanych przez przewoźników lotniczych do celów zarobkowych).
    /// </remarks>
    [XmlElement("P_42_8")]
    public int? AircraftOperatingHours { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy dane dotyczą pojazdu lądowego
    /// </summary>
    [XmlIgnore]
    public bool IsLandVehicle => FirstRegistrationDate.HasValue || Mileage.HasValue;

    /// <summary>
    /// Sprawdza czy dane dotyczą jednostki pływającej
    /// </summary>
    [XmlIgnore]
    public bool IsWatercraft => BoatOperatingHours.HasValue;

    /// <summary>
    /// Sprawdza czy dane dotyczą statku powietrznego
    /// </summary>
    [XmlIgnore]
    public bool IsAircraft => AircraftOperatingHours.HasValue;

    #endregion
}
