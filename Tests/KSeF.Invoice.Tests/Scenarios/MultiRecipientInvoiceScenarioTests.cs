using FluentAssertions;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Scenarios;

/// <summary>
/// Testy scenariuszy dla faktur z wieloma odbiorcami (Podmiot3)
/// </summary>
public class MultiRecipientInvoiceScenarioTests
{
    #region Invoice with Factor (Faktor)

    [Fact]
    public void CreateInvoice_WithFactor_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura z faktorem (cesja wierzytelności)
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
            .AddRecipient(r => r
                .WithTaxId("5555555555")
                .WithName("Faktoring Bank S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Bankowa 1")
                    .WithAddressLine2("00-950 Warszawa"))
                .AsFactor())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/FAK/001/2024")
                .AddDescription("Informacja", "Płatność na rachunek faktora"))
            .AddLineItem(l => l
                .WithProductName("Dostawa towarów")
                .WithNetAmount(50000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(11500.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 2, 14)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .WithFactoringBankAccount(ba => ba
                    .WithAccountNumber("PL11111111111111111111111111")
                    .WithBankName("Faktoring Bank S.A.")
                    .WithSwiftCode("FAKTPLPW")))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients.Should().HaveCount(1);
        invoice.Recipients![0].Role.Should().Be(SubjectRole.Factor);
        invoice.Recipients[0].TaxId.Should().Be("5555555555");
        invoice.Recipients[0].Name.Should().Be("Faktoring Bank S.A.");

        // Płatność na rachunek faktora
        invoice.InvoiceData.Payment!.FactoringBankAccount.Should().NotBeNull();
        invoice.InvoiceData.Payment.FactoringBankAccount!.AccountNumber.Should().Be("PL11111111111111111111111111");
    }

    #endregion

    #region Invoice with Recipient (Odbiorca)

    [Fact]
    public void CreateInvoice_WithRecipient_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura z odbiorcą różnym od nabywcy
        // (np. oddział firmy, magazyn, punkt dostawy)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Hurtownia Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Sieć Sklepów S.A. - Centrala")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Centralna 100")
                    .WithAddressLine2("00-001 Warszawa")))
            .AddRecipient(r => r
                .WithInternalId("0987654321-001")
                .WithName("Sieć Sklepów S.A. - Magazyn Północ")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Magazynowa 50")
                    .WithAddressLine2("80-001 Gdańsk"))
                .AsRecipient())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/REC/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Dostawa towarów - Magazyn Północ")
                .WithNetAmount(25000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(5750.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients![0].Role.Should().Be(SubjectRole.Recipient);
        invoice.Recipients[0].InternalId.Should().Be("0987654321-001");
        invoice.Recipients[0].Name.Should().Contain("Magazyn Północ");
    }

    [Fact]
    public void CreateInvoice_WithMultipleRecipients_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura z wieloma odbiorcami (punktami dostawy)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Dystrybutor Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Sieć Marketów S.A."))
            // Odbiorca 1 - Sklep Warszawa
            .AddRecipient(r => r
                .WithInternalId("0987654321-WAW")
                .WithName("Sieć Marketów - Sklep Warszawa")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Warszawska 1")
                    .WithAddressLine2("00-001 Warszawa"))
                .AsRecipient())
            // Odbiorca 2 - Sklep Kraków
            .AddRecipient(r => r
                .WithInternalId("0987654321-KRK")
                .WithName("Sieć Marketów - Sklep Kraków")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Krakowska 2")
                    .WithAddressLine2("31-000 Kraków"))
                .AsRecipient())
            // Odbiorca 3 - Sklep Gdańsk
            .AddRecipient(r => r
                .WithInternalId("0987654321-GDA")
                .WithName("Sieć Marketów - Sklep Gdańsk")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Gdańska 3")
                    .WithAddressLine2("80-001 Gdańsk"))
                .AsRecipient())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/MULTI/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Dostawa zbiorcza do 3 lokalizacji")
                .WithNetAmount(75000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(17250.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients.Should().HaveCount(3);
        invoice.Recipients!.All(r => r.Role == SubjectRole.Recipient).Should().BeTrue();
    }

    #endregion

    #region Invoice with Additional Buyers (Dodatkowi nabywcy)

    [Fact]
    public void CreateInvoice_WithAdditionalBuyers_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura z dodatkowymi nabywcami (współwłaściciele)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Deweloper Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("1111111111")
                .WithName("Jan Kowalski - wspólnik 1")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Przykładowa 1")
                    .WithAddressLine2("00-001 Warszawa")))
            // Dodatkowy nabywca 1 - wspólnik 2
            .AddRecipient(r => r
                .WithTaxId("2222222222")
                .WithName("Anna Nowak - wspólnik 2")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Inna 2")
                    .WithAddressLine2("00-002 Warszawa"))
                .AsAdditionalBuyer(50.00m)) // 50% udziału
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/WSPOL/001/2024")
                .AddDescription("Udział nabywcy głównego", "50%")
                .AddDescription("Udział nabywcy dodatkowego", "50%"))
            .AddLineItem(l => l
                .WithProductName("Sprzedaż lokalu mieszkalnego - 50% udziału")
                .WithNetAmount(200000.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(16000.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients![0].Role.Should().Be(SubjectRole.AdditionalBuyer);
        invoice.Recipients[0].SharePercentage.Should().Be(50.00m);
    }

    [Fact]
    public void CreateInvoice_WithThreeCoOwners_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura na trzech współwłaścicieli
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("1111111111")
                .WithName("Nabywca 1 (33.33%)"))
            .AddRecipient(r => r
                .WithTaxId("2222222222")
                .WithName("Nabywca 2 (33.33%)")
                .AsAdditionalBuyer(33.33m))
            .AddRecipient(r => r
                .WithTaxId("3333333333")
                .WithName("Nabywca 3 (33.34%)")
                .AsAdditionalBuyer(33.34m))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/3WSPOL/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Nieruchomość - udział")
                .WithNetAmount(300000.00m)
                .WithVatRate(VatRate.Exempt)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.Recipients.Should().HaveCount(2);
        invoice.Recipients!.All(r => r.Role == SubjectRole.AdditionalBuyer).Should().BeTrue();
        var totalShare = invoice.Recipients.Sum(r => r.SharePercentage ?? 0) + 33.33m; // główny nabywca
        totalShare.Should().BeApproximately(100.00m, 0.01m);
    }

    #endregion

    #region Invoice with Payer (Płatnik)

    [Fact]
    public void CreateInvoice_WithPayer_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura gdzie płatnik jest różny od nabywcy
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Uczelnia Wyższa"))
            .WithBuyer(b => b
                .WithNoIdentifier()
                .WithName("Jan Kowalski - student")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Studencka 10")
                    .WithAddressLine2("00-001 Warszawa")))
            .AddRecipient(r => r
                .WithTaxId("9999999999")
                .WithName("Rodzice Jana Kowalskiego - płatnicy")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Rodzinna 5")
                    .WithAddressLine2("00-002 Warszawa"))
                .AsPayer())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/EDU/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Czesne za semestr zimowy 2024")
                .WithNetAmount(5000.00m)
                .WithVatRate(VatRate.Exempt)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients![0].Role.Should().Be(SubjectRole.Payer);
        invoice.Buyer.NoIdentifier.Should().Be(1);
    }

    #endregion

    #region Invoice with Original Entity (Podmiot pierwotny)

    [Fact]
    public void CreateInvoice_WithOriginalEntity_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura z podmiotem pierwotnym (po przejęciu/połączeniu)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Nowa Firma Sp. z o.o. (następca prawny)"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .AddRecipient(r => r
                .WithTaxId("8888888888")
                .WithName("Stara Firma Sp. z o.o. (podmiot przejęty)")
                .WithRole(SubjectRole.OriginalEntity)
                .WithRoleDescription("Podmiot przejęty w wyniku połączenia z dnia 01.01.2024"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/SUKCESJA/001/2024")
                .AddDescription("Informacja", "Faktura wystawiona przez następcę prawnego za zobowiązania podmiotu przejętego"))
            .AddLineItem(l => l
                .WithProductName("Usługi wykonane przez podmiot przejęty")
                .WithNetAmount(10000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(2300.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients![0].Role.Should().Be(SubjectRole.OriginalEntity);
        invoice.Recipients[0].RoleDescription.Should().Contain("połączenia");
    }

    #endregion

    #region Invoice with Invoice Issuer (Wystawca faktury)

    [Fact]
    public void CreateInvoice_WithInvoiceIssuer_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Faktura wystawiona przez biuro rachunkowe w imieniu klienta
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Klient Biura Rachunkowego Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca S.A."))
            .AddRecipient(r => r
                .WithTaxId("7777777777")
                .WithName("Biuro Rachunkowe ABC Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Księgowa 1")
                    .WithAddressLine2("00-001 Warszawa"))
                .WithRole(SubjectRole.InvoiceIssuer))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/BR/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługi")
                .WithNetAmount(5000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(1150.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients![0].Role.Should().Be(SubjectRole.InvoiceIssuer);
    }

    #endregion

    #region Complex Multi-Recipient Scenario

    [Fact]
    public void CreateInvoice_WithMultipleRoles_ShouldBuildValidInvoice()
    {
        // Arrange & Act - Kompleksowy scenariusz z wieloma rolami
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Producent Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Główny Nabywca S.A."))
            // Faktor
            .AddRecipient(r => r
                .WithTaxId("5555555555")
                .WithName("Bank Faktoring S.A.")
                .AsFactor())
            // Odbiorca towarów
            .AddRecipient(r => r
                .WithInternalId("0987654321-MAG1")
                .WithName("Magazyn Centralny")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Magazynowa 1")
                    .WithAddressLine2("00-001 Warszawa"))
                .AsRecipient())
            // Dodatkowy nabywca
            .AddRecipient(r => r
                .WithTaxId("6666666666")
                .WithName("Współnabywca Sp. z o.o.")
                .AsAdditionalBuyer(30.00m))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/COMPLEX/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Dostawa towarów")
                .WithNetAmount(100000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23000.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 2, 14)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .WithFactoringBankAccount(ba => ba
                    .WithAccountNumber("PL55555555555555555555555555")
                    .WithBankName("Bank Faktoring S.A.")))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients.Should().HaveCount(3);

        // Weryfikacja różnych ról
        invoice.Recipients!.Should().Contain(r => r.Role == SubjectRole.Factor);
        invoice.Recipients.Should().Contain(r => r.Role == SubjectRole.Recipient);
        invoice.Recipients.Should().Contain(r => r.Role == SubjectRole.AdditionalBuyer);
    }

    #endregion

    #region XML Serialization Tests

    [Fact]
    public void CreateInvoice_WithRecipients_ShouldSerializeToValidXml()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .AddRecipient(r => r
                .WithTaxId("5555555555")
                .WithName("Odbiorca")
                .AsRecipient())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Towar")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Act
        var xml = XmlSerializationHelper.Serialize(invoice);

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("Podmiot3");
        xml.Should().Contain("5555555555");
    }

    [Fact]
    public void CreateInvoice_WithRecipients_ShouldRoundTripSerialize()
    {
        // Arrange
        var originalInvoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Nabywca"))
            .AddRecipient(r => r
                .WithTaxId("5555555555")
                .WithName("Faktor")
                .AsFactor())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Towar")
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Act
        var deserializedInvoice = XmlSerializationHelper.RoundTrip(originalInvoice);

        // Assert
        deserializedInvoice.Should().NotBeNull();
        deserializedInvoice!.HasRecipients.Should().BeTrue();
        deserializedInvoice.Recipients![0].Role.Should().Be(SubjectRole.Factor);
    }

    #endregion
}
