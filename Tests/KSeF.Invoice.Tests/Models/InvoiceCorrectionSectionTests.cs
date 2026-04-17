using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class InvoiceCorrectionSectionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateEmptySection()
    {
        // Arrange & Act
        var section = new InvoiceCorrectionSection();

        // Assert
        section.CorrectionReason.Should().BeNull();
        section.CorrectionType.Should().BeNull();
        section.CorrectedInvoiceData.Should().BeNull();
        section.CorrectedInvoicePeriod.Should().BeNull();
        section.CorrectedInvoiceNumberAmended.Should().BeNull();
        section.AmountBeforeCorrection.Should().BeNull();
        section.ExchangeRateBeforeCorrection.Should().BeNull();
        section.HasAnyData.Should().BeFalse();
    }

    [Fact]
    public void SetCorrectionReason_ShouldUpdateProperty()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();

        // Act
        section.CorrectionReason = "Korekta ilości";

        // Assert
        section.CorrectionReason.Should().Be("Korekta ilości");
        section.HasAnyData.Should().BeTrue();
    }

    [Fact]
    public void SetCorrectionType_ShouldUpdateProperty()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();

        // Act
        section.CorrectionType = 2;

        // Assert
        section.CorrectionType.Should().Be(2);
        section.HasAnyData.Should().BeTrue();
    }

    [Fact]
    public void SetCorrectedInvoiceData_ShouldUpdateProperty()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var data = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/001",
            IsIssuedOutsideKSeF = true
        };

        // Act
        section.CorrectedInvoiceData = data;

        // Assert
        section.CorrectedInvoiceData.Should().BeSameAs(data);
        section.HasAnyData.Should().BeTrue();
    }

    [Fact]
    public void SetCorrectedInvoicePeriod_ShouldUpdateProperty()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var period = new SalePeriod
        {
            PeriodFrom = new DateOnly(2025, 1, 1),
            PeriodTo = new DateOnly(2025, 1, 31)
        };

        // Act
        section.CorrectedInvoicePeriod = period;

        // Assert
        section.CorrectedInvoicePeriod.Should().BeSameAs(period);
        section.HasAnyData.Should().BeTrue();
    }

    [Fact]
    public void SetCorrectedInvoiceNumberAmended_ShouldUpdateProperty()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();

        // Act
        section.CorrectedInvoiceNumberAmended = "FV/2025/001-POPRAWIONY";

        // Assert
        section.CorrectedInvoiceNumberAmended.Should().Be("FV/2025/001-POPRAWIONY");
        section.HasAnyData.Should().BeTrue();
    }

    [Fact]
    public void SetAmountBeforeCorrection_ShouldUpdateProperty()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();

        // Act
        section.AmountBeforeCorrection = 1500.50m;

        // Assert
        section.AmountBeforeCorrection.Should().Be(1500.50m);
        section.HasAnyData.Should().BeTrue();
    }

    [Fact]
    public void SetExchangeRateBeforeCorrection_ShouldUpdateProperty()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();

        // Act
        section.ExchangeRateBeforeCorrection = 4.3521m;

        // Assert
        section.ExchangeRateBeforeCorrection.Should().Be(4.3521m);
        section.HasAnyData.Should().BeTrue();
    }

    #region Proxy delegation tests in InvoiceData

    [Fact]
    public void InvoiceData_CorrectionReason_ShouldDelegateToCorrection()
    {
        // Arrange
        var invoiceData = new InvoiceData();

        // Act
        invoiceData.CorrectionReason = "Błąd w cenie";

        // Assert
        invoiceData.Correction.CorrectionReason.Should().Be("Błąd w cenie");
        invoiceData.CorrectionReason.Should().Be("Błąd w cenie");
    }

    [Fact]
    public void InvoiceData_CorrectionType_ShouldDelegateToCorrection()
    {
        // Arrange
        var invoiceData = new InvoiceData();

        // Act
        invoiceData.CorrectionType = 3;

        // Assert
        invoiceData.Correction.CorrectionType.Should().Be(3);
    }

    [Fact]
    public void InvoiceData_CorrectedInvoiceData_ShouldDelegateToCorrection()
    {
        // Arrange
        var invoiceData = new InvoiceData();
        var corrData = new CorrectedInvoiceData { CorrectedInvoiceNumber = "FV/001" };

        // Act
        invoiceData.CorrectedInvoiceData = corrData;

        // Assert
        invoiceData.Correction.CorrectedInvoiceData.Should().BeSameAs(corrData);
    }

    [Fact]
    public void InvoiceData_CorrectedInvoicePeriod_ShouldDelegateToCorrection()
    {
        // Arrange
        var invoiceData = new InvoiceData();
        var period = new SalePeriod { PeriodFrom = new DateOnly(2025, 3, 1), PeriodTo = new DateOnly(2025, 3, 31) };

        // Act
        invoiceData.CorrectedInvoicePeriod = period;

        // Assert
        invoiceData.Correction.CorrectedInvoicePeriod.Should().BeSameAs(period);
    }

    [Fact]
    public void InvoiceData_NewCorrectionFields_ShouldDelegateToCorrection()
    {
        // Arrange
        var invoiceData = new InvoiceData();

        // Act
        invoiceData.CorrectedInvoiceNumberAmended = "FV/2025/001-AMENDED";
        invoiceData.AmountBeforeCorrection = 999.99m;
        invoiceData.ExchangeRateBeforeCorrection = 4.1234m;

        // Assert
        invoiceData.Correction.CorrectedInvoiceNumberAmended.Should().Be("FV/2025/001-AMENDED");
        invoiceData.Correction.AmountBeforeCorrection.Should().Be(999.99m);
        invoiceData.Correction.ExchangeRateBeforeCorrection.Should().Be(4.1234m);
    }

    [Fact]
    public void InvoiceData_SetCorrection_ShouldReflectInProxyProperties()
    {
        // Arrange
        var invoiceData = new InvoiceData();
        var section = new InvoiceCorrectionSection
        {
            CorrectionReason = "Test reason",
            CorrectionType = 2,
            AmountBeforeCorrection = 500m
        };

        // Act
        invoiceData.Correction = section;

        // Assert
        invoiceData.CorrectionReason.Should().Be("Test reason");
        invoiceData.CorrectionType.Should().Be(2);
        invoiceData.AmountBeforeCorrection.Should().Be(500m);
    }

    [Fact]
    public void InvoiceData_SetCorrectionToNull_ShouldResetToEmpty()
    {
        // Arrange
        var invoiceData = new InvoiceData();
        invoiceData.CorrectionReason = "Something";

        // Act
        invoiceData.Correction = null!;

        // Assert
        invoiceData.Correction.Should().NotBeNull();
        invoiceData.CorrectionReason.Should().BeNull();
    }

    #endregion
}
