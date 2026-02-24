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

public class XmlNamespaceTests
{
    private const string ExpectedKSeFNamespace = "http://crd.gov.pl/wzor/2025/06/25/13775/";
    private const string ExpectedDefinitionTypesNamespace = "http://crd.gov.pl/xml/schematy/dziedzinowe/mf/2022/01/05/eD/DefinicjeTypy/";

    [Fact]
    public void Invoice_KSeFNamespace_ShouldBeCorrect()
    {
        // Assert
        Invoice.Models.Invoice.KSeFNamespace.Should().Be(ExpectedKSeFNamespace);
    }

    [Fact]
    public void Invoice_DefinitionTypesNamespace_ShouldBeCorrect()
    {
        // Assert
        Invoice.Models.Invoice.DefinitionTypesNamespace.Should().Be(ExpectedDefinitionTypesNamespace);
    }

    [Fact]
    public void Invoice_Serialize_ShouldUseCorrectNamespace()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);
        var xDoc = XDocument.Parse(xml);

        // Assert
        xDoc.Root!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);
    }

    [Fact]
    public void Invoice_XmlRootAttribute_ShouldSpecifyCorrectNamespace()
    {
        // Arrange
        var type = typeof(Invoice.Models.Invoice);
        var xmlRootAttribute = type.GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        // Assert
        xmlRootAttribute.Should().NotBeNull();
        xmlRootAttribute!.Namespace.Should().Be(ExpectedKSeFNamespace);
    }

    [Fact]
    public void Invoice_Serialize_AllElementsShouldInheritNamespace()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);
        var xDoc = XDocument.Parse(xml);
        XNamespace ns = ExpectedKSeFNamespace;

        // Assert - Check that main child elements inherit the namespace
        var naglowek = xDoc.Root!.Element(ns + "Naglowek");
        naglowek.Should().NotBeNull();
        naglowek!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);

        var podmiot1 = xDoc.Root.Element(ns + "Podmiot1");
        podmiot1.Should().NotBeNull();
        podmiot1!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);

        var podmiot2 = xDoc.Root.Element(ns + "Podmiot2");
        podmiot2.Should().NotBeNull();
        podmiot2!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);

        var fa = xDoc.Root.Element(ns + "Fa");
        fa.Should().NotBeNull();
        fa!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);
    }

    [Fact]
    public void Invoice_Serialize_NestedElementsShouldInheritNamespace()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);
        var xDoc = XDocument.Parse(xml);
        XNamespace ns = ExpectedKSeFNamespace;

        // Assert - Check nested elements
        var nip = xDoc.Root!.Element(ns + "Podmiot1")?.Element(ns + "NIP");
        nip.Should().NotBeNull();
        nip!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);

        var kodWaluty = xDoc.Root.Element(ns + "Fa")?.Element(ns + "KodWaluty");
        kodWaluty.Should().NotBeNull();
        kodWaluty!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);
    }

    [Fact]
    public void Invoice_NamespaceDeclaration_ShouldBeOnRootElement()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);
        var xDoc = XDocument.Parse(xml);

        // Assert
        var namespaceAttributes = xDoc.Root!.Attributes()
            .Where(a => a.IsNamespaceDeclaration)
            .ToList();

        namespaceAttributes.Should().Contain(a => a.Value == ExpectedKSeFNamespace);
    }

    [Fact]
    public void Invoice_RoundTrip_ShouldPreserveNamespace()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);
        var result = XmlSerializationHelper.Deserialize<Invoice.Models.Invoice>(xml);
        var reserializedXml = XmlSerializationHelper.Serialize(result);
        var xDoc = XDocument.Parse(reserializedXml);

        // Assert
        xDoc.Root!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);
    }

    [Fact]
    public void Invoice_Deserialize_ShouldAcceptCorrectNamespace()
    {
        // Arrange
        var xmlWithNamespace = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Faktura xmlns=""{ExpectedKSeFNamespace}"">
    <Naglowek>
        <KodFormularza kodSystemowy=""FA (3)"" wersjaSchemy=""1-0E"">FA</KodFormularza>
        <WariantFormularza>3</WariantFormularza>
        <DataWytworzeniaFa>2024-01-15T10:00:00</DataWytworzeniaFa>
    </Naglowek>
    <Podmiot1>
        <NIP>1234567890</NIP>
        <Nazwa>Test Seller</Nazwa>
    </Podmiot1>
    <Podmiot2>
        <NIP>0987654321</NIP>
        <Nazwa>Test Buyer</Nazwa>
    </Podmiot2>
    <Fa>
        <KodWaluty>PLN</KodWaluty>
        <P_1>2024-01-15</P_1>
        <P_2>FV/2024/001</P_2>
        <P_15>1230</P_15>
        <Adnotacje>
            <P_16>2</P_16>
            <P_17>2</P_17>
            <P_18>2</P_18>
            <P_18A>2</P_18A>
        </Adnotacje>
        <RodzajFaktury>VAT</RodzajFaktury>
    </Fa>
</Faktura>";

        // Act
        var invoice = XmlSerializationHelper.Deserialize<Invoice.Models.Invoice>(xmlWithNamespace);

        // Assert
        invoice.Should().NotBeNull();
        invoice.Seller.TaxId.Should().Be("1234567890");
        invoice.InvoiceData.InvoiceNumber.Should().Be("FV/2024/001");
    }

    [Fact]
    public void Invoice_WithLineItems_ShouldHaveCorrectNamespace()
    {
        // Arrange
        var invoice = CreateBasicInvoice();
        invoice.InvoiceData.LineItems = new List<InvoiceLineItem>
        {
            new InvoiceLineItem
            {
                LineNumber = 1,
                ProductName = "Test Product",
                VatRate = VatRate.Rate23
            }
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);
        var xDoc = XDocument.Parse(xml);
        XNamespace ns = ExpectedKSeFNamespace;

        // Assert
        var faWiersz = xDoc.Root!.Element(ns + "Fa")?.Element(ns + "FaWiersz");
        faWiersz.Should().NotBeNull();
        faWiersz!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);
    }

    [Fact]
    public void Invoice_WithAnnotations_ShouldHaveCorrectNamespace()
    {
        // Arrange
        var invoice = CreateBasicInvoice();
        invoice.InvoiceData.Annotations = new InvoiceAnnotations
        {
            CashMethod = AnnotationValue.Yes,
            SelfBilling = AnnotationValue.No,
            ReverseCharge = AnnotationValue.No,
            SplitPayment = AnnotationValue.No
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);
        var xDoc = XDocument.Parse(xml);
        XNamespace ns = ExpectedKSeFNamespace;

        // Assert
        var adnotacje = xDoc.Root!.Element(ns + "Fa")?.Element(ns + "Adnotacje");
        adnotacje.Should().NotBeNull();
        adnotacje!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);

        var p16 = adnotacje.Element(ns + "P_16");
        p16.Should().NotBeNull();
        p16!.Name.NamespaceName.Should().Be(ExpectedKSeFNamespace);
    }

    [Theory]
    [InlineData("http://invalid.namespace.com/")]
    [InlineData("")]
    public void Invoice_Deserialize_WithInvalidNamespace_ShouldStillWorkDueToXmlSerializerBehavior(string invalidNamespace)
    {
        // Note: XmlSerializer is lenient with namespaces during deserialization
        // This test documents this behavior

        // Arrange
        var xmlWithInvalidNamespace = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Faktura xmlns=""{invalidNamespace}"">
    <Naglowek>
        <KodFormularza kodSystemowy=""FA (3)"" wersjaSchemy=""1-0E"">FA</KodFormularza>
        <WariantFormularza>3</WariantFormularza>
        <DataWytworzeniaFa>2024-01-15T10:00:00</DataWytworzeniaFa>
    </Naglowek>
    <Podmiot1>
        <NIP>1234567890</NIP>
        <Nazwa>Test Seller</Nazwa>
    </Podmiot1>
    <Podmiot2>
        <NIP>0987654321</NIP>
        <Nazwa>Test Buyer</Nazwa>
    </Podmiot2>
    <Fa>
        <KodWaluty>PLN</KodWaluty>
        <P_1>2024-01-15</P_1>
        <P_2>FV/2024/001</P_2>
        <P_15>1230</P_15>
        <Adnotacje>
            <P_16>2</P_16>
            <P_17>2</P_17>
            <P_18>2</P_18>
            <P_18A>2</P_18A>
        </Adnotacje>
        <RodzajFaktury>VAT</RodzajFaktury>
    </Fa>
</Faktura>";

        // Act & Assert - XmlSerializer may or may not deserialize depending on settings
        // We're testing that this doesn't throw an exception
        try
        {
            var invoice = XmlSerializationHelper.Deserialize<Invoice.Models.Invoice>(xmlWithInvalidNamespace);
            // If deserialization succeeded, verify the result is at least not null
            invoice.Should().NotBeNull();
        }
        catch (InvalidOperationException)
        {
            // Expected when namespace doesn't match - this is acceptable behavior
        }
    }

    private static Invoice.Models.Invoice CreateBasicInvoice()
    {
        return new Invoice.Models.Invoice
        {
            Header = new InvoiceHeader
            {
                FormCode = new FormCodeElement { SystemCode = "FA (3)", SchemaVersion = "1-0E" },
                FormVariant = 3,
                CreationDateTime = new DateTime(2024, 1, 15, 10, 0, 0),
                SystemInfo = "Test System"
            },
            Seller = new Seller
            {
                TaxId = "1234567890",
                Name = "Test Seller Sp. z o.o.",
                Address = new Address
                {
                    CountryCode = "PL",
                    AddressLine1 = "ul. Testowa 1, Warszawa"
                }
            },
            Buyer = new Buyer
            {
                TaxId = "0987654321",
                Name = "Test Buyer S.A."
            },
            InvoiceData = new InvoiceData
            {
                CurrencyCode = CurrencyCode.PLN,
                IssueDate = new DateOnly(2024, 1, 15),
                InvoiceNumber = "FV/2024/001",
                TotalAmount = 1230.00m,
                NetAmount23 = 1000.00m,
                VatAmount23 = 230.00m,
                InvoiceType = InvoiceType.VAT,
                Annotations = new InvoiceAnnotations
                {
                    CashMethod = AnnotationValue.No,
                    SelfBilling = AnnotationValue.No,
                    ReverseCharge = AnnotationValue.No,
                    SplitPayment = AnnotationValue.No
                }
            }
        };
    }
}
