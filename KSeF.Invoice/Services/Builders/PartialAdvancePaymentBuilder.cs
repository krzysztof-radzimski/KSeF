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

/// <summary>
/// Builder do budowania danych zaliczki częściowej (PartialAdvancePayment)
/// </summary>
public class PartialAdvancePaymentBuilder
{
    private readonly PartialAdvancePayment _data = new();

    /// <summary>
    /// Ustawia datę otrzymania zaliczki częściowej (P_6Z)
    /// </summary>
    /// <param name="date">Data otrzymania zaliczki</param>
    /// <returns>Ten sam builder do fluent chaining</returns>
    public PartialAdvancePaymentBuilder WithPaymentDate(DateOnly date)
    {
        _data.PaymentDate = date;
        return this;
    }

    /// <summary>
    /// Ustawia datę otrzymania zaliczki częściowej (P_6Z) z komponentów
    /// </summary>
    /// <param name="year">Rok</param>
    /// <param name="month">Miesiąc (1-12)</param>
    /// <param name="day">Dzień (1-31)</param>
    /// <returns>Ten sam builder do fluent chaining</returns>
    public PartialAdvancePaymentBuilder WithPaymentDate(int year, int month, int day)
    {
        _data.PaymentDate = new DateOnly(year, month, day);
        return this;
    }

    /// <summary>
    /// Ustawia kwotę zaliczki brutto (P_15Z)
    /// </summary>
    /// <param name="amount">Kwota całkowita (brutto) zaliczki</param>
    /// <returns>Ten sam builder do fluent chaining</returns>
    public PartialAdvancePaymentBuilder WithAmount(decimal amount)
    {
        _data.Amount = amount;
        return this;
    }

    /// <summary>
    /// Ustawia kurs waluty dla zaliczki (KursWalutyZW)
    /// </summary>
    /// <param name="rate">Kurs waluty stosowany przy przeliczeniu zaliczki</param>
    /// <returns>Ten sam builder do fluent chaining</returns>
    public PartialAdvancePaymentBuilder WithExchangeRate(decimal rate)
    {
        _data.ExchangeRate = rate;
        return this;
    }

    /// <summary>
    /// Buduje obiekt PartialAdvancePayment
    /// </summary>
    /// <returns>Skonfigurowany obiekt zaliczki częściowej</returns>
    public PartialAdvancePayment Build() => _data;
}
