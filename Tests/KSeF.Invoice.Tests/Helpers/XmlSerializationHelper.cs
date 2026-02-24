using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using KSeF.Invoice.Services.Serialization;

namespace KSeF.Invoice.Tests.Helpers;

public class XmlSerializationHelper
{
    private static readonly KsefInvoiceSerializer _ksefSerializer = new();

    public static string Serialize<T>(T obj)
    {
        // Użyj specjalizowanego serializera dla Invoice
        if (obj is KSeF.Invoice.Models.Invoice invoice)
        {
            return _ksefSerializer.SerializeToXml(invoice);
        }

        var serializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        });
        serializer.Serialize(xmlWriter, obj);
        return stringWriter.ToString();
    }

    public static T Deserialize<T>(string xml) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringReader = new StringReader(xml);
        return (T)serializer.Deserialize(stringReader)!;
    }

    public static string SerializeWithNamespace<T>(T obj, string namespaceUri)
    {
        var serializer = new XmlSerializer(typeof(T));
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", namespaceUri);

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        });
        serializer.Serialize(xmlWriter, obj, namespaces);
        return stringWriter.ToString();
    }

    public static T? RoundTrip<T>(T obj) where T : class
    {
        var xml = Serialize(obj);
        return Deserialize<T>(xml);
    }

    public static XDocument ToXDocument<T>(T obj)
    {
        var xml = Serialize(obj);
        return XDocument.Parse(xml);
    }

    public static string GetElementName<T>()
    {
        var type = typeof(T);
        var xmlRootAttribute = type.GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        if (xmlRootAttribute != null && !string.IsNullOrEmpty(xmlRootAttribute.ElementName))
        {
            return xmlRootAttribute.ElementName;
        }

        var xmlTypeAttribute = type.GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        if (xmlTypeAttribute != null && !string.IsNullOrEmpty(xmlTypeAttribute.TypeName))
        {
            return xmlTypeAttribute.TypeName;
        }

        return type.Name;
    }

    public static string? GetRootNamespace<T>()
    {
        var type = typeof(T);
        var xmlRootAttribute = type.GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        return xmlRootAttribute?.Namespace;
    }
}
