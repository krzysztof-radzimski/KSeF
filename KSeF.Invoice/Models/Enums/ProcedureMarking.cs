using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Oznaczenia dotyczące procedur dla faktur (TOznaczenieProcedury)
/// </summary>
public enum ProcedureMarking
{
    /// <summary>
    /// Wewnątrzwspólnotowa sprzedaż towarów na odległość oraz świadczenie usług telekomunikacyjnych,
    /// nadawczych i elektronicznych (WSTO_EE)
    /// </summary>
    [Description("WSTO_EE - Wewnątrzwspólnotowa sprzedaż towarów na odległość")]
    [XmlEnum("WSTO_EE")]
    WSTO_EE,

    /// <summary>
    /// Dostawa przez ułatwiającego, o którym mowa w art. 7a ust. 2 ustawy (IED)
    /// </summary>
    [Description("IED - Dostawa przez ułatwiającego")]
    [XmlEnum("IED")]
    IED,

    /// <summary>
    /// Transakcja trójstronna - dostawa (TT_D)
    /// </summary>
    [Description("TT_D - Transakcja trójstronna - dostawa")]
    [XmlEnum("TT_D")]
    TT_D,

    /// <summary>
    /// Import towarów w procedurze celnej 42 (I_42)
    /// </summary>
    [Description("I_42 - Import towarów w procedurze celnej 42")]
    [XmlEnum("I_42")]
    I_42,

    /// <summary>
    /// Import towarów w procedurze celnej 63 (I_63)
    /// </summary>
    [Description("I_63 - Import towarów w procedurze celnej 63")]
    [XmlEnum("I_63")]
    I_63,

    /// <summary>
    /// Transfer bonu jednego przeznaczenia (B_SPV)
    /// </summary>
    [Description("B_SPV - Transfer bonu jednego przeznaczenia")]
    [XmlEnum("B_SPV")]
    B_SPV,

    /// <summary>
    /// Dostawa towarów oraz świadczenie usług, dla których przekazano bon jednego przeznaczenia (B_SPV_DOSTAWA)
    /// </summary>
    [Description("B_SPV_DOSTAWA - Dostawa związana z bonem jednego przeznaczenia")]
    [XmlEnum("B_SPV_DOSTAWA")]
    B_SPV_DOSTAWA,

    /// <summary>
    /// Świadczenie usług pośrednictwa oraz innych usług dotyczących bonów różnego przeznaczenia (B_MPV_PROWIZJA)
    /// </summary>
    [Description("B_MPV_PROWIZJA - Usługi pośrednictwa dot. bonów różnego przeznaczenia")]
    [XmlEnum("B_MPV_PROWIZJA")]
    B_MPV_PROWIZJA
}
