using FluentAssertions;
using KSeF.Invoice.Models;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class PartialAdvancePaymentTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateDefaultValues()
    {
        var payment = new PartialAdvancePayment();

        payment.PaymentDate.Should().Be(default(DateOnly));
        payment.Amount.Should().Be(0m);
        payment.ExchangeRate.Should().BeNull();
    }

    [Fact]
    public void PaymentDate_SetAndGet_ShouldRoundTrip()
    {
        var payment = new PartialAdvancePayment
        {
            PaymentDate = new DateOnly(2026, 3, 15)
        };

        payment.PaymentDate.Should().Be(new DateOnly(2026, 3, 15));
    }

    [Fact]
    public void PaymentDateString_ShouldSerializeAsYyyyMmDd()
    {
        var payment = new PartialAdvancePayment
        {
            PaymentDate = new DateOnly(2026, 1, 5)
        };

        payment.PaymentDateString.Should().Be("2026-01-05");
    }

    [Fact]
    public void PaymentDateString_Set_ShouldParseDateOnly()
    {
        var payment = new PartialAdvancePayment();
        payment.PaymentDateString = "2026-04-17";

        payment.PaymentDate.Should().Be(new DateOnly(2026, 4, 17));
    }

    [Fact]
    public void PaymentDateString_SetEmpty_ShouldSetDefault()
    {
        var payment = new PartialAdvancePayment();
        payment.PaymentDateString = "";

        payment.PaymentDate.Should().Be(default(DateOnly));
    }

    [Fact]
    public void Amount_SetAndGet_ShouldRoundTrip()
    {
        var payment = new PartialAdvancePayment { Amount = 1500.50m };

        payment.Amount.Should().Be(1500.50m);
    }

    [Fact]
    public void ExchangeRate_SetAndGet_ShouldRoundTrip()
    {
        var payment = new PartialAdvancePayment { ExchangeRate = 4.3521m };

        payment.ExchangeRate.Should().Be(4.3521m);
    }

    [Fact]
    public void ShouldSerializeExchangeRate_WithValue_ShouldReturnTrue()
    {
        var payment = new PartialAdvancePayment { ExchangeRate = 4.5m };

        payment.ShouldSerializeExchangeRate().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeExchangeRate_WithNull_ShouldReturnFalse()
    {
        var payment = new PartialAdvancePayment();

        payment.ShouldSerializeExchangeRate().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializePartialAdvancePayments_WithItems_ShouldReturnTrue()
    {
        var data = new InvoiceData
        {
            PartialAdvancePayments = new List<PartialAdvancePayment>
            {
                new PartialAdvancePayment { PaymentDate = new DateOnly(2026, 1, 1), Amount = 100m }
            }
        };

        data.ShouldSerializePartialAdvancePayments().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializePartialAdvancePayments_WithNull_ShouldReturnFalse()
    {
        var data = new InvoiceData();

        data.ShouldSerializePartialAdvancePayments().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializePartialAdvancePayments_WithEmptyList_ShouldReturnFalse()
    {
        var data = new InvoiceData { PartialAdvancePayments = new List<PartialAdvancePayment>() };

        data.ShouldSerializePartialAdvancePayments().Should().BeFalse();
    }
}
