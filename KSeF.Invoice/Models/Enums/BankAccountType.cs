using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Typy rachunków własnych banku (TRachunekWlasnyBanku)
/// </summary>
public enum BankAccountType
{
    /// <summary>
    /// Rachunek banku lub rachunek spółdzielczej kasy oszczędnościowo-kredytowej służący do dokonywania
    /// rozliczeń z tytułu nabywanych przez ten bank lub tę kasę wierzytelności pieniężnych
    /// </summary>
    [Description("Rachunek banku/SKOK do rozliczeń z tytułu nabywanych wierzytelności")]
    [XmlEnum("1")]
    ReceivablesSettlement = 1,

    /// <summary>
    /// Rachunek banku lub rachunek spółdzielczej kasy oszczędnościowo-kredytowej wykorzystywany przez ten bank
    /// lub tę kasę do pobrania należności od nabywcy towarów lub usług za dostawę towarów lub świadczenie usług,
    /// potwierdzone fakturą, i przekazania jej w całości albo części dostawcy towarów lub usługodawcy
    /// </summary>
    [Description("Rachunek banku/SKOK do pobierania należności i przekazywania dostawcy")]
    [XmlEnum("2")]
    CollectionAndTransfer = 2,

    /// <summary>
    /// Rachunek banku lub rachunek spółdzielczej kasy oszczędnościowo-kredytowej prowadzony przez ten bank
    /// lub tę kasę w ramach gospodarki własnej, niebędący rachunkiem rozliczeniowym
    /// </summary>
    [Description("Rachunek banku/SKOK w ramach gospodarki własnej")]
    [XmlEnum("3")]
    InternalOperations = 3
}
