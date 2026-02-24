using System.ComponentModel;
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Enums;

/// <summary>
/// Oznaczenie dotyczące dostawy towarów i świadczenia usług - GTU (TGTU)
/// </summary>
public enum GoodsServicesMarking
{
    /// <summary>
    /// GTU_01 - Dostawa napojów alkoholowych o zawartości alkoholu powyżej 1,2%,
    /// piwa oraz napojów alkoholowych będących mieszaniną piwa i napojów bezalkoholowych
    /// </summary>
    [Description("GTU_01 - Napoje alkoholowe")]
    [XmlEnum("GTU_01")]
    GTU_01,

    /// <summary>
    /// GTU_02 - Dostawa towarów, o których mowa w art. 103 ust. 5aa ustawy
    /// (paliwa, benzyny, oleje napędowe, gazy)
    /// </summary>
    [Description("GTU_02 - Paliwa")]
    [XmlEnum("GTU_02")]
    GTU_02,

    /// <summary>
    /// GTU_03 - Dostawa oleju opałowego oraz olejów smarowych, pozostałych olejów
    /// </summary>
    [Description("GTU_03 - Oleje opałowe i smarowe")]
    [XmlEnum("GTU_03")]
    GTU_03,

    /// <summary>
    /// GTU_04 - Dostawa wyrobów tytoniowych, suszu tytoniowego, płynu do papierosów elektronicznych
    /// </summary>
    [Description("GTU_04 - Wyroby tytoniowe")]
    [XmlEnum("GTU_04")]
    GTU_04,

    /// <summary>
    /// GTU_05 - Dostawa odpadów - wyłącznie określonych w poz. 79-91 załącznika nr 15 do ustawy
    /// </summary>
    [Description("GTU_05 - Odpady")]
    [XmlEnum("GTU_05")]
    GTU_05,

    /// <summary>
    /// GTU_06 - Dostawa urządzeń elektronicznych oraz części i materiałów do nich
    /// </summary>
    [Description("GTU_06 - Urządzenia elektroniczne")]
    [XmlEnum("GTU_06")]
    GTU_06,

    /// <summary>
    /// GTU_07 - Dostawa pojazdów oraz części samochodowych
    /// </summary>
    [Description("GTU_07 - Pojazdy i części samochodowe")]
    [XmlEnum("GTU_07")]
    GTU_07,

    /// <summary>
    /// GTU_08 - Dostawa metali szlachetnych oraz nieszlachetnych
    /// </summary>
    [Description("GTU_08 - Metale szlachetne i nieszlachetne")]
    [XmlEnum("GTU_08")]
    GTU_08,

    /// <summary>
    /// GTU_09 - Dostawa produktów leczniczych, środków spożywczych specjalnego przeznaczenia
    /// żywieniowego oraz wyrobów medycznych
    /// </summary>
    [Description("GTU_09 - Produkty lecznicze i wyroby medyczne")]
    [XmlEnum("GTU_09")]
    GTU_09,

    /// <summary>
    /// GTU_10 - Dostawa budynków, budowli i gruntów
    /// </summary>
    [Description("GTU_10 - Budynki, budowle i grunty")]
    [XmlEnum("GTU_10")]
    GTU_10,

    /// <summary>
    /// GTU_11 - Świadczenie usług w zakresie przenoszenia uprawnień do emisji gazów cieplarnianych
    /// </summary>
    [Description("GTU_11 - Usługi przenoszenia uprawnień do emisji gazów cieplarnianych")]
    [XmlEnum("GTU_11")]
    GTU_11,

    /// <summary>
    /// GTU_12 - Świadczenie usług o charakterze niematerialnym
    /// (doradcze, księgowe, prawne, zarządcze, szkoleniowe, marketingowe, reklamowe, badania rynku itp.)
    /// </summary>
    [Description("GTU_12 - Usługi niematerialne")]
    [XmlEnum("GTU_12")]
    GTU_12,

    /// <summary>
    /// GTU_13 - Świadczenie usług transportowych i gospodarki magazynowej
    /// </summary>
    [Description("GTU_13 - Usługi transportowe i magazynowe")]
    [XmlEnum("GTU_13")]
    GTU_13
}
