/*=====================================================================================

    KSeF.Invoice
    Klasa reprezentująca dane kontaktowe podmiotu na fakturze.
    Zawiera adres e-mail oraz numer telefonu. Wykorzystywana
    w danych sprzedawcy, nabywcy i podmiotów trzecich.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Common;

/// <summary>
/// Dane kontaktowe podmiotu
/// </summary>
public class ContactData
{
    /// <summary>
    /// Adres e-mail
    /// </summary>
    [XmlElement("Email")]
    public string? Email { get; set; }

    /// <summary>
    /// Numer telefonu (maksymalnie 16 znaków)
    /// </summary>
    [XmlElement("Telefon")]
    public string? Phone { get; set; }
}
