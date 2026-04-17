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

public class InvoiceCorrectionSectionBuilder
{
    private readonly InvoiceCorrectionSection _section;

    public InvoiceCorrectionSectionBuilder(InvoiceCorrectionSection section)
    {
#if NETSTANDARD2_0
        ThrowHelper.ThrowIfNull(section);
#else
        ArgumentNullException.ThrowIfNull(section);
#endif
        _section = section;
    }

    public InvoiceCorrectionSectionBuilder WithReason(string reason)
    {
        _section.CorrectionReason = reason;
        return this;
    }

    public InvoiceCorrectionSectionBuilder WithType(int type)
    {
        _section.CorrectionType = type;
        return this;
    }

    public InvoiceCorrectionSectionBuilder WithType(CorrectionType type)
    {
        _section.CorrectionType = (int)type;
        return this;
    }

    public InvoiceCorrectionSectionBuilder WithCorrectedInvoice(Action<CorrectedInvoiceDataBuilder> configure)
    {
        var builder = new CorrectedInvoiceDataBuilder();
        configure(builder);
        _section.CorrectedInvoiceData = builder.Build();
        return this;
    }

    public InvoiceCorrectionSectionBuilder WithPeriod(DateOnly periodFrom, DateOnly periodTo)
    {
        _section.CorrectedInvoicePeriod = new SalePeriod
        {
            PeriodFrom = periodFrom,
            PeriodTo = periodTo
        };
        return this;
    }

    public InvoiceCorrectionSectionBuilder WithAmendedInvoiceNumber(string correctedNumber)
    {
        _section.CorrectedInvoiceNumberAmended = correctedNumber;
        return this;
    }

    public InvoiceCorrectionSectionBuilder WithAmountBeforeCorrection(decimal amount)
    {
        _section.AmountBeforeCorrection = amount;
        return this;
    }

    public InvoiceCorrectionSectionBuilder WithExchangeRateBeforeCorrection(decimal rate)
    {
        _section.ExchangeRateBeforeCorrection = rate;
        return this;
    }

    public InvoiceCorrectionSectionBuilder WithCorrectedSeller(Action<CorrectedSellerDataBuilder> configure)
    {
        var builder = new CorrectedSellerDataBuilder();
        configure(builder);
        _section.CorrectedSeller = builder.Build();
        return this;
    }

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

    public InvoiceCorrectionSection Build() => _section;
}
