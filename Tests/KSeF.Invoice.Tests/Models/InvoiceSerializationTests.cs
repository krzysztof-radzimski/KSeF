using System.Xml.Linq;
using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Summary;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class InvoiceSerializationTests
{
    [Fact]
    public void Invoice_Serialize_ShouldProduceValidXml()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().NotBeNullOrEmpty();
        var xDoc = XDocument.Parse(xml);
        xDoc.Root.Should().NotBeNull();
        xDoc.Root!.Name.LocalName.Should().Be("Faktura");
    }

    [Fact]
    public void Invoice_RoundTrip_ShouldPreserveData()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var result = XmlSerializationHelper.RoundTrip(invoice);

        // Assert
        result.Should().NotBeNull();
        result!.Seller.TaxId.Should().Be(invoice.Seller.TaxId);
        result.Seller.Name.Should().Be(invoice.Seller.Name);
        result.Buyer.TaxId.Should().Be(invoice.Buyer.TaxId);
        result.InvoiceData.InvoiceNumber.Should().Be(invoice.InvoiceData.InvoiceNumber);
        result.InvoiceData.TotalAmount.Should().Be(invoice.InvoiceData.TotalAmount);
    }

    [Fact]
    public void Invoice_Serialize_ShouldHaveCorrectRootNamespace()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);
        var xDoc = XDocument.Parse(xml);

        // Assert
        xDoc.Root!.Name.NamespaceName.Should().Be(Invoice.Models.Invoice.KSeFNamespace);
    }

    [Fact]
    public void Invoice_Serialize_ShouldIncludeRequiredElements()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Act
        var xDoc = XmlSerializationHelper.ToXDocument(invoice);
        var ns = Invoice.Models.Invoice.KSeFNamespace;
        XNamespace xns = ns;

        // Assert
        xDoc.Root!.Element(xns + "Naglowek").Should().NotBeNull();
        xDoc.Root!.Element(xns + "Podmiot1").Should().NotBeNull();
        xDoc.Root!.Element(xns + "Podmiot2").Should().NotBeNull();
        xDoc.Root!.Element(xns + "Fa").Should().NotBeNull();
    }

    [Theory]
    [InlineData(InvoiceType.VAT)]
    [InlineData(InvoiceType.KOR)]
    [InlineData(InvoiceType.ZAL)]
    [InlineData(InvoiceType.ROZ)]
    [InlineData(InvoiceType.UPR)]
    public void Invoice_RoundTrip_ShouldPreserveInvoiceType(InvoiceType invoiceType)
    {
        // Arrange
        var invoice = CreateBasicInvoice();
        invoice.InvoiceData.InvoiceType = invoiceType;

        // Act
        var result = XmlSerializationHelper.RoundTrip(invoice);

        // Assert
        result.Should().NotBeNull();
        result!.InvoiceData.InvoiceType.Should().Be(invoiceType);
    }

    [Theory]
    [InlineData(CurrencyCode.PLN)]
    [InlineData(CurrencyCode.EUR)]
    [InlineData(CurrencyCode.USD)]
    [InlineData(CurrencyCode.GBP)]
    public void Invoice_RoundTrip_ShouldPreserveCurrencyCode(CurrencyCode currencyCode)
    {
        // Arrange
        var invoice = CreateBasicInvoice();
        invoice.InvoiceData.CurrencyCode = currencyCode;

        // Act
        var result = XmlSerializationHelper.RoundTrip(invoice);

        // Assert
        result.Should().NotBeNull();
        result!.InvoiceData.CurrencyCode.Should().Be(currencyCode);
    }

    [Fact]
    public void Invoice_WithLineItems_RoundTrip_ShouldPreserveLineItems()
    {
        // Arrange
        var invoice = CreateInvoiceWithLineItems();

        // Act
        var result = XmlSerializationHelper.RoundTrip(invoice);

        // Assert
        result.Should().NotBeNull();
        result!.InvoiceData.LineItems.Should().NotBeNull();
        result.InvoiceData.LineItems!.Should().HaveCount(2);
        result.InvoiceData.LineItems[0].ProductName.Should().Be("Produkt testowy 1");
        result.InvoiceData.LineItems[0].VatRate.Should().Be(VatRate.Rate23);
        result.InvoiceData.LineItems[1].ProductName.Should().Be("Produkt testowy 2");
        result.InvoiceData.LineItems[1].VatRate.Should().Be(VatRate.Rate8);
    }

    [Fact]
    public void Invoice_HelperProperties_ShouldWorkCorrectly()
    {
        // Arrange
        var invoice = CreateBasicInvoice();

        // Assert
        invoice.HasRecipients.Should().BeFalse();
        invoice.HasAuthorizedEntity.Should().BeFalse();
        invoice.HasFooter.Should().BeFalse();
        invoice.HasAttachments.Should().BeFalse();
        invoice.IsCorrection.Should().BeFalse();
        invoice.IsAdvancePayment.Should().BeFalse();
        invoice.IsSettlement.Should().BeFalse();
        invoice.IsSimplified.Should().BeFalse();
    }

    [Theory]
    [InlineData(InvoiceType.KOR, true)]
    [InlineData(InvoiceType.KOR_ZAL, true)]
    [InlineData(InvoiceType.KOR_ROZ, true)]
    [InlineData(InvoiceType.VAT, false)]
    [InlineData(InvoiceType.ZAL, false)]
    [InlineData(InvoiceType.ROZ, false)]
    public void Invoice_IsCorrection_ShouldReturnCorrectValue(InvoiceType invoiceType, bool expectedIsCorrection)
    {
        // Arrange
        var invoice = CreateBasicInvoice();
        invoice.InvoiceData.InvoiceType = invoiceType;

        // Assert
        invoice.IsCorrection.Should().Be(expectedIsCorrection);
    }

    private static Invoice.Models.Invoice CreateBasicInvoice()
    {
        return new Invoice.Models.Invoice
        {
            Header = new InvoiceHeader
            {
                FormCode = new FormCodeElement { SystemCode = "FA (3)", SchemaVersion = "1-0E" },
                FormVariant = 3,
                CreationDateTime = DateTime.Now,
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
                IssueDate = DateOnly.FromDateTime(DateTime.Now),
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

    private static Invoice.Models.Invoice CreateInvoiceWithLineItems()
    {
        var invoice = CreateBasicInvoice();
        invoice.InvoiceData.LineItems = new List<InvoiceLineItem>
        {
            new InvoiceLineItem
            {
                LineNumber = 1,
                ProductName = "Produkt testowy 1",
                Unit = "szt.",
                Quantity = 10,
                UnitNetPrice = 50.00m,
                NetAmount = 500.00m,
                VatRate = VatRate.Rate23
            },
            new InvoiceLineItem
            {
                LineNumber = 2,
                ProductName = "Produkt testowy 2",
                Unit = "szt.",
                Quantity = 5,
                UnitNetPrice = 100.00m,
                NetAmount = 500.00m,
                VatRate = VatRate.Rate8
            }
        };
        return invoice;
    }
}
