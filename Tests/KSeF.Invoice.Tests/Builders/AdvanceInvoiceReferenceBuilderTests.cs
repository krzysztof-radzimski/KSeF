using FluentAssertions;
using KSeF.Invoice.Services.Builders;
using Xunit;

namespace KSeF.Invoice.Tests.Builders;

public class AdvanceInvoiceReferenceBuilderTests
{
    [Fact]
    public void WithKSeFNumber_ShouldBuildWithKSeFNumber()
    {
        var result = new AdvanceInvoiceReferenceBuilder()
            .WithKSeFNumber("1234567890-20260401-ABC123DEF456-78")
            .Build();

        result.KSeFNumber.Should().Be("1234567890-20260401-ABC123DEF456-78");
        result.IsIssuedOutsideKSeF.Should().BeFalse();
        result.AdvanceInvoiceNumber.Should().BeNull();
    }

    [Fact]
    public void IssuedOutsideKSeF_ShouldBuildWithMarkerAndNumber()
    {
        var result = new AdvanceInvoiceReferenceBuilder()
            .IssuedOutsideKSeF("FV/2026/ZAL/001")
            .Build();

        result.IsIssuedOutsideKSeF.Should().BeTrue();
        result.AdvanceInvoiceNumber.Should().Be("FV/2026/ZAL/001");
        result.KSeFNumber.Should().BeNull();
    }

    [Fact]
    public void Build_WithBothBranches_ShouldThrow()
    {
        var builder = new AdvanceInvoiceReferenceBuilder()
            .WithKSeFNumber("1234567890-20260401-ABC123DEF456-78");

        var act = () => builder.IssuedOutsideKSeF("FV/001");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*xsd:choice*");
    }

    [Fact]
    public void Build_WithBothBranches_Reverse_ShouldThrow()
    {
        var builder = new AdvanceInvoiceReferenceBuilder()
            .IssuedOutsideKSeF("FV/001");

        var act = () => builder.WithKSeFNumber("1234567890-20260401-ABC123DEF456-78");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*xsd:choice*");
    }

    [Fact]
    public void Build_WithNoBranch_ShouldThrow()
    {
        var builder = new AdvanceInvoiceReferenceBuilder();

        var act = () => builder.Build();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*xsd:choice*");
    }

    [Fact]
    public void InvoiceDetailsBuilder_AddAdvanceInvoiceReference_ShouldAddToList()
    {
        var builder = new InvoiceDetailsBuilder();
        var result = builder
            .AddAdvanceInvoiceReference(r => r
                .WithKSeFNumber("1234567890-20260401-ABC123DEF456-78"))
            .AddAdvanceInvoiceReference(r => r
                .IssuedOutsideKSeF("FV/2026/ZAL/002"))
            .Build();

        result.AdvanceInvoiceReferences.Should().HaveCount(2);
        result.AdvanceInvoiceReferences![0].KSeFNumber.Should().Be("1234567890-20260401-ABC123DEF456-78");
        result.AdvanceInvoiceReferences[1].IsIssuedOutsideKSeF.Should().BeTrue();
        result.AdvanceInvoiceReferences[1].AdvanceInvoiceNumber.Should().Be("FV/2026/ZAL/002");
    }

    [Fact]
    public void InvoiceDetailsBuilder_AddAdvanceInvoiceReference_Over100_ShouldThrow()
    {
        var builder = new InvoiceDetailsBuilder();
        for (int i = 0; i < 100; i++)
        {
            builder.AddAdvanceInvoiceReference(r => r
                .WithKSeFNumber($"1234567890-20260401-ABC123DEF{i:D3}-78"));
        }

        var act = () => builder.AddAdvanceInvoiceReference(r => r
            .WithKSeFNumber("1234567890-20260401-OVERFLOW000-78"));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*100*");
    }
}
