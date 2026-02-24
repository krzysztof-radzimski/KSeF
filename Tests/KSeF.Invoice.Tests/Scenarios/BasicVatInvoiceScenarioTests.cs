using FluentAssertions;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Summary;
using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Scenarios;

/// <summary>
/// Testy scenariuszy dla podstawowej faktury VAT
/// </summary>
public class BasicVatInvoiceScenarioTests
{
    #region Complete VAT Invoice Scenario

    [Fact]
    public void CreateBasicVatInvoice_WithAllRequiredFields_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Tworzenie pełnej faktury VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Handlowa 10/5")
                    .WithAddressLine2("00-001 Warszawa"))
                .WithContactData("sprzedawca@example.com", "+48123456789"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Biznesowa 20")
                    .WithAddressLine2("31-000 Kraków"))
                .WithContactData("nabywca@example.com", "+48987654321"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/01/2024")
                .WithSaleDate(2024, 1, 14)
                .WithIssuePlace("Warszawa")
                .WithCurrency(CurrencyCode.PLN)
                .WithInvoiceType(InvoiceType.VAT))
            .AddLineItem(l => l
                .WithProductName("Usługa konsultingowa IT")
                .WithQuantity(40)
                .WithUnit("godz.")
                .WithUnitNetPrice(200.00m)
                .WithNetAmount(8000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(1840.00m)
                .WithPkwiuCode("62.02.30.0"))
            .AddLineItem(l => l
                .WithProductName("Licencja oprogramowania")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithUnitNetPrice(5000.00m)
                .WithNetAmount(5000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(1150.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 2, 14)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL61109010140000071219812874", "PKO BP S.A."))
            .WithSystemInfo("System Fakturowania v2.0")
            .Build();

        // Assert
        invoice.Should().NotBeNull();

        // Weryfikacja sprzedawcy
        invoice.Seller.TaxId.Should().Be("1234567890");
        invoice.Seller.Name.Should().Be("Sprzedawca Sp. z o.o.");
        invoice.Seller.Address.Should().NotBeNull();
        invoice.Seller.Address!.CountryCode.Should().Be("PL");

        // Weryfikacja nabywcy
        invoice.Buyer.TaxId.Should().Be("0987654321");
        invoice.Buyer.Name.Should().Be("Nabywca S.A.");

        // Weryfikacja danych faktury
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.VAT);
        invoice.InvoiceData.InvoiceNumber.Should().Be("FV/001/01/2024");
        invoice.InvoiceData.IssueDate.Should().Be(new DateOnly(2024, 1, 15));
        invoice.InvoiceData.SaleDate.Should().Be(new DateOnly(2024, 1, 14));
        invoice.InvoiceData.CurrencyCode.Should().Be(CurrencyCode.PLN);

        // Weryfikacja pozycji
        invoice.InvoiceData.LineItems.Should().HaveCount(2);
        invoice.InvoiceData.LineItems![0].LineNumber.Should().Be(1);
        invoice.InvoiceData.LineItems[1].LineNumber.Should().Be(2);

        // Weryfikacja sum VAT (automatycznie obliczone)
        invoice.InvoiceData.NetAmount23.Should().Be(13000.00m); // 8000 + 5000
        invoice.InvoiceData.VatAmount23.Should().Be(2990.00m);  // 1840 + 1150
        invoice.InvoiceData.TotalAmount.Should().Be(15990.00m); // 13000 + 2990

        // Weryfikacja płatności
        invoice.InvoiceData.Payment.Should().NotBeNull();
        invoice.InvoiceData.Payment!.PaymentTerms.Should().HaveCount(1);
        invoice.InvoiceData.Payment.BankAccounts.Should().HaveCount(1);

        // Weryfikacja właściwości pomocniczych
        invoice.IsCorrection.Should().BeFalse();
        invoice.IsAdvancePayment.Should().BeFalse();
        invoice.IsSettlement.Should().BeFalse();
        invoice.IsSimplified.Should().BeFalse();
    }

    [Fact]
    public void CreateBasicVatInvoice_WithMinimalData_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Minimalna faktura VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługa")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Assert
        invoice.Should().NotBeNull();
        invoice.Seller.TaxId.Should().Be("1234567890");
        invoice.Buyer.TaxId.Should().Be("0987654321");
        invoice.InvoiceData.InvoiceNumber.Should().Be("FV/001/2024");
        invoice.InvoiceData.LineItems.Should().HaveCount(1);
        invoice.InvoiceData.TotalAmount.Should().Be(1230.00m);
    }

    #endregion

    #region Multi-Rate VAT Invoice Scenario

    [Fact]
    public void CreateMultiRateVatInvoice_ShouldCalculateCorrectSummaries()
    {
        // Arrange & Act - Faktura z różnymi stawkami VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sklep Wielobranżowy Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Klient S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            // Stawka 23% - elektronika
            .AddLineItem(l => l
                .WithProductName("Laptop")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithUnitNetPrice(4000.00m)
                .WithNetAmount(4000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(920.00m))
            // Stawka 8% - książka z płytą
            .AddLineItem(l => l
                .WithProductName("Książka programistyczna z DVD")
                .WithQuantity(2)
                .WithUnit("szt.")
                .WithUnitNetPrice(100.00m)
                .WithNetAmount(200.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(16.00m))
            // Stawka 5% - książka
            .AddLineItem(l => l
                .WithProductName("Książka papierowa")
                .WithQuantity(5)
                .WithUnit("szt.")
                .WithUnitNetPrice(50.00m)
                .WithNetAmount(250.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(12.50m))
            // Zwolnione - usługa medyczna
            .AddLineItem(l => l
                .WithProductName("Konsultacja medyczna")
                .WithQuantity(1)
                .WithUnit("usł.")
                .WithUnitNetPrice(200.00m)
                .WithNetAmount(200.00m)
                .WithVatRate(VatRate.Exempt)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.LineItems.Should().HaveCount(4);

        // Weryfikacja sum dla każdej stawki
        invoice.InvoiceData.NetAmount23.Should().Be(4000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(920.00m);

        invoice.InvoiceData.NetAmount8.Should().Be(200.00m);
        invoice.InvoiceData.VatAmount8.Should().Be(16.00m);

        invoice.InvoiceData.NetAmount5.Should().Be(250.00m);
        invoice.InvoiceData.VatAmount5.Should().Be(12.50m);

        invoice.InvoiceData.ExemptAmount.Should().Be(200.00m);

        // Łączna kwota: 4000+920 + 200+16 + 250+12.50 + 200 = 5598.50
        invoice.InvoiceData.TotalAmount.Should().Be(5598.50m);
    }

    #endregion

    #region VAT Invoice with Foreign Buyer

    [Fact]
    public void CreateVatInvoice_WithEuBuyer_ShouldConfigureBuyerCorrectly()
    {
        // Arrange & Act - Faktura dla kontrahenta z UE
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Polski Eksporter Sp. z o.o."))
            .WithBuyer(b => b
                .WithEuVatId(EUCountryCode.DE, "123456789")
                .WithName("German GmbH")
                .WithAddress(a => a
                    .WithCountryCode("DE")
                    .WithAddressLine1("Hauptstraße 1")
                    .WithAddressLine2("10115 Berlin")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/WDT/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Towary eksportowe")
                .WithNetAmount(10000.00m)
                .WithVatRate(VatRate.Rate0IntraCommunitySupply)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.Buyer.EuCountryCode.Should().Be(EUCountryCode.DE);
        invoice.Buyer.EuVatId.Should().Be("123456789");
        invoice.InvoiceData.NetAmountWdt.Should().Be(10000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(10000.00m);
    }

    [Fact]
    public void CreateVatInvoice_WithNonEuBuyer_ShouldConfigureBuyerCorrectly()
    {
        // Arrange & Act - Faktura dla kontrahenta spoza UE
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Polski Eksporter Sp. z o.o."))
            .WithBuyer(b => b
                .WithForeignId("US", "12-3456789")
                .WithName("American Corporation Inc.")
                .WithAddress(a => a
                    .WithCountryCode("US")
                    .WithAddressLine1("123 Main Street")
                    .WithAddressLine2("New York, NY 10001")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/EXP/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Towary eksportowe")
                .WithNetAmount(15000.00m)
                .WithVatRate(VatRate.Rate0Export)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.Buyer.OtherIdCountryCode.Should().Be("US");
        invoice.Buyer.OtherId.Should().Be("12-3456789");
        invoice.InvoiceData.NetAmountExport.Should().Be(15000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(15000.00m);
    }

    #endregion

    #region VAT Invoice with Period

    [Fact]
    public void CreateVatInvoice_WithSalePeriod_ShouldSetPeriodCorrectly()
    {
        // Arrange & Act - Faktura za okres rozliczeniowy
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Dostawca Mediów Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Odbiorca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 1)
                .WithInvoiceNumber("FV/MEDIA/01/2024")
                .WithSalePeriod(2024, 1, 1, 2024, 1, 31))
            .AddLineItem(l => l
                .WithProductName("Dostawa energii elektrycznej - styczeń 2024")
                .WithQuantity(1500)
                .WithUnit("kWh")
                .WithUnitNetPrice(0.80m)
                .WithNetAmount(1200.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(276.00m))
            .Build();

        // Assert
        invoice.InvoiceData.SalePeriod.Should().NotBeNull();
        invoice.InvoiceData.SalePeriod!.PeriodFrom.Should().Be(new DateOnly(2024, 1, 1));
        invoice.InvoiceData.SalePeriod.PeriodTo.Should().Be(new DateOnly(2024, 1, 31));
        invoice.InvoiceData.SaleDate.Should().BeNull();
    }

    #endregion

    #region VAT Invoice with Annotations

    [Fact]
    public void CreateVatInvoice_WithSplitPayment_ShouldSetAnnotation()
    {
        // Arrange & Act - Faktura z obowiązkowym MPP
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/MPP/001/2024")
                .WithSplitPayment())
            .AddLineItem(l => l
                .WithProductName("Stal budowlana (załącznik 15)")
                .WithNetAmount(50000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(11500.00m)
                .WithPkwiuCode("24.10.31.0"))
            .Build();

        // Assert
        invoice.InvoiceData.Annotations.Should().NotBeNull();
        invoice.InvoiceData.Annotations.SplitPayment.Should().Be(AnnotationValue.Yes);
        invoice.InvoiceData.TotalAmount.Should().Be(61500.00m);
    }

    [Fact]
    public void CreateVatInvoice_WithCashMethod_ShouldSetAnnotation()
    {
        // Arrange & Act - Faktura metoda kasowa
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Mały Przedsiębiorca"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/KAS/001/2024")
                .WithCashMethod())
            .AddLineItem(l => l
                .WithProductName("Usługa")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Assert
        invoice.InvoiceData.Annotations.CashMethod.Should().Be(AnnotationValue.Yes);
    }

    [Fact]
    public void CreateVatInvoice_WithReverseCharge_ShouldSetAnnotation()
    {
        // Arrange & Act - Faktura z odwrotnym obciążeniem
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Podwykonawca Budowlany Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Generalny Wykonawca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/OO/001/2024")
                .WithReverseCharge())
            .AddLineItem(l => l
                .WithProductName("Roboty budowlane - fundamenty")
                .WithNetAmount(50000.00m)
                .WithVatRate(VatRate.ReverseCharge)
                .WithVatAmount(0.00m)
                .WithPkwiuCode("41.00.40.0"))
            .Build();

        // Assert
        invoice.InvoiceData.Annotations.ReverseCharge.Should().Be(AnnotationValue.Yes);
        invoice.InvoiceData.NotTaxableAmount.Should().Be(50000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(50000.00m);
    }

    [Fact]
    public void CreateVatInvoice_WithSelfBilling_ShouldSetAnnotation()
    {
        // Arrange & Act - Samofakturowanie
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/SF/001/2024")
                .WithSelfBilling())
            .AddLineItem(l => l
                .WithProductName("Usługa")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Assert
        invoice.InvoiceData.Annotations.SelfBilling.Should().Be(AnnotationValue.Yes);
    }

    [Fact]
    public void CreateVatInvoice_WithVatExemption_ShouldSetExemptionData()
    {
        // Arrange & Act - Faktura ze zwolnieniem z VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Placówka Medyczna Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Pacjent / Firma"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/MED/001/2024")
                .WithVatExemption("Art. 43 ust. 1 pkt 18 ustawy o VAT"))
            .AddLineItem(l => l
                .WithProductName("Konsultacja lekarska")
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.Exempt)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.Annotations.Exemption.Should().NotBeNull();
        invoice.InvoiceData.Annotations.Exemption!.Reason.Should().Contain("Art. 43");
        invoice.InvoiceData.ExemptAmount.Should().Be(500.00m);
    }

    #endregion

    #region XML Serialization Tests

    [Fact]
    public void CreateBasicVatInvoice_ShouldSerializeToValidXml()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługa")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("Faktura");
        xml.Should().Contain("1234567890"); // NIP sprzedawcy
        xml.Should().Contain("0987654321"); // NIP nabywcy
        xml.Should().Contain("FV/001/2024"); // Numer faktury
    }

    [Fact]
    public void CreateBasicVatInvoice_ShouldRoundTripSerialize()
    {
        // Arrange
        var originalInvoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługa")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Act
        var deserializedInvoice = XmlSerializationHelper.RoundTrip(originalInvoice);

        // Assert
        deserializedInvoice.Should().NotBeNull();
        deserializedInvoice!.Seller.TaxId.Should().Be(originalInvoice.Seller.TaxId);
        deserializedInvoice.Buyer.TaxId.Should().Be(originalInvoice.Buyer.TaxId);
        deserializedInvoice.InvoiceData.InvoiceNumber.Should().Be(originalInvoice.InvoiceData.InvoiceNumber);
    }

    #endregion
}
