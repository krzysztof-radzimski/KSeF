using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using KSeF.Invoice.Services.Serialization;

namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Walidator XML względem schematu XSD dla faktur KSeF
/// </summary>
public class XsdValidator : IXsdValidator, IInvoiceValidator
{
    private readonly IInvoiceSerializer _serializer;
    private readonly ConcurrentDictionary<SchemaVersion, XmlSchemaSet> _schemaCache;
    private readonly object _loadLock = new();
    private bool _schemasInitialized;

    /// <summary>
    /// Przestrzeń nazw dla schematu FA(3)
    /// </summary>
    public const string FA3Namespace = "http://crd.gov.pl/wzor/2025/06/25/13775/";

    /// <summary>
    /// Przestrzeń nazw dla schematu FA(2)
    /// </summary>
    public const string FA2Namespace = "http://crd.gov.pl/wzor/2023/06/29/12648/";

    /// <summary>
    /// Przestrzeń nazw dla typów definicji
    /// </summary>
    public const string DefinitionTypesNamespace = "http://crd.gov.pl/xml/schematy/dziedzinowe/mf/2022/01/05/eD/DefinicjeTypy/";

    /// <summary>
    /// Tworzy nową instancję walidatora XSD
    /// </summary>
    /// <param name="serializer">Serializer faktur do XML</param>
    public XsdValidator(IInvoiceSerializer serializer)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _schemaCache = new ConcurrentDictionary<SchemaVersion, XmlSchemaSet>();
    }

    /// <inheritdoc />
    public bool AreSchemasLoaded => _schemasInitialized && _schemaCache.Count > 0;

    /// <inheritdoc />
    public IReadOnlyCollection<SchemaVersion> AvailableSchemas
    {
        get
        {
            EnsureSchemasLoaded();
            return _schemaCache.Keys.Where(k => k != SchemaVersion.Auto).ToList().AsReadOnly();
        }
    }

    /// <inheritdoc />
    public ValidationResult Validate(Models.Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        try
        {
            var xml = _serializer.SerializeToXml(invoice);
            return ValidateXml(xml, SchemaVersion.Auto);
        }
        catch (Exception ex)
        {
            var result = new ValidationResult();
            result.AddError("XSD_SERIALIZATION_ERROR", $"Błąd serializacji faktury do XML: {ex.Message}");
            return result;
        }
    }

    /// <inheritdoc />
    public ValidationResult ValidateXml(string xml, SchemaVersion schemaVersion = SchemaVersion.Auto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        using var reader = new StringReader(xml);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return ValidateXmlInternal(stream, schemaVersion);
    }

    /// <inheritdoc />
    public ValidationResult ValidateXml(Stream stream, SchemaVersion schemaVersion = SchemaVersion.Auto)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return ValidateXmlInternal(stream, schemaVersion);
    }

    private ValidationResult ValidateXmlInternal(Stream stream, SchemaVersion schemaVersion)
    {
        var result = new ValidationResult();
        var errors = new List<ValidationEventArgs>();

        try
        {
            EnsureSchemasLoaded();

            // Określ wersję schematu
            var effectiveVersion = schemaVersion;
            if (schemaVersion == SchemaVersion.Auto)
            {
                effectiveVersion = DetectSchemaVersion(stream);
                stream.Position = 0; // Reset pozycji po detekcji
            }

            // Pobierz skompilowany schemat
            if (!_schemaCache.TryGetValue(effectiveVersion, out var schemaSet))
            {
                result.AddError("XSD_SCHEMA_NOT_FOUND", $"Schemat dla wersji {effectiveVersion} nie jest dostępny");
                return result;
            }

            // Konfiguracja walidacji
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet,
                ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints |
                                 XmlSchemaValidationFlags.ReportValidationWarnings
            };

            settings.ValidationEventHandler += (sender, e) => errors.Add(e);

            // Wykonaj walidację
            using var reader = XmlReader.Create(stream, settings);
            while (reader.Read()) { }

            // Przekonwertuj błędy XSD na ValidationResult
            foreach (var error in errors)
            {
                ConvertXsdError(error, result);
            }
        }
        catch (XmlSchemaException ex)
        {
            result.AddError("XSD_SCHEMA_ERROR", $"Błąd schematu XSD: {ex.Message}", GetFieldNameFromXsdError(ex));
        }
        catch (XmlException ex)
        {
            result.AddError("XSD_XML_ERROR", $"Błąd parsowania XML: {ex.Message} (linia {ex.LineNumber}, pozycja {ex.LinePosition})");
        }
        catch (Exception ex)
        {
            result.AddError("XSD_VALIDATION_ERROR", $"Nieoczekiwany błąd walidacji: {ex.Message}");
        }

        return result;
    }

    private SchemaVersion DetectSchemaVersion(Stream stream)
    {
        try
        {
            using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var ns = reader.NamespaceURI;

                    if (ns == FA3Namespace)
                        return SchemaVersion.FA3;

                    if (ns == FA2Namespace)
                        return SchemaVersion.FA2;

                    // Domyślnie FA3 jeśli nie rozpoznano
                    break;
                }
            }
        }
        catch
        {
            // W przypadku błędu, domyślnie FA3
        }

        return SchemaVersion.FA3;
    }

    private void ConvertXsdError(ValidationEventArgs e, ValidationResult result)
    {
        var fieldName = GetFieldNameFromXsdError(e.Exception);
        var lineInfo = e.Exception is XmlSchemaException xse && xse.LineNumber > 0
            ? $" (linia {xse.LineNumber}, pozycja {xse.LinePosition})"
            : "";

        var message = $"{e.Message}{lineInfo}";

        if (e.Severity == XmlSeverityType.Error)
        {
            var code = GetErrorCode(e.Exception);
            result.AddError(code, message, fieldName);
        }
        else
        {
            var code = GetWarningCode(e.Exception);
            result.AddWarning(code, message, fieldName);
        }
    }

    private static string GetErrorCode(XmlSchemaException? ex)
    {
        if (ex == null)
            return "XSD_ERROR";

        // Mapowanie typowych błędów XSD na kody
        var message = ex.Message.ToLowerInvariant();

        if (message.Contains("required") || message.Contains("wymagany"))
            return "XSD_REQUIRED_ELEMENT";

        if (message.Contains("invalid") || message.Contains("nieprawidłow"))
            return "XSD_INVALID_VALUE";

        if (message.Contains("pattern") || message.Contains("wzorzec"))
            return "XSD_PATTERN_MISMATCH";

        if (message.Contains("length") || message.Contains("długość"))
            return "XSD_LENGTH_ERROR";

        if (message.Contains("enumeration") || message.Contains("enumeracja"))
            return "XSD_ENUMERATION_ERROR";

        if (message.Contains("minoccurs") || message.Contains("maxoccurs"))
            return "XSD_OCCURRENCE_ERROR";

        if (message.Contains("type") || message.Contains("typ"))
            return "XSD_TYPE_ERROR";

        return "XSD_VALIDATION_ERROR";
    }

    private static string GetWarningCode(XmlSchemaException? ex)
    {
        return "XSD_WARNING";
    }

    private static string? GetFieldNameFromXsdError(XmlSchemaException? ex)
    {
        if (ex == null)
            return null;

        // Próba wyciągnięcia nazwy elementu z komunikatu
        var message = ex.Message;

        // Szukaj wzorca 'element_name' lub "element_name"
        var singleQuoteStart = message.IndexOf('\'');
        if (singleQuoteStart >= 0)
        {
            var singleQuoteEnd = message.IndexOf('\'', singleQuoteStart + 1);
            if (singleQuoteEnd > singleQuoteStart)
            {
                return message.Substring(singleQuoteStart + 1, singleQuoteEnd - singleQuoteStart - 1);
            }
        }

        return null;
    }

    private void EnsureSchemasLoaded()
    {
        if (_schemasInitialized)
            return;

        lock (_loadLock)
        {
            if (_schemasInitialized)
                return;

            LoadSchemasFromResources();
            _schemasInitialized = true;
        }
    }

    private void LoadSchemasFromResources()
    {
        var assembly = typeof(XsdValidator).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        // Załaduj schemat FA(3)
        var fa3SchemaSet = LoadSchemaSet(assembly, "FA3.xsd", "StrukturyDanych.xsd");
        if (fa3SchemaSet != null)
        {
            _schemaCache[SchemaVersion.FA3] = fa3SchemaSet;
        }

        // FA(2) nie jest jeszcze dostępny - dodaj obsługę gdy schemat będzie dodany
        // var fa2SchemaSet = LoadSchemaSet(assembly, "FA2.xsd", "StrukturyDanych.xsd");
        // if (fa2SchemaSet != null)
        // {
        //     _schemaCache[SchemaVersion.FA2] = fa2SchemaSet;
        // }
    }

    private XmlSchemaSet? LoadSchemaSet(Assembly assembly, params string[] schemaFileNames)
    {
        try
        {
            var schemaSet = new XmlSchemaSet();

            // Resolver do obsługi importów z zasobów
            schemaSet.XmlResolver = new EmbeddedResourceResolver(assembly);

            foreach (var fileName in schemaFileNames)
            {
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                    continue;

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    continue;

                using var reader = XmlReader.Create(stream);
                var schema = XmlSchema.Read(reader, (sender, e) =>
                {
                    // Ignoruj ostrzeżenia podczas ładowania schematu
                    if (e.Severity == XmlSeverityType.Error)
                    {
                        throw new XmlSchemaException($"Błąd ładowania schematu {fileName}: {e.Message}");
                    }
                });

                if (schema != null)
                {
                    schemaSet.Add(schema);
                }
            }

            if (schemaSet.Count == 0)
                return null;

            schemaSet.Compile();
            return schemaSet;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Resolver XML do ładowania schematów z zasobów osadzonych
    /// </summary>
    private class EmbeddedResourceResolver : XmlResolver
    {
        private readonly Assembly _assembly;
        private readonly Dictionary<string, string> _uriToResourceMapping;

        public EmbeddedResourceResolver(Assembly assembly)
        {
            _assembly = assembly;
            _uriToResourceMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Mapuj znane URI na zasoby
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase))
                {
                    var fileName = resourceName.Split('.').Reverse().Skip(1).First() + ".xsd";
                    _uriToResourceMapping[fileName] = resourceName;
                }
            }
        }

        public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
        {
            // Sprawdź czy to lokalne odwołanie
            var fileName = Path.GetFileName(absoluteUri.LocalPath);

            if (_uriToResourceMapping.TryGetValue(fileName, out var resourceName))
            {
                return _assembly.GetManifestResourceStream(resourceName);
            }

            // Sprawdź po pełnej nazwie zasobu
            var matchingResource = _assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (matchingResource != null)
            {
                return _assembly.GetManifestResourceStream(matchingResource);
            }

            // Dla zewnętrznych schematów, pozwól na domyślne zachowanie lub zwróć null
            // Można tutaj dodać pobieranie z sieci jeśli potrzebne
            return null;
        }

        public override Uri ResolveUri(Uri? baseUri, string? relativeUri)
        {
            if (string.IsNullOrEmpty(relativeUri))
                return baseUri ?? new Uri("embedded://");

            // Dla HTTP/HTTPS URI, zwróć jak jest
            if (Uri.TryCreate(relativeUri, UriKind.Absolute, out var absoluteUri) &&
                (absoluteUri.Scheme == "http" || absoluteUri.Scheme == "https"))
            {
                return absoluteUri;
            }

            // Dla względnych ścieżek, użyj bazowego URI
            if (baseUri != null && Uri.TryCreate(baseUri, relativeUri, out var resolvedUri))
            {
                return resolvedUri;
            }

            return new Uri("embedded://" + relativeUri);
        }

        public override bool SupportsType(Uri absoluteUri, Type? type)
        {
            return type == null || type == typeof(Stream);
        }
    }
}
