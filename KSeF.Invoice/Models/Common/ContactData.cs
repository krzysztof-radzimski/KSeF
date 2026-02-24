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
