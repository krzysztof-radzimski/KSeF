using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Scenarios;

/// <summary>
/// Testy scenariuszy dla faktur korygujących (KOR, KOR_ZAL, KOR_ROZ)
/// </summary>
public class CorrectionInvoiceScenarioTests
{
    #region Basic Correction Invoice (KOR)

    [Fact]
    public void CreateCorrectionInvoice_WithQuantityCorrection_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Korekta ilości
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Handlowa 10")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Biznesowa 20")
                    .WithAddressLine2("31-000 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 20)
                .WithInvoiceNumber("FV/001/01/2024/KOR")
                .AsCorrection("Korekta ilości - błędnie wprowadzono 10 szt. zamiast 8 szt.", c => c
                    .WithInvoiceNumber("FV/001/01/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 15))))
            // Pozycja korygująca - różnica (zmniejszenie o 2 szt.)
            .AddLineItem(l => l
                .WithProductName("Laptop - korekta ilości")
                .WithQuantity(-2)
                .WithUnit("szt.")
                .WithUnitNetPrice(3000.00m)
                .WithNetAmount(-6000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-1380.00m))
            .WithPayment(p => p
                .AddPaymentTermDescription("Zwrot na konto nabywcy w ciągu 14 dni"))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        invoice.IsCorrection.Should().BeTrue();
        invoice.InvoiceData.CorrectionReason.Should().Contain("Korekta ilości");
        invoice.InvoiceData.CorrectedInvoiceData.Should().NotBeNull();
        invoice.InvoiceData.CorrectedInvoiceData!.CorrectedInvoiceNumber.Should().Be("FV/001/01/2024");
        invoice.InvoiceData.CorrectedInvoiceData.CorrectedInvoiceIssueDate.Should().Be(new DateOnly(2024, 1, 15));

        // Weryfikacja kwot ujemnych
        invoice.InvoiceData.NetAmount23.Should().Be(-6000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(-1380.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(-7380.00m);
    }

    [Fact]
    public void CreateCorrectionInvoice_WithPriceCorrection_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Korekta ceny
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 25)
                .WithInvoiceNumber("FV/002/01/2024/KOR")
                .AsCorrection("Korekta ceny - udzielono rabatu retroaktywnego 10%", c => c
                    .WithInvoiceNumber("FV/002/01/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 10))))
            // Pozycja korygująca - rabat 10% od kwoty 10000 netto
            .AddLineItem(l => l
                .WithProductName("Usługa konsultingowa - rabat 10%")
                .WithNetAmount(-1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-230.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        invoice.InvoiceData.CorrectionReason.Should().Contain("rabatu retroaktywnego");
        invoice.InvoiceData.NetAmount23.Should().Be(-1000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(-230.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(-1230.00m);
    }

    [Fact]
    public void CreateCorrectionInvoice_WithVatRateCorrection_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Korekta stawki VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Księgarnia Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 30)
                .WithInvoiceNumber("FV/003/01/2024/KOR")
                .AsCorrection("Korekta stawki VAT - błędnie zastosowano 23% zamiast 5% dla książki", c => c
                    .WithInvoiceNumber("FV/003/01/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 5))))
            // Storno starej pozycji (23%)
            .AddLineItem(l => l
                .WithProductName("Książka - storno stawki 23%")
                .WithNetAmount(-100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-23.00m))
            // Nowa pozycja z prawidłową stawką (5%)
            .AddLineItem(l => l
                .WithProductName("Książka - prawidłowa stawka 5%")
                .WithNetAmount(100.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(5.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        invoice.InvoiceData.LineItems.Should().HaveCount(2);

        // Różnica w stawkach
        invoice.InvoiceData.NetAmount23.Should().Be(-100.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(-23.00m);
        invoice.InvoiceData.NetAmount5.Should().Be(100.00m);
        invoice.InvoiceData.VatAmount5.Should().Be(5.00m);

        // Suma netto = 0, ale różnica VAT = -18
        invoice.InvoiceData.TotalAmount.Should().Be(-18.00m);
    }

    [Fact]
    public void CreateCorrectionInvoice_WithKSeFReference_ShouldIncludeKSeFNumber()
    {
        // Arrange & Act - Korekta z referencją do KSeF
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 1)
                .WithInvoiceNumber("FV/004/02/2024/KOR")
                .AsCorrection("Korekta danych nabywcy", c => c
                    .WithInvoiceNumber("FV/004/01/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 20))
                    .WithKSeFNumber("1234567890-20240120-ABC123DEF456-78")))
            .AddLineItem(l => l
                .WithProductName("Pozycja bez zmian kwot")
                .WithNetAmount(0.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.InvoiceData.CorrectedInvoiceData!.CorrectedInvoiceKSeFNumber.Should().Be("1234567890-20240120-ABC123DEF456-78");
    }

    #endregion

    #region Correction Invoice for Advance Payment (KOR_ZAL)

    [Fact]
    public void CreateAdvancePaymentCorrectionInvoice_ShouldSetCorrectType()
    {
        // Arrange & Act - Korekta faktury zaliczkowej
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 15)
                .WithInvoiceNumber("FV/ZAL/001/2024/KOR")
                .AsCorrection("Korekta faktury zaliczkowej - zmniejszenie zaliczki", c => c
                    .WithInvoiceNumber("FV/ZAL/001/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 10)))
                .WithInvoiceType(InvoiceType.KOR_ZAL))
            .AddLineItem(l => l
                .WithProductName("Zaliczka na dostawę towaru - korekta")
                .WithNetAmount(-5000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-1150.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR_ZAL);
        invoice.IsCorrection.Should().BeTrue();
        invoice.InvoiceData.TotalAmount.Should().Be(-6150.00m);
    }

    #endregion

    #region Correction Invoice for Settlement (KOR_ROZ)

    [Fact]
    public void CreateSettlementCorrectionInvoice_ShouldSetCorrectType()
    {
        // Arrange & Act - Korekta faktury rozliczeniowej
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 3, 20)
                .WithInvoiceNumber("FV/ROZ/001/2024/KOR")
                .AsCorrection("Korekta faktury rozliczeniowej - błąd w rozliczeniu zaliczek", c => c
                    .WithInvoiceNumber("FV/ROZ/001/2024")
                    .WithIssueDate(new DateOnly(2024, 2, 28)))
                .WithInvoiceType(InvoiceType.KOR_ROZ))
            .AddLineItem(l => l
                .WithProductName("Dostawa towaru - korekta rozliczenia")
                .WithNetAmount(-2000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-460.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR_ROZ);
        invoice.IsCorrection.Should().BeTrue();
        invoice.InvoiceData.TotalAmount.Should().Be(-2460.00m);
    }

    #endregion

    #region Positive Correction (Increase)

    [Fact]
    public void CreateCorrectionInvoice_WithIncrease_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Korekta zwiększająca
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 5)
                .WithInvoiceNumber("FV/005/02/2024/KOR")
                .AsCorrection("Korekta zwiększająca - dodatkowa usługa nieuwzględniona w fakturze pierwotnej", c => c
                    .WithInvoiceNumber("FV/005/01/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 25))))
            .AddLineItem(l => l
                .WithProductName("Dodatkowa usługa serwisowa")
                .WithQuantity(5)
                .WithUnit("godz.")
                .WithUnitNetPrice(200.00m)
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        invoice.InvoiceData.NetAmount23.Should().Be(1000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(230.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1230.00m);
    }

    #endregion

    #region Multiple Line Items Correction

    [Fact]
    public void CreateCorrectionInvoice_WithMultipleLineItems_ShouldCalculateTotalsCorrectly()
    {
        // Arrange & Act - Korekta z wieloma pozycjami
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Hurtownia Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Sklep S.A."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 10)
                .WithInvoiceNumber("FV/006/02/2024/KOR")
                .AsCorrection("Korekta zbiorcza - zwrot części towaru", c => c
                    .WithInvoiceNumber("FV/006/01/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 15))))
            // Zwrot elektroniki (23%)
            .AddLineItem(l => l
                .WithProductName("Telefon - zwrot")
                .WithQuantity(-2)
                .WithUnit("szt.")
                .WithUnitNetPrice(1500.00m)
                .WithNetAmount(-3000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-690.00m))
            // Zwrot książek (5%)
            .AddLineItem(l => l
                .WithProductName("Książki - zwrot")
                .WithQuantity(-5)
                .WithUnit("szt.")
                .WithUnitNetPrice(50.00m)
                .WithNetAmount(-250.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(-12.50m))
            // Zwrot artykułów spożywczych (8%)
            .AddLineItem(l => l
                .WithProductName("Artykuły spożywcze - zwrot")
                .WithQuantity(-10)
                .WithUnit("szt.")
                .WithUnitNetPrice(20.00m)
                .WithNetAmount(-200.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(-16.00m))
            .Build();

        // Assert
        invoice.InvoiceData.LineItems.Should().HaveCount(3);
        invoice.InvoiceData.NetAmount23.Should().Be(-3000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(-690.00m);
        invoice.InvoiceData.NetAmount5.Should().Be(-250.00m);
        invoice.InvoiceData.VatAmount5.Should().Be(-12.50m);
        invoice.InvoiceData.NetAmount8.Should().Be(-200.00m);
        invoice.InvoiceData.VatAmount8.Should().Be(-16.00m);

        // Total = -3690 - 262.50 - 216 = -4168.50
        invoice.InvoiceData.TotalAmount.Should().Be(-4168.50m);
    }

    #endregion

    #region XML Serialization Tests

    [Fact]
    public void CreateCorrectionInvoice_ShouldSerializeToValidXml()
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
                .WithIssueDate(2024, 1, 20)
                .WithInvoiceNumber("FV/001/2024/KOR")
                .AsCorrection("Korekta ilości", c => c
                    .WithInvoiceNumber("FV/001/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 15))))
            .AddLineItem(l => l
                .WithProductName("Towar - korekta")
                .WithNetAmount(-1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-230.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("KOR");
        xml.Should().Contain("FV/001/2024"); // Numer korygowanej faktury
    }

    [Fact]
    public void CreateCorrectionInvoice_ShouldRoundTripSerialize()
    {
        // Arrange
        var originalInvoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 20)
                .WithInvoiceNumber("FV/KOR/001/2024")
                .AsCorrection("Korekta", c => c
                    .WithInvoiceNumber("FV/001/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 15))))
            .AddLineItem(l => l
                .WithProductName("Korekta")
                .WithNetAmount(-100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-23.00m))
            .Build();

        // Act
        var deserializedInvoice = XmlSerializationHelper.RoundTrip(originalInvoice);

        // Assert
        deserializedInvoice.Should().NotBeNull();
        deserializedInvoice!.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        deserializedInvoice.IsCorrection.Should().BeTrue();
    }

    #endregion
}
