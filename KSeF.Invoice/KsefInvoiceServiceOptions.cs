using KSeF.Invoice.Services.Validation;

namespace KSeF.Invoice;

/// <summary>
/// Opcje konfiguracyjne dla serwisu KSeF Invoice
/// </summary>
public class KsefInvoiceServiceOptions
{
    /// <summary>
    /// Wersja schematu faktury (FA2/FA3)
    /// Domyślnie: Auto (automatyczne wykrywanie)
    /// </summary>
    public SchemaVersion SchemaVersion { get; set; } = SchemaVersion.Auto;

    /// <summary>
    /// Czy walidować fakturę przed serializacją do XML
    /// Domyślnie: true
    /// </summary>
    public bool ValidateBeforeSerialize { get; set; } = true;

    /// <summary>
    /// Czy walidować fakturę względem schematu XSD
    /// Domyślnie: true
    /// </summary>
    public bool ValidateAgainstXsd { get; set; } = true;

    /// <summary>
    /// Informacja o systemie wytwarzającym faktury
    /// Używana jako domyślna wartość dla nagłówka faktury
    /// </summary>
    public string? DefaultSystemInfo { get; set; }
}
