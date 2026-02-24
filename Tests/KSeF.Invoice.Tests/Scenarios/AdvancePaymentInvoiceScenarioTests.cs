using FluentAssertions;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Scenarios;

/// <summary>
/// Testy scenariuszy dla faktur zaliczkowych (ZAL) i rozliczeniowych (ROZ)
/// </summary>
public class AdvancePaymentInvoiceScenarioTests
{
    #region Advance Payment Invoice (ZAL)

    [Fact]
    public void CreateAdvancePaymentInvoice_WithSinglePayment_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura zaliczkowa na 100% zamówienia
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Producent Maszyn Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Przemysłowa 50")
                    .WithAddressLine2("40-001 Katowice")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Fabryka S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Fabryczna 100")
                    .WithAddressLine2("30-001 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/ZAL/001/2024")
                .AsAdvancePayment())
            .AddLineItem(l => l
                .WithProductName("Zaliczka na maszynę CNC Model X500 - 100% wartości zamówienia")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithNetAmount(100000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23000.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 1, 22)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL61109010140000071219812874", "PKO BP S.A."))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ZAL);
        invoice.IsAdvancePayment.Should().BeTrue();
        invoice.IsCorrection.Should().BeFalse();
        invoice.IsSettlement.Should().BeFalse();

        invoice.InvoiceData.NetAmount23.Should().Be(100000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(23000.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(123000.00m);
    }

    [Fact]
    public void CreateAdvancePaymentInvoice_WithPartialPayment_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura zaliczkowa na 30% zamówienia
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Developer Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Klient S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 10)
                .WithInvoiceNumber("FV/ZAL/002/2024")
                .AsAdvancePayment()
                .AddDescription("Zamówienie", "ZAM/001/2024")
                .AddDescription("Procent zaliczki", "30%")
                .AddDescription("Wartość całego zamówienia netto", "500000.00 PLN"))
            .AddLineItem(l => l
                .WithProductName("Zaliczka 30% na budowę hali magazynowej - etap I")
                .WithNetAmount(150000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(34500.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 1, 17)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL61109010140000071219812874"))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ZAL);
        invoice.InvoiceData.AdditionalDescription.Should().HaveCount(3);
        invoice.InvoiceData.TotalAmount.Should().Be(184500.00m);
    }

    [Fact]
    public void CreateAdvancePaymentInvoice_SecondAdvance_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Druga faktura zaliczkowa (kolejne 30%)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Developer Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Klient S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 3, 15)
                .WithInvoiceNumber("FV/ZAL/003/2024")
                .AsAdvancePayment()
                .AddDescription("Zamówienie", "ZAM/001/2024")
                .AddDescription("Procent zaliczki", "30% (druga transza)")
                .AddDescription("Poprzednia faktura zaliczkowa", "FV/ZAL/002/2024"))
            .AddLineItem(l => l
                .WithProductName("Zaliczka 30% na budowę hali magazynowej - etap II")
                .WithNetAmount(150000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(34500.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ZAL);
        invoice.InvoiceData.TotalAmount.Should().Be(184500.00m);
    }

    [Fact]
    public void CreateAdvancePaymentInvoice_WithMultipleVatRates_ShouldCalculateCorrectly()
    {
        // Arrange & Act - Zaliczka z różnymi stawkami VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Wydawnictwo Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Księgarnia S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 20)
                .WithInvoiceNumber("FV/ZAL/004/2024")
                .AsAdvancePayment())
            // Zaliczka na książki (5%)
            .AddLineItem(l => l
                .WithProductName("Zaliczka na dostawę książek")
                .WithNetAmount(10000.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(500.00m))
            // Zaliczka na materiały biurowe (23%)
            .AddLineItem(l => l
                .WithProductName("Zaliczka na materiały biurowe")
                .WithNetAmount(5000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(1150.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ZAL);
        invoice.InvoiceData.NetAmount5.Should().Be(10000.00m);
        invoice.InvoiceData.VatAmount5.Should().Be(500.00m);
        invoice.InvoiceData.NetAmount23.Should().Be(5000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(1150.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(16650.00m);
    }

    #endregion

    #region Settlement Invoice (ROZ)

    [Fact]
    public void CreateSettlementInvoice_WithSingleAdvance_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura rozliczeniowa po jednej zaliczce 100%
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Producent Maszyn Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Przemysłowa 50")
                    .WithAddressLine2("40-001 Katowice")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Fabryka S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Fabryczna 100")
                    .WithAddressLine2("30-001 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 3, 1)
                .WithInvoiceNumber("FV/ROZ/001/2024")
                .WithSaleDate(2024, 2, 28)
                .AsSettlement()
                .AddDescription("Faktura zaliczkowa", "FV/ZAL/001/2024"))
            // Pełna wartość dostawy
            .AddLineItem(l => l
                .WithProductName("Maszyna CNC Model X500 - dostawa")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithNetAmount(100000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23000.00m))
            // Odliczenie zaliczki (ujemna wartość)
            .AddLineItem(l => l
                .WithProductName("Zaliczka FV/ZAL/001/2024 - rozliczenie")
                .WithNetAmount(-100000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-23000.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ROZ);
        invoice.IsSettlement.Should().BeTrue();
        invoice.IsAdvancePayment.Should().BeFalse();
        invoice.IsCorrection.Should().BeFalse();

        // Suma wynosi 0 - zaliczka była na 100%
        invoice.InvoiceData.NetAmount23.Should().Be(0.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(0.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(0.00m);
    }

    [Fact]
    public void CreateSettlementInvoice_WithPartialAdvances_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura rozliczeniowa po dwóch zaliczkach (30% + 30% = 60%)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Developer Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Klient S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 6, 30)
                .WithInvoiceNumber("FV/ROZ/002/2024")
                .WithSaleDate(2024, 6, 28)
                .AsSettlement()
                .AddDescription("Faktura zaliczkowa 1", "FV/ZAL/002/2024 - 150000 netto")
                .AddDescription("Faktura zaliczkowa 2", "FV/ZAL/003/2024 - 150000 netto")
                .AddDescription("Suma wpłaconych zaliczek netto", "300000.00 PLN"))
            // Pełna wartość usługi
            .AddLineItem(l => l
                .WithProductName("Budowa hali magazynowej - usługa kompleksowa")
                .WithNetAmount(500000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(115000.00m))
            // Odliczenie pierwszej zaliczki
            .AddLineItem(l => l
                .WithProductName("Rozliczenie zaliczki FV/ZAL/002/2024")
                .WithNetAmount(-150000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-34500.00m))
            // Odliczenie drugiej zaliczki
            .AddLineItem(l => l
                .WithProductName("Rozliczenie zaliczki FV/ZAL/003/2024")
                .WithNetAmount(-150000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-34500.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 7, 14)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL61109010140000071219812874"))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ROZ);
        invoice.InvoiceData.LineItems.Should().HaveCount(3);

        // Pozostałe 40% do zapłaty
        invoice.InvoiceData.NetAmount23.Should().Be(200000.00m); // 500000 - 150000 - 150000
        invoice.InvoiceData.VatAmount23.Should().Be(46000.00m);  // 115000 - 34500 - 34500
        invoice.InvoiceData.TotalAmount.Should().Be(246000.00m);
    }

    [Fact]
    public void CreateSettlementInvoice_WithDifferentVatRates_ShouldCalculateCorrectly()
    {
        // Arrange & Act - Faktura rozliczeniowa z różnymi stawkami VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Wydawnictwo Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Księgarnia S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 28)
                .WithInvoiceNumber("FV/ROZ/003/2024")
                .WithSaleDate(2024, 2, 27)
                .AsSettlement())
            // Dostawa książek
            .AddLineItem(l => l
                .WithProductName("Dostawa książek")
                .WithNetAmount(20000.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(1000.00m))
            // Dostawa materiałów biurowych
            .AddLineItem(l => l
                .WithProductName("Dostawa materiałów biurowych")
                .WithNetAmount(10000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(2300.00m))
            // Rozliczenie zaliczki na książki
            .AddLineItem(l => l
                .WithProductName("Rozliczenie zaliczki - książki")
                .WithNetAmount(-10000.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(-500.00m))
            // Rozliczenie zaliczki na materiały biurowe
            .AddLineItem(l => l
                .WithProductName("Rozliczenie zaliczki - materiały biurowe")
                .WithNetAmount(-5000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-1150.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ROZ);

        // Pozostało do zapłaty
        invoice.InvoiceData.NetAmount5.Should().Be(10000.00m);  // 20000 - 10000
        invoice.InvoiceData.VatAmount5.Should().Be(500.00m);    // 1000 - 500
        invoice.InvoiceData.NetAmount23.Should().Be(5000.00m);  // 10000 - 5000
        invoice.InvoiceData.VatAmount23.Should().Be(1150.00m);  // 2300 - 1150

        // Total = 10500 + 6150 = 16650
        invoice.InvoiceData.TotalAmount.Should().Be(16650.00m);
    }

    [Fact]
    public void CreateSettlementInvoice_WithOverpaidAdvance_ShouldShowNegativeAmount()
    {
        // Arrange & Act - Faktura rozliczeniowa z nadpłatą (zaliczka > wartość końcowa)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 3, 15)
                .WithInvoiceNumber("FV/ROZ/004/2024")
                .AsSettlement()
                .AddDescription("Uwaga", "Zwrot nadpłaty 5000 PLN netto + VAT"))
            // Wartość końcowa usługi
            .AddLineItem(l => l
                .WithProductName("Usługa konsultingowa - wartość końcowa")
                .WithNetAmount(45000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(10350.00m))
            // Rozliczenie zaliczki (wyższa niż wartość końcowa)
            .AddLineItem(l => l
                .WithProductName("Rozliczenie zaliczki (nadpłata)")
                .WithNetAmount(-50000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-11500.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ROZ);
        invoice.InvoiceData.NetAmount23.Should().Be(-5000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(-1150.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(-6150.00m);
    }

    #endregion

    #region Complex Scenario - Full Advance Payment Cycle

    [Fact]
    public void FullAdvancePaymentCycle_ShouldWorkCorrectly()
    {
        // Scenariusz: Zamówienie na 100000 netto, 3 zaliczki po 30%, potem rozliczenie

        // Krok 1: Pierwsza zaliczka (30%)
        var zaliczka1 = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/ZAL/001/2024")
                .AsAdvancePayment())
            .AddLineItem(l => l
                .WithProductName("Zaliczka 30% - etap I")
                .WithNetAmount(30000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(6900.00m))
            .Build();

        // Krok 2: Druga zaliczka (30%)
        var zaliczka2 = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 15)
                .WithInvoiceNumber("FV/ZAL/002/2024")
                .AsAdvancePayment())
            .AddLineItem(l => l
                .WithProductName("Zaliczka 30% - etap II")
                .WithNetAmount(30000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(6900.00m))
            .Build();

        // Krok 3: Trzecia zaliczka (30%)
        var zaliczka3 = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 3, 15)
                .WithInvoiceNumber("FV/ZAL/003/2024")
                .AsAdvancePayment())
            .AddLineItem(l => l
                .WithProductName("Zaliczka 30% - etap III")
                .WithNetAmount(30000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(6900.00m))
            .Build();

        // Krok 4: Faktura rozliczeniowa (pozostałe 10%)
        var rozliczenie = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 4, 30)
                .WithInvoiceNumber("FV/ROZ/001/2024")
                .WithSaleDate(2024, 4, 28)
                .AsSettlement())
            // Pełna wartość
            .AddLineItem(l => l
                .WithProductName("Realizacja zamówienia - wartość całkowita")
                .WithNetAmount(100000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23000.00m))
            // Rozliczenie zaliczek
            .AddLineItem(l => l
                .WithProductName("Rozliczenie FV/ZAL/001/2024")
                .WithNetAmount(-30000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-6900.00m))
            .AddLineItem(l => l
                .WithProductName("Rozliczenie FV/ZAL/002/2024")
                .WithNetAmount(-30000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-6900.00m))
            .AddLineItem(l => l
                .WithProductName("Rozliczenie FV/ZAL/003/2024")
                .WithNetAmount(-30000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-6900.00m))
            .Build();

        // Assert - Weryfikacja sum
        zaliczka1.InvoiceData.TotalAmount.Should().Be(36900.00m);
        zaliczka2.InvoiceData.TotalAmount.Should().Be(36900.00m);
        zaliczka3.InvoiceData.TotalAmount.Should().Be(36900.00m);

        // Pozostało 10% do zapłaty w fakturze rozliczeniowej
        rozliczenie.InvoiceData.NetAmount23.Should().Be(10000.00m);
        rozliczenie.InvoiceData.VatAmount23.Should().Be(2300.00m);
        rozliczenie.InvoiceData.TotalAmount.Should().Be(12300.00m);

        // Suma wszystkich faktur = 100000 + 23000 = 123000
        var sumaFaktur = zaliczka1.InvoiceData.TotalAmount +
                         zaliczka2.InvoiceData.TotalAmount +
                         zaliczka3.InvoiceData.TotalAmount +
                         rozliczenie.InvoiceData.TotalAmount;

        sumaFaktur.Should().Be(123000.00m);
    }

    #endregion

    #region XML Serialization Tests

    [Fact]
    public void CreateAdvancePaymentInvoice_ShouldSerializeToValidXml()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/ZAL/001/2024")
                .AsAdvancePayment())
            .AddLineItem(l => l
                .WithProductName("Zaliczka")
                .WithNetAmount(10000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(2300.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("ZAL");
    }

    [Fact]
    public void CreateSettlementInvoice_ShouldSerializeToValidXml()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 28)
                .WithInvoiceNumber("FV/ROZ/001/2024")
                .AsSettlement())
            .AddLineItem(l => l
                .WithProductName("Dostawa")
                .WithNetAmount(10000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(2300.00m))
            .AddLineItem(l => l
                .WithProductName("Rozliczenie zaliczki")
                .WithNetAmount(-10000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-2300.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("ROZ");
    }

    #endregion
}
