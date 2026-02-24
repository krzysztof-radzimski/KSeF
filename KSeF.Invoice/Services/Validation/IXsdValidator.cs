namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Wersja schematu XSD faktury KSeF
/// </summary>
public enum SchemaVersion
{
    /// <summary>
    /// Automatyczne wykrywanie wersji na podstawie przestrzeni nazw XML
    /// </summary>
    Auto,

    /// <summary>
    /// Schemat FA(2) - starsza wersja
    /// </summary>
    FA2,

    /// <summary>
    /// Schemat FA(3) - aktualna wersja
    /// </summary>
    FA3
}

/// <summary>
/// Interfejs walidatora XML względem schematu XSD
/// </summary>
public interface IXsdValidator
{
    /// <summary>
    /// Waliduje dokument XML względem schematu XSD
    /// </summary>
    /// <param name="xml">Dokument XML jako string</param>
    /// <param name="schemaVersion">Wersja schematu (domyślnie automatyczne wykrywanie)</param>
    /// <returns>Wynik walidacji z listą błędów i ostrzeżeń</returns>
    ValidationResult ValidateXml(string xml, SchemaVersion schemaVersion = SchemaVersion.Auto);

    /// <summary>
    /// Waliduje dokument XML ze strumienia względem schematu XSD
    /// </summary>
    /// <param name="stream">Strumień z dokumentem XML</param>
    /// <param name="schemaVersion">Wersja schematu (domyślnie automatyczne wykrywanie)</param>
    /// <returns>Wynik walidacji z listą błędów i ostrzeżeń</returns>
    ValidationResult ValidateXml(Stream stream, SchemaVersion schemaVersion = SchemaVersion.Auto);

    /// <summary>
    /// Waliduje fakturę względem schematu XSD (serializuje do XML i waliduje)
    /// </summary>
    /// <param name="invoice">Faktura do walidacji</param>
    /// <returns>Wynik walidacji z listą błędów i ostrzeżeń</returns>
    ValidationResult Validate(Models.Invoice invoice);

    /// <summary>
    /// Sprawdza czy schematy XSD są prawidłowo załadowane
    /// </summary>
    /// <returns>True jeśli schematy są dostępne</returns>
    bool AreSchemasLoaded { get; }

    /// <summary>
    /// Dostępne wersje schematów
    /// </summary>
    IReadOnlyCollection<SchemaVersion> AvailableSchemas { get; }
}
