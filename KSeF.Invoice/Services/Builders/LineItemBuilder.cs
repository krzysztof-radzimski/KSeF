using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania pozycji faktury (FaWiersz)
/// </summary>
public class LineItemBuilder
{
    private readonly InvoiceLineItem _lineItem = new();

    /// <summary>
    /// Ustawia nazwę produktu/usługi
    /// </summary>
    public LineItemBuilder WithProductName(string productName)
    {
        _lineItem.ProductName = productName;
        return this;
    }

    /// <summary>
    /// Ustawia jednostkę miary
    /// </summary>
    public LineItemBuilder WithUnit(string unit)
    {
        _lineItem.Unit = unit;
        return this;
    }

    /// <summary>
    /// Ustawia ilość
    /// </summary>
    public LineItemBuilder WithQuantity(decimal quantity)
    {
        _lineItem.Quantity = quantity;
        return this;
    }

    /// <summary>
    /// Ustawia cenę jednostkową netto
    /// </summary>
    public LineItemBuilder WithUnitNetPrice(decimal unitNetPrice)
    {
        _lineItem.UnitNetPrice = unitNetPrice;
        return this;
    }

    /// <summary>
    /// Ustawia cenę jednostkową brutto (metoda "w stu")
    /// </summary>
    public LineItemBuilder WithUnitGrossPrice(decimal unitGrossPrice)
    {
        _lineItem.UnitGrossPrice = unitGrossPrice;
        return this;
    }

    /// <summary>
    /// Ustawia rabat
    /// </summary>
    public LineItemBuilder WithDiscount(decimal discount)
    {
        _lineItem.Discount = discount;
        return this;
    }

    /// <summary>
    /// Ustawia wartość netto pozycji
    /// </summary>
    public LineItemBuilder WithNetAmount(decimal netAmount)
    {
        _lineItem.NetAmount = netAmount;
        return this;
    }

    /// <summary>
    /// Ustawia kwotę VAT pozycji
    /// </summary>
    public LineItemBuilder WithVatAmount(decimal vatAmount)
    {
        _lineItem.VatAmount = vatAmount;
        return this;
    }

    /// <summary>
    /// Ustawia wartość brutto pozycji (metoda "w stu")
    /// </summary>
    public LineItemBuilder WithGrossAmount(decimal grossAmount)
    {
        _lineItem.GrossAmount = grossAmount;
        return this;
    }

    /// <summary>
    /// Ustawia stawkę VAT
    /// </summary>
    public LineItemBuilder WithVatRate(VatRate vatRate)
    {
        _lineItem.VatRate = vatRate;
        return this;
    }

    /// <summary>
    /// Ustawia datę sprzedaży dla tej pozycji
    /// </summary>
    public LineItemBuilder WithSaleDate(DateOnly saleDate)
    {
        _lineItem.SaleDate = saleDate;
        return this;
    }

    /// <summary>
    /// Ustawia datę sprzedaży dla tej pozycji
    /// </summary>
    public LineItemBuilder WithSaleDate(int year, int month, int day)
    {
        _lineItem.SaleDate = new DateOnly(year, month, day);
        return this;
    }

    /// <summary>
    /// Ustawia kod GTIN produktu
    /// </summary>
    public LineItemBuilder WithGtinCode(string gtinCode)
    {
        _lineItem.GtinCode = gtinCode;
        return this;
    }

    /// <summary>
    /// Ustawia kod PKWiU
    /// </summary>
    public LineItemBuilder WithPkwiuCode(string pkwiuCode)
    {
        _lineItem.PkwiuCode = pkwiuCode;
        return this;
    }

    /// <summary>
    /// Ustawia kod CN (taryfy celnej)
    /// </summary>
    public LineItemBuilder WithCnCode(string cnCode)
    {
        _lineItem.CnCode = cnCode;
        return this;
    }

    /// <summary>
    /// Automatycznie oblicza wartość netto na podstawie ilości i ceny jednostkowej netto
    /// </summary>
    public LineItemBuilder CalculateNetAmount()
    {
        if (_lineItem.Quantity.HasValue && _lineItem.UnitNetPrice.HasValue)
        {
            var netAmount = _lineItem.Quantity.Value * _lineItem.UnitNetPrice.Value;
            if (_lineItem.Discount.HasValue)
            {
                netAmount -= _lineItem.Discount.Value;
            }
            _lineItem.NetAmount = Math.Round(netAmount, 2);
        }
        return this;
    }

    /// <summary>
    /// Automatycznie oblicza wartość netto i VAT na podstawie ilości, ceny i stawki VAT
    /// </summary>
    public LineItemBuilder CalculateAmounts()
    {
        CalculateNetAmount();

        if (_lineItem.NetAmount.HasValue)
        {
            var vatMultiplier = GetVatMultiplier(_lineItem.VatRate);
            _lineItem.VatAmount = Math.Round(_lineItem.NetAmount.Value * vatMultiplier, 2);
        }

        return this;
    }

    /// <summary>
    /// Pobiera mnożnik VAT dla danej stawki
    /// </summary>
    private static decimal GetVatMultiplier(VatRate vatRate)
    {
        return vatRate switch
        {
            VatRate.Rate23 => 0.23m,
            VatRate.Rate22 => 0.22m,
            VatRate.Rate8 => 0.08m,
            VatRate.Rate7 => 0.07m,
            VatRate.Rate5 => 0.05m,
            VatRate.Rate4 => 0.04m,
            VatRate.Rate3 => 0.03m,
            VatRate.Rate0Domestic => 0m,
            VatRate.Rate0IntraCommunitySupply => 0m,
            VatRate.Rate0Export => 0m,
            VatRate.Exempt => 0m,
            VatRate.ReverseCharge => 0m,
            VatRate.NotSubjectToTaxI => 0m,
            VatRate.NotSubjectToTaxII => 0m,
            _ => 0m
        };
    }

    /// <summary>
    /// Ustawia numer wiersza (zwykle ustawiany automatycznie przez InvoiceBuilder)
    /// </summary>
    internal LineItemBuilder WithLineNumber(int lineNumber)
    {
        _lineItem.LineNumber = lineNumber;
        return this;
    }

    /// <summary>
    /// Buduje pozycję faktury
    /// </summary>
    public InvoiceLineItem Build() => _lineItem;
}
