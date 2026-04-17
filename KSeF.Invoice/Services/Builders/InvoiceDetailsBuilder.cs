/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych merytorycznych faktury (sekcja Fa)
    z wykorzystaniem wzorca Fluent API. Zawiera również buildery
    pomocnicze: CorrectedInvoiceDataBuilder do danych faktury
    korygowanej oraz InvoiceAnnotationsBuilder do adnotacji.
    Umożliwia konfigurację typu faktury, dat, waluty, adnotacji
    (metoda kasowa, samofakturowanie, split payment, zwolnienie
    z VAT) oraz dodatkowych opisów.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Summary;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania danych merytorycznych faktury (InvoiceData / Fa)
/// </summary>
public class InvoiceDetailsBuilder
{
    private readonly InvoiceData _invoiceData = new();

    /// <summary>
    /// Ustawia kod waluty faktury (domyślnie PLN)
    /// </summary>
    public InvoiceDetailsBuilder WithCurrency(CurrencyCode currencyCode)
    {
        _invoiceData.CurrencyCode = currencyCode;
        return this;
    }

    /// <summary>
    /// Ustawia datę wystawienia faktury
    /// </summary>
    public InvoiceDetailsBuilder WithIssueDate(DateOnly issueDate)
    {
        _invoiceData.IssueDate = issueDate;
        return this;
    }

    /// <summary>
    /// Ustawia datę wystawienia faktury
    /// </summary>
    public InvoiceDetailsBuilder WithIssueDate(int year, int month, int day)
    {
        _invoiceData.IssueDate = new DateOnly(year, month, day);
        return this;
    }

    /// <summary>
    /// Ustawia miejsce wystawienia faktury
    /// </summary>
    public InvoiceDetailsBuilder WithIssuePlace(string issuePlace)
    {
        _invoiceData.IssuePlace = issuePlace;
        return this;
    }

    /// <summary>
    /// Ustawia numer faktury
    /// </summary>
    public InvoiceDetailsBuilder WithInvoiceNumber(string invoiceNumber)
    {
        _invoiceData.InvoiceNumber = invoiceNumber;
        return this;
    }

    /// <summary>
    /// Dodaje numer dokumentu magazynowego WZ
    /// </summary>
    public InvoiceDetailsBuilder AddWarehouseDocument(string documentNumber)
    {
        _invoiceData.WarehouseDocuments ??= new List<string>();
        _invoiceData.WarehouseDocuments.Add(documentNumber);
        return this;
    }

    /// <summary>
    /// Ustawia datę sprzedaży
    /// </summary>
    public InvoiceDetailsBuilder WithSaleDate(DateOnly saleDate)
    {
        _invoiceData.SaleDate = saleDate;
        return this;
    }

    /// <summary>
    /// Ustawia datę sprzedaży
    /// </summary>
    public InvoiceDetailsBuilder WithSaleDate(int year, int month, int day)
    {
        _invoiceData.SaleDate = new DateOnly(year, month, day);
        return this;
    }

    /// <summary>
    /// Ustawia okres rozliczeniowy faktury (zamiast konkretnej daty sprzedaży)
    /// </summary>
    public InvoiceDetailsBuilder WithSalePeriod(DateOnly periodFrom, DateOnly periodTo)
    {
        _invoiceData.SalePeriod = new SalePeriod
        {
            PeriodFrom = periodFrom,
            PeriodTo = periodTo
        };
        return this;
    }

    /// <summary>
    /// Ustawia okres rozliczeniowy faktury
    /// </summary>
    public InvoiceDetailsBuilder WithSalePeriod(int startYear, int startMonth, int startDay,
        int endYear, int endMonth, int endDay)
    {
        _invoiceData.SalePeriod = new SalePeriod
        {
            PeriodFrom = new DateOnly(startYear, startMonth, startDay),
            PeriodTo = new DateOnly(endYear, endMonth, endDay)
        };
        return this;
    }

    /// <summary>
    /// Ustawia rodzaj faktury
    /// </summary>
    public InvoiceDetailsBuilder WithInvoiceType(InvoiceType invoiceType)
    {
        _invoiceData.InvoiceType = invoiceType;
        return this;
    }

    /// <summary>
    /// Konfiguruje fakturę jako korygującą
    /// </summary>
    public InvoiceDetailsBuilder AsCorrection(string correctionReason, Action<CorrectedInvoiceDataBuilder> configure)
    {
        _invoiceData.InvoiceType = InvoiceType.KOR;
        _invoiceData.CorrectionReason = correctionReason;
        _invoiceData.CorrectionType = 1;

        var builder = new CorrectedInvoiceDataBuilder();
        configure(builder);
        _invoiceData.CorrectedInvoiceData = builder.Build();

        return this;
    }

    /// <summary>
    /// Konfiguruje fakturę jako zaliczkową
    /// </summary>
    public InvoiceDetailsBuilder AsAdvancePayment()
    {
        _invoiceData.InvoiceType = InvoiceType.ZAL;
        return this;
    }

    /// <summary>
    /// Konfiguruje fakturę jako rozliczeniową
    /// </summary>
    public InvoiceDetailsBuilder AsSettlement()
    {
        _invoiceData.InvoiceType = InvoiceType.ROZ;
        return this;
    }

    /// <summary>
    /// Konfiguruje fakturę jako uproszczoną
    /// </summary>
    public InvoiceDetailsBuilder AsSimplified()
    {
        _invoiceData.InvoiceType = InvoiceType.UPR;
        return this;
    }

    /// <summary>
    /// Konfiguruje adnotacje faktury
    /// </summary>
    public InvoiceDetailsBuilder WithAnnotations(Action<InvoiceAnnotationsBuilder> configure)
    {
        var builder = new InvoiceAnnotationsBuilder();
        configure(builder);
        _invoiceData.Annotations = builder.Build();
        return this;
    }

    /// <summary>
    /// Włącza metodę kasową
    /// </summary>
    public InvoiceDetailsBuilder WithCashMethod()
    {
        _invoiceData.Annotations.CashMethod = AnnotationValue.Yes;
        return this;
    }

    /// <summary>
    /// Włącza samofakturowanie
    /// </summary>
    public InvoiceDetailsBuilder WithSelfBilling()
    {
        _invoiceData.Annotations.SelfBilling = AnnotationValue.Yes;
        return this;
    }

    /// <summary>
    /// Włącza odwrotne obciążenie
    /// </summary>
    public InvoiceDetailsBuilder WithReverseCharge()
    {
        _invoiceData.Annotations.ReverseCharge = AnnotationValue.Yes;
        return this;
    }

    /// <summary>
    /// Włącza obowiązkowy split payment (MPP)
    /// </summary>
    public InvoiceDetailsBuilder WithSplitPayment()
    {
        _invoiceData.Annotations.SplitPayment = AnnotationValue.Yes;
        return this;
    }

    /// <summary>
    /// Ustawia podstawę zwolnienia z VAT
    /// </summary>
    public InvoiceDetailsBuilder WithVatExemption(string reason, string? directiveBasis = null)
    {
        _invoiceData.Annotations.Exemption = new VatExemption
        {
            IsExempt = true,
            P_19 = 1,
            DirectiveBasis = reason,
            EuDirectiveBasis = directiveBasis
        };
        return this;
    }

    /// <summary>
    /// Dodaje dodatkowy opis do faktury
    /// </summary>
    public InvoiceDetailsBuilder AddDescription(string key, string value)
    {
        _invoiceData.AdditionalDescription ??= new List<KeyValue>();
        _invoiceData.AdditionalDescription.Add(new KeyValue
        {
            Key = key,
            Value = value
        });
        return this;
    }

    /// <summary>
    /// Buduje dane merytoryczne faktury
    /// </summary>
    public InvoiceData Build() => _invoiceData;
}

public class CorrectedInvoiceDataBuilder
{
    private readonly CorrectedInvoiceData _data = new();

    public CorrectedInvoiceDataBuilder WithInvoiceNumber(string invoiceNumber)
    {
        _data.CorrectedInvoiceNumber = invoiceNumber;
        return this;
    }

    public CorrectedInvoiceDataBuilder WithIssueDate(DateOnly issueDate)
    {
        _data.CorrectedInvoiceIssueDate = issueDate;
        return this;
    }

    public CorrectedInvoiceDataBuilder WithKSeFNumber(string ksefNumber)
    {
        if (_data.IsIssuedOutsideKSeF)
            throw new InvalidOperationException("Nie można ustawić NrKSeF i NrKSeFN jednocześnie (xsd:choice).");

        _data.IsIssuedInKSeF = true;
        _data.CorrectedInvoiceKSeFNumber = ksefNumber;
        return this;
    }

    public CorrectedInvoiceDataBuilder IssuedOutsideKSeF()
    {
        if (_data.IsIssuedInKSeF)
            throw new InvalidOperationException("Nie można ustawić NrKSeFN i NrKSeF jednocześnie (xsd:choice).");

        _data.IsIssuedOutsideKSeF = true;
        return this;
    }

    public CorrectedInvoiceData Build()
    {
        if (!_data.IsIssuedInKSeF && !_data.IsIssuedOutsideKSeF)
            throw new InvalidOperationException(
                "DaneFaKorygowanej wymagają NrKSeF+NrKSeFFaKorygowanej lub NrKSeFN (xsd:choice).");

        return _data;
    }
}

/// <summary>
/// Builder do budowania adnotacji faktury
/// </summary>
public class InvoiceAnnotationsBuilder
{
    private readonly InvoiceAnnotations _annotations = new();

    /// <summary>
    /// Włącza metodę kasową
    /// </summary>
    public InvoiceAnnotationsBuilder WithCashMethod()
    {
        _annotations.CashMethod = AnnotationValue.Yes;
        return this;
    }

    /// <summary>
    /// Włącza samofakturowanie
    /// </summary>
    public InvoiceAnnotationsBuilder WithSelfBilling()
    {
        _annotations.SelfBilling = AnnotationValue.Yes;
        return this;
    }

    /// <summary>
    /// Włącza odwrotne obciążenie
    /// </summary>
    public InvoiceAnnotationsBuilder WithReverseCharge()
    {
        _annotations.ReverseCharge = AnnotationValue.Yes;
        return this;
    }

    /// <summary>
    /// Włącza obowiązkowy split payment (MPP)
    /// </summary>
    public InvoiceAnnotationsBuilder WithSplitPayment()
    {
        _annotations.SplitPayment = AnnotationValue.Yes;
        return this;
    }

    /// <summary>
    /// Ustawia podstawę zwolnienia z VAT
    /// </summary>
    public InvoiceAnnotationsBuilder WithVatExemption(string reason, string? directiveBasis = null, string? otherLegalBasis = null)
    {
        _annotations.Exemption = new VatExemption
        {
            IsExempt = true,
            P_19 = 1,
            DirectiveBasis = reason,
            EuDirectiveBasis = directiveBasis,
            OtherLegalBasis = otherLegalBasis
        };
        return this;
    }

    /// <summary>
    /// Buduje adnotacje faktury
    /// </summary>
    public InvoiceAnnotations Build() => _annotations;
}
