using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using Xunit;

namespace KSeF.Invoice.Tests.Builders;

/// <summary>
/// Testy jednostkowe dla automatycznego obliczania sum VAT w InvoiceBuilder
/// </summary>
public class VatSummaryCalculationTests
{
    #region Single VAT Rate Tests

    [Fact]
    public void Build_WithRate23_ShouldCalculateCorrectVatSummary()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 1")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .AddLineItem(l => l
                .WithProductName("Item 2")
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(115.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount23.Should().Be(1500.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(345.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1845.00m);
    }

    [Fact]
    public void Build_WithRate22_ShouldAggregateWithRate23()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 23%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .AddLineItem(l => l
                .WithProductName("Item 22%")
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.Rate22)
                .WithVatAmount(110.00m))
            .Build();

        // Assert - Rate22 agreguje się z Rate23
        invoice.InvoiceData.NetAmount23.Should().Be(1500.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(340.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1840.00m);
    }

    [Fact]
    public void Build_WithRate8_ShouldCalculateCorrectVatSummary()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 8%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(80.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount8.Should().Be(1000.00m);
        invoice.InvoiceData.VatAmount8.Should().Be(80.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1080.00m);
    }

    [Fact]
    public void Build_WithRate7_ShouldAggregateWithRate8()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 8%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(80.00m))
            .AddLineItem(l => l
                .WithProductName("Item 7%")
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.Rate7)
                .WithVatAmount(35.00m))
            .Build();

        // Assert - Rate7 agreguje się z Rate8
        invoice.InvoiceData.NetAmount8.Should().Be(1500.00m);
        invoice.InvoiceData.VatAmount8.Should().Be(115.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1615.00m);
    }

    [Fact]
    public void Build_WithRate5_ShouldCalculateCorrectVatSummary()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 5%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(50.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount5.Should().Be(1000.00m);
        invoice.InvoiceData.VatAmount5.Should().Be(50.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1050.00m);
    }

    [Fact]
    public void Build_WithRate4_ShouldCalculateCorrectVatSummary()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 4%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate4)
                .WithVatAmount(40.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount4.Should().Be(1000.00m);
        invoice.InvoiceData.VatAmount4.Should().Be(40.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1040.00m);
    }

    [Fact]
    public void Build_WithRate3_ShouldAggregateWithRate4()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 4%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate4)
                .WithVatAmount(40.00m))
            .AddLineItem(l => l
                .WithProductName("Item 3%")
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.Rate3)
                .WithVatAmount(15.00m))
            .Build();

        // Assert - Rate3 agreguje się z Rate4
        invoice.InvoiceData.NetAmount4.Should().Be(1500.00m);
        invoice.InvoiceData.VatAmount4.Should().Be(55.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1555.00m);
    }

    #endregion

    #region Zero Rate Tests

    [Fact]
    public void Build_WithRate0Domestic_ShouldCalculateNetAmountOnly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 0% KR")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate0Domestic)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount0.Should().Be(1000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1000.00m);
    }

    [Fact]
    public void Build_WithRate0IntraCommunitySupply_ShouldCalculateWdtAmount()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item WDT")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate0IntraCommunitySupply)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmountWdt.Should().Be(1000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1000.00m);
    }

    [Fact]
    public void Build_WithRate0Export_ShouldCalculateExportAmount()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item Export")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate0Export)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmountExport.Should().Be(1000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1000.00m);
    }

    #endregion

    #region Special Rate Tests

    [Fact]
    public void Build_WithExempt_ShouldCalculateExemptAmount()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item Zwolnione")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Exempt)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.ExemptAmount.Should().Be(1000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1000.00m);
    }

    [Fact]
    public void Build_WithReverseCharge_ShouldCalculateNotTaxableAmount()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item Odwrotne obciążenie")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.ReverseCharge)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NotTaxableAmount.Should().Be(1000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1000.00m);
    }

    [Fact]
    public void Build_WithNotSubjectToTaxI_ShouldCalculateNotTaxableAmount()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item np I")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.NotSubjectToTaxI)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NotTaxableAmount.Should().Be(1000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1000.00m);
    }

    [Fact]
    public void Build_WithNotSubjectToTaxII_ShouldCalculateNotTaxableAmount()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item np II")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.NotSubjectToTaxII)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NotTaxableAmount.Should().Be(1000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1000.00m);
    }

    #endregion

    #region Mixed Rates Tests

    [Fact]
    public void Build_WithMultipleRates_ShouldCalculateAllSummariesCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 23%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .AddLineItem(l => l
                .WithProductName("Item 8%")
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(40.00m))
            .AddLineItem(l => l
                .WithProductName("Item 5%")
                .WithNetAmount(300.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(15.00m))
            .AddLineItem(l => l
                .WithProductName("Item Zwolnione")
                .WithNetAmount(200.00m)
                .WithVatRate(VatRate.Exempt)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount23.Should().Be(1000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(230.00m);
        invoice.InvoiceData.NetAmount8.Should().Be(500.00m);
        invoice.InvoiceData.VatAmount8.Should().Be(40.00m);
        invoice.InvoiceData.NetAmount5.Should().Be(300.00m);
        invoice.InvoiceData.VatAmount5.Should().Be(15.00m);
        invoice.InvoiceData.ExemptAmount.Should().Be(200.00m);
        // Total = 1000 + 230 + 500 + 40 + 300 + 15 + 200 = 2285
        invoice.InvoiceData.TotalAmount.Should().Be(2285.00m);
    }

    [Fact]
    public void Build_WithAllZeroRates_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 0% KR")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate0Domestic)
                .WithVatAmount(0.00m))
            .AddLineItem(l => l
                .WithProductName("Item WDT")
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.Rate0IntraCommunitySupply)
                .WithVatAmount(0.00m))
            .AddLineItem(l => l
                .WithProductName("Item Export")
                .WithNetAmount(300.00m)
                .WithVatRate(VatRate.Rate0Export)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount0.Should().Be(1000.00m);
        invoice.InvoiceData.NetAmountWdt.Should().Be(500.00m);
        invoice.InvoiceData.NetAmountExport.Should().Be(300.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1800.00m);
    }

    [Fact]
    public void Build_WithSpecialAndRegularRates_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 23%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .AddLineItem(l => l
                .WithProductName("Item odwrotne obciążenie")
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.ReverseCharge)
                .WithVatAmount(0.00m))
            .AddLineItem(l => l
                .WithProductName("Item np I")
                .WithNetAmount(300.00m)
                .WithVatRate(VatRate.NotSubjectToTaxI)
                .WithVatAmount(0.00m))
            .AddLineItem(l => l
                .WithProductName("Item np II")
                .WithNetAmount(200.00m)
                .WithVatRate(VatRate.NotSubjectToTaxII)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount23.Should().Be(1000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(230.00m);
        invoice.InvoiceData.NotTaxableAmount.Should().Be(1000.00m); // 500 + 300 + 200
        // Total = 1000 + 230 + 500 + 300 + 200 = 2230
        invoice.InvoiceData.TotalAmount.Should().Be(2230.00m);
    }

    #endregion

    #region Rounding Tests

    [Fact]
    public void Build_WithDecimalAmounts_ShouldRoundTotalCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item 1")
                .WithNetAmount(100.33m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23.08m))
            .AddLineItem(l => l
                .WithProductName("Item 2")
                .WithNetAmount(200.67m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(46.15m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount23.Should().Be(301.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(69.23m);
        invoice.InvoiceData.TotalAmount.Should().Be(370.23m);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Build_WithNoLineItems_ShouldNotCalculateSummary()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.InvoiceData.LineItems.Should().BeNull();
        invoice.InvoiceData.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void Build_WithLineItemsWithoutNetAmount_ShouldIgnoreThoseItems()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item with amounts")
                .WithNetAmount(100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23.00m))
            .AddLineItem(l => l
                .WithProductName("Item without amounts")
                .WithVatRate(VatRate.Rate23))
            .Build();

        // Assert - tylko pierwszy item jest liczony
        invoice.InvoiceData.NetAmount23.Should().Be(100.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(23.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(123.00m);
    }

    [Fact]
    public void Build_WithSameRateMultipleItems_ShouldSumCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l.WithProductName("Item 1").WithNetAmount(100.00m).WithVatRate(VatRate.Rate23).WithVatAmount(23.00m))
            .AddLineItem(l => l.WithProductName("Item 2").WithNetAmount(200.00m).WithVatRate(VatRate.Rate23).WithVatAmount(46.00m))
            .AddLineItem(l => l.WithProductName("Item 3").WithNetAmount(300.00m).WithVatRate(VatRate.Rate23).WithVatAmount(69.00m))
            .AddLineItem(l => l.WithProductName("Item 4").WithNetAmount(400.00m).WithVatRate(VatRate.Rate23).WithVatAmount(92.00m))
            .AddLineItem(l => l.WithProductName("Item 5").WithNetAmount(500.00m).WithVatRate(VatRate.Rate23).WithVatAmount(115.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount23.Should().Be(1500.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(345.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1845.00m);
    }

    [Fact]
    public void Build_WithVerySmallAmounts_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Small Item")
                .WithNetAmount(0.01m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount23.Should().Be(0.01m);
        invoice.InvoiceData.VatAmount23.Should().Be(0.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(0.01m);
    }

    [Fact]
    public void Build_WithLargeAmounts_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Large Item")
                .WithNetAmount(999999999.99m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230000000.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount23.Should().Be(999999999.99m);
        invoice.InvoiceData.VatAmount23.Should().Be(230000000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1229999999.99m);
    }

    #endregion

    #region CalculateAmounts in LineItemBuilder Tests

    [Fact]
    public void LineItemBuilder_CalculateAmounts_ShouldCalculateVatFor23Percent()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Auto-calculated Item")
                .WithQuantity(10)
                .WithUnitNetPrice(100.00m)
                .WithVatRate(VatRate.Rate23)
                .CalculateAmounts())
            .Build();

        // Assert
        var item = invoice.InvoiceData.LineItems![0];
        item.NetAmount.Should().Be(1000.00m);
        item.VatAmount.Should().Be(230.00m);
    }

    [Fact]
    public void LineItemBuilder_CalculateAmounts_ShouldCalculateVatFor8Percent()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Auto-calculated 8%")
                .WithQuantity(5)
                .WithUnitNetPrice(200.00m)
                .WithVatRate(VatRate.Rate8)
                .CalculateAmounts())
            .Build();

        // Assert
        var item = invoice.InvoiceData.LineItems![0];
        item.NetAmount.Should().Be(1000.00m);
        item.VatAmount.Should().Be(80.00m);
    }

    [Fact]
    public void LineItemBuilder_CalculateAmounts_ShouldCalculateVatFor5Percent()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Auto-calculated 5%")
                .WithQuantity(4)
                .WithUnitNetPrice(250.00m)
                .WithVatRate(VatRate.Rate5)
                .CalculateAmounts())
            .Build();

        // Assert
        var item = invoice.InvoiceData.LineItems![0];
        item.NetAmount.Should().Be(1000.00m);
        item.VatAmount.Should().Be(50.00m);
    }

    [Fact]
    public void LineItemBuilder_CalculateAmounts_ShouldCalculateZeroVatForExempt()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Exempt Item")
                .WithQuantity(2)
                .WithUnitNetPrice(500.00m)
                .WithVatRate(VatRate.Exempt)
                .CalculateAmounts())
            .Build();

        // Assert
        var item = invoice.InvoiceData.LineItems![0];
        item.NetAmount.Should().Be(1000.00m);
        item.VatAmount.Should().Be(0.00m);
    }

    [Theory]
    [InlineData(VatRate.Rate23, 0.23)]
    [InlineData(VatRate.Rate22, 0.22)]
    [InlineData(VatRate.Rate8, 0.08)]
    [InlineData(VatRate.Rate7, 0.07)]
    [InlineData(VatRate.Rate5, 0.05)]
    [InlineData(VatRate.Rate4, 0.04)]
    [InlineData(VatRate.Rate3, 0.03)]
    [InlineData(VatRate.Rate0Domestic, 0.00)]
    [InlineData(VatRate.Rate0IntraCommunitySupply, 0.00)]
    [InlineData(VatRate.Rate0Export, 0.00)]
    [InlineData(VatRate.Exempt, 0.00)]
    [InlineData(VatRate.ReverseCharge, 0.00)]
    [InlineData(VatRate.NotSubjectToTaxI, 0.00)]
    [InlineData(VatRate.NotSubjectToTaxII, 0.00)]
    public void LineItemBuilder_CalculateAmounts_ShouldCalculateCorrectVatForAllRates(VatRate vatRate, double expectedMultiplier)
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test Item")
                .WithQuantity(1)
                .WithUnitNetPrice(1000.00m)
                .WithVatRate(vatRate)
                .CalculateAmounts())
            .Build();

        // Assert
        var item = invoice.InvoiceData.LineItems![0];
        item.NetAmount.Should().Be(1000.00m);
        item.VatAmount.Should().Be((decimal)(1000.00 * expectedMultiplier));
    }

    #endregion
}
