using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace KSeF.Invoice.Services.Serialization;

/// <summary>
/// Implementacja serializacji faktury do formatu XML zgodnego z KSeF FA(3)
/// </summary>
public class KsefInvoiceSerializer : IInvoiceSerializer
{
    private readonly XmlSerializer _serializer;
    private readonly XmlSerializerNamespaces _namespaces;
    private readonly XmlWriterSettings _writerSettings;

    /// <summary>
    /// Tworzy nową instancję serializera KSeF
    /// </summary>
    public KsefInvoiceSerializer()
    {
        _serializer = new XmlSerializer(typeof(Models.Invoice));

        // Konfiguracja przestrzeni nazw zgodnie ze schematem FA(3)
        _namespaces = new XmlSerializerNamespaces();
        _namespaces.Add("", Models.Invoice.KSeFNamespace); // Domyślna przestrzeń nazw
        _namespaces.Add("tns", Models.Invoice.KSeFNamespace);
        _namespaces.Add("dt", Models.Invoice.DefinitionTypesNamespace);
        _namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

        // Ustawienia formatowania XML
        _writerSettings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false), // UTF-8 bez BOM
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace,
            OmitXmlDeclaration = false
        };
    }

    /// <inheritdoc />
    public string SerializeToXml(Models.Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        using var stringWriter = new Utf8StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, _writerSettings);

        _serializer.Serialize(xmlWriter, invoice, _namespaces);

        return stringWriter.ToString();
    }

    /// <inheritdoc />
    public byte[] SerializeToBytes(Models.Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        using var memoryStream = new MemoryStream();
        SerializeToStream(invoice, memoryStream);
        return memoryStream.ToArray();
    }

    /// <inheritdoc />
    public void SerializeToStream(Models.Invoice invoice, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentNullException.ThrowIfNull(stream);

        using var xmlWriter = XmlWriter.Create(stream, _writerSettings);
        _serializer.Serialize(xmlWriter, invoice, _namespaces);
        xmlWriter.Flush();
    }

    /// <inheritdoc />
    public void SerializeToFile(Models.Invoice invoice, string filePath)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        SerializeToStream(invoice, fileStream);
    }

    /// <inheritdoc />
    public Models.Invoice? DeserializeFromXml(string xml)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader);

        return (Models.Invoice?)_serializer.Deserialize(xmlReader);
    }

    /// <inheritdoc />
    public Models.Invoice? DeserializeFromBytes(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        using var memoryStream = new MemoryStream(bytes);
        return DeserializeFromStream(memoryStream);
    }

    /// <inheritdoc />
    public Models.Invoice? DeserializeFromStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var xmlReader = XmlReader.Create(stream);
        return (Models.Invoice?)_serializer.Deserialize(xmlReader);
    }

    /// <inheritdoc />
    public Models.Invoice? DeserializeFromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Plik faktury nie został znaleziony.", filePath);
        }

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return DeserializeFromStream(fileStream);
    }

    /// <summary>
    /// StringWriter używający kodowania UTF-8
    /// Standardowy StringWriter używa UTF-16, co jest niezgodne z wymaganiami KSeF
    /// </summary>
    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
