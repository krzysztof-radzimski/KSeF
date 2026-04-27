/*=====================================================================================

    KSeF.Invoice
    Builder do budowania sekcji korekty faktury (InvoiceCorrectionSection)
    z wykorzystaniem wzorca Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania sekcji korekty faktury (InvoiceCorrectionSection)
/// Umożliwia fluent API dla konfiguracji danych korekty
/// </summary>
public class InvoiceCorrectionSectionBuilder
{
    private readonly InvoiceCorrectionSection _section;

    /// <summary>
    /// Tworzy nowy builder dla istniejącego obiektu InvoiceCorrectionSection
    /// </summary>
    /// <param name="section">Obiekt sekcji korekty do konfiguracji</param>
    public InvoiceCorrectionSectionBuilder(InvoiceCorrectionSection section)
    {
#if NETSTANDARD2_0
        ThrowHelper.ThrowIfNull(section);
#else
        ArgumentNullException.ThrowIfNull(section);
#endif
        _section = section;
    }

    /// <summary>
    /// Ustawia przyczynę korekty (P_15)
    /// </summary>
    /// <param name="reason">Opis słowny przyczyny korekty, maksymalnie 3500 znaków</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithReason(string reason)
    {
        _section.CorrectionReason = reason;
        return this;
    }

    /// <summary>
    /// Ustawia typ korekty (TypKorekty) jako wartość liczbową
    /// </summary>
    /// <param name="type">Kod typu korekty: 1=rozliczeniowa, 2=treści, 3=zmiana formy rozliczeń</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithType(int type)
    {
        _section.CorrectionType = type;
        return this;
    }

    /// <summary>
    /// Ustawia typ korekty (TypKorekty) jako enum
    /// </summary>
    /// <param name="type">Typ korekty z wyliczenia CorrectionType</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithType(CorrectionType type)
    {
        _section.CorrectionType = (int)type;
        return this;
    }

    /// <summary>
    /// Konfiguruje dane faktury korygowanej (DaneFaKorygowanej) przez nested builder
    /// </summary>
    /// <param name="configure">Akcja konfigurująca dane faktury pierwotnej</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithCorrectedInvoice(Action<CorrectedInvoiceDataBuilder> configure)
    {
        var builder = new CorrectedInvoiceDataBuilder();
        configure(builder);
        _section.CorrectedInvoiceData = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia okres faktury korygowanej (OkresFaKorygowanej)
    /// </summary>
    /// <param name="periodFrom">Data rozpoczęcia okresu sprzedaży faktury pierwotnej</param>
    /// <param name="periodTo">Data zakończenia okresu sprzedaży faktury pierwotnej</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithPeriod(DateOnly periodFrom, DateOnly periodTo)
    {
        _section.CorrectedInvoicePeriod = new SalePeriod
        {
            PeriodFrom = periodFrom,
            PeriodTo = periodTo
        };
        return this;
    }

    /// <summary>
    /// Ustawia numer faktury korygowanej po zmianie numeracji (NrFaKorygowany)
    /// </summary>
    /// <param name="correctedNumber">Zmieniony numer faktury pierwotnej, maksymalnie 256 znaków</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithAmendedInvoiceNumber(string correctedNumber)
    {
        _section.CorrectedInvoiceNumberAmended = correctedNumber;
        return this;
    }

    /// <summary>
    /// Ustawia kwotę brutto przed korektą (P_15ZK)
    /// </summary>
    /// <param name="amount">Kwota brutto z faktury pierwotnej</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithAmountBeforeCorrection(decimal amount)
    {
        _section.AmountBeforeCorrection = amount;
        return this;
    }

    /// <summary>
    /// Ustawia kurs waluty przed korektą (KursWalutyZK)
    /// </summary>
    /// <param name="rate">Kurs waluty stosowany w fakturze pierwotnej</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithExchangeRateBeforeCorrection(decimal rate)
    {
        _section.ExchangeRateBeforeCorrection = rate;
        return this;
    }

    /// <summary>
    /// Konfiguruje dane sprzedawcy z faktury korygowanej (Podmiot1K) przez nested builder
    /// </summary>
    /// <param name="configure">Akcja konfigurująca dane sprzedawcy z faktury pierwotnej</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder WithCorrectedSeller(Action<CorrectedSellerDataBuilder> configure)
    {
        var builder = new CorrectedSellerDataBuilder();
        configure(builder);
        _section.CorrectedSeller = builder.Build();
        return this;
    }

    /// <summary>
    /// Dodaje dane nabywcy z faktury korygowanej (Podmiot2K) przez nested builder
    /// Maksymalnie 101 podmiotów wg schematu FA(3)
    /// </summary>
    /// <param name="configure">Akcja konfigurująca dane nabywcy z faktury pierwotnej</param>
    /// <returns>Ten sam builder, umożliwia łańcuchowanie wywołań</returns>
    public InvoiceCorrectionSectionBuilder AddCorrectedBuyer(Action<CorrectedBuyerDataBuilder> configure)
    {
        _section.CorrectedBuyers ??= new List<CorrectedBuyerData>();
        if (_section.CorrectedBuyers.Count >= 101)
            throw new InvalidOperationException("Max 101 Podmiot2K elements allowed per XSD FA(3).");

        var builder = new CorrectedBuyerDataBuilder();
        configure(builder);
        _section.CorrectedBuyers.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Buduje obiekt InvoiceCorrectionSection
    /// </summary>
    /// <returns>Skonfigurowany obiekt sekcji korekty</returns>
    public InvoiceCorrectionSection Build() => _section;
}
