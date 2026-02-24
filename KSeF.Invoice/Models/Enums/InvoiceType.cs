using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Rodzaj faktury (TRodzajFaktury)
/// </summary>
public enum InvoiceType
{
    /// <summary>
    /// Faktura podstawowa
    /// </summary>
    [Description("Faktura podstawowa")]
    [XmlEnum("VAT")]
    VAT,

    /// <summary>
    /// Faktura korygująca
    /// </summary>
    [Description("Faktura korygująca")]
    [XmlEnum("KOR")]
    KOR,

    /// <summary>
    /// Faktura dokumentująca otrzymanie zapłaty lub jej części przed dokonaniem czynności
    /// oraz faktura wystawiona w związku z art. 106f ust. 4 ustawy (faktura zaliczkowa)
    /// </summary>
    [Description("Faktura zaliczkowa - dokumentująca otrzymanie zapłaty lub jej części przed dokonaniem czynności")]
    [XmlEnum("ZAL")]
    ZAL,

    /// <summary>
    /// Faktura wystawiona w związku z art. 106f ust. 3 ustawy (faktura rozliczeniowa)
    /// </summary>
    [Description("Faktura rozliczeniowa - wystawiona w związku z art. 106f ust. 3 ustawy")]
    [XmlEnum("ROZ")]
    ROZ,

    /// <summary>
    /// Faktura, o której mowa w art. 106e ust. 5 pkt 3 ustawy (faktura uproszczona)
    /// </summary>
    [Description("Faktura uproszczona - o której mowa w art. 106e ust. 5 pkt 3 ustawy")]
    [XmlEnum("UPR")]
    UPR,

    /// <summary>
    /// Faktura korygująca fakturę dokumentującą otrzymanie zapłaty lub jej części przed dokonaniem czynności
    /// oraz fakturę wystawioną w związku z art. 106f ust. 4 ustawy (faktura korygująca fakturę zaliczkową)
    /// </summary>
    [Description("Faktura korygująca fakturę zaliczkową")]
    [XmlEnum("KOR_ZAL")]
    KOR_ZAL,

    /// <summary>
    /// Faktura korygująca fakturę wystawioną w związku z art. 106f ust. 3 ustawy
    /// </summary>
    [Description("Faktura korygująca fakturę rozliczeniową")]
    [XmlEnum("KOR_ROZ")]
    KOR_ROZ
}
