using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Typy ładunków/opakowań (TLadunek)
/// </summary>
public enum PackageType
{
    /// <summary>
    /// Bańka
    /// </summary>
    [Description("Bańka")]
    [XmlEnum("1")]
    Canister = 1,

    /// <summary>
    /// Beczka
    /// </summary>
    [Description("Beczka")]
    [XmlEnum("2")]
    Barrel = 2,

    /// <summary>
    /// Butla
    /// </summary>
    [Description("Butla")]
    [XmlEnum("3")]
    Cylinder = 3,

    /// <summary>
    /// Karton
    /// </summary>
    [Description("Karton")]
    [XmlEnum("4")]
    Carton = 4,

    /// <summary>
    /// Kanister
    /// </summary>
    [Description("Kanister")]
    [XmlEnum("5")]
    JerryCan = 5,

    /// <summary>
    /// Klatka
    /// </summary>
    [Description("Klatka")]
    [XmlEnum("6")]
    Cage = 6,

    /// <summary>
    /// Kontener
    /// </summary>
    [Description("Kontener")]
    [XmlEnum("7")]
    Container = 7,

    /// <summary>
    /// Kosz/koszyk
    /// </summary>
    [Description("Kosz/koszyk")]
    [XmlEnum("8")]
    Basket = 8,

    /// <summary>
    /// Łubianka
    /// </summary>
    [Description("Łubianka")]
    [XmlEnum("9")]
    Punnet = 9,

    /// <summary>
    /// Opakowanie zbiorcze
    /// </summary>
    [Description("Opakowanie zbiorcze")]
    [XmlEnum("10")]
    BulkPackage = 10,

    /// <summary>
    /// Paczka
    /// </summary>
    [Description("Paczka")]
    [XmlEnum("11")]
    Parcel = 11,

    /// <summary>
    /// Pakiet
    /// </summary>
    [Description("Pakiet")]
    [XmlEnum("12")]
    Bundle = 12,

    /// <summary>
    /// Paleta
    /// </summary>
    [Description("Paleta")]
    [XmlEnum("13")]
    Pallet = 13,

    /// <summary>
    /// Pojemnik
    /// </summary>
    [Description("Pojemnik")]
    [XmlEnum("14")]
    Receptacle = 14,

    /// <summary>
    /// Pojemnik do ładunków masowych stałych
    /// </summary>
    [Description("Pojemnik do ładunków masowych stałych")]
    [XmlEnum("15")]
    BulkSolidContainer = 15,

    /// <summary>
    /// Pojemnik do ładunków masowych w postaci płynnej
    /// </summary>
    [Description("Pojemnik do ładunków masowych w postaci płynnej")]
    [XmlEnum("16")]
    BulkLiquidContainer = 16,

    /// <summary>
    /// Pudełko
    /// </summary>
    [Description("Pudełko")]
    [XmlEnum("17")]
    Box = 17,

    /// <summary>
    /// Puszka
    /// </summary>
    [Description("Puszka")]
    [XmlEnum("18")]
    Can = 18,

    /// <summary>
    /// Skrzynia
    /// </summary>
    [Description("Skrzynia")]
    [XmlEnum("19")]
    Crate = 19,

    /// <summary>
    /// Worek
    /// </summary>
    [Description("Worek")]
    [XmlEnum("20")]
    Sack = 20
}
