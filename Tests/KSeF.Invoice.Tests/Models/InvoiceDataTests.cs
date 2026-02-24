using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Payments;
using KSeF.Invoice.Models.Summary;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class InvoiceDataTests
{
    #region Default Values Tests

    [Fact]
    public void InvoiceData_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var invoiceData = new InvoiceData();

        // Assert
        invoiceData.CurrencyCode.Should().Be(CurrencyCode.PLN);
        invoiceData.InvoiceNumber.Should().Be(string.Empty);
        invoiceData.InvoiceType.Should().Be(InvoiceType.VAT);
    }

    [Fact]
    public void InvoiceData_Annotations_ShouldBeInitialized()
    {
        // Arrange & Act
        var invoiceData = new InvoiceData();

        // Assert
        invoiceData.Annotations.Should().NotBeNull();
    }

    #endregion

    #region Helper Properties Tests

    [Fact]
    public void InvoiceData_HasSaleDate_ShouldReturnFalseWhenNull()
    {
        // Arrange
        var invoiceData = new InvoiceData { SaleDate = null };

        // Assert
        invoiceData.HasSaleDate.Should().BeFalse();
    }

    [Fact]
    public void InvoiceData_HasSaleDate_ShouldReturnTrueWhenSet()
    {
        // Arrange
        var invoiceData = new InvoiceData { SaleDate = new DateOnly(2024, 1, 15) };

        // Assert
        invoiceData.HasSaleDate.Should().BeTrue();
    }

    [Fact]
    public void InvoiceData_HasSalePeriod_ShouldReturnCorrectValue()
    {
        // Arrange
        var invoiceData1 = new InvoiceData { SalePeriod = null };
        var invoiceData2 = new InvoiceData { SalePeriod = new SalePeriod() };

        // Assert
        invoiceData1.HasSalePeriod.Should().BeFalse();
        invoiceData2.HasSalePeriod.Should().BeTrue();
    }

    [Fact]
    public void InvoiceData_HasWarehouseDocuments_ShouldReturnCorrectValue()
    {
        // Arrange
        var invoiceData1 = new InvoiceData { WarehouseDocuments = null };
        var invoiceData2 = new InvoiceData { WarehouseDocuments = new List<string>() };
        var invoiceData3 = new InvoiceData { WarehouseDocuments = new List<string> { "WZ/001" } };

        // Assert
        invoiceData1.HasWarehouseDocuments.Should().BeFalse();
        invoiceData2.HasWarehouseDocuments.Should().BeFalse();
        invoiceData3.HasWarehouseDocuments.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Warszawa", true)]
    public void InvoiceData_HasIssuePlace_ShouldReturnCorrectValue(string? issuePlace, bool expected)
    {
        // Arrange
        var invoiceData = new InvoiceData { IssuePlace = issuePlace };

        // Assert
        invoiceData.HasIssuePlace.Should().Be(expected);
    }

    [Fact]
    public void InvoiceData_HasLineItems_ShouldReturnCorrectValue()
    {
        // Arrange
        var invoiceData1 = new InvoiceData { LineItems = null };
        var invoiceData2 = new InvoiceData { LineItems = new List<InvoiceLineItem>() };
        var invoiceData3 = new InvoiceData
        {
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { LineNumber = 1, ProductName = "Test" }
            }
        };

        // Assert
        invoiceData1.HasLineItems.Should().BeFalse();
        invoiceData2.HasLineItems.Should().BeFalse();
        invoiceData3.HasLineItems.Should().BeTrue();
    }

    [Fact]
    public void InvoiceData_HasPayment_ShouldReturnCorrectValue()
    {
        // Arrange
        var invoiceData1 = new InvoiceData { Payment = null };
        var invoiceData2 = new InvoiceData { Payment = new Payment() };

        // Assert
        invoiceData1.HasPayment.Should().BeFalse();
        invoiceData2.HasPayment.Should().BeTrue();
    }

    #endregion

    #region IsCorrection Tests

    [Theory]
    [InlineData(InvoiceType.KOR, true)]
    [InlineData(InvoiceType.KOR_ZAL, true)]
    [InlineData(InvoiceType.KOR_ROZ, true)]
    [InlineData(InvoiceType.VAT, false)]
    [InlineData(InvoiceType.ZAL, false)]
    [InlineData(InvoiceType.ROZ, false)]
    [InlineData(InvoiceType.UPR, false)]
    public void InvoiceData_IsCorrection_ShouldReturnCorrectValue(InvoiceType type, bool expectedIsCorrection)
    {
        // Arrange
        var invoiceData = new InvoiceData { InvoiceType = type };

        // Assert
        invoiceData.IsCorrection.Should().Be(expectedIsCorrection);
    }

    #endregion

    #region TotalNetAmount Calculation Tests

    [Fact]
    public void InvoiceData_TotalNetAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            NetAmount23 = 1000.00m,
            NetAmount8 = 500.00m,
            NetAmount5 = 250.00m,
            NetAmount4 = 100.00m,
            NetAmount0 = 50.00m,
            NetAmountWdt = 200.00m,
            NetAmountExport = 150.00m,
            ExemptAmount = 75.00m,
            NetAmountTaxi = 25.00m,
            NetAmountOSS = 30.00m,
            MarginAmount = 20.00m,
            NotTaxableAmount = 10.00m
        };

        // Assert
        invoiceData.TotalNetAmount.Should().Be(2410.00m);
    }

    [Fact]
    public void InvoiceData_TotalNetAmount_ShouldHandlePartialValues()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            NetAmount23 = 1000.00m,
            NetAmount8 = null,
            NetAmount5 = 500.00m,
            NetAmount4 = null
        };

        // Assert
        invoiceData.TotalNetAmount.Should().Be(1500.00m);
    }

    #endregion

    #region TotalVatAmount Calculation Tests

    [Fact]
    public void InvoiceData_TotalVatAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            VatAmount23 = 230.00m,
            VatAmount8 = 40.00m,
            VatAmount5 = 12.50m,
            VatAmount4 = 4.00m,
            VatAmountTaxi = 5.00m,
            VatAmountOSS = 3.00m,
            MarginVatAmount = 2.00m
        };

        // Assert
        invoiceData.TotalVatAmount.Should().Be(296.50m);
    }

    [Fact]
    public void InvoiceData_TotalVatAmount_ShouldHandlePartialValues()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            VatAmount23 = 230.00m,
            VatAmount8 = null,
            VatAmount5 = 25.00m,
            VatAmount4 = null
        };

        // Assert
        invoiceData.TotalVatAmount.Should().Be(255.00m);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    public void InvoiceData_RoundTrip_ShouldPreserveMainValues()
    {
        // Arrange
        // Note: DateOnly serialization requires special handling, so we test other fields
        var invoiceData = new InvoiceData
        {
            CurrencyCode = CurrencyCode.EUR,
            IssuePlace = "Warszawa",
            InvoiceNumber = "FV/2024/001",
            NetAmount23 = 1000.00m,
            VatAmount23 = 230.00m,
            TotalAmount = 1230.00m,
            InvoiceType = InvoiceType.VAT,
            Annotations = new InvoiceAnnotations
            {
                CashMethod = AnnotationValue.No,
                SelfBilling = AnnotationValue.No,
                ReverseCharge = AnnotationValue.No,
                SplitPayment = AnnotationValue.No
            }
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(invoiceData);

        // Assert
        result.Should().NotBeNull();
        result!.CurrencyCode.Should().Be(CurrencyCode.EUR);
        result.IssuePlace.Should().Be("Warszawa");
        result.InvoiceNumber.Should().Be("FV/2024/001");
        result.NetAmount23.Should().Be(1000.00m);
        result.VatAmount23.Should().Be(230.00m);
        result.TotalAmount.Should().Be(1230.00m);
        result.InvoiceType.Should().Be(InvoiceType.VAT);
    }

    [Theory]
    [InlineData(CurrencyCode.PLN)]
    [InlineData(CurrencyCode.EUR)]
    [InlineData(CurrencyCode.USD)]
    [InlineData(CurrencyCode.GBP)]
    [InlineData(CurrencyCode.CHF)]
    public void InvoiceData_RoundTrip_ShouldPreserveCurrencyCode(CurrencyCode currencyCode)
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            CurrencyCode = currencyCode,
            IssueDate = new DateOnly(2024, 1, 15),
            InvoiceNumber = "FV/001",
            TotalAmount = 1000m,
            Annotations = new InvoiceAnnotations()
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(invoiceData);

        // Assert
        result.Should().NotBeNull();
        result!.CurrencyCode.Should().Be(currencyCode);
    }

    #endregion
}
