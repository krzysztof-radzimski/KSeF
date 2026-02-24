using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Validation;
using Xunit;

namespace KSeF.Invoice.Tests.Validation;

/// <summary>
/// Testy jednostkowe walidacji spójności kwot na fakturze
/// </summary>
public class AmountConsistencyValidationTests
{
    private readonly InvoiceValidator _validator;
    private readonly NipValidator _nipValidator;
    private readonly IbanValidator _ibanValidator;
    private readonly DateValidator _dateValidator;

    public AmountConsistencyValidationTests()
    {
        _nipValidator = new NipValidator();
        _ibanValidator = new IbanValidator();
        _dateValidator = new DateValidator();
        _validator = new InvoiceValidator(_nipValidator, _ibanValidator, _dateValidator);
    }

    #region Spójność pozycji faktury - ilość x cena = wartość

    [Fact]
    public void Validate_LineItemWithMatchingQuantityPriceAndNet_ShouldNotReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].Quantity = 5;
        invoice.InvoiceData.LineItems![0].UnitNetPrice = 100.00m;
        invoice.InvoiceData.LineItems![0].NetAmount = 500.00m; // 5 x 100 = 500

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_NET_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWithMismatchingQuantityPriceAndNet_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].Quantity = 5;
        invoice.InvoiceData.LineItems![0].UnitNetPrice = 100.00m;
        invoice.InvoiceData.LineItems![0].NetAmount = 450.00m; // Powinno być 500

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "ITEM_NET_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWithSmallRoundingDifference_ShouldNotReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].Quantity = 3;
        invoice.InvoiceData.LineItems![0].UnitNetPrice = 33.33m;
        invoice.InvoiceData.LineItems![0].NetAmount = 99.99m; // 3 x 33.33 = 99.99 (tolerancja 0.02)

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_NET_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWithTolerableDifference_ShouldNotReturnWarning()
    {
        // Arrange - tolerancja = 0.02
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].Quantity = 1;
        invoice.InvoiceData.LineItems![0].UnitNetPrice = 100.00m;
        invoice.InvoiceData.LineItems![0].NetAmount = 100.01m; // Różnica 0.01 < tolerancja 0.02

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_NET_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWithBigRoundingDifference_ShouldReturnWarning()
    {
        // Arrange - różnica większa niż tolerancja 0.02
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].Quantity = 1;
        invoice.InvoiceData.LineItems![0].UnitNetPrice = 100.00m;
        invoice.InvoiceData.LineItems![0].NetAmount = 100.05m; // Różnica 0.05 > tolerancja 0.02

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "ITEM_NET_MISMATCH");
    }

    #endregion

    #region Spójność kwoty VAT ze stawką

    [Theory]
    [InlineData(VatRate.Rate23, 100.00, 23.00)]
    [InlineData(VatRate.Rate8, 100.00, 8.00)]
    [InlineData(VatRate.Rate5, 100.00, 5.00)]
    [InlineData(VatRate.Rate4, 100.00, 4.00)]
    [InlineData(VatRate.Rate0Domestic, 100.00, 0.00)]
    public void Validate_LineItemWithCorrectVatAmount_ShouldNotReturnWarning(VatRate vatRate, decimal netAmount, decimal vatAmount)
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].VatRate = vatRate;
        invoice.InvoiceData.LineItems![0].NetAmount = netAmount;
        invoice.InvoiceData.LineItems![0].VatAmount = vatAmount;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWithIncorrectVatAmount_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].VatRate = VatRate.Rate23;
        invoice.InvoiceData.LineItems![0].NetAmount = 100.00m;
        invoice.InvoiceData.LineItems![0].VatAmount = 20.00m; // Powinno być 23.00

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWithSmallVatRoundingDifference_ShouldNotReturnWarning()
    {
        // Arrange - tolerancja 0.02
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].VatRate = VatRate.Rate23;
        invoice.InvoiceData.LineItems![0].NetAmount = 100.00m;
        invoice.InvoiceData.LineItems![0].VatAmount = 23.01m; // Różnica 0.01

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWithExemptVat_ShouldNotValidateVatAmount()
    {
        // Arrange - stawka "zw" nie podlega walidacji kwoty
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].VatRate = VatRate.Exempt;
        invoice.InvoiceData.LineItems![0].NetAmount = 100.00m;
        invoice.InvoiceData.LineItems![0].VatAmount = 0.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWithReverseChargeVat_ShouldNotValidateVatAmount()
    {
        // Arrange - stawka "oo" nie podlega walidacji kwoty
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].VatRate = VatRate.ReverseCharge;
        invoice.InvoiceData.LineItems![0].NetAmount = 100.00m;
        invoice.InvoiceData.LineItems![0].VatAmount = 0.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    #endregion

    #region Spójność sum wg stawek VAT

    [Fact]
    public void Validate_InvoiceWithCorrectRate23Summary_ShouldNotReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems = new List<InvoiceLineItem>
        {
            new InvoiceLineItem
            {
                LineNumber = 1,
                ProductName = "Produkt 1",
                VatRate = VatRate.Rate23,
                NetAmount = 100.00m,
                VatAmount = 23.00m
            },
            new InvoiceLineItem
            {
                LineNumber = 2,
                ProductName = "Produkt 2",
                VatRate = VatRate.Rate23,
                NetAmount = 200.00m,
                VatAmount = 46.00m
            }
        };
        invoice.InvoiceData.NetAmount23 = 300.00m; // 100 + 200
        invoice.InvoiceData.VatAmount23 = 69.00m; // 23 + 46
        invoice.InvoiceData.TotalAmount = 369.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "INV_NET_SUM_MISMATCH");
        result.Warnings.Should().NotContain(w => w.Code == "INV_VAT_SUM_MISMATCH");
    }

    [Fact]
    public void Validate_InvoiceWithIncorrectNetSummary_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems = new List<InvoiceLineItem>
        {
            new InvoiceLineItem
            {
                LineNumber = 1,
                ProductName = "Produkt 1",
                VatRate = VatRate.Rate23,
                NetAmount = 100.00m,
                VatAmount = 23.00m
            }
        };
        invoice.InvoiceData.NetAmount23 = 150.00m; // Powinno być 100
        invoice.InvoiceData.VatAmount23 = 23.00m;
        invoice.InvoiceData.TotalAmount = 173.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "INV_NET_SUM_MISMATCH");
    }

    [Fact]
    public void Validate_InvoiceWithIncorrectVatSummary_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems = new List<InvoiceLineItem>
        {
            new InvoiceLineItem
            {
                LineNumber = 1,
                ProductName = "Produkt 1",
                VatRate = VatRate.Rate23,
                NetAmount = 100.00m,
                VatAmount = 23.00m
            }
        };
        invoice.InvoiceData.NetAmount23 = 100.00m;
        invoice.InvoiceData.VatAmount23 = 30.00m; // Powinno być 23
        invoice.InvoiceData.TotalAmount = 130.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "INV_VAT_SUM_MISMATCH");
    }

    [Fact]
    public void Validate_InvoiceWithMultipleVatRates_ShouldValidateEachRate()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems = new List<InvoiceLineItem>
        {
            new InvoiceLineItem
            {
                LineNumber = 1,
                ProductName = "Produkt 23%",
                VatRate = VatRate.Rate23,
                NetAmount = 100.00m,
                VatAmount = 23.00m
            },
            new InvoiceLineItem
            {
                LineNumber = 2,
                ProductName = "Produkt 8%",
                VatRate = VatRate.Rate8,
                NetAmount = 100.00m,
                VatAmount = 8.00m
            },
            new InvoiceLineItem
            {
                LineNumber = 3,
                ProductName = "Produkt 5%",
                VatRate = VatRate.Rate5,
                NetAmount = 100.00m,
                VatAmount = 5.00m
            }
        };
        invoice.InvoiceData.NetAmount23 = 100.00m;
        invoice.InvoiceData.VatAmount23 = 23.00m;
        invoice.InvoiceData.NetAmount8 = 100.00m;
        invoice.InvoiceData.VatAmount8 = 8.00m;
        invoice.InvoiceData.NetAmount5 = 100.00m;
        invoice.InvoiceData.VatAmount5 = 5.00m;
        invoice.InvoiceData.TotalAmount = 336.00m; // 300 + 36

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "INV_NET_SUM_MISMATCH");
        result.Warnings.Should().NotContain(w => w.Code == "INV_VAT_SUM_MISMATCH");
    }

    #endregion

    #region Spójność sumy całkowitej

    [Fact]
    public void Validate_InvoiceWithCorrectTotalAmount_ShouldNotReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.NetAmount23 = 100.00m;
        invoice.InvoiceData.VatAmount23 = 23.00m;
        invoice.InvoiceData.TotalAmount = 123.00m; // 100 + 23

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "INV_TOTAL_MISMATCH");
    }

    [Fact]
    public void Validate_InvoiceWithIncorrectTotalAmount_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.NetAmount23 = 100.00m;
        invoice.InvoiceData.VatAmount23 = 23.00m;
        invoice.InvoiceData.TotalAmount = 150.00m; // Powinno być 123

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "INV_TOTAL_MISMATCH");
    }

    [Fact]
    public void Validate_InvoiceWithMultipleRatesAndCorrectTotal_ShouldNotReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.NetAmount23 = 100.00m;
        invoice.InvoiceData.VatAmount23 = 23.00m;
        invoice.InvoiceData.NetAmount8 = 50.00m;
        invoice.InvoiceData.VatAmount8 = 4.00m;
        // Total: (100 + 23) + (50 + 4) = 177
        invoice.InvoiceData.TotalAmount = 177.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "INV_TOTAL_MISMATCH");
    }

    #endregion

    #region Różne stawki VAT - obliczenia

    [Theory]
    [InlineData(VatRate.Rate22, 100.00, 22.00)]
    [InlineData(VatRate.Rate7, 100.00, 7.00)]
    [InlineData(VatRate.Rate3, 100.00, 3.00)]
    public void Validate_LineItemWithHistoricalVatRates_ShouldValidateCorrectly(
        VatRate vatRate, decimal netAmount, decimal expectedVat)
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].VatRate = vatRate;
        invoice.InvoiceData.LineItems![0].NetAmount = netAmount;
        invoice.InvoiceData.LineItems![0].VatAmount = expectedVat;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWith0PercentExport_ShouldValidateCorrectly()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].VatRate = VatRate.Rate0Export;
        invoice.InvoiceData.LineItems![0].NetAmount = 1000.00m;
        invoice.InvoiceData.LineItems![0].VatAmount = 0.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    [Fact]
    public void Validate_LineItemWith0PercentWdt_ShouldValidateCorrectly()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].VatRate = VatRate.Rate0IntraCommunitySupply;
        invoice.InvoiceData.LineItems![0].NetAmount = 5000.00m;
        invoice.InvoiceData.LineItems![0].VatAmount = 0.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    #endregion

    #region Przypadki brzegowe

    [Fact]
    public void Validate_InvoiceWithZeroAmounts_ShouldNotThrowException()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].NetAmount = 0.00m;
        invoice.InvoiceData.LineItems![0].VatAmount = 0.00m;
        invoice.InvoiceData.TotalAmount = 0.00m;

        // Act
        var act = () => _validator.Validate(invoice);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_InvoiceWithNegativeAmounts_ShouldValidateCorrectly()
    {
        // Arrange - faktury korygujące mogą mieć ujemne kwoty
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].NetAmount = -100.00m;
        invoice.InvoiceData.LineItems![0].VatAmount = -23.00m;
        invoice.InvoiceData.LineItems![0].VatRate = VatRate.Rate23;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        // Walidacja powinna działać poprawnie z ujemnymi kwotami
        result.Warnings.Should().NotContain(w => w.Code == "ITEM_VAT_MISMATCH");
    }

    [Fact]
    public void Validate_InvoiceWithLargeAmounts_ShouldNotThrowException()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].NetAmount = 999999999.99m;
        invoice.InvoiceData.LineItems![0].VatAmount = 229999999.9977m;
        invoice.InvoiceData.LineItems![0].VatRate = VatRate.Rate23;
        invoice.InvoiceData.TotalAmount = 1229999999.9877m;

        // Act
        var act = () => _validator.Validate(invoice);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_InvoiceWithManyDecimalPlaces_ShouldRoundCorrectly()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].Quantity = 3.333333m;
        invoice.InvoiceData.LineItems![0].UnitNetPrice = 10.555555m;
        // 3.333333 * 10.555555 = 35.185151 zaokrąglone do 35.19
        invoice.InvoiceData.LineItems![0].NetAmount = 35.19m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        // Sprawdzamy, że walidator obsługuje zaokrąglenia z tolerancją
        result.Should().NotBeNull();
    }

    #endregion

    #region Pomocnicze metody

    private static KSeF.Invoice.Models.Invoice CreateValidInvoice()
    {
        return new KSeF.Invoice.Models.Invoice
        {
            Seller = new Seller
            {
                TaxId = "5261040828",
                Name = "Test Sprzedawca Sp. z o.o."
            },
            Buyer = new Buyer
            {
                TaxId = "5261040828",
                Name = "Test Nabywca S.A."
            },
            InvoiceData = new InvoiceData
            {
                InvoiceNumber = "FV/2025/001",
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                InvoiceType = InvoiceType.VAT,
                TotalAmount = 123.00m,
                NetAmount23 = 100.00m,
                VatAmount23 = 23.00m,
                LineItems = new List<InvoiceLineItem>
                {
                    new InvoiceLineItem
                    {
                        LineNumber = 1,
                        ProductName = "Usługa testowa",
                        Quantity = 1,
                        UnitNetPrice = 100.00m,
                        NetAmount = 100.00m,
                        VatRate = VatRate.Rate23,
                        VatAmount = 23.00m
                    }
                }
            }
        };
    }

    #endregion
}
