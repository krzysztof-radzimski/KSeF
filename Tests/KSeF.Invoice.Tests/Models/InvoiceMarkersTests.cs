using FluentAssertions;
using KSeF.Invoice.Models;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class InvoiceMarkersTests
{
    #region FP (Article109_3d)

    [Fact]
    public void IsArticle109_3dInvoice_True_ShouldReturnMarkerValue1()
    {
        var data = new InvoiceData { IsArticle109_3dInvoice = true };

        data.Article109_3dMarker.Should().Be("1");
    }

    [Fact]
    public void IsArticle109_3dInvoice_False_ShouldReturnNull()
    {
        var data = new InvoiceData { IsArticle109_3dInvoice = false };

        data.Article109_3dMarker.Should().BeNull();
    }

    [Fact]
    public void ShouldSerializeArticle109_3dMarker_True_ShouldReturnTrue()
    {
        var data = new InvoiceData { IsArticle109_3dInvoice = true };

        data.ShouldSerializeArticle109_3dMarker().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeArticle109_3dMarker_False_ShouldReturnFalse()
    {
        var data = new InvoiceData { IsArticle109_3dInvoice = false };

        data.ShouldSerializeArticle109_3dMarker().Should().BeFalse();
    }

    [Fact]
    public void Article109_3dMarker_SetValue1_ShouldSetBoolTrue()
    {
        var data = new InvoiceData();
        data.Article109_3dMarker = "1";

        data.IsArticle109_3dInvoice.Should().BeTrue();
    }

    [Fact]
    public void Article109_3dMarker_SetNull_ShouldSetBoolFalse()
    {
        var data = new InvoiceData();
        data.Article109_3dMarker = null;

        data.IsArticle109_3dInvoice.Should().BeFalse();
    }

    #endregion

    #region TP (RelatedParty)

    [Fact]
    public void HasRelatedPartyTransaction_True_ShouldReturnMarkerValue1()
    {
        var data = new InvoiceData { HasRelatedPartyTransaction = true };

        data.RelatedPartyMarker.Should().Be("1");
    }

    [Fact]
    public void HasRelatedPartyTransaction_False_ShouldReturnNull()
    {
        var data = new InvoiceData { HasRelatedPartyTransaction = false };

        data.RelatedPartyMarker.Should().BeNull();
    }

    [Fact]
    public void ShouldSerializeRelatedPartyMarker_True_ShouldReturnTrue()
    {
        var data = new InvoiceData { HasRelatedPartyTransaction = true };

        data.ShouldSerializeRelatedPartyMarker().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeRelatedPartyMarker_False_ShouldReturnFalse()
    {
        var data = new InvoiceData { HasRelatedPartyTransaction = false };

        data.ShouldSerializeRelatedPartyMarker().Should().BeFalse();
    }

    [Fact]
    public void RelatedPartyMarker_SetValue1_ShouldSetBoolTrue()
    {
        var data = new InvoiceData();
        data.RelatedPartyMarker = "1";

        data.HasRelatedPartyTransaction.Should().BeTrue();
    }

    [Fact]
    public void RelatedPartyMarker_SetNull_ShouldSetBoolFalse()
    {
        var data = new InvoiceData();
        data.RelatedPartyMarker = null;

        data.HasRelatedPartyTransaction.Should().BeFalse();
    }

    #endregion

    #region ZwrotAkcyzy (ExciseRefund)

    [Fact]
    public void IsExciseRefundEligible_True_ShouldReturnMarkerValue1()
    {
        var data = new InvoiceData { IsExciseRefundEligible = true };

        data.ExciseRefundMarker.Should().Be("1");
    }

    [Fact]
    public void IsExciseRefundEligible_False_ShouldReturnNull()
    {
        var data = new InvoiceData { IsExciseRefundEligible = false };

        data.ExciseRefundMarker.Should().BeNull();
    }

    [Fact]
    public void ShouldSerializeExciseRefundMarker_True_ShouldReturnTrue()
    {
        var data = new InvoiceData { IsExciseRefundEligible = true };

        data.ShouldSerializeExciseRefundMarker().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeExciseRefundMarker_False_ShouldReturnFalse()
    {
        var data = new InvoiceData { IsExciseRefundEligible = false };

        data.ShouldSerializeExciseRefundMarker().Should().BeFalse();
    }

    [Fact]
    public void ExciseRefundMarker_SetValue1_ShouldSetBoolTrue()
    {
        var data = new InvoiceData();
        data.ExciseRefundMarker = "1";

        data.IsExciseRefundEligible.Should().BeTrue();
    }

    [Fact]
    public void ExciseRefundMarker_SetNull_ShouldSetBoolFalse()
    {
        var data = new InvoiceData();
        data.ExciseRefundMarker = null;

        data.IsExciseRefundEligible.Should().BeFalse();
    }

    #endregion
}
