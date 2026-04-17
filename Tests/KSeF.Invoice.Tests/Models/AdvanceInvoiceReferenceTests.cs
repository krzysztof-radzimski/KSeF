using FluentAssertions;
using KSeF.Invoice.Models;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class AdvanceInvoiceReferenceTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateDefaultValues()
    {
        var reference = new AdvanceInvoiceReference();

        reference.IsIssuedOutsideKSeF.Should().BeFalse();
        reference.AdvanceInvoiceNumber.Should().BeNull();
        reference.KSeFNumber.Should().BeNull();
    }

    [Fact]
    public void WithKSeFNumber_ShouldSetKSeFNumber()
    {
        var reference = new AdvanceInvoiceReference
        {
            KSeFNumber = "1234567890-20260401-ABC123DEF456-78"
        };

        reference.KSeFNumber.Should().Be("1234567890-20260401-ABC123DEF456-78");
        reference.IsIssuedOutsideKSeF.Should().BeFalse();
    }

    [Fact]
    public void IssuedOutsideKSeF_ShouldSetMarkerAndNumber()
    {
        var reference = new AdvanceInvoiceReference
        {
            IsIssuedOutsideKSeF = true,
            AdvanceInvoiceNumber = "FV/2026/ZAL/001"
        };

        reference.IsIssuedOutsideKSeF.Should().BeTrue();
        reference.AdvanceInvoiceNumber.Should().Be("FV/2026/ZAL/001");
    }

    [Fact]
    public void IssuedOutsideKSeFMarker_ShouldReturn1_WhenTrue()
    {
        var reference = new AdvanceInvoiceReference { IsIssuedOutsideKSeF = true };

        reference.IssuedOutsideKSeFMarker.Should().Be("1");
    }

    [Fact]
    public void IssuedOutsideKSeFMarker_ShouldReturnNull_WhenFalse()
    {
        var reference = new AdvanceInvoiceReference { IsIssuedOutsideKSeF = false };

        reference.IssuedOutsideKSeFMarker.Should().BeNull();
    }

    [Fact]
    public void IssuedOutsideKSeFMarker_Set1_ShouldSetFlagTrue()
    {
        var reference = new AdvanceInvoiceReference();
        reference.IssuedOutsideKSeFMarker = "1";

        reference.IsIssuedOutsideKSeF.Should().BeTrue();
    }

    [Fact]
    public void IssuedOutsideKSeFMarker_SetNull_ShouldSetFlagFalse()
    {
        var reference = new AdvanceInvoiceReference();
        reference.IssuedOutsideKSeFMarker = null;

        reference.IsIssuedOutsideKSeF.Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeIssuedOutsideKSeFMarker_WhenTrue_ShouldReturnTrue()
    {
        var reference = new AdvanceInvoiceReference { IsIssuedOutsideKSeF = true };

        reference.ShouldSerializeIssuedOutsideKSeFMarker().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeIssuedOutsideKSeFMarker_WhenFalse_ShouldReturnFalse()
    {
        var reference = new AdvanceInvoiceReference { IsIssuedOutsideKSeF = false };

        reference.ShouldSerializeIssuedOutsideKSeFMarker().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeAdvanceInvoiceNumber_WhenOutsideKSeFAndSet_ShouldReturnTrue()
    {
        var reference = new AdvanceInvoiceReference
        {
            IsIssuedOutsideKSeF = true,
            AdvanceInvoiceNumber = "FV/001"
        };

        reference.ShouldSerializeAdvanceInvoiceNumber().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeAdvanceInvoiceNumber_WhenNotOutsideKSeF_ShouldReturnFalse()
    {
        var reference = new AdvanceInvoiceReference
        {
            IsIssuedOutsideKSeF = false,
            AdvanceInvoiceNumber = "FV/001"
        };

        reference.ShouldSerializeAdvanceInvoiceNumber().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeKSeFNumber_WhenSetAndNotOutside_ShouldReturnTrue()
    {
        var reference = new AdvanceInvoiceReference
        {
            KSeFNumber = "1234567890-20260401-ABC123DEF456-78"
        };

        reference.ShouldSerializeKSeFNumber().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeKSeFNumber_WhenOutsideKSeF_ShouldReturnFalse()
    {
        var reference = new AdvanceInvoiceReference
        {
            IsIssuedOutsideKSeF = true,
            KSeFNumber = "1234567890-20260401-ABC123DEF456-78"
        };

        reference.ShouldSerializeKSeFNumber().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeAdvanceInvoiceReferences_WithItems_ShouldReturnTrue()
    {
        var data = new InvoiceData
        {
            AdvanceInvoiceReferences = new List<AdvanceInvoiceReference>
            {
                new AdvanceInvoiceReference { KSeFNumber = "1234567890-20260401-ABC123DEF456-78" }
            }
        };

        data.ShouldSerializeAdvanceInvoiceReferences().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeAdvanceInvoiceReferences_WithNull_ShouldReturnFalse()
    {
        var data = new InvoiceData();

        data.ShouldSerializeAdvanceInvoiceReferences().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeAdvanceInvoiceReferences_WithEmptyList_ShouldReturnFalse()
    {
        var data = new InvoiceData { AdvanceInvoiceReferences = new List<AdvanceInvoiceReference>() };

        data.ShouldSerializeAdvanceInvoiceReferences().Should().BeFalse();
    }
}
