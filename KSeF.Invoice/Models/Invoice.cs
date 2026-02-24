using System.Xml.Serialization;
using KSeF.Invoice.Models.Attachments;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Payments;
using KSeF.Invoice.Models.Summary;

namespace KSeF.Invoice.Models;

/// <summary>
/// Główna klasa reprezentująca fakturę ustrukturyzowaną KSeF (Faktura)
/// Odpowiednik głównego elementu Faktura w schemacie FA(3)
/// Zgodna ze schematem http://crd.gov.pl/wzor/2025/06/25/13775/
/// </summary>
[XmlRoot("Faktura", Namespace = Invoice.KSeFNamespace)]
public class Invoice
{
    /// <summary>
    /// Główna przestrzeń nazw XML schematu KSeF FA(3)
    /// </summary>
    public const string KSeFNamespace = "http://crd.gov.pl/wzor/2025/06/25/13775/";

    /// <summary>
    /// Przestrzeń nazw dla typów definicji (DefinicjeTypy)
    /// </summary>
    public const string DefinitionTypesNamespace = "http://crd.gov.pl/xml/schematy/dziedzinowe/mf/2022/01/05/eD/DefinicjeTypy/";

    #region Nagłówek (Naglowek)

    /// <summary>
    /// Nagłówek faktury zawierający metadane formularza
    /// Element Naglowek w schemacie - zawiera informacje o wersji schematu,
    /// dacie wytworzenia i systemie informatycznym
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("Naglowek", Order = 0)]
    public InvoiceHeader Header { get; set; } = new InvoiceHeader();

    #endregion

    #region Podmioty (Podmiot1, Podmiot2, Podmiot3, PodmiotUpowazniony)

    /// <summary>
    /// Dane sprzedawcy / wystawcy faktury (Podmiot1)
    /// Zgodnie z art. 106e ust. 1 pkt 3 ustawy o VAT
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("Podmiot1", Order = 1)]
    public Seller Seller { get; set; } = new Seller();

    /// <summary>
    /// Dane nabywcy (Podmiot2)
    /// Zgodnie z art. 106e ust. 1 pkt 4 ustawy o VAT
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("Podmiot2", Order = 2)]
    public Buyer Buyer { get; set; } = new Buyer();

    /// <summary>
    /// Lista podmiotów trzecich występujących na fakturze (Podmiot3)
    /// Mogą to być: faktor, odbiorca, podmiot pierwotny, dodatkowy nabywca, wystawca faktury, płatnik
    /// Pole opcjonalne - może zawierać do 100 podmiotów
    /// </summary>
    [XmlElement("Podmiot3", Order = 3)]
    public List<ThirdParty>? Recipients { get; set; }

    /// <summary>
    /// Dane podmiotu upoważnionego do wystawienia faktury (PodmiotUpowazniony)
    /// Dotyczy: organu egzekucyjnego, komornika sądowego, przedstawiciela podatkowego
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("PodmiotUpowazniony", Order = 4)]
    public AuthorizedEntity? AuthorizedEntity { get; set; }

    #endregion

    #region Dane faktury (Fa)

    /// <summary>
    /// Dane merytoryczne faktury (Fa)
    /// Zawiera wszystkie dane dotyczące transakcji: podstawowe dane faktury,
    /// pozycje, podsumowania podatkowe, adnotacje, dane korekt, płatności
    /// Zgodnie z art. 106a - 106q ustawy o VAT
    /// Pole obowiązkowe
    /// </summary>
    [XmlElement("Fa", Order = 5)]
    public InvoiceData InvoiceData { get; set; } = new InvoiceData();

    #endregion

    #region Stopka (Stopka)

    /// <summary>
    /// Stopka faktury zawierająca dodatkowe informacje (Stopka)
    /// Może zawierać: informacje dodatkowe, dane rejestrowe (KRS, REGON, BDO)
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("Stopka", Order = 6)]
    public InvoiceFooter? Footer { get; set; }

    #endregion

    #region Załączniki (Zalacznik)

    /// <summary>
    /// Załącznik do faktury (Zalacznik)
    /// Strukturalne dane dodatkowe dołączone do faktury
    /// Pole opcjonalne
    /// </summary>
    [XmlElement("Zalacznik", Order = 7)]
    public InvoiceAttachmentSection? Attachments { get; set; }

    #endregion

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy faktura ma przypisane podmioty trzecie
    /// </summary>
    [XmlIgnore]
    public bool HasRecipients => Recipients != null && Recipients.Count > 0;

    /// <summary>
    /// Sprawdza czy faktura ma podmiot upoważniony
    /// </summary>
    [XmlIgnore]
    public bool HasAuthorizedEntity => AuthorizedEntity != null;

    /// <summary>
    /// Sprawdza czy faktura ma stopkę
    /// </summary>
    [XmlIgnore]
    public bool HasFooter => Footer != null;

    /// <summary>
    /// Sprawdza czy faktura ma załączniki
    /// </summary>
    [XmlIgnore]
    public bool HasAttachments => Attachments != null && Attachments.DataBlocks != null && Attachments.DataBlocks.Count > 0;

    /// <summary>
    /// Sprawdza czy faktura jest fakturą korygującą
    /// </summary>
    [XmlIgnore]
    public bool IsCorrection => InvoiceData.InvoiceType == InvoiceType.KOR ||
                                 InvoiceData.InvoiceType == InvoiceType.KOR_ZAL ||
                                 InvoiceData.InvoiceType == InvoiceType.KOR_ROZ;

    /// <summary>
    /// Sprawdza czy faktura jest fakturą zaliczkową
    /// </summary>
    [XmlIgnore]
    public bool IsAdvancePayment => InvoiceData.InvoiceType == InvoiceType.ZAL;

    /// <summary>
    /// Sprawdza czy faktura jest fakturą rozliczeniową
    /// </summary>
    [XmlIgnore]
    public bool IsSettlement => InvoiceData.InvoiceType == InvoiceType.ROZ;

    /// <summary>
    /// Sprawdza czy faktura jest fakturą uproszczoną
    /// </summary>
    [XmlIgnore]
    public bool IsSimplified => InvoiceData.InvoiceType == InvoiceType.UPR;

    #endregion
}
