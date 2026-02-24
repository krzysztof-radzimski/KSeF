using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Payments;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Główna klasa fluent API do budowania faktury KSeF
/// </summary>
public class InvoiceBuilder
{
    private readonly Models.Invoice _invoice = new();
    private readonly List<InvoiceLineItem> _lineItems = new();
    private int _lineNumber = 1;
    private bool _autoCalculateSummary = true;

    /// <summary>
    /// Tworzy nowy builder faktury
    /// </summary>
    public static InvoiceBuilder Create() => new();

    /// <summary>
    /// Konfiguruje sprzedawcę (Podmiot1)
    /// </summary>
    public InvoiceBuilder WithSeller(Action<SellerBuilder> configure)
    {
        var builder = new SellerBuilder();
        configure(builder);
        _invoice.Seller = builder.Build();
        return this;
    }

    /// <summary>
    /// Konfiguruje sprzedawcę (Podmiot1) za pomocą gotowego obiektu
    /// </summary>
    public InvoiceBuilder WithSeller(Seller seller)
    {
        _invoice.Seller = seller;
        return this;
    }

    /// <summary>
    /// Konfiguruje nabywcę (Podmiot2)
    /// </summary>
    public InvoiceBuilder WithBuyer(Action<BuyerBuilder> configure)
    {
        var builder = new BuyerBuilder();
        configure(builder);
        _invoice.Buyer = builder.Build();
        return this;
    }

    /// <summary>
    /// Konfiguruje nabywcę (Podmiot2) za pomocą gotowego obiektu
    /// </summary>
    public InvoiceBuilder WithBuyer(Buyer buyer)
    {
        _invoice.Buyer = buyer;
        return this;
    }

    /// <summary>
    /// Dodaje podmiot trzeci (Podmiot3) - odbiorca, faktor, płatnik itp.
    /// </summary>
    public InvoiceBuilder AddRecipient(Action<ThirdPartyBuilder> configure)
    {
        var builder = new ThirdPartyBuilder();
        configure(builder);

        _invoice.Recipients ??= new List<ThirdParty>();
        _invoice.Recipients.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Dodaje podmiot trzeci (Podmiot3) za pomocą gotowego obiektu
    /// </summary>
    public InvoiceBuilder AddRecipient(ThirdParty thirdParty)
    {
        _invoice.Recipients ??= new List<ThirdParty>();
        _invoice.Recipients.Add(thirdParty);
        return this;
    }

    /// <summary>
    /// Konfiguruje dane merytoryczne faktury (Fa)
    /// </summary>
    public InvoiceBuilder WithInvoiceDetails(Action<InvoiceDetailsBuilder> configure)
    {
        var builder = new InvoiceDetailsBuilder();
        configure(builder);
        _invoice.InvoiceData = builder.Build();
        return this;
    }

    /// <summary>
    /// Konfiguruje dane merytoryczne faktury za pomocą gotowego obiektu
    /// </summary>
    public InvoiceBuilder WithInvoiceDetails(InvoiceData invoiceData)
    {
        _invoice.InvoiceData = invoiceData;
        return this;
    }

    /// <summary>
    /// Dodaje pozycję faktury (FaWiersz) z automatycznym numerowaniem
    /// </summary>
    public InvoiceBuilder AddLineItem(Action<LineItemBuilder> configure)
    {
        var builder = new LineItemBuilder();
        configure(builder);
        builder.WithLineNumber(_lineNumber++);
        _lineItems.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Dodaje pozycję faktury za pomocą gotowego obiektu
    /// Numer wiersza zostanie nadany automatycznie
    /// </summary>
    public InvoiceBuilder AddLineItem(InvoiceLineItem lineItem)
    {
        lineItem.LineNumber = _lineNumber++;
        _lineItems.Add(lineItem);
        return this;
    }

    /// <summary>
    /// Dodaje wiele pozycji faktury
    /// </summary>
    public InvoiceBuilder AddLineItems(IEnumerable<Action<LineItemBuilder>> configures)
    {
        foreach (var configure in configures)
        {
            AddLineItem(configure);
        }
        return this;
    }

    /// <summary>
    /// Konfiguruje informacje o płatnościach
    /// </summary>
    public InvoiceBuilder WithPayment(Action<PaymentBuilder> configure)
    {
        var builder = new PaymentBuilder();
        configure(builder);
        _invoice.InvoiceData.Payment = builder.Build();
        return this;
    }

    /// <summary>
    /// Konfiguruje informacje o płatnościach za pomocą gotowego obiektu
    /// </summary>
    public InvoiceBuilder WithPayment(Payment payment)
    {
        _invoice.InvoiceData.Payment = payment;
        return this;
    }

    /// <summary>
    /// Wyłącza automatyczne obliczanie podsumowań VAT
    /// </summary>
    public InvoiceBuilder DisableAutoCalculation()
    {
        _autoCalculateSummary = false;
        return this;
    }

    /// <summary>
    /// Ustawia datę i godzinę wytworzenia dokumentu
    /// </summary>
    public InvoiceBuilder WithCreationDateTime(DateTime creationDateTime)
    {
        _invoice.Header.CreationDateTime = creationDateTime;
        return this;
    }

    /// <summary>
    /// Ustawia informację o systemie wytwarzającym fakturę
    /// </summary>
    public InvoiceBuilder WithSystemInfo(string systemInfo)
    {
        _invoice.Header.SystemInfo = systemInfo;
        return this;
    }

    /// <summary>
    /// Buduje fakturę z automatycznym obliczaniem sum i podsumowań VAT
    /// </summary>
    public Models.Invoice Build()
    {
        // Przypisz pozycje do faktury
        if (_lineItems.Count > 0)
        {
            _invoice.InvoiceData.LineItems = _lineItems;
        }

        // Automatyczne obliczanie podsumowań VAT
        if (_autoCalculateSummary && _lineItems.Count > 0)
        {
            CalculateVatSummary();
        }

        // Ustaw domyślną datę wytworzenia jeśli nie ustawiona
        if (_invoice.Header.CreationDateTime == default)
        {
            _invoice.Header.CreationDateTime = DateTime.Now;
        }

        return _invoice;
    }

    /// <summary>
    /// Oblicza podsumowania VAT na podstawie pozycji faktury
    /// </summary>
    private void CalculateVatSummary()
    {
        var invoiceData = _invoice.InvoiceData;

        // Grupuj pozycje wg stawki VAT i oblicz sumy
        var groupedItems = _lineItems
            .Where(item => item.NetAmount.HasValue || item.GrossAmount.HasValue)
            .GroupBy(item => item.VatRate);

        decimal totalAmount = 0;

        foreach (var group in groupedItems)
        {
            var netSum = group.Sum(item => item.NetAmount ?? 0);
            var vatSum = group.Sum(item => item.VatAmount ?? 0);

            switch (group.Key)
            {
                case VatRate.Rate23:
                    invoiceData.NetAmount23 = netSum;
                    invoiceData.VatAmount23 = vatSum;
                    totalAmount += netSum + vatSum;
                    break;

                case VatRate.Rate22:
                    // Stara stawka 22% - używamy pól dla 23%
                    invoiceData.NetAmount23 = (invoiceData.NetAmount23 ?? 0) + netSum;
                    invoiceData.VatAmount23 = (invoiceData.VatAmount23 ?? 0) + vatSum;
                    totalAmount += netSum + vatSum;
                    break;

                case VatRate.Rate8:
                    invoiceData.NetAmount8 = netSum;
                    invoiceData.VatAmount8 = vatSum;
                    totalAmount += netSum + vatSum;
                    break;

                case VatRate.Rate7:
                    // Stara stawka 7% - używamy pól dla 8%
                    invoiceData.NetAmount8 = (invoiceData.NetAmount8 ?? 0) + netSum;
                    invoiceData.VatAmount8 = (invoiceData.VatAmount8 ?? 0) + vatSum;
                    totalAmount += netSum + vatSum;
                    break;

                case VatRate.Rate5:
                    invoiceData.NetAmount5 = netSum;
                    invoiceData.VatAmount5 = vatSum;
                    totalAmount += netSum + vatSum;
                    break;

                case VatRate.Rate4:
                    invoiceData.NetAmount4 = netSum;
                    invoiceData.VatAmount4 = vatSum;
                    totalAmount += netSum + vatSum;
                    break;

                case VatRate.Rate3:
                    // Stawka 3% - używamy pól dla 4%
                    invoiceData.NetAmount4 = (invoiceData.NetAmount4 ?? 0) + netSum;
                    invoiceData.VatAmount4 = (invoiceData.VatAmount4 ?? 0) + vatSum;
                    totalAmount += netSum + vatSum;
                    break;

                case VatRate.Rate0Domestic:
                    invoiceData.NetAmount0 = (invoiceData.NetAmount0 ?? 0) + netSum;
                    totalAmount += netSum;
                    break;

                case VatRate.Rate0IntraCommunitySupply:
                    invoiceData.NetAmountWdt = (invoiceData.NetAmountWdt ?? 0) + netSum;
                    totalAmount += netSum;
                    break;

                case VatRate.Rate0Export:
                    invoiceData.NetAmountExport = (invoiceData.NetAmountExport ?? 0) + netSum;
                    totalAmount += netSum;
                    break;

                case VatRate.Exempt:
                    invoiceData.ExemptAmount = (invoiceData.ExemptAmount ?? 0) + netSum;
                    totalAmount += netSum;
                    break;

                case VatRate.ReverseCharge:
                    // Odwrotne obciążenie - dodajemy do wartości netto bez VAT
                    invoiceData.NotTaxableAmount = (invoiceData.NotTaxableAmount ?? 0) + netSum;
                    totalAmount += netSum;
                    break;

                case VatRate.NotSubjectToTaxI:
                case VatRate.NotSubjectToTaxII:
                    invoiceData.NotTaxableAmount = (invoiceData.NotTaxableAmount ?? 0) + netSum;
                    totalAmount += netSum;
                    break;
            }
        }

        // Ustaw kwotę należności ogółem
        invoiceData.TotalAmount = Math.Round(totalAmount, 2);
    }
}
