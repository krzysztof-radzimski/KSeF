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

public class NullHandlingTests
{
    #region InvoiceData Null Tests

    [Fact]
    public void InvoiceData_WithNullOptionalFields_ShouldSerializeWithoutErrors()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            CurrencyCode = CurrencyCode.PLN,
            IssueDate = new DateOnly(2024, 1, 15),
            InvoiceNumber = "FV/2024/001",
            TotalAmount = 1230.00m,
            InvoiceType = InvoiceType.VAT,
            Annotations = new InvoiceAnnotations(),
            // All optional fields are null by default
            IssuePlace = null,
            SaleDate = null,
            SalePeriod = null,
            LineItems = null,
            Payment = null,
            TransactionTerms = null,
            CorrectionReason = null,
            CorrectedInvoiceData = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(invoiceData);

        // Assert
        xml.Should().NotBeNullOrEmpty();
        var xDoc = XDocument.Parse(xml);
        xDoc.Root.Should().NotBeNull();
    }

    [Fact]
    public void InvoiceData_WithNullOptionalFields_ShouldNotIncludeNullElements()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            CurrencyCode = CurrencyCode.PLN,
            IssueDate = new DateOnly(2024, 1, 15),
            InvoiceNumber = "FV/2024/001",
            TotalAmount = 1230.00m,
            InvoiceType = InvoiceType.VAT,
            Annotations = new InvoiceAnnotations(),
            IssuePlace = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(invoiceData);
        var xDoc = XDocument.Parse(xml);

        // Assert
        xDoc.Root!.Element(XName.Get("P_1M", xDoc.Root.Name.NamespaceName)).Should().BeNull();
    }

    [Fact]
    public void InvoiceData_WithNullNetAmounts_ShouldSerializeCorrectly()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            CurrencyCode = CurrencyCode.PLN,
            IssueDate = new DateOnly(2024, 1, 15),
            InvoiceNumber = "FV/2024/001",
            TotalAmount = 0m,
            InvoiceType = InvoiceType.VAT,
            Annotations = new InvoiceAnnotations(),
            NetAmount23 = null,
            NetAmount8 = null,
            NetAmount5 = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(invoiceData);
        var xDoc = XDocument.Parse(xml);

        // Assert - XmlSerializer may include xsi:nil="true" for nullable types or omit them entirely
        // We verify that the XML is valid and can be deserialized
        var deserialized = XmlSerializationHelper.Deserialize<InvoiceData>(xml);
        deserialized.Should().NotBeNull();
        deserialized!.NetAmount23.Should().BeNull();
        deserialized.NetAmount8.Should().BeNull();
        deserialized.NetAmount5.Should().BeNull();
    }

    [Fact]
    public void InvoiceData_RoundTrip_ShouldPreserveNullValues()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            CurrencyCode = CurrencyCode.PLN,
            IssueDate = new DateOnly(2024, 1, 15),
            InvoiceNumber = "FV/2024/001",
            TotalAmount = 1000.00m,
            InvoiceType = InvoiceType.VAT,
            Annotations = new InvoiceAnnotations(),
            IssuePlace = null,
            SaleDate = null,
            NetAmount23 = null
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(invoiceData);

        // Assert
        result.Should().NotBeNull();
        result!.IssuePlace.Should().BeNull();
        result.SaleDate.Should().BeNull();
        result.NetAmount23.Should().BeNull();
    }

    #endregion

    #region InvoiceLineItem Null Tests

    [Fact]
    public void InvoiceLineItem_WithNullOptionalFields_ShouldSerializeWithoutErrors()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            VatRate = VatRate.Rate23,
            // Optional fields
            Unit = null,
            Quantity = null,
            UnitNetPrice = null,
            UnitGrossPrice = null,
            Discount = null,
            NetAmount = null,
            GrossAmount = null,
            VatAmount = null,
            SaleDate = null,
            GtinCode = null,
            PkwiuCode = null,
            CnCode = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(lineItem);

        // Assert
        xml.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void InvoiceLineItem_HelperProperties_ShouldHandleNullValues()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            VatRate = VatRate.Rate23
        };

        // Assert
        lineItem.HasUnitNetPrice.Should().BeFalse();
        lineItem.HasUnitGrossPrice.Should().BeFalse();
        lineItem.HasDiscount.Should().BeFalse();
        lineItem.HasSaleDate.Should().BeFalse();
        lineItem.HasGtinCode.Should().BeFalse();
        lineItem.HasPkwiuCode.Should().BeFalse();
        lineItem.HasCnCode.Should().BeFalse();
        lineItem.UsesNetCalculation.Should().BeFalse();
        lineItem.UsesGrossCalculation.Should().BeFalse();
    }

    [Fact]
    public void InvoiceLineItem_HelperProperties_ShouldReturnTrueWhenValuesSet()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            VatRate = VatRate.Rate23,
            UnitNetPrice = 100.00m,
            NetAmount = 1000.00m,
            Discount = 50.00m,
            SaleDate = new DateOnly(2024, 1, 15),
            GtinCode = "1234567890123",
            PkwiuCode = "12.34.56.0",
            CnCode = "12345678"
        };

        // Assert
        lineItem.HasUnitNetPrice.Should().BeTrue();
        lineItem.HasDiscount.Should().BeTrue();
        lineItem.HasSaleDate.Should().BeTrue();
        lineItem.HasGtinCode.Should().BeTrue();
        lineItem.HasPkwiuCode.Should().BeTrue();
        lineItem.HasCnCode.Should().BeTrue();
        lineItem.UsesNetCalculation.Should().BeTrue();
    }

    #endregion

    #region Seller Null Tests

    [Fact]
    public void Seller_WithNullOptionalFields_ShouldSerializeWithoutErrors()
    {
        // Arrange
        var seller = new Seller
        {
            TaxId = "1234567890",
            Name = "Test Seller",
            Address = null,
            CorrespondenceAddress = null,
            ContactData = null,
            EoriNumber = null,
            StatusInfo = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(seller);

        // Assert
        xml.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Seller_HelperProperties_ShouldHandleNullValues()
    {
        // Arrange
        var seller = new Seller
        {
            TaxId = "1234567890",
            Name = "Test Seller"
        };

        // Assert
        seller.HasCorrespondenceAddress.Should().BeFalse();
        seller.HasContactData.Should().BeFalse();
        seller.HasEoriNumber.Should().BeFalse();
        seller.HasStatusInfo.Should().BeFalse();
    }

    #endregion

    #region Buyer Null Tests

    [Fact]
    public void Buyer_WithNullOptionalFields_ShouldSerializeWithoutErrors()
    {
        // Arrange
        var buyer = new Buyer
        {
            TaxId = "0987654321",
            Name = "Test Buyer",
            Address = null,
            CorrespondenceAddress = null,
            ContactData = null,
            CustomerNumber = null,
            EuCountryCode = null,
            EuVatId = null,
            OtherIdCountryCode = null,
            OtherId = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(buyer);

        // Assert
        xml.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Buyer_HelperProperties_ShouldHandleNullValues()
    {
        // Arrange
        var buyer = new Buyer
        {
            TaxId = "0987654321",
            Name = "Test Buyer"
        };

        // Assert
        buyer.HasPolishTaxId.Should().BeTrue();
        buyer.HasEuVatId.Should().BeFalse();
        buyer.HasOtherId.Should().BeFalse();
        buyer.HasNoIdentifier.Should().BeFalse();
        buyer.HasCorrespondenceAddress.Should().BeFalse();
        buyer.HasContactData.Should().BeFalse();
        buyer.IsJST.Should().BeFalse();
        buyer.IsVATGroup.Should().BeFalse();
    }

    #endregion

    #region Address Null Tests

    [Fact]
    public void Address_WithNullOptionalFields_ShouldSerializeWithoutErrors()
    {
        // Arrange
        var address = new Address
        {
            CountryCode = "PL",
            AddressLine1 = "ul. Testowa 1",
            AddressLine2 = null,
            Gln = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(address);

        // Assert
        xml.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void PolishAddress_WithNullOptionalFields_ShouldSerializeWithoutErrors()
    {
        // Arrange
        var address = new PolishAddress
        {
            CountryCode = "PL",
            Province = "mazowieckie",
            County = "Warszawa",
            Municipality = "Warszawa",
            City = "Warszawa",
            BuildingNumber = "1",
            PostalCode = "00-001",
            Street = null,
            ApartmentNumber = null,
            PostOffice = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(address);

        // Assert
        xml.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ForeignAddress_WithNullOptionalFields_ShouldSerializeWithoutErrors()
    {
        // Arrange
        var address = new ForeignAddress
        {
            CountryCode = "DE",
            City = "Berlin",
            PostalCode = null,
            Street = null,
            BuildingNumber = null,
            ApartmentNumber = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(address);

        // Assert
        xml.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region InvoiceAnnotations Null Tests

    [Fact]
    public void InvoiceAnnotations_WithNullOptionalFields_ShouldSerializeWithoutErrors()
    {
        // Arrange
        var annotations = new InvoiceAnnotations
        {
            CashMethod = AnnotationValue.No,
            SelfBilling = AnnotationValue.No,
            ReverseCharge = AnnotationValue.No,
            SplitPayment = AnnotationValue.No,
            Exemption = null,
            NewTransportMeans = null
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(annotations);

        // Assert
        xml.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void InvoiceAnnotations_HelperProperties_ShouldHandleNullValues()
    {
        // Arrange
        var annotations = new InvoiceAnnotations
        {
            CashMethod = AnnotationValue.No,
            SelfBilling = AnnotationValue.No,
            ReverseCharge = AnnotationValue.No,
            SplitPayment = AnnotationValue.No
        };

        // Assert
        annotations.IsCashMethod.Should().BeFalse();
        annotations.IsSelfBilling.Should().BeFalse();
        annotations.IsReverseCharge.Should().BeFalse();
        annotations.IsSplitPayment.Should().BeFalse();
        annotations.HasExemption.Should().BeFalse();
        annotations.HasNewTransportMeans.Should().BeFalse();
    }

    [Fact]
    public void InvoiceAnnotations_HelperProperties_ShouldReturnTrueWhenSet()
    {
        // Arrange
        var annotations = new InvoiceAnnotations
        {
            CashMethod = AnnotationValue.Yes,
            SelfBilling = AnnotationValue.Yes,
            ReverseCharge = AnnotationValue.Yes,
            SplitPayment = AnnotationValue.Yes,
            Exemption = new VatExemption { Reason = "Test reason" },
            NewTransportMeans = new NewTransportMeans { IsNewTransportMeans = true }
        };

        // Assert
        annotations.IsCashMethod.Should().BeTrue();
        annotations.IsSelfBilling.Should().BeTrue();
        annotations.IsReverseCharge.Should().BeTrue();
        annotations.IsSplitPayment.Should().BeTrue();
        annotations.HasExemption.Should().BeTrue();
        annotations.HasNewTransportMeans.Should().BeTrue();
    }

    #endregion

    #region Invoice Helper Properties Tests

    [Fact]
    public void Invoice_HelperProperties_ShouldHandleNullCollections()
    {
        // Arrange
        var invoice = new Invoice.Models.Invoice
        {
            Header = new InvoiceHeader(),
            Seller = new Seller { TaxId = "1234567890", Name = "Test" },
            Buyer = new Buyer { TaxId = "0987654321", Name = "Test" },
            InvoiceData = new InvoiceData
            {
                InvoiceNumber = "FV/001",
                TotalAmount = 1000m,
                Annotations = new InvoiceAnnotations()
            },
            Recipients = null,
            AuthorizedEntity = null,
            Footer = null,
            Attachments = null
        };

        // Assert
        invoice.HasRecipients.Should().BeFalse();
        invoice.HasAuthorizedEntity.Should().BeFalse();
        invoice.HasFooter.Should().BeFalse();
        invoice.HasAttachments.Should().BeFalse();
    }

    [Fact]
    public void Invoice_HelperProperties_ShouldHandleEmptyCollections()
    {
        // Arrange
        var invoice = new Invoice.Models.Invoice
        {
            Header = new InvoiceHeader(),
            Seller = new Seller { TaxId = "1234567890", Name = "Test" },
            Buyer = new Buyer { TaxId = "0987654321", Name = "Test" },
            InvoiceData = new InvoiceData
            {
                InvoiceNumber = "FV/001",
                TotalAmount = 1000m,
                Annotations = new InvoiceAnnotations()
            },
            Recipients = new List<ThirdParty>()
        };

        // Assert
        invoice.HasRecipients.Should().BeFalse();
    }

    #endregion

    #region InvoiceData Calculation Tests

    [Fact]
    public void InvoiceData_TotalNetAmount_ShouldHandleNullValues()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            InvoiceNumber = "FV/001",
            TotalAmount = 1000m,
            Annotations = new InvoiceAnnotations(),
            NetAmount23 = null,
            NetAmount8 = null,
            NetAmount5 = null,
            NetAmount4 = null
        };

        // Assert
        invoiceData.TotalNetAmount.Should().Be(0);
    }

    [Fact]
    public void InvoiceData_TotalNetAmount_ShouldSumAllNonNullValues()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            InvoiceNumber = "FV/001",
            TotalAmount = 1230m,
            Annotations = new InvoiceAnnotations(),
            NetAmount23 = 500m,
            NetAmount8 = 300m,
            NetAmount5 = 200m,
            ExemptAmount = 100m
        };

        // Assert
        invoiceData.TotalNetAmount.Should().Be(1100m);
    }

    [Fact]
    public void InvoiceData_TotalVatAmount_ShouldHandleNullValues()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            InvoiceNumber = "FV/001",
            TotalAmount = 1000m,
            Annotations = new InvoiceAnnotations(),
            VatAmount23 = null,
            VatAmount8 = null,
            VatAmount5 = null,
            VatAmount4 = null
        };

        // Assert
        invoiceData.TotalVatAmount.Should().Be(0);
    }

    [Fact]
    public void InvoiceData_TotalVatAmount_ShouldSumAllNonNullValues()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            InvoiceNumber = "FV/001",
            TotalAmount = 1230m,
            Annotations = new InvoiceAnnotations(),
            VatAmount23 = 115m,
            VatAmount8 = 24m,
            VatAmount5 = 10m
        };

        // Assert
        invoiceData.TotalVatAmount.Should().Be(149m);
    }

    #endregion
}
