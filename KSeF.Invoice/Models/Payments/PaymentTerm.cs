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
    /// Opis terminu płatności (TerminOpis) - complex type
    /// Zawiera Ilosc, Jednostka, ZdarzeniePoczatkowe
    /// </summary>
    [XmlElement("TerminOpis")]
    public PaymentTermDescription? DueDateDescription { get; set; }

    #region ShouldSerialize

    public bool ShouldSerializeDueDateString() => DueDate.HasValue;

    public bool ShouldSerializeDueDateDescription() => DueDateDescription != null;

    #endregion
}
