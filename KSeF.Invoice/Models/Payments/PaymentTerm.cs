using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Termin płatności faktury (TerminPlatnosci)
/// Określa datę lub opis terminu płatności
/// </summary>
public class PaymentTerm
{
    /// <summary>
    /// Termin płatności - data (Termin) - proxy string dla serializacji XML
    /// </summary>
    [XmlElement("Termin")]
    public string? DueDateString
    {
        get => DueDate?.ToString("yyyy-MM-dd");
        set => DueDate = string.IsNullOrEmpty(value) ? null : DateOnly.Parse(value);
    }

    /// <summary>
    /// Termin płatności - data (Termin)
    /// </summary>
    [XmlIgnore]
    public DateOnly? DueDate { get; set; }

    /// <summary>
    /// Termin płatności - opis (TerminOpis)
    /// Tekstowy opis terminu płatności (np. "14 dni od daty wystawienia")
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("TerminOpis")]
    public string? DueDateDescription { get; set; }
}
