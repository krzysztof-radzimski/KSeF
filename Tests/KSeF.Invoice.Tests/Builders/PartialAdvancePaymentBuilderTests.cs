using FluentAssertions;
using KSeF.Invoice.Services.Builders;
using Xunit;

namespace KSeF.Invoice.Tests.Builders;

public class PartialAdvancePaymentBuilderTests
{
    [Fact]
    public void Build_Default_ShouldCreateObjectWithDefaults()
    {
        var result = new PartialAdvancePaymentBuilder().Build();

        result.PaymentDate.Should().Be(default(DateOnly));
        result.Amount.Should().Be(0m);
        result.ExchangeRate.Should().BeNull();
    }

    [Fact]
    public void WithPaymentDate_DateOnly_ShouldSetDate()
    {
        var date = new DateOnly(2026, 3, 15);
        var result = new PartialAdvancePaymentBuilder()
            .WithPaymentDate(date)
            .Build();

        result.PaymentDate.Should().Be(date);
    }

    [Fact]
    public void WithPaymentDate_YearMonthDay_ShouldSetDate()
    {
        var result = new PartialAdvancePaymentBuilder()
            .WithPaymentDate(2026, 4, 17)
            .Build();

        result.PaymentDate.Should().Be(new DateOnly(2026, 4, 17));
    }

    [Fact]
    public void WithAmount_ShouldSetAmount()
    {
        var result = new PartialAdvancePaymentBuilder()
            .WithAmount(1500.50m)
            .Build();

        result.Amount.Should().Be(1500.50m);
    }

    [Fact]
    public void WithExchangeRate_ShouldSetExchangeRate()
    {
        var result = new PartialAdvancePaymentBuilder()
            .WithExchangeRate(4.3521m)
            .Build();

        result.ExchangeRate.Should().Be(4.3521m);
    }

    [Fact]
    public void FluentChaining_ShouldSetAllProperties()
    {
        var result = new PartialAdvancePaymentBuilder()
            .WithPaymentDate(2026, 1, 15)
            .WithAmount(2000m)
            .WithExchangeRate(4.25m)
            .Build();

        result.PaymentDate.Should().Be(new DateOnly(2026, 1, 15));
        result.Amount.Should().Be(2000m);
        result.ExchangeRate.Should().Be(4.25m);
    }

    [Fact]
    public void InvoiceDetailsBuilder_AddPartialAdvancePayment_ShouldAddToList()
    {
        var builder = new InvoiceDetailsBuilder();
        var result = builder
            .AddPartialAdvancePayment(p => p
                .WithPaymentDate(2026, 1, 10)
                .WithAmount(500m))
            .AddPartialAdvancePayment(p => p
                .WithPaymentDate(2026, 2, 10)
                .WithAmount(700m))
            .Build();

        result.PartialAdvancePayments.Should().HaveCount(2);
        result.PartialAdvancePayments![0].Amount.Should().Be(500m);
        result.PartialAdvancePayments[1].Amount.Should().Be(700m);
    }

    [Fact]
    public void InvoiceDetailsBuilder_AddPartialAdvancePayment_Over31_ShouldThrow()
    {
        var builder = new InvoiceDetailsBuilder();
        for (int i = 0; i < 31; i++)
        {
            builder.AddPartialAdvancePayment(p => p
                .WithPaymentDate(2026, 1, 1)
                .WithAmount(100m));
        }

        var act = () => builder.AddPartialAdvancePayment(p => p
            .WithPaymentDate(2026, 1, 1)
            .WithAmount(100m));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*31*");
    }

    [Fact]
    public void InvoiceDetailsBuilder_WithArticle109_3d_ShouldSetFlag()
    {
        var result = new InvoiceDetailsBuilder()
            .WithArticle109_3d()
            .Build();

        result.IsArticle109_3dInvoice.Should().BeTrue();
    }

    [Fact]
    public void InvoiceDetailsBuilder_WithRelatedPartyTransaction_ShouldSetFlag()
    {
        var result = new InvoiceDetailsBuilder()
            .WithRelatedPartyTransaction()
            .Build();

        result.HasRelatedPartyTransaction.Should().BeTrue();
    }

    [Fact]
    public void InvoiceDetailsBuilder_WithExciseRefund_ShouldSetFlag()
    {
        var result = new InvoiceDetailsBuilder()
            .WithExciseRefund()
            .Build();

        result.IsExciseRefundEligible.Should().BeTrue();
    }

    [Fact]
    public void InvoiceDetailsBuilder_FluentChainingAllNewFields_ShouldWork()
    {
        var result = new InvoiceDetailsBuilder()
            .WithArticle109_3d()
            .WithRelatedPartyTransaction()
            .WithExciseRefund()
            .AddPartialAdvancePayment(p => p
                .WithPaymentDate(2026, 3, 1)
                .WithAmount(1000m))
            .Build();

        result.IsArticle109_3dInvoice.Should().BeTrue();
        result.HasRelatedPartyTransaction.Should().BeTrue();
        result.IsExciseRefundEligible.Should().BeTrue();
        result.PartialAdvancePayments.Should().HaveCount(1);
    }
}
