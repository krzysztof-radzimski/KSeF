using FluentAssertions;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using Xunit;

namespace KSeF.Invoice.Tests.Builders;

public class InvoiceCorrectionSectionBuilderTests
{
    [Fact]
    public void Constructor_WithNullSection_ShouldThrow()
    {
        // Arrange & Act
        var act = () => new InvoiceCorrectionSectionBuilder(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithReason_ShouldSetCorrectionReason()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        builder.WithReason("Korekta ilości");

        // Assert
        section.CorrectionReason.Should().Be("Korekta ilości");
    }

    [Fact]
    public void WithType_Int_ShouldSetCorrectionType()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        builder.WithType(2);

        // Assert
        section.CorrectionType.Should().Be(2);
    }

    [Fact]
    public void WithType_Enum_ShouldSetCorrectionType()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        builder.WithType(CorrectionType.CorrectionInvoiceDate);

        // Assert
        section.CorrectionType.Should().Be(2);
    }

    [Fact]
    public void WithCorrectedInvoice_ShouldBuildAndSetData()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        builder.WithCorrectedInvoice(inv => inv
            .WithInvoiceNumber("FV/2025/001")
            .WithIssueDate(new DateOnly(2025, 1, 15))
            .IssuedOutsideKSeF());

        // Assert
        section.CorrectedInvoiceData.Should().NotBeNull();
        section.CorrectedInvoiceData!.CorrectedInvoiceNumber.Should().Be("FV/2025/001");
        section.CorrectedInvoiceData.CorrectedInvoiceIssueDate.Should().Be(new DateOnly(2025, 1, 15));
        section.CorrectedInvoiceData.IsIssuedOutsideKSeF.Should().BeTrue();
    }

    [Fact]
    public void WithPeriod_ShouldSetCorrectedInvoicePeriod()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);
        var from = new DateOnly(2025, 1, 1);
        var to = new DateOnly(2025, 1, 31);

        // Act
        builder.WithPeriod(from, to);

        // Assert
        section.CorrectedInvoicePeriod.Should().NotBeNull();
        section.CorrectedInvoicePeriod!.PeriodFrom.Should().Be(from);
        section.CorrectedInvoicePeriod.PeriodTo.Should().Be(to);
    }

    [Fact]
    public void WithAmendedInvoiceNumber_ShouldSetNumber()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        builder.WithAmendedInvoiceNumber("FV/2025/001-CORRECTED");

        // Assert
        section.CorrectedInvoiceNumberAmended.Should().Be("FV/2025/001-CORRECTED");
    }

    [Fact]
    public void WithAmountBeforeCorrection_ShouldSetAmount()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        builder.WithAmountBeforeCorrection(1500.50m);

        // Assert
        section.AmountBeforeCorrection.Should().Be(1500.50m);
    }

    [Fact]
    public void WithExchangeRateBeforeCorrection_ShouldSetRate()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        builder.WithExchangeRateBeforeCorrection(4.3521m);

        // Assert
        section.ExchangeRateBeforeCorrection.Should().Be(4.3521m);
    }

    [Fact]
    public void FluentChain_ShouldSetAllFields()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        var result = builder
            .WithReason("Błąd w cenie")
            .WithType(CorrectionType.OriginalInvoiceDate)
            .WithCorrectedInvoice(inv => inv
                .WithInvoiceNumber("FV/2025/001")
                .WithIssueDate(new DateOnly(2025, 1, 15))
                .IssuedOutsideKSeF())
            .WithPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31))
            .WithAmendedInvoiceNumber("FV/2025/001-POPRAWIONY")
            .WithAmountBeforeCorrection(5000.00m)
            .WithExchangeRateBeforeCorrection(4.5678m)
            .Build();

        // Assert
        result.CorrectionReason.Should().Be("Błąd w cenie");
        result.CorrectionType.Should().Be(1);
        result.CorrectedInvoiceData.Should().NotBeNull();
        result.CorrectedInvoiceData!.CorrectedInvoiceNumber.Should().Be("FV/2025/001");
        result.CorrectedInvoicePeriod.Should().NotBeNull();
        result.CorrectedInvoicePeriod!.PeriodFrom.Should().Be(new DateOnly(2025, 1, 1));
        result.CorrectedInvoiceNumberAmended.Should().Be("FV/2025/001-POPRAWIONY");
        result.AmountBeforeCorrection.Should().Be(5000.00m);
        result.ExchangeRateBeforeCorrection.Should().Be(4.5678m);
        result.HasAnyData.Should().BeTrue();
    }

    [Fact]
    public void Build_ShouldReturnSameInstance()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var builder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        var result = builder.Build();

        // Assert
        result.Should().BeSameAs(section);
    }

    #region Integration with InvoiceDetailsBuilder

    [Fact]
    public void InvoiceDetailsBuilder_AsCorrectionWithSection_ShouldSetAllFields()
    {
        // Arrange
        var detailsBuilder = new InvoiceDetailsBuilder();

        // Act
        var invoiceData = detailsBuilder
            .AsCorrection(c => c
                .WithReason("Korekta ceny")
                .WithType(CorrectionType.OriginalInvoiceDate)
                .WithCorrectedInvoice(inv => inv
                    .WithInvoiceNumber("FV/2025/099")
                    .WithIssueDate(new DateOnly(2025, 3, 10))
                    .IssuedOutsideKSeF())
                .WithAmendedInvoiceNumber("FV/2025/099-POPRAWIONY")
                .WithAmountBeforeCorrection(2500m))
            .Build();

        // Assert
        invoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        invoiceData.CorrectionReason.Should().Be("Korekta ceny");
        invoiceData.CorrectionType.Should().Be(1);
        invoiceData.CorrectedInvoiceData.Should().NotBeNull();
        invoiceData.CorrectedInvoiceNumberAmended.Should().Be("FV/2025/099-POPRAWIONY");
        invoiceData.AmountBeforeCorrection.Should().Be(2500m);
        invoiceData.Correction.HasAnyData.Should().BeTrue();
    }

    [Fact]
    public void InvoiceDetailsBuilder_AsCorrectionLegacy_ShouldStillWork()
    {
        // Arrange
        var detailsBuilder = new InvoiceDetailsBuilder();

        // Act
        var invoiceData = detailsBuilder
            .AsCorrection("Korekta ilości", inv => inv
                .WithInvoiceNumber("FV/2025/001")
                .WithIssueDate(new DateOnly(2025, 1, 15))
                .IssuedOutsideKSeF())
            .Build();

        // Assert
        invoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        invoiceData.CorrectionReason.Should().Be("Korekta ilości");
        invoiceData.CorrectionType.Should().Be(1);
        invoiceData.CorrectedInvoiceData.Should().NotBeNull();
        invoiceData.CorrectedInvoiceData!.CorrectedInvoiceNumber.Should().Be("FV/2025/001");
    }

    #endregion
}
