using System.Xml.Serialization;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Payments;

/// <summary>
/// Informacja o formie płatności (FormaPlatnosci)
/// Określa metodę płatności wraz z opcjonalną kwotą
/// </summary>
public class PaymentMethodInfo
{
    /// <summary>
    /// Forma płatności (FormaPlatnosci)
    /// Określa sposób realizacji płatności za fakturę
    /// </summary>
    [XmlElement("FormaPlatnosci")]
    public PaymentMethod Method { get; set; }

    /// <summary>
    /// Kwota płatności dla danej formy płatności (KwotaPlatnosci)
    /// Opcjonalna - używana gdy faktura jest opłacana różnymi metodami
    /// </summary>
    [XmlElement("KwotaPlatnosci")]
    public decimal? Amount { get; set; }

    /// <summary>
    /// Pomocnicza właściwość dla serializacji - czy serializować kwotę
    /// </summary>
    [XmlIgnore]
    public bool AmountSpecified => Amount.HasValue;
}
