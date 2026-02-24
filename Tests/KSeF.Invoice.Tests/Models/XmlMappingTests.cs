using System.Xml.Linq;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Summary;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class XmlMappingTests
{
    [Fact]
    public void Invoice_ShouldHaveCorrectXmlRootAttribute()
    {
        // Arrange
        var type = typeof(Invoice.Models.Invoice);
        var xmlRootAttribute = type.GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        // Assert
        xmlRootAttribute.Should().NotBeNull();
        xmlRootAttribute!.ElementName.Should().Be("Faktura");
        xmlRootAttribute.Namespace.Should().Be(Invoice.Models.Invoice.KSeFNamespace);
    }

    [Fact]
    public void InvoiceData_ShouldHaveCorrectXmlTypeAttribute()
    {
        // Arrange
        var type = typeof(InvoiceData);
        var xmlTypeAttribute = type.GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        // Assert
        xmlTypeAttribute.Should().NotBeNull();
        xmlTypeAttribute!.TypeName.Should().Be("Fa");
    }

    [Fact]
    public void InvoiceLineItem_ShouldHaveCorrectXmlTypeAttribute()
    {
        // Arrange
        var type = typeof(InvoiceLineItem);
        var xmlTypeAttribute = type.GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        // Assert
        xmlTypeAttribute.Should().NotBeNull();
        xmlTypeAttribute!.TypeName.Should().Be("FaWiersz");
    }

    [Fact]
    public void Seller_ShouldHaveCorrectXmlRootAttribute()
    {
        // Arrange
        var type = typeof(Seller);
        var xmlRootAttribute = type.GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        // Assert
        xmlRootAttribute.Should().NotBeNull();
        xmlRootAttribute!.ElementName.Should().Be("Podmiot1");
    }

    [Fact]
    public void Buyer_ShouldHaveCorrectXmlRootAttribute()
    {
        // Arrange
        var type = typeof(Buyer);
        var xmlRootAttribute = type.GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        // Assert
        xmlRootAttribute.Should().NotBeNull();
        xmlRootAttribute!.ElementName.Should().Be("Podmiot2");
    }

    [Fact]
    public void InvoiceAnnotations_ShouldHaveCorrectXmlTypeAttribute()
    {
        // Arrange
        var type = typeof(InvoiceAnnotations);
        var xmlTypeAttribute = type.GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        // Assert
        xmlTypeAttribute.Should().NotBeNull();
        xmlTypeAttribute!.TypeName.Should().Be("Adnotacje");
    }

    [Fact]
    public void InvoiceData_Serialize_ShouldHaveCorrectElementNames()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            CurrencyCode = CurrencyCode.PLN,
            IssueDate = new DateOnly(2024, 1, 15),
            InvoiceNumber = "FV/2024/001",
            TotalAmount = 1230.00m,
            InvoiceType = InvoiceType.VAT,
            Annotations = new InvoiceAnnotations()
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(invoiceData);
        var xDoc = XDocument.Parse(xml);

        // Assert
        xDoc.Root!.Name.LocalName.Should().Be("Fa");
        xDoc.Root.Element(XName.Get("KodWaluty", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_1", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_2", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_15", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("RodzajFaktury", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
    }

    [Fact]
    public void InvoiceLineItem_Serialize_ShouldHaveCorrectElementNames()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            Unit = "szt.",
            Quantity = 10,
            UnitNetPrice = 100.00m,
            NetAmount = 1000.00m,
            VatRate = VatRate.Rate23
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(lineItem);
        var xDoc = XDocument.Parse(xml);

        // Assert
        xDoc.Root!.Name.LocalName.Should().Be("FaWiersz");
        xDoc.Root.Element(XName.Get("NrWiersza", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_7", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_8A", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_8B", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_9A", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_11", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_12", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
    }

    [Fact]
    public void Seller_Serialize_ShouldHaveCorrectElementNames()
    {
        // Arrange
        var seller = new Seller
        {
            TaxId = "1234567890",
            Name = "Test Seller",
            Address = new Address
            {
                CountryCode = "PL",
                AddressLine1 = "ul. Test 1"
            }
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(seller);
        var xDoc = XDocument.Parse(xml);

        // Assert
        xDoc.Root!.Name.LocalName.Should().Be("Podmiot1");
        xDoc.Root.Element(XName.Get("NIP", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("Nazwa", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("Adres", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
    }

    [Fact]
    public void Address_Serialize_ShouldHaveCorrectElementNames()
    {
        // Arrange
        var address = new Address
        {
            CountryCode = "PL",
            AddressLine1 = "ul. Testowa 1",
            AddressLine2 = "00-001 Warszawa",
            Gln = "1234567890123"
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(address);
        var xDoc = XDocument.Parse(xml);

        // Assert
        xDoc.Root!.Element(XName.Get("KodKraju", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("AdresL1", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("AdresL2", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("GLN", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
    }

    [Fact]
    public void InvoiceAnnotations_Serialize_ShouldHaveCorrectElementNames()
    {
        // Arrange
        var annotations = new InvoiceAnnotations
        {
            CashMethod = AnnotationValue.Yes,
            SelfBilling = AnnotationValue.No,
            ReverseCharge = AnnotationValue.No,
            SplitPayment = AnnotationValue.Yes
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(annotations);
        var xDoc = XDocument.Parse(xml);

        // Assert
        xDoc.Root!.Name.LocalName.Should().Be("Adnotacje");
        xDoc.Root.Element(XName.Get("P_16", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_17", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_18", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
        xDoc.Root.Element(XName.Get("P_18A", xDoc.Root.Name.NamespaceName)).Should().NotBeNull();
    }

    [Fact]
    public void Invoice_Constants_ShouldHaveCorrectValues()
    {
        // Assert
        Invoice.Models.Invoice.KSeFNamespace.Should().Be("http://crd.gov.pl/wzor/2025/06/25/13775/");
        Invoice.Models.Invoice.DefinitionTypesNamespace.Should().Be("http://crd.gov.pl/xml/schematy/dziedzinowe/mf/2022/01/05/eD/DefinicjeTypy/");
    }

    [Theory]
    [InlineData(typeof(Invoice.Models.Invoice), "Header", "Naglowek")]
    [InlineData(typeof(Invoice.Models.Invoice), "Seller", "Podmiot1")]
    [InlineData(typeof(Invoice.Models.Invoice), "Buyer", "Podmiot2")]
    [InlineData(typeof(Invoice.Models.Invoice), "Recipients", "Podmiot3")]
    [InlineData(typeof(Invoice.Models.Invoice), "AuthorizedEntity", "PodmiotUpowazniony")]
    [InlineData(typeof(Invoice.Models.Invoice), "InvoiceData", "Fa")]
    [InlineData(typeof(Invoice.Models.Invoice), "Footer", "Stopka")]
    [InlineData(typeof(Invoice.Models.Invoice), "Attachments", "Zalacznik")]
    public void Invoice_Properties_ShouldHaveCorrectXmlElementNames(Type type, string propertyName, string expectedElementName)
    {
        // Arrange
        var property = type.GetProperty(propertyName);
        var xmlElementAttribute = property?.GetCustomAttributes(typeof(XmlElementAttribute), false)
            .Cast<XmlElementAttribute>()
            .FirstOrDefault();

        // Assert
        xmlElementAttribute.Should().NotBeNull();
        xmlElementAttribute!.ElementName.Should().Be(expectedElementName);
    }
}
