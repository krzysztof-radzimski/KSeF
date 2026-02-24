using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Scenarios;

/// <summary>
/// Testy scenariuszy dla faktur uproszczonych (UPR)
/// Faktura uproszczona zgodnie z art. 106e ust. 5 pkt 3 ustawy o VAT
/// </summary>
public class SimplifiedInvoiceScenarioTests
{
    #region Basic Simplified Invoice (UPR)

    [Fact]
    public void CreateSimplifiedInvoice_BasicScenario_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Podstawowa faktura uproszczona
        // Faktura uproszczona może być wystawiona gdy kwota należności ogółem
        // nie przekracza 450 PLN (lub 100 EUR)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Mały Sklep Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Lokalna 5")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/001/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Artykuły biurowe")
                .WithQuantity(1)
                .WithUnit("kpl.")
                .WithNetAmount(300.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(69.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.IsSimplified.Should().BeTrue();
        invoice.IsCorrection.Should().BeFalse();
        invoice.IsAdvancePayment.Should().BeFalse();
        invoice.IsSettlement.Should().BeFalse();

        // Kwota <= 450 PLN
        invoice.InvoiceData.TotalAmount.Should().Be(369.00m);
        invoice.InvoiceData.TotalAmount.Should().BeLessThanOrEqualTo(450.00m);
    }

    [Fact]
    public void CreateSimplifiedInvoice_WithMaximumAmount_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura uproszczona na maksymalną kwotę (450 PLN)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/002/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Usługa serwisowa")
                .WithNetAmount(365.85m) // 365.85 * 1.23 = 450.00
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(84.15m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.InvoiceData.TotalAmount.Should().Be(450.00m);
    }

    [Fact]
    public void CreateSimplifiedInvoice_WithMultipleVatRates_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura uproszczona z różnymi stawkami VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Kiosk Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/003/2024")
                .AsSimplified())
            // Gazeta (8%)
            .AddLineItem(l => l
                .WithProductName("Gazeta codzienna")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithNetAmount(5.56m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(0.44m))
            // Książka (5%)
            .AddLineItem(l => l
                .WithProductName("Książka kieszonkowa")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithNetAmount(19.05m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(0.95m))
            // Napój (23%)
            .AddLineItem(l => l
                .WithProductName("Woda mineralna")
                .WithQuantity(2)
                .WithUnit("szt.")
                .WithNetAmount(4.07m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(0.93m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.InvoiceData.LineItems.Should().HaveCount(3);

        // Sumy per stawka
        invoice.InvoiceData.NetAmount8.Should().Be(5.56m);
        invoice.InvoiceData.VatAmount8.Should().Be(0.44m);
        invoice.InvoiceData.NetAmount5.Should().Be(19.05m);
        invoice.InvoiceData.VatAmount5.Should().Be(0.95m);
        invoice.InvoiceData.NetAmount23.Should().Be(4.07m);
        invoice.InvoiceData.VatAmount23.Should().Be(0.93m);

        // Total = 6.00 + 20.00 + 5.00 = 31.00
        invoice.InvoiceData.TotalAmount.Should().Be(31.00m);
        invoice.InvoiceData.TotalAmount.Should().BeLessThanOrEqualTo(450.00m);
    }

    #endregion

    #region Simplified Invoice with Different Buyer Types

    [Fact]
    public void CreateSimplifiedInvoice_WithBuyerWithoutIdentifier_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura uproszczona dla nabywcy bez identyfikatora
        // (np. osoba fizyczna nieprowadząca działalności)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sklep Detaliczny"))
            .WithBuyer(b => b
                .WithNoIdentifier()
                .WithName("Jan Kowalski"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/004/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Zakupy detaliczne")
                .WithNetAmount(200.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(46.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.Buyer.NoIdentifier.Should().Be(1);
        invoice.Buyer.TaxId.Should().BeNull();
        invoice.InvoiceData.TotalAmount.Should().Be(246.00m);
    }

    [Fact]
    public void CreateSimplifiedInvoice_WithMinimalBuyerData_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura uproszczona z minimalnymi danymi nabywcy
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/005/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Towar")
                .WithNetAmount(100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        // W fakturze uproszczonej nie jest wymagany pełny adres nabywcy
        invoice.Buyer.Address.Should().BeNull();
        invoice.InvoiceData.TotalAmount.Should().Be(123.00m);
    }

    #endregion

    #region Simplified Invoice Service Scenarios

    [Fact]
    public void CreateSimplifiedInvoice_ForParkingService_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Paragon/faktura za parking
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Parking Miejski Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Parkowa 1")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Firma ABC"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/PARK/001/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Opłata parkingowa - 3 godziny")
                .WithQuantity(3)
                .WithUnit("godz.")
                .WithUnitNetPrice(8.13m)
                .WithNetAmount(24.39m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(5.61m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.InvoiceData.TotalAmount.Should().Be(30.00m);
    }

    [Fact]
    public void CreateSimplifiedInvoice_ForTaxiService_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura za przejazd taxi
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Taxi Ekspres Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Korporacja XYZ"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/TAXI/001/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Przejazd taxi - trasa A-B")
                .WithNetAmount(65.04m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(5.20m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.InvoiceData.NetAmount8.Should().Be(65.04m);
        invoice.InvoiceData.TotalAmount.Should().Be(70.24m);
    }

    [Fact]
    public void CreateSimplifiedInvoice_ForCoffeeShop_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura z kawiarni
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Kawiarnia Pod Lipą"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Biuro Rachunkowe ABC"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/CAFE/001/2024")
                .AsSimplified())
            // Usługi gastronomiczne (8%)
            .AddLineItem(l => l
                .WithProductName("Kawa latte x2")
                .WithQuantity(2)
                .WithUnit("szt.")
                .WithUnitNetPrice(12.96m)
                .WithNetAmount(25.92m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(2.08m))
            .AddLineItem(l => l
                .WithProductName("Ciasto czekoladowe")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithNetAmount(13.89m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(1.11m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.InvoiceData.NetAmount8.Should().Be(39.81m);
        invoice.InvoiceData.VatAmount8.Should().Be(3.19m);
        invoice.InvoiceData.TotalAmount.Should().Be(43.00m);
    }

    #endregion

    #region Simplified Invoice with Cash Payment

    [Fact]
    public void CreateSimplifiedInvoice_WithCashPayment_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura uproszczona płatna gotówką
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sklep Spożywczy"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/006/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Zakupy")
                .WithNetAmount(150.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(34.50m))
            .WithPayment(p => p
                .AsCash(184.50m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.InvoiceData.Payment.Should().NotBeNull();
        invoice.InvoiceData.Payment!.PaymentMethods![0].Method.Should().Be(PaymentMethod.Cash);
        invoice.InvoiceData.Payment.PaymentMethods[0].Amount.Should().Be(184.50m);
    }

    #endregion

    #region Simplified Invoice Limits

    [Fact]
    public void CreateSimplifiedInvoice_ValidatesAmountLimit()
    {
        // Arrange & Act - Test pokazujący że faktura uproszczona może być wystawiona
        // tylko do kwoty 450 PLN brutto (zgodnie z przepisami)

        // Poniżej limitu - OK
        var invoiceBelowLimit = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/007/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Towar")
                .WithNetAmount(365.85m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(84.15m))
            .Build();

        // Na granicy limitu - OK
        var invoiceAtLimit = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/008/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Towar")
                .WithNetAmount(365.85m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(84.15m))
            .Build();

        // Assert
        invoiceBelowLimit.InvoiceData.TotalAmount.Should().BeLessThanOrEqualTo(450.00m);
        invoiceAtLimit.InvoiceData.TotalAmount.Should().Be(450.00m);
    }

    #endregion

    #region XML Serialization Tests

    [Fact]
    public void CreateSimplifiedInvoice_ShouldSerializeToValidXml()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/001/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Towar")
                .WithNetAmount(100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("UPR");
        xml.Should().Contain("FV/UPR/001/2024");
    }

    [Fact]
    public void CreateSimplifiedInvoice_ShouldRoundTripSerialize()
    {
        // Arrange
        var originalInvoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/001/2024")
                .AsSimplified())
            .AddLineItem(l => l
                .WithProductName("Towar")
                .WithNetAmount(100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23.00m))
            .Build();

        // Act
        var deserializedInvoice = XmlSerializationHelper.RoundTrip(originalInvoice);

        // Assert
        deserializedInvoice.Should().NotBeNull();
        deserializedInvoice!.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        deserializedInvoice.IsSimplified.Should().BeTrue();
    }

    #endregion
}
