using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Payments;
using KSeF.Invoice.Services.Validation;
using Xunit;

namespace KSeF.Invoice.Tests.Validation;

/// <summary>
/// Testy jednostkowe walidacji wymagalności pól na fakturze
/// </summary>
public class RequiredFieldsValidationTests
{
    private readonly InvoiceValidator _validator;
    private readonly NipValidator _nipValidator;
    private readonly IbanValidator _ibanValidator;
    private readonly DateValidator _dateValidator;

    public RequiredFieldsValidationTests()
    {
        _nipValidator = new NipValidator();
        _ibanValidator = new IbanValidator();
        _dateValidator = new DateValidator();
        _validator = new InvoiceValidator(_nipValidator, _ibanValidator, _dateValidator);
    }

    #region Główna struktura faktury

    [Fact]
    public void Validate_InvoiceWithNullSeller_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Seller = null!;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_SELLER_MISSING");
    }

    [Fact]
    public void Validate_InvoiceWithNullBuyer_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Buyer = null!;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_BUYER_MISSING");
    }

    [Fact]
    public void Validate_InvoiceWithNullInvoiceData_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData = null!;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_DATA_MISSING");
    }

    #endregion

    #region Dane sprzedawcy

    [Fact]
    public void Validate_SellerWithoutNip_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Seller.TaxId = null!;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "SELLER_NIP_MISSING");
    }

    [Fact]
    public void Validate_SellerWithEmptyNip_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Seller.TaxId = "";

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "SELLER_NIP_MISSING");
    }

    [Fact]
    public void Validate_SellerWithoutName_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Seller.Name = null!;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "SELLER_NAME_MISSING");
    }

    [Fact]
    public void Validate_SellerWithEmptyName_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Seller.Name = "";

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "SELLER_NAME_MISSING");
    }

    [Fact]
    public void Validate_SellerWithTooLongName_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Seller.Name = new string('A', 513); // Max 512

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "SELLER_NAME_TOO_LONG");
    }

    [Fact]
    public void Validate_SellerWithInvalidNip_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Seller.TaxId = "1234567890"; // Nieprawidłowa suma kontrolna

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "NIP_INVALID_CHECKSUM");
    }

    #endregion

    #region Dane nabywcy

    [Fact]
    public void Validate_BuyerWithoutAnyIdentifier_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Buyer = new Buyer
        {
            Name = "Test Nabywca"
            // Brak: TaxId, EuVatId, OtherId, NoIdentifier
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "BUYER_ID_MISSING");
    }

    [Fact]
    public void Validate_BuyerWithPolishTaxId_ShouldNotReturnIdMissingError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Buyer = new Buyer
        {
            TaxId = "5261040828", // Prawidłowy NIP
            Name = "Test Nabywca"
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Errors.Should().NotContain(e => e.Code == "BUYER_ID_MISSING");
    }

    [Fact]
    public void Validate_BuyerWithEuVatId_ShouldNotReturnIdMissingError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Buyer = new Buyer
        {
            EuCountryCode = EUCountryCode.DE,
            EuVatId = "123456789",
            Name = "Test Nabywca"
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Errors.Should().NotContain(e => e.Code == "BUYER_ID_MISSING");
    }

    [Fact]
    public void Validate_BuyerWithOtherId_ShouldNotReturnIdMissingError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Buyer = new Buyer
        {
            OtherIdCountryCode = "US",
            OtherId = "123456789",
            Name = "Test Nabywca"
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Errors.Should().NotContain(e => e.Code == "BUYER_ID_MISSING");
    }

    [Fact]
    public void Validate_BuyerWithNoIdentifierFlag_ShouldNotReturnIdMissingError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Buyer = new Buyer
        {
            NoIdentifier = 1,
            Name = "Test Nabywca"
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Errors.Should().NotContain(e => e.Code == "BUYER_ID_MISSING");
    }

    [Fact]
    public void Validate_BuyerWithInvalidPolishTaxId_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Buyer.TaxId = "1234567890"; // Nieprawidłowa suma kontrolna

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "NIP_INVALID_CHECKSUM");
    }

    [Fact]
    public void Validate_BuyerWithTooLongName_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.Buyer.Name = new string('A', 513); // Max 512

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "BUYER_NAME_TOO_LONG");
    }

    #endregion

    #region Dane faktury

    [Fact]
    public void Validate_InvoiceDataWithoutNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.InvoiceNumber = null!;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_NUMBER_MISSING");
    }

    [Fact]
    public void Validate_InvoiceDataWithEmptyNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.InvoiceNumber = "";

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_NUMBER_MISSING");
    }

    [Fact]
    public void Validate_InvoiceDataWithTooLongNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.InvoiceNumber = new string('X', 257); // Max 256

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_NUMBER_TOO_LONG");
    }

    [Fact]
    public void Validate_InvoiceDataWithDefaultIssueDate_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.IssueDate = default;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_DATE_MISSING");
    }

    [Fact]
    public void Validate_InvoiceDataWithTooLongIssuePlace_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.IssuePlace = new string('W', 257); // Max 256

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_PLACE_TOO_LONG");
    }

    #endregion

    #region Pozycje faktury

    [Fact]
    public void Validate_InvoiceWithNoLineItems_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems = null;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "INV_NO_ITEMS");
    }

    [Fact]
    public void Validate_InvoiceWithEmptyLineItems_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems = new List<InvoiceLineItem>();

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "INV_NO_ITEMS");
    }

    [Fact]
    public void Validate_InvoiceWithTooManyLineItems_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems = Enumerable.Range(1, 10001)
            .Select(i => new InvoiceLineItem
            {
                LineNumber = i,
                ProductName = $"Produkt {i}",
                VatRate = VatRate.Rate23,
                NetAmount = 100m
            })
            .ToList();

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_TOO_MANY_ITEMS");
    }

    [Fact]
    public void Validate_LineItemWithZeroOrNegativeNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].LineNumber = 0;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "ITEM_LINE_NUMBER_INVALID");
    }

    [Fact]
    public void Validate_LineItemWithDuplicateNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems!.Add(new InvoiceLineItem
        {
            LineNumber = 1, // Duplikat
            ProductName = "Drugi produkt",
            VatRate = VatRate.Rate23,
            NetAmount = 100m
        });

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "ITEM_LINE_NUMBER_DUPLICATE");
    }

    [Fact]
    public void Validate_LineItemWithoutProductName_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].ProductName = null!;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "ITEM_NAME_MISSING");
    }

    [Fact]
    public void Validate_LineItemWithEmptyProductName_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].ProductName = "";

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "ITEM_NAME_MISSING");
    }

    [Fact]
    public void Validate_LineItemWithTooLongProductName_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].ProductName = new string('P', 513); // Max 512

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "ITEM_NAME_TOO_LONG");
    }

    [Fact]
    public void Validate_LineItemWithTooLongUnit_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].Unit = new string('U', 257); // Max 256

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "ITEM_UNIT_TOO_LONG");
    }

    [Fact]
    public void Validate_LineItemWithoutNetOrGrossAmount_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.LineItems![0].NetAmount = null;
        invoice.InvoiceData.LineItems![0].GrossAmount = null;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "ITEM_NO_AMOUNT");
    }

    #endregion

    #region Dane korekty

    [Fact]
    public void Validate_CorrectionInvoiceWithoutReason_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = null;
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/001",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10))
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_CORR_REASON_MISSING");
    }

    [Fact]
    public void Validate_CorrectionInvoiceWithEmptyReason_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "";
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/001",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10))
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_CORR_REASON_MISSING");
    }

    [Fact]
    public void Validate_CorrectionInvoiceWithTooLongReason_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = new string('K', 257); // Max 256
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/001",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10))
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_CORR_REASON_TOO_LONG");
    }

    [Fact]
    public void Validate_CorrectionInvoiceWithoutCorrectedData_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta ilości";
        invoice.InvoiceData.CorrectedInvoiceData = null;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_CORR_DATA_MISSING");
    }

    #endregion

    #region Płatności - rachunki bankowe

    [Fact]
    public void Validate_PaymentWithEmptyBankAccountNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.Payment = new Payment
        {
            BankAccounts = new List<BankAccount>
            {
                new BankAccount { AccountNumber = "" }
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "PAYMENT_ACCOUNT_EMPTY");
    }

    [Fact]
    public void Validate_PaymentWithInvalidBankAccountNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.Payment = new Payment
        {
            BankAccounts = new List<BankAccount>
            {
                new BankAccount { AccountNumber = "PL12345678901234567890123456" } // Zła suma kontrolna
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "IBAN_INVALID_CHECKSUM");
    }

    [Fact]
    public void Validate_PaymentWithValidBankAccountNumber_ShouldNotReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.Payment = new Payment
        {
            BankAccounts = new List<BankAccount>
            {
                new BankAccount { AccountNumber = "PL61109010140000071219812874" }
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Errors.Should().NotContain(e => e.Code.StartsWith("IBAN_") || e.Code.StartsWith("PAYMENT_ACCOUNT"));
    }

    [Fact]
    public void Validate_FactoringAccountWithEmptyNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.Payment = new Payment
        {
            FactoringBankAccount = new BankAccount { AccountNumber = "" }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "PAYMENT_FACTORING_EMPTY");
    }

    [Fact]
    public void Validate_FactoringAccountWithInvalidNumber_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateValidInvoice();
        invoice.InvoiceData.Payment = new Payment
        {
            FactoringBankAccount = new BankAccount { AccountNumber = "PL12345678901234567890123456" } // Zła suma kontrolna
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "IBAN_INVALID_CHECKSUM");
    }

    #endregion

    #region Pomocnicze metody

    private static KSeF.Invoice.Models.Invoice CreateValidInvoice()
    {
        return new KSeF.Invoice.Models.Invoice
        {
            Seller = new Seller
            {
                TaxId = "5261040828", // Prawidłowy NIP
                Name = "Test Sprzedawca Sp. z o.o."
            },
            Buyer = new Buyer
            {
                TaxId = "5261040828", // Prawidłowy NIP
                Name = "Test Nabywca S.A."
            },
            InvoiceData = new InvoiceData
            {
                InvoiceNumber = "FV/2025/001",
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                InvoiceType = InvoiceType.VAT,
                TotalAmount = 123.00m,
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
