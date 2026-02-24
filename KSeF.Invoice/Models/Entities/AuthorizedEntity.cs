using System.Xml.Serialization;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Entities;

/// <summary>
/// Podmiot upoważniony do wystawienia faktury (PodmiotUpowazniony)
/// Odpowiednik elementu PodmiotUpowazniony w schemacie KSeF
/// Dotyczy: organu egzekucyjnego, komornika sądowego, przedstawiciela podatkowego
/// </summary>
[XmlRoot("PodmiotUpowazniony")]
public class AuthorizedEntity
{
    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP) podmiotu upoważnionego
    /// Format: 10 cyfr bez kresek
    /// </summary>
    [XmlElement("NIP")]
    public string TaxId { get; set; } = string.Empty;

    /// <summary>
    /// Imię i nazwisko lub pełna nazwa firmy podmiotu upoważnionego
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("Nazwa")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Adres podmiotu upoważnionego
    /// </summary>
    [XmlElement("Adres")]
    public Address? Address { get; set; }

    /// <summary>
    /// Rola podmiotu upoważnionego
    /// Określa czy jest to organ egzekucyjny, komornik sądowy czy przedstawiciel podatkowy
    /// </summary>
    [XmlElement("Rola")]
    public AuthorizedSubjectRole Role { get; set; }

    /// <summary>
    /// Sprawdza czy podmiot jest organem egzekucyjnym
    /// </summary>
    [XmlIgnore]
    public bool IsEnforcementAuthority => Role == AuthorizedSubjectRole.EnforcementAuthority;

    /// <summary>
    /// Sprawdza czy podmiot jest komornikiem sądowym
    /// </summary>
    [XmlIgnore]
    public bool IsBailiff => Role == AuthorizedSubjectRole.Bailiff;

    /// <summary>
    /// Sprawdza czy podmiot jest przedstawicielem podatkowym
    /// </summary>
    [XmlIgnore]
    public bool IsTaxRepresentative => Role == AuthorizedSubjectRole.TaxRepresentative;

    /// <summary>
    /// Sprawdza czy adres jest zdefiniowany
    /// </summary>
    [XmlIgnore]
    public bool HasAddress => Address != null;
}
