/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych zaliczki częściowej (ZaliczkaCzesciowa)
    z wykorzystaniem wzorca Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using KSeF.Invoice.Models;

namespace KSeF.Invoice.Services.Builders;

public class PartialAdvancePaymentBuilder
{
    private readonly PartialAdvancePayment _data = new();

    public PartialAdvancePaymentBuilder WithPaymentDate(DateOnly date)
    {
        _data.PaymentDate = date;
        return this;
    }

    public PartialAdvancePaymentBuilder WithPaymentDate(int year, int month, int day)
    {
        _data.PaymentDate = new DateOnly(year, month, day);
        return this;
    }

    public PartialAdvancePaymentBuilder WithAmount(decimal amount)
    {
        _data.Amount = amount;
        return this;
    }

    public PartialAdvancePaymentBuilder WithExchangeRate(decimal rate)
    {
        _data.ExchangeRate = rate;
        return this;
    }

    public PartialAdvancePayment Build() => _data;
}
