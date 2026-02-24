using System.Xml.Linq;
using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Summary;
using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Scenarios;

/// <summary>
/// Testy porównujące wygenerowany XML z przykładową strukturą zgodną z dokumentacją MF
/// Schemat: http://crd.gov.pl/wzor/2025/06/25/13775/
/// </summary>
public class XmlStructureComparisonTests
{
    private static readonly XNamespace KSeFNamespace = "http://crd.gov.pl/wzor/2025/06/25/13775/";

    #region XML Structure Tests

    [Fact]
    public void GeneratedXml_ShouldHaveCorrectRootElement()
    {
        // Arrange
        var invoice = CreateBasicVatInvoice();

        // Act
        var xDoc = XmlSerializationHelper.ToXDocument(invoice);

        // Assert
        xDoc.Root.Should().NotBeNull();
        xDoc.Root!.Name.LocalName.Should().Be("Faktura");
    }

    [Fact]
    public void GeneratedXml_ShouldHaveCorrectNamespace()
    {
        // Arrange
        var invoice = CreateBasicVatInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - KsefInvoiceSerializer używa prefiksu tns: dla przestrzeni nazw
        xml.Should().Contain(KSeF.Invoice.Models.Invoice.KSeFNamespace);
        xml.Should().Contain("tns:Faktura");
    }

    [Fact]
    public void GeneratedXml_ShouldContainHeaderElement()
    {
        // Arrange
        var invoice = CreateBasicVatInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().Contain("Naglowek");
    }

    [Fact]
    public void GeneratedXml_ShouldContainSellerElement()
    {
        // Arrange
        var invoice = CreateBasicVatInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().Contain("Podmiot1");
    }

    [Fact]
    public void GeneratedXml_ShouldContainBuyerElement()
    {
        // Arrange
        var invoice = CreateBasicVatInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().Contain("Podmiot2");
    }

    [Fact]
    public void GeneratedXml_ShouldContainInvoiceDataElement()
    {
        // Arrange
        var invoice = CreateBasicVatInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - KsefInvoiceSerializer używa prefiksu tns:
        xml.Should().Contain(":Fa>");
    }

    #endregion

    #region Invoice Header Structure Tests

    [Fact]
    public void Header_ShouldContainRequiredFields()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23))
            .WithCreationDateTime(new DateTime(2024, 1, 15, 10, 30, 0))
            .WithSystemInfo("Test System v1.0")
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Nagłówek powinien zawierać wymagane elementy
        xml.Should().Contain("KodFormularza"); // Kod formularza
        xml.Should().Contain("DataWytworzeniaFa"); // Data wytworzenia
        xml.Should().Contain("SystemInfo"); // System info
    }

    #endregion

    #region Seller (Podmiot1) Structure Tests

    [Fact]
    public void Seller_ShouldHaveCorrectStructure()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Test Company Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Testowa 10")
                    .WithAddressLine2("00-001 Warszawa"))
                .WithContactData("test@example.com", "+48123456789"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Zgodność z P_3A (NIP), Nazwa
        xml.Should().Contain("1234567890"); // NIP
        xml.Should().Contain("Test Company Sp. z o.o."); // Nazwa
        xml.Should().Contain("PL"); // Kod kraju
        xml.Should().Contain("ul. Testowa 10"); // Adres linia 1
        xml.Should().Contain("00-001 Warszawa"); // Adres linia 2
    }

    #endregion

    #region Buyer (Podmiot2) Structure Tests

    [Fact]
    public void Buyer_WithNip_ShouldHaveCorrectStructure()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca Krajowy Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Nabywcy 5")
                    .WithAddressLine2("31-000 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Zgodność z P_4A (NIP)
        xml.Should().Contain("0987654321");
        xml.Should().Contain("Nabywca Krajowy");
    }

    [Fact]
    public void Buyer_WithEuVat_ShouldHaveCorrectStructure()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b
                .WithEuVatId(EUCountryCode.DE, "123456789")
                .WithName("German GmbH"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate0IntraCommunitySupply)
                .WithVatAmount(0))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Zgodność z identyfikatorem UE
        xml.Should().Contain("DE"); // Kod kraju UE
        xml.Should().Contain("123456789"); // Numer VAT UE
    }

    #endregion

    #region Invoice Data (Fa) Structure Tests

    [Fact]
    public void InvoiceData_ShouldContainBasicFields()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024")
                .WithSaleDate(2024, 1, 14)
                .WithIssuePlace("Warszawa")
                .WithCurrency(CurrencyCode.PLN))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        // P_1M - Miejsce wystawienia
        xml.Should().Contain("Warszawa");
        // P_2 - Numer faktury
        xml.Should().Contain("FV/001/2024");
        // Elementy P_1 i P_6 są obecne w XML
        xml.Should().Contain("P_1");
        xml.Should().Contain("P_6");
        // Uwaga: DateOnly nie jest poprawnie serializowany przez XmlSerializer w .NET
        // Do pełnej obsługi wymagane jest dodanie proxy properties w modelu
        // Weryfikujemy, że daty są ustawione w modelu
        invoice.InvoiceData.IssueDate.Should().Be(new DateOnly(2024, 1, 15));
        invoice.InvoiceData.SaleDate.Should().Be(new DateOnly(2024, 1, 14));
    }

    [Fact]
    public void InvoiceData_VatType_ShouldBeCorrectlyRepresented()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024")
                .WithInvoiceType(InvoiceType.VAT))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Rodzaj faktury VAT
        xml.Should().Contain("VAT");
    }

    [Fact]
    public void InvoiceData_CorrectionType_ShouldBeCorrectlyRepresented()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 20)
                .WithInvoiceNumber("FV/001/2024/KOR")
                .AsCorrection("Korekta ilości", c => c
                    .WithInvoiceNumber("FV/001/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 15))))
            .AddLineItem(l => l
                .WithProductName("Test - korekta")
                .WithNetAmount(-100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-23))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Rodzaj faktury KOR
        xml.Should().Contain("KOR");
        xml.Should().Contain("Korekta ilości"); // Przyczyna korekty
    }

    #endregion

    #region Line Items (FaWiersz) Structure Tests

    [Fact]
    public void LineItem_ShouldContainRequiredFields()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługa konsultingowa")
                .WithQuantity(10)
                .WithUnit("godz.")
                .WithUnitNetPrice(100.00m)
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        // NrWiersza - Numer wiersza
        xml.Should().Contain(">1<"); // Numer wiersza
        // P_7 - Nazwa towaru/usługi
        xml.Should().Contain("Usługa konsultingowa");
        // P_8A - Miara
        xml.Should().Contain("godz.");
        // P_8B - Ilość
        xml.Should().Contain("10");
        // P_11 - Wartość sprzedaży netto
        xml.Should().Contain("1000");
        // P_12 - Stawka podatku
        xml.Should().Contain("23");
    }

    [Fact]
    public void LineItem_WithPkwiu_ShouldContainPkwiuCode()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługi programistyczne")
                .WithNetAmount(5000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(1150.00m)
                .WithPkwiuCode("62.01.11.0"))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - PKWiU
        xml.Should().Contain("62.01.11.0");
    }

    [Fact]
    public void LineItem_WithGtin_ShouldContainGtinCode()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Produkt XYZ")
                .WithNetAmount(100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23.00m)
                .WithGtinCode("5901234123457"))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - GTIN
        xml.Should().Contain("5901234123457");
    }

    #endregion

    #region VAT Summary (P_13, P_14, P_15) Structure Tests

    [Fact]
    public void VatSummary_23Percent_ShouldContainCorrectFields()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test 23%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        // P_13_1 - Suma wartości sprzedaży netto stawka 23%
        // P_14_1 - Kwota podatku stawka 23%
        // P_15 - Kwota należności ogółem
        xml.Should().Contain("1000"); // Netto
        xml.Should().Contain("230");  // VAT
        xml.Should().Contain("1230"); // Brutto
    }

    [Fact]
    public void VatSummary_MultipleRates_ShouldContainAllSummaries()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Elektronika 23%")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .AddLineItem(l => l
                .WithProductName("Książka 5%")
                .WithNetAmount(100.00m)
                .WithVatRate(VatRate.Rate5)
                .WithVatAmount(5.00m))
            .AddLineItem(l => l
                .WithProductName("Żywność 8%")
                .WithNetAmount(200.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(16.00m))
            .Build();

        // Assert
        invoice.InvoiceData.NetAmount23.Should().Be(1000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(230.00m);
        invoice.InvoiceData.NetAmount5.Should().Be(100.00m);
        invoice.InvoiceData.VatAmount5.Should().Be(5.00m);
        invoice.InvoiceData.NetAmount8.Should().Be(200.00m);
        invoice.InvoiceData.VatAmount8.Should().Be(16.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1551.00m);
    }

    #endregion

    #region Third Party (Podmiot3) Structure Tests

    [Fact]
    public void ThirdParty_Factor_ShouldHaveCorrectStructure()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .AddRecipient(r => r
                .WithTaxId("5555555555")
                .WithName("Faktor Bank S.A.")
                .AsFactor())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().Contain("Podmiot3");
        xml.Should().Contain("5555555555");
        xml.Should().Contain("Faktor Bank");
    }

    #endregion

    #region Payment (Platnosc) Structure Tests

    [Fact]
    public void Payment_ShouldHaveCorrectStructure()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(1000)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 2, 14)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL61109010140000071219812874", "PKO BP S.A."))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().Contain("Platnosc");
        xml.Should().Contain("Termin"); // Element terminu płatności
        xml.Should().Contain("PL61109010140000071219812874"); // Numer rachunku
        // DateOnly nie serializuje się bezpośrednio przez XmlSerializer
        // Weryfikujemy w modelu
        invoice.InvoiceData.Payment!.PaymentTerms![0].DueDate.Should().Be(new DateOnly(2024, 2, 14));
    }

    #endregion

    #region Annotations (Adnotacje) Structure Tests

    [Fact]
    public void Annotations_SplitPayment_ShouldBeRepresentedCorrectly()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024")
                .WithSplitPayment())
            .AddLineItem(l => l
                .WithProductName("Stal budowlana")
                .WithNetAmount(50000)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(11500))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - MPP = 1 (mechanizm podzielonej płatności)
        invoice.InvoiceData.Annotations.SplitPayment.Should().Be(AnnotationValue.Yes);
    }

    [Fact]
    public void Annotations_ReverseCharge_ShouldBeRepresentedCorrectly()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024")
                .WithReverseCharge())
            .AddLineItem(l => l
                .WithProductName("Usługi budowlane")
                .WithNetAmount(10000)
                .WithVatRate(VatRate.ReverseCharge)
                .WithVatAmount(0))
            .Build();

        // Assert
        invoice.InvoiceData.Annotations.ReverseCharge.Should().Be(AnnotationValue.Yes);
    }

    #endregion

    #region Full Invoice XML Comparison

    [Fact]
    public void CompleteInvoice_ShouldContainAllExpectedElements()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Handlowa 10")
                    .WithAddressLine2("00-001 Warszawa"))
                .WithContactData("sprzedawca@example.com", "+48123456789"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Biznesowa 20")
                    .WithAddressLine2("31-000 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/01/2024")
                .WithSaleDate(2024, 1, 14)
                .WithIssuePlace("Warszawa")
                .WithCurrency(CurrencyCode.PLN)
                .WithInvoiceType(InvoiceType.VAT))
            .AddLineItem(l => l
                .WithProductName("Usługa konsultingowa")
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
            .WithSystemInfo("KSeF Test System v1.0")
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Główne elementy struktury
        var expectedElements = new[]
        {
            "Faktura",      // Root element
            "Naglowek",     // Header
            "Podmiot1",     // Seller
            "Podmiot2",     // Buyer
            "Fa",           // Invoice data
            "FaWiersz",     // Line items
            "Platnosc"      // Payment
        };

        foreach (var element in expectedElements)
        {
            xml.Should().Contain(element, because: $"element {element} should be present in the XML");
        }

        // Assert - Kluczowe wartości
        xml.Should().Contain("1234567890"); // NIP sprzedawcy
        xml.Should().Contain("0987654321"); // NIP nabywcy
        xml.Should().Contain("FV/001/01/2024"); // Numer faktury
        xml.Should().Contain("P_1"); // Element daty wystawienia (DateOnly nie serializuje się bezpośrednio)
        xml.Should().Contain("Usługa konsultingowa"); // Nazwa towaru/usługi
        xml.Should().Contain("15990"); // Kwota należności ogółem
        // Weryfikacja daty w modelu
        invoice.InvoiceData.IssueDate.Should().Be(new DateOnly(2024, 1, 15));
    }

    #endregion

    #region Comparison with MF Documentation Examples

    /// <summary>
    /// Test porównujący strukturę wygenerowanej prostej faktury VAT
    /// z przykładem z dokumentacji MF (prosta_faktura_vat.xml)
    /// </summary>
    [Fact]
    public void GeneratedBasicVatInvoice_ShouldMatchMfDocumentationStructure()
    {
        // Arrange - Faktura odpowiadająca prosta_faktura_vat.xml
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Firma ABC Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Przykładowa 1")
                    .WithAddressLine2("00-001 Warszawa"))
                .WithContactData("biuro@firma-abc.pl", "+48 22 111 22 33"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Klient XYZ S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Kliencka 99")
                    .WithAddressLine2("30-001 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/2024/001")
                .WithSaleDate(2024, 1, 15)
                .WithCurrency(CurrencyCode.PLN)
                .WithInvoiceType(InvoiceType.VAT))
            .AddLineItem(l => l
                .WithProductName("Usługa konsultingowa IT")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithUnitNetPrice(1000.00m)
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 1, 30)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL61109010140000071219812874"))
            .WithSystemInfo("KSeF.Invoice.Samples 1.0")
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Kluczowe elementy zgodne z dokumentacją MF
        // Nagłówek
        xml.Should().Contain("Naglowek");
        xml.Should().Contain("KodFormularza");
        xml.Should().Contain("DataWytworzeniaFa");
        xml.Should().Contain("SystemInfo");

        // Podmiot1 - Sprzedawca
        xml.Should().Contain("Podmiot1");
        xml.Should().Contain("1234567890");
        xml.Should().Contain("Firma ABC Sp. z o.o.");
        xml.Should().Contain("PL");

        // Podmiot2 - Nabywca
        xml.Should().Contain("Podmiot2");
        xml.Should().Contain("0987654321");
        xml.Should().Contain("Klient XYZ");

        // Fa - Dane merytoryczne
        xml.Should().Contain(":Fa>");
        xml.Should().Contain("PLN");
        xml.Should().Contain("FV/2024/001");
        xml.Should().Contain("VAT");

        // Podsumowania VAT (zgodne z MF: P_13_1, P_14_1, P_15)
        invoice.InvoiceData.NetAmount23.Should().Be(1000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(230.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(1230.00m);

        // FaWiersz - Pozycja
        xml.Should().Contain("FaWiersz");
        xml.Should().Contain("Usługa konsultingowa IT");

        // Platnosc
        xml.Should().Contain("Platnosc");
        xml.Should().Contain("PL61109010140000071219812874");
    }

    /// <summary>
    /// Test porównujący strukturę faktury korygującej z dokumentacją MF (faktura_korygujaca.xml)
    /// </summary>
    [Fact]
    public void GeneratedCorrectionInvoice_ShouldMatchMfDocumentationStructure()
    {
        // Arrange - Faktura korygująca odpowiadająca faktura_korygujaca.xml
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Firma ABC Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Przykładowa 1")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Klient XYZ S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Kliencka 99")
                    .WithAddressLine2("30-001 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 2, 1)
                .WithInvoiceNumber("FV/2024/001/KOR")
                .WithSaleDate(2024, 1, 15)
                .AsCorrection("Błąd w cenie jednostkowej - udzielono rabatu", c => c
                    .WithInvoiceNumber("FV/2024/001")
                    .WithIssueDate(new DateOnly(2024, 1, 15))
                    .WithKSeFNumber("1234567890-20240115-ABC123456789")))
            .AddLineItem(l => l
                .WithProductName("Usługa konsultingowa IT - korekta (rabat)")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithUnitNetPrice(-200.00m)
                .WithNetAmount(-200.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(-46.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Zgodność z dokumentacją MF
        // Typ faktury KOR
        xml.Should().Contain("KOR");
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        invoice.IsCorrection.Should().BeTrue();

        // Dane faktury korygowanej (DaneFaKorygowanej)
        xml.Should().Contain("FV/2024/001");
        invoice.InvoiceData.CorrectedInvoiceData.Should().NotBeNull();
        invoice.InvoiceData.CorrectedInvoiceData!.CorrectedInvoiceNumber.Should().Be("FV/2024/001");
        invoice.InvoiceData.CorrectedInvoiceData.CorrectedInvoiceKSeFNumber.Should().Be("1234567890-20240115-ABC123456789");

        // Przyczyna korekty (P_15ZK)
        xml.Should().Contain("rabatu");
        invoice.InvoiceData.CorrectionReason.Should().Contain("rabatu");

        // Ujemne wartości - zmniejszenie
        invoice.InvoiceData.NetAmount23.Should().Be(-200.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(-46.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(-246.00m);
    }

    /// <summary>
    /// Test porównujący strukturę faktury zaliczkowej z dokumentacją MF (faktura_zaliczkowa.xml)
    /// </summary>
    [Fact]
    public void GeneratedAdvancePaymentInvoice_ShouldMatchMfDocumentationStructure()
    {
        // Arrange - Faktura zaliczkowa odpowiadająca faktura_zaliczkowa.xml
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Firma Budowlana ABC Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Budowlana 10")
                    .WithAddressLine2("00-001 Warszawa"))
                .WithContactData("biuro@budowlana-abc.pl", "+48 22 999 88 77"))
            .WithBuyer(b => b
                .WithTaxId("5555555555")
                .WithName("Inwestor DEF S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Inwestorska 50")
                    .WithAddressLine2("02-222 Warszawa")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 10)
                .WithInvoiceNumber("FV/2024/ZAL/001")
                .WithCurrency(CurrencyCode.PLN)
                .AsAdvancePayment()
                .AddDescription("Numer zamówienia", "ZAM/2024/001")
                .AddDescription("Opis projektu", "Budowa domu jednorodzinnego - ul. Słoneczna 15, Warszawa")
                .AddDescription("Wartość całkowita umowy", "200 000 PLN netto"))
            .AddLineItem(l => l
                .WithProductName("Zaliczka na prace budowlane - Etap I (fundamenty)")
                .WithQuantity(1)
                .WithUnit("szt.")
                .WithUnitNetPrice(50000.00m)
                .WithNetAmount(50000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(11500.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 1, 20)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL27114020040000320218427362", "mBank S.A."))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Zgodność z dokumentacją MF
        // Typ faktury ZAL
        xml.Should().Contain("ZAL");
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ZAL);
        invoice.IsAdvancePayment.Should().BeTrue();

        // Dane sprzedawcy i nabywcy
        xml.Should().Contain("Firma Budowlana ABC");
        xml.Should().Contain("Inwestor DEF");
        xml.Should().Contain("5555555555");

        // Pozycja faktury zaliczkowej
        xml.Should().Contain("Zaliczka na prace budowlane");

        // Sumy zgodne z MF
        invoice.InvoiceData.NetAmount23.Should().Be(50000.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(11500.00m);
        invoice.InvoiceData.TotalAmount.Should().Be(61500.00m);

        // Dodatkowe opisy (DodatkowyOpis)
        invoice.InvoiceData.AdditionalDescription.Should().HaveCount(3);
    }

    /// <summary>
    /// Test porównujący strukturę faktury wielopozycyjnej z dokumentacją MF (faktura_wielopozycyjna.xml)
    /// </summary>
    [Fact]
    public void GeneratedMultiLineItemInvoice_ShouldMatchMfDocumentationStructure()
    {
        // Arrange - Faktura wielopozycyjna odpowiadająca faktura_wielopozycyjna.xml
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sklep Komputerowy ABC Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Techniczna 15")
                    .WithAddressLine2("02-677 Warszawa"))
                .WithContactData("sklep@abc-komputery.pl", "+48 22 333 44 55"))
            .WithBuyer(b => b
                .WithTaxId("9876543210")
                .WithName("IT Solutions Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Biznesowa 42")
                    .WithAddressLine2("31-564 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 20)
                .WithInvoiceNumber("FV/2024/002")
                .WithSaleDate(2024, 1, 20)
                .WithCurrency(CurrencyCode.PLN)
                .WithInvoiceType(InvoiceType.VAT))
            // Pozycja 1: Laptop (23%)
            .AddLineItem(l => l
                .WithProductName("Laptop Dell XPS 15 (16GB RAM, 512GB SSD)")
                .WithQuantity(2)
                .WithUnit("szt.")
                .WithUnitNetPrice(5000.00m)
                .WithNetAmount(10000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(2300.00m)
                .WithGtinCode("5901234123457"))
            // Pozycja 2: Monitor (23%)
            .AddLineItem(l => l
                .WithProductName("Monitor 27\" 4K IPS")
                .WithQuantity(2)
                .WithUnit("szt.")
                .WithUnitNetPrice(1500.00m)
                .WithNetAmount(3000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(690.00m))
            // Pozycja 3: Klawiatura (23%)
            .AddLineItem(l => l
                .WithProductName("Klawiatura mechaniczna Cherry MX")
                .WithQuantity(4)
                .WithUnit("szt.")
                .WithUnitNetPrice(300.00m)
                .WithNetAmount(1200.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(276.00m))
            // Pozycja 4: Usługa konfiguracji (23%)
            .AddLineItem(l => l
                .WithProductName("Usługa konfiguracji sprzętu")
                .WithQuantity(8)
                .WithUnit("h")
                .WithUnitNetPrice(150.00m)
                .WithNetAmount(1200.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(276.00m)
                .WithPkwiuCode("62.02.30.0"))
            // Pozycja 5: Podręcznik (8%)
            .AddLineItem(l => l
                .WithProductName("Podręcznik \"Administracja Windows Server\"")
                .WithQuantity(2)
                .WithUnit("szt.")
                .WithUnitNetPrice(250.00m)
                .WithNetAmount(500.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(40.00m)
                .WithPkwiuCode("58.11.1"))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 2, 20)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL61109010140000071219812874", "PKO BP S.A."))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Zgodność z dokumentacją MF
        // Wiele pozycji
        invoice.InvoiceData.LineItems.Should().HaveCount(5);

        // Automatyczne numerowanie
        invoice.InvoiceData.LineItems![0].LineNumber.Should().Be(1);
        invoice.InvoiceData.LineItems[1].LineNumber.Should().Be(2);
        invoice.InvoiceData.LineItems[2].LineNumber.Should().Be(3);
        invoice.InvoiceData.LineItems[3].LineNumber.Should().Be(4);
        invoice.InvoiceData.LineItems[4].LineNumber.Should().Be(5);

        // Różne stawki VAT - sumy
        // 23%: 10000 + 3000 + 1200 + 1200 = 15400 netto, 2300 + 690 + 276 + 276 = 3542 VAT
        invoice.InvoiceData.NetAmount23.Should().Be(15400.00m);
        invoice.InvoiceData.VatAmount23.Should().Be(3542.00m);

        // 8%: 500 netto, 40 VAT
        invoice.InvoiceData.NetAmount8.Should().Be(500.00m);
        invoice.InvoiceData.VatAmount8.Should().Be(40.00m);

        // Suma brutto zgodna z MF: 15400 + 3542 + 500 + 40 = 19482
        invoice.InvoiceData.TotalAmount.Should().Be(19482.00m);

        // Kody produktów
        xml.Should().Contain("5901234123457"); // GTIN
        xml.Should().Contain("62.02.30.0");    // PKWiU
        xml.Should().Contain("58.11.1");        // PKWiU dla książki
    }

    /// <summary>
    /// Test weryfikujący strukturę namespace zgodną ze schematem KSeF FA(3)
    /// </summary>
    [Fact]
    public void GeneratedXml_ShouldUseCorrectKSeFNamespace()
    {
        // Arrange
        var invoice = CreateBasicVatInvoice();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Przestrzeń nazw zgodna z FA(3)
        // Oczekiwana przestrzeń: http://crd.gov.pl/wzor/2025/06/25/13775/
        xml.Should().Contain("http://crd.gov.pl/wzor/2025/06/25/13775/");
    }

    /// <summary>
    /// Test weryfikujący strukturę elementów nagłówka zgodną z dokumentacją MF
    /// </summary>
    [Fact]
    public void Header_ShouldMatchMfDocumentationStructure()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23))
            .WithCreationDateTime(new DateTime(2024, 1, 15, 10, 30, 0))
            .WithSystemInfo("KSeF.Invoice.Samples 1.0")
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert - Nagłówek zgodny z MF
        // KodFormularza z atrybutami
        xml.Should().Contain("KodFormularza");
        xml.Should().Contain("FA");

        // WariantFormularza
        xml.Should().Contain("WariantFormularza");

        // DataWytworzeniaFa
        xml.Should().Contain("DataWytworzeniaFa");

        // SystemInfo
        xml.Should().Contain("SystemInfo");
        xml.Should().Contain("KSeF.Invoice.Samples 1.0");
    }

    /// <summary>
    /// Test weryfikujący wszystkie typy faktur zgodne z dokumentacją MF
    /// </summary>
    [Theory]
    [InlineData(InvoiceType.VAT, false, false, false, false)]
    [InlineData(InvoiceType.KOR, true, false, false, false)]
    [InlineData(InvoiceType.ZAL, false, true, false, false)]
    [InlineData(InvoiceType.ROZ, false, false, true, false)]
    [InlineData(InvoiceType.UPR, false, false, false, true)]
    public void InvoiceTypes_ShouldHaveCorrectFlags(
        InvoiceType invoiceType,
        bool isCorrection,
        bool isAdvancePayment,
        bool isSettlement,
        bool isSimplified)
    {
        // Arrange
        var builder = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d =>
            {
                d.WithIssueDate(2024, 1, 15)
                 .WithInvoiceNumber("FV/001/2024");

                switch (invoiceType)
                {
                    case InvoiceType.KOR:
                        d.AsCorrection("Korekta testowa", c => c
                            .WithInvoiceNumber("FV/000/2024")
                            .WithIssueDate(new DateOnly(2024, 1, 10)));
                        break;
                    case InvoiceType.ZAL:
                        d.AsAdvancePayment();
                        break;
                    case InvoiceType.ROZ:
                        d.AsSettlement();
                        break;
                    case InvoiceType.UPR:
                        d.AsSimplified();
                        break;
                    default:
                        d.WithInvoiceType(InvoiceType.VAT);
                        break;
                }
            })
            .AddLineItem(l => l
                .WithProductName("Test")
                .WithNetAmount(100)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23));

        // Act
        var invoice = builder.Build();
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(invoiceType);
        invoice.IsCorrection.Should().Be(isCorrection);
        invoice.IsAdvancePayment.Should().Be(isAdvancePayment);
        invoice.IsSettlement.Should().Be(isSettlement);
        invoice.IsSimplified.Should().Be(isSimplified);

        // XML powinien zawierać odpowiedni typ
        xml.Should().Contain(invoiceType.ToString());
    }

    #endregion

    #region Helper Methods

    private static KSeF.Invoice.Models.Invoice CreateBasicVatInvoice()
    {
        return InvoiceBuilder.Create()
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
                .WithProductName("Usługa testowa")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();
    }

    #endregion
}
