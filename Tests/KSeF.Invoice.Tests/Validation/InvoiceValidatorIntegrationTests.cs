using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Payments;
using KSeF.Invoice.Models.Summary;
using KSeF.Invoice.Services.Validation;
using Xunit;

namespace KSeF.Invoice.Tests.Validation;

/// <summary>
/// Testy integracyjne walidacji całej faktury
/// Łączą walidację biznesową z walidacją NIP, IBAN i dat
/// </summary>
public class InvoiceValidatorIntegrationTests
{
    private readonly InvoiceValidator _validator;
    private readonly NipValidator _nipValidator;
    private readonly IbanValidator _ibanValidator;
    private readonly DateValidator _dateValidator;

    public InvoiceValidatorIntegrationTests()
    {
        _nipValidator = new NipValidator();
        _ibanValidator = new IbanValidator();
        _dateValidator = new DateValidator();
        _validator = new InvoiceValidator(_nipValidator, _ibanValidator, _dateValidator);
    }

    #region Scenariusze pozytywne - poprawne faktury

    [Fact]
    public void Validate_CompleteValidInvoice_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateCompleteValidInvoice();

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
        result.HasErrors.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_MinimalValidInvoice_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
        result.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void Validate_InvoiceWithMultipleLineItems_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.LineItems = Enumerable.Range(1, 100)
            .Select(i => new InvoiceLineItem
            {
                LineNumber = i,
                ProductName = $"Produkt {i}",
                Quantity = i,
                UnitNetPrice = 10.00m,
                NetAmount = i * 10.00m,
                VatRate = VatRate.Rate23,
                VatAmount = i * 2.30m
            })
            .ToList();

        // Aktualizuj sumy
        invoice.InvoiceData.NetAmount23 = 50500.00m; // Suma 1..100 * 10
        invoice.InvoiceData.VatAmount23 = 11615.00m;
        invoice.InvoiceData.TotalAmount = 62115.00m;

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvoiceWithAllVatRates_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateInvoiceWithAllVatRates();

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_CorrectionInvoice_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateValidCorrectionInvoice();

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvoiceWithPaymentAndBankAccount_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.Payment = new Payment
        {
            BankAccounts = new List<BankAccount>
            {
                new BankAccount
                {
                    AccountNumber = "PL61109010140000071219812874",
                    BankName = "PKO BP"
                }
            },
            PaymentTerms = new List<PaymentTerm>
            {
                new PaymentTerm
                {
                    DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(14))
                }
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvoiceWithEuBuyer_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.Buyer = new Buyer
        {
            EuCountryCode = EUCountryCode.DE,
            EuVatId = "123456789",
            Name = "German Company GmbH"
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvoiceWithForeignBuyer_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.Buyer = new Buyer
        {
            OtherIdCountryCode = "US",
            OtherId = "123-45-6789",
            Name = "American Company Inc."
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvoiceWithBuyerNoId_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.Buyer = new Buyer
        {
            NoIdentifier = 1,
            Name = "Osoba fizyczna"
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Scenariusze negatywne - błędne faktury

    [Fact]
    public void Validate_InvoiceWithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var invoice = new KSeF.Invoice.Models.Invoice
        {
            Seller = new Seller
            {
                TaxId = "1234567890", // Błędna suma kontrolna NIP
                Name = "" // Pusta nazwa
            },
            Buyer = new Buyer
            {
                // Brak identyfikatora
                Name = "Test"
            },
            InvoiceData = new InvoiceData
            {
                InvoiceNumber = "", // Pusty numer
                IssueDate = default, // Brak daty
                TotalAmount = 100.00m
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();

        // Sprawdź czy są wszystkie oczekiwane błędy
        result.Errors.Should().Contain(e => e.Code == "NIP_INVALID_CHECKSUM"); // Błędny NIP sprzedawcy
        result.Errors.Should().Contain(e => e.Code == "SELLER_NAME_MISSING"); // Brak nazwy sprzedawcy
        result.Errors.Should().Contain(e => e.Code == "BUYER_ID_MISSING"); // Brak ID nabywcy
        result.Errors.Should().Contain(e => e.Code == "INV_NUMBER_MISSING"); // Brak numeru faktury
        result.Errors.Should().Contain(e => e.Code == "INV_DATE_MISSING"); // Brak daty
    }

    [Fact]
    public void Validate_InvoiceWithInvalidNipAndIban_ShouldReturnBothErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.Seller.TaxId = "1234567890"; // Błędna suma kontrolna
        invoice.InvoiceData.Payment = new Payment
        {
            BankAccounts = new List<BankAccount>
            {
                new BankAccount { AccountNumber = "PL12345678901234567890123456" } // Błędna suma kontrolna
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "NIP_INVALID_CHECKSUM");
        result.Errors.Should().Contain(e => e.Code == "IBAN_INVALID_CHECKSUM");
    }

    [Fact]
    public void Validate_InvoiceWithFutureSaleDate_ShouldReturnDateError()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.SaleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)); // Data w przyszłości

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "DATE_SALE_FUTURE");
    }

    [Fact]
    public void Validate_CorrectionInvoiceWithoutRequiredData_ShouldReturnErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        // Brak: CorrectionReason, CorrectedInvoiceData

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "INV_CORR_REASON_MISSING");
        result.Errors.Should().Contain(e => e.Code == "INV_CORR_DATA_MISSING");
    }

    [Fact]
    public void Validate_InvoiceWithDuplicateLineNumbers_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.LineItems = new List<InvoiceLineItem>
        {
            new InvoiceLineItem
            {
                LineNumber = 1,
                ProductName = "Produkt 1",
                VatRate = VatRate.Rate23,
                NetAmount = 100.00m
            },
            new InvoiceLineItem
            {
                LineNumber = 1, // Duplikat!
                ProductName = "Produkt 2",
                VatRate = VatRate.Rate23,
                NetAmount = 100.00m
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "ITEM_LINE_NUMBER_DUPLICATE");
    }

    [Fact]
    public void Validate_InvoiceWithInconsistentAmounts_ShouldReturnWarnings()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.LineItems![0].Quantity = 5;
        invoice.InvoiceData.LineItems![0].UnitNetPrice = 100.00m;
        invoice.InvoiceData.LineItems![0].NetAmount = 400.00m; // Powinno być 500

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "ITEM_NET_MISMATCH");
    }

    #endregion

    #region Scenariusze walidacji dat

    [Fact]
    public void Validate_InvoiceWithSaleDateAndPeriod_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.SaleDate = DateOnly.FromDateTime(DateTime.Today);
        invoice.InvoiceData.SalePeriod = new SalePeriod
        {
            PeriodFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)),
            PeriodTo = DateOnly.FromDateTime(DateTime.Today)
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "INV_DATE_AND_PERIOD");
    }

    [Fact]
    public void Validate_InvoiceWithInvalidPeriod_ShouldReturnError()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.SalePeriod = new SalePeriod
        {
            PeriodFrom = DateOnly.FromDateTime(DateTime.Today),
            PeriodTo = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)) // Data końcowa przed początkową
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "DATE_PERIOD_INVALID");
    }

    [Fact]
    public void Validate_InvoiceWithVeryOldIssueDate_ShouldReturnWarning()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.IssueDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-6)); // Ponad 5 lat

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Code == "DATE_ISSUE_OLD");
    }

    #endregion

    #region Scenariusze walidacji wielu rachunków bankowych

    [Fact]
    public void Validate_InvoiceWithMultipleBankAccounts_ShouldValidateAll()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.Payment = new Payment
        {
            BankAccounts = new List<BankAccount>
            {
                new BankAccount { AccountNumber = "PL61109010140000071219812874" }, // Poprawny
                new BankAccount { AccountNumber = "DE89370400440532013000" }, // Poprawny DE
                new BankAccount { AccountNumber = "PL12345678901234567890123456" } // Niepoprawny - zła suma kontrolna
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Code == "IBAN_INVALID_CHECKSUM");
    }

    [Fact]
    public void Validate_InvoiceWithValidFactoringAccount_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.Payment = new Payment
        {
            FactoringBankAccount = new BankAccount
            {
                AccountNumber = "PL61109010140000071219812874",
                BankName = "Faktor Bank"
            }
        };

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Scenariusze walidacji długości pól

    [Fact]
    public void Validate_InvoiceWithMaxLengthFields_ShouldReturnNoErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.Seller.Name = new string('A', 512); // Max 512
        invoice.Buyer.Name = new string('B', 512);
        invoice.InvoiceData.InvoiceNumber = new string('X', 256); // Max 256
        invoice.InvoiceData.IssuePlace = new string('W', 256);
        invoice.InvoiceData.LineItems![0].ProductName = new string('P', 512);
        invoice.InvoiceData.LineItems![0].Unit = new string('U', 256);

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvoiceWithOverLengthFields_ShouldReturnErrors()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.Seller.Name = new string('A', 513); // Przekroczenie limitu
        invoice.Buyer.Name = new string('B', 513);
        invoice.InvoiceData.InvoiceNumber = new string('X', 257);

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "SELLER_NAME_TOO_LONG");
        result.Errors.Should().Contain(e => e.Code == "BUYER_NAME_TOO_LONG");
        result.Errors.Should().Contain(e => e.Code == "INV_NUMBER_TOO_LONG");
    }

    #endregion

    #region Pomocnicze metody tworzenia faktur

    private static KSeF.Invoice.Models.Invoice CreateMinimalValidInvoice()
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
                NetAmount23 = 100.00m,
                VatAmount23 = 23.00m,
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

    private static KSeF.Invoice.Models.Invoice CreateCompleteValidInvoice()
    {
        return new KSeF.Invoice.Models.Invoice
        {
            Header = new InvoiceHeader
            {
                FormCode = new FormCodeElement
                {
                    Value = "FA",
                    SystemCode = "FA (3)",
                    SchemaVersion = "1-2E"
                },
                FormVariant = 1,
                CreationDateTime = DateTime.Now,
                SystemInfo = "Test System"
            },
            Seller = new Seller
            {
                TaxId = "5261040828",
                Name = "Kompleksowy Sprzedawca Sp. z o.o.",
                Address = new Address
                {
                    CountryCode = "PL",
                    AddressLine1 = "ul. Testowa 1",
                    AddressLine2 = "00-001 Warszawa"
                }
            },
            Buyer = new Buyer
            {
                TaxId = "5252248481",
                Name = "Kompleksowy Nabywca S.A.",
                Address = new Address
                {
                    CountryCode = "PL",
                    AddressLine1 = "ul. Przykładowa 10",
                    AddressLine2 = "30-001 Kraków"
                }
            },
            InvoiceData = new InvoiceData
            {
                CurrencyCode = CurrencyCode.PLN,
                InvoiceNumber = "FV/2025/001",
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                IssuePlace = "Warszawa",
                SaleDate = DateOnly.FromDateTime(DateTime.Today),
                InvoiceType = InvoiceType.VAT,
                NetAmount23 = 1000.00m,
                VatAmount23 = 230.00m,
                TotalAmount = 1230.00m,
                Annotations = new InvoiceAnnotations
                {
                    SelfBilling = AnnotationValue.No,
                    ReverseCharge = AnnotationValue.No,
                    SplitPayment = AnnotationValue.No
                },
                LineItems = new List<InvoiceLineItem>
                {
                    new InvoiceLineItem
                    {
                        LineNumber = 1,
                        ProductName = "Usługa programistyczna",
                        Unit = "godz.",
                        Quantity = 10,
                        UnitNetPrice = 100.00m,
                        NetAmount = 1000.00m,
                        VatRate = VatRate.Rate23,
                        VatAmount = 230.00m
                    }
                },
                Payment = new Payment
                {
                    BankAccounts = new List<BankAccount>
                    {
                        new BankAccount
                        {
                            AccountNumber = "PL61109010140000071219812874",
                            BankName = "Santander Bank Polska S.A."
                        }
                    },
                    PaymentTerms = new List<PaymentTerm>
                    {
                        new PaymentTerm
                        {
                            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(14))
                        }
                    }
                }
            }
        };
    }

    private static KSeF.Invoice.Models.Invoice CreateInvoiceWithAllVatRates()
    {
        return new KSeF.Invoice.Models.Invoice
        {
            Seller = new Seller
            {
                TaxId = "5261040828",
                Name = "Test Sprzedawca"
            },
            Buyer = new Buyer
            {
                TaxId = "5261040828",
                Name = "Test Nabywca"
            },
            InvoiceData = new InvoiceData
            {
                InvoiceNumber = "FV/2025/MULTI",
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                InvoiceType = InvoiceType.VAT,
                NetAmount23 = 100.00m,
                VatAmount23 = 23.00m,
                NetAmount8 = 100.00m,
                VatAmount8 = 8.00m,
                NetAmount5 = 100.00m,
                VatAmount5 = 5.00m,
                NetAmount4 = 100.00m,
                VatAmount4 = 4.00m,
                NetAmount0 = 100.00m,
                ExemptAmount = 100.00m,
                TotalAmount = 740.00m, // 600 netto + 140 VAT
                LineItems = new List<InvoiceLineItem>
                {
                    new InvoiceLineItem { LineNumber = 1, ProductName = "Produkt 23%", VatRate = VatRate.Rate23, NetAmount = 100m, VatAmount = 23m },
                    new InvoiceLineItem { LineNumber = 2, ProductName = "Produkt 8%", VatRate = VatRate.Rate8, NetAmount = 100m, VatAmount = 8m },
                    new InvoiceLineItem { LineNumber = 3, ProductName = "Produkt 5%", VatRate = VatRate.Rate5, NetAmount = 100m, VatAmount = 5m },
                    new InvoiceLineItem { LineNumber = 4, ProductName = "Produkt 4%", VatRate = VatRate.Rate4, NetAmount = 100m, VatAmount = 4m },
                    new InvoiceLineItem { LineNumber = 5, ProductName = "Produkt 0%", VatRate = VatRate.Rate0Domestic, NetAmount = 100m, VatAmount = 0m },
                    new InvoiceLineItem { LineNumber = 6, ProductName = "Produkt zw", VatRate = VatRate.Exempt, NetAmount = 100m, VatAmount = 0m }
                }
            }
        };
    }

    private static KSeF.Invoice.Models.Invoice CreateValidCorrectionInvoice()
    {
        return new KSeF.Invoice.Models.Invoice
        {
            Seller = new Seller
            {
                TaxId = "5261040828",
                Name = "Test Sprzedawca"
            },
            Buyer = new Buyer
            {
                TaxId = "5261040828",
                Name = "Test Nabywca"
            },
            InvoiceData = new InvoiceData
            {
                InvoiceNumber = "FV-KOR/2025/001",
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                InvoiceType = InvoiceType.KOR,
                CorrectionReason = "Korekta ilości towaru",
                CorrectionType = 1,
                CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
                {
                    CorrectedInvoiceNumber = "FV/2025/001",
                    CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10))
                },
                NetAmount23 = -50.00m, // Korekta ujemna
                VatAmount23 = -11.50m,
                TotalAmount = -61.50m,
                LineItems = new List<InvoiceLineItem>
                {
                    new InvoiceLineItem
                    {
                        LineNumber = 1,
                        ProductName = "Korekta - Produkt X",
                        Quantity = -5,
                        UnitNetPrice = 10.00m,
                        NetAmount = -50.00m,
                        VatRate = VatRate.Rate23,
                        VatAmount = -11.50m
                    }
                }
            }
        };
    }

    #endregion
}
