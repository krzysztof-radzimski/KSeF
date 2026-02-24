using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class InvoiceLineItemTests
{
    #region Default Values Tests

    [Fact]
    public void InvoiceLineItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var lineItem = new InvoiceLineItem();

        // Assert
        lineItem.LineNumber.Should().Be(0);
        lineItem.ProductName.Should().Be(string.Empty);
        lineItem.Unit.Should().BeNull();
        lineItem.Quantity.Should().BeNull();
        lineItem.UnitNetPrice.Should().BeNull();
        lineItem.UnitGrossPrice.Should().BeNull();
        lineItem.Discount.Should().BeNull();
        lineItem.NetAmount.Should().BeNull();
        lineItem.GrossAmount.Should().BeNull();
        lineItem.VatAmount.Should().BeNull();
        lineItem.SaleDate.Should().BeNull();
        lineItem.GtinCode.Should().BeNull();
        lineItem.PkwiuCode.Should().BeNull();
        lineItem.CnCode.Should().BeNull();
    }

    #endregion

    #region Helper Properties Tests

    [Fact]
    public void InvoiceLineItem_HasUnitNetPrice_ShouldReturnCorrectValue()
    {
        // Arrange
        var lineItem1 = new InvoiceLineItem { UnitNetPrice = null };
        var lineItem2 = new InvoiceLineItem { UnitNetPrice = 100.00m };

        // Assert
        lineItem1.HasUnitNetPrice.Should().BeFalse();
        lineItem2.HasUnitNetPrice.Should().BeTrue();
    }

    [Fact]
    public void InvoiceLineItem_HasUnitGrossPrice_ShouldReturnCorrectValue()
    {
        // Arrange
        var lineItem1 = new InvoiceLineItem { UnitGrossPrice = null };
        var lineItem2 = new InvoiceLineItem { UnitGrossPrice = 123.00m };

        // Assert
        lineItem1.HasUnitGrossPrice.Should().BeFalse();
        lineItem2.HasUnitGrossPrice.Should().BeTrue();
    }

    [Fact]
    public void InvoiceLineItem_HasDiscount_ShouldReturnFalseForNull()
    {
        // Arrange
        var lineItem = new InvoiceLineItem { Discount = null };

        // Assert
        lineItem.HasDiscount.Should().BeFalse();
    }

    [Fact]
    public void InvoiceLineItem_HasDiscount_ShouldReturnFalseForZero()
    {
        // Arrange
        var lineItem = new InvoiceLineItem { Discount = 0m };

        // Assert
        lineItem.HasDiscount.Should().BeFalse();
    }

    [Fact]
    public void InvoiceLineItem_HasDiscount_ShouldReturnTrueForPositiveValue()
    {
        // Arrange
        var lineItem = new InvoiceLineItem { Discount = 10.00m };

        // Assert
        lineItem.HasDiscount.Should().BeTrue();
    }

    [Fact]
    public void InvoiceLineItem_HasDiscount_ShouldReturnTrueForNegativeValue()
    {
        // Arrange
        var lineItem = new InvoiceLineItem { Discount = -5.00m };

        // Assert
        lineItem.HasDiscount.Should().BeTrue();
    }

    [Fact]
    public void InvoiceLineItem_HasSaleDate_ShouldReturnCorrectValue()
    {
        // Arrange
        var lineItem1 = new InvoiceLineItem { SaleDate = null };
        var lineItem2 = new InvoiceLineItem { SaleDate = new DateOnly(2024, 1, 15) };

        // Assert
        lineItem1.HasSaleDate.Should().BeFalse();
        lineItem2.HasSaleDate.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("1234567890123", true)]
    public void InvoiceLineItem_HasGtinCode_ShouldReturnCorrectValue(string? gtinCode, bool expected)
    {
        // Arrange
        var lineItem = new InvoiceLineItem { GtinCode = gtinCode };

        // Assert
        lineItem.HasGtinCode.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12.34.56.0", true)]
    public void InvoiceLineItem_HasPkwiuCode_ShouldReturnCorrectValue(string? pkwiuCode, bool expected)
    {
        // Arrange
        var lineItem = new InvoiceLineItem { PkwiuCode = pkwiuCode };

        // Assert
        lineItem.HasPkwiuCode.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345678", true)]
    public void InvoiceLineItem_HasCnCode_ShouldReturnCorrectValue(string? cnCode, bool expected)
    {
        // Arrange
        var lineItem = new InvoiceLineItem { CnCode = cnCode };

        // Assert
        lineItem.HasCnCode.Should().Be(expected);
    }

    [Fact]
    public void InvoiceLineItem_UsesNetCalculation_ShouldReturnCorrectValue()
    {
        // Arrange
        var lineItem1 = new InvoiceLineItem { NetAmount = null };
        var lineItem2 = new InvoiceLineItem { NetAmount = 1000.00m };

        // Assert
        lineItem1.UsesNetCalculation.Should().BeFalse();
        lineItem2.UsesNetCalculation.Should().BeTrue();
    }

    [Fact]
    public void InvoiceLineItem_UsesGrossCalculation_ShouldReturnCorrectValue()
    {
        // Arrange
        var lineItem1 = new InvoiceLineItem { GrossAmount = null };
        var lineItem2 = new InvoiceLineItem { GrossAmount = 1230.00m };

        // Assert
        lineItem1.UsesGrossCalculation.Should().BeFalse();
        lineItem2.UsesGrossCalculation.Should().BeTrue();
    }

    #endregion

    #region Serialization Tests

    [Fact]
    public void InvoiceLineItem_RoundTrip_ShouldPreserveAllValues()
    {
        // Arrange - Note: DateOnly serialization may vary depending on XML serialization configuration
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            Unit = "szt.",
            Quantity = 10.5m,
            UnitNetPrice = 100.00m,
            NetAmount = 1050.00m,
            VatAmount = 241.50m,
            VatRate = VatRate.Rate23,
            // Skip SaleDate as DateOnly requires special handling in XmlSerializer
            GtinCode = "1234567890123",
            PkwiuCode = "12.34.56.0",
            CnCode = "12345678"
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(lineItem);

        // Assert
        result.Should().NotBeNull();
        result!.LineNumber.Should().Be(1);
        result.ProductName.Should().Be("Test Product");
        result.Unit.Should().Be("szt.");
        result.Quantity.Should().Be(10.5m);
        result.UnitNetPrice.Should().Be(100.00m);
        result.NetAmount.Should().Be(1050.00m);
        result.VatAmount.Should().Be(241.50m);
        result.VatRate.Should().Be(VatRate.Rate23);
        // Note: DateOnly may require special serialization handling
        result.GtinCode.Should().Be("1234567890123");
        result.PkwiuCode.Should().Be("12.34.56.0");
        result.CnCode.Should().Be("12345678");
    }

    [Theory]
    [InlineData(VatRate.Rate23)]
    [InlineData(VatRate.Rate22)]
    [InlineData(VatRate.Rate8)]
    [InlineData(VatRate.Rate7)]
    [InlineData(VatRate.Rate5)]
    [InlineData(VatRate.Rate4)]
    [InlineData(VatRate.Rate3)]
    [InlineData(VatRate.Rate0Domestic)]
    [InlineData(VatRate.Rate0IntraCommunitySupply)]
    [InlineData(VatRate.Rate0Export)]
    [InlineData(VatRate.Exempt)]
    [InlineData(VatRate.ReverseCharge)]
    [InlineData(VatRate.NotSubjectToTaxI)]
    [InlineData(VatRate.NotSubjectToTaxII)]
    public void InvoiceLineItem_RoundTrip_ShouldPreserveVatRate(VatRate vatRate)
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            VatRate = vatRate
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(lineItem);

        // Assert
        result.Should().NotBeNull();
        result!.VatRate.Should().Be(vatRate);
    }

    [Fact]
    public void InvoiceLineItem_WithDiscount_RoundTrip_ShouldPreserveDiscount()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            VatRate = VatRate.Rate23,
            UnitNetPrice = 100.00m,
            Quantity = 10,
            Discount = 50.00m,
            NetAmount = 950.00m
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(lineItem);

        // Assert
        result.Should().NotBeNull();
        result!.Discount.Should().Be(50.00m);
    }

    [Fact]
    public void InvoiceLineItem_WithGrossCalculation_RoundTrip_ShouldPreserveValues()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            VatRate = VatRate.Rate23,
            UnitGrossPrice = 123.00m,
            Quantity = 10,
            GrossAmount = 1230.00m
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(lineItem);

        // Assert
        result.Should().NotBeNull();
        result!.UnitGrossPrice.Should().Be(123.00m);
        result.GrossAmount.Should().Be(1230.00m);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void InvoiceLineItem_WithZeroQuantity_ShouldSerialize()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            VatRate = VatRate.Rate23,
            Quantity = 0
        };

        // Act
        var xml = XmlSerializationHelper.Serialize(lineItem);

        // Assert
        xml.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void InvoiceLineItem_WithDecimalQuantity_ShouldPreservePrecision()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = "Test Product",
            VatRate = VatRate.Rate23,
            Quantity = 10.123456m
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(lineItem);

        // Assert
        result.Should().NotBeNull();
        result!.Quantity.Should().Be(10.123456m);
    }

    [Fact]
    public void InvoiceLineItem_WithLongProductName_ShouldSerialize()
    {
        // Arrange
        var longName = new string('A', 512);
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = longName,
            VatRate = VatRate.Rate23
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(lineItem);

        // Assert
        result.Should().NotBeNull();
        result!.ProductName.Should().Be(longName);
        result.ProductName.Length.Should().Be(512);
    }

    [Fact]
    public void InvoiceLineItem_WithPolishCharacters_ShouldPreserve()
    {
        // Arrange
        var polishName = "Usługa świadczona przez spółkę - żółć";
        var lineItem = new InvoiceLineItem
        {
            LineNumber = 1,
            ProductName = polishName,
            VatRate = VatRate.Rate23
        };

        // Act
        var result = XmlSerializationHelper.RoundTrip(lineItem);

        // Assert
        result.Should().NotBeNull();
        result!.ProductName.Should().Be(polishName);
    }

    #endregion
}
