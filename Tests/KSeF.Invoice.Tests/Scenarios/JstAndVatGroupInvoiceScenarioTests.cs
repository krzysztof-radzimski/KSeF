using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Tests.Helpers;
using Xunit;

namespace KSeF.Invoice.Tests.Scenarios;

/// <summary>
/// Testy scenariuszy dla faktur z jednostkami samorządu terytorialnego (JST)
/// oraz grupami VAT
/// </summary>
public class JstAndVatGroupInvoiceScenarioTests
{
    #region Local Government Unit (JST) - Buyer

    [Fact]
    public void CreateInvoice_WithJstAsBuyer_ShouldSetJstFlag()
    {
        // Arrange & Act - Faktura dla jednostki samorządu terytorialnego jako nabywcy
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Dostawca Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Handlowa 10")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(b => b
                .WithTaxId("5252248481") // NIP Gminy
                .WithName("Gmina Miasto Warszawa")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("Plac Bankowy 3/5")
                    .WithAddressLine2("00-950 Warszawa"))
                .AsLocalGovernmentUnit())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/JST/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Dostawa sprzętu komputerowego dla Urzędu")
                .WithQuantity(10)
                .WithUnit("szt.")
                .WithUnitNetPrice(3000.00m)
                .WithNetAmount(30000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(6900.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 2, 14)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL61109010140000071219812874"))
            .Build();

        // Assert
        invoice.Buyer.IsLocalGovernmentUnit.Should().Be(1);
        invoice.Buyer.TaxId.Should().Be("5252248481");
        invoice.Buyer.Name.Should().Contain("Gmina");
        invoice.InvoiceData.TotalAmount.Should().Be(36900.00m);
    }

    [Fact]
    public void CreateInvoice_WithJstAsIssuer_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Faktura wystawiona przez jednostkę JST z określeniem
        // konkretnej jednostki wewnętrznej jako wystawcy
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("5252248481")
                .WithName("Gmina Miasto Warszawa")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("Plac Bankowy 3/5")
                    .WithAddressLine2("00-950 Warszawa")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Firma Prywatna Sp. z o.o."))
            // Określenie jednostki wewnętrznej JST jako wystawcy
            .AddRecipient(r => r
                .WithInternalId("5252248481-ZDM")
                .WithName("Zarząd Dróg Miejskich m.st. Warszawy")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Chmielna 120")
                    .WithAddressLine2("00-801 Warszawa"))
                .WithRole(SubjectRole.LocalGovernmentIssuer))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/ZDM/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Opłata za zajęcie pasa drogowego")
                .WithNetAmount(5000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(1150.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients![0].Role.Should().Be(SubjectRole.LocalGovernmentIssuer);
        invoice.Recipients[0].InternalId.Should().Be("5252248481-ZDM");
    }

    [Fact]
    public void CreateInvoice_WithJstAsRecipient_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Faktura z jednostką wewnętrzną JST jako odbiorcą
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Dostawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("5252248481")
                .WithName("Gmina Miasto Warszawa")
                .AsLocalGovernmentUnit())
            // Określenie jednostki wewnętrznej jako odbiorcy
            .AddRecipient(r => r
                .WithInternalId("5252248481-SP123")
                .WithName("Szkoła Podstawowa nr 123 w Warszawie")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Szkolna 15")
                    .WithAddressLine2("00-123 Warszawa"))
                .WithRole(SubjectRole.LocalGovernmentRecipient))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/SP/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Dostawa artykułów papierniczych dla szkoły")
                .WithNetAmount(2000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(460.00m))
            .Build();

        // Assert
        invoice.Buyer.IsLocalGovernmentUnit.Should().Be(1);
        invoice.Recipients![0].Role.Should().Be(SubjectRole.LocalGovernmentRecipient);
    }

    [Fact]
    public void CreateInvoice_WithJstBothIssuerAndRecipient_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Transakcja wewnątrz JST (między jednostkami wewnętrznymi)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("5252248481")
                .WithName("Gmina Miasto Warszawa"))
            .WithBuyer(b => b
                .WithTaxId("5252248481")
                .WithName("Gmina Miasto Warszawa")
                .AsLocalGovernmentUnit())
            // Wystawca - Zakład Gospodarowania Nieruchomościami
            .AddRecipient(r => r
                .WithInternalId("5252248481-ZGN")
                .WithName("Zakład Gospodarowania Nieruchomościami Dzielnicy Mokotów")
                .WithRole(SubjectRole.LocalGovernmentIssuer))
            // Odbiorca - Ośrodek Pomocy Społecznej
            .AddRecipient(r => r
                .WithInternalId("5252248481-OPS")
                .WithName("Ośrodek Pomocy Społecznej Dzielnicy Mokotów")
                .WithRole(SubjectRole.LocalGovernmentRecipient))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/WEW/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługi administracji nieruchomością")
                .WithNetAmount(10000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(2300.00m))
            .Build();

        // Assert
        invoice.Recipients.Should().HaveCount(2);
        invoice.Recipients!.Should().Contain(r => r.Role == SubjectRole.LocalGovernmentIssuer);
        invoice.Recipients.Should().Contain(r => r.Role == SubjectRole.LocalGovernmentRecipient);
    }

    #endregion

    #region VAT Group - Buyer

    [Fact]
    public void CreateInvoice_WithVatGroupAsBuyer_ShouldSetVatGroupFlag()
    {
        // Arrange & Act - Faktura dla grupy VAT jako nabywcy
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Dostawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("9999999999") // NIP Grupy VAT
                .WithName("Grupa VAT Holding XYZ")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Korporacyjna 100")
                    .WithAddressLine2("00-001 Warszawa"))
                .AsVatGroup())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/GV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługi doradcze")
                .WithNetAmount(50000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(11500.00m))
            .Build();

        // Assert
        invoice.Buyer.IsVatGroup.Should().Be(1);
        invoice.Buyer.TaxId.Should().Be("9999999999");
        invoice.Buyer.Name.Should().Contain("Grupa VAT");
    }

    [Fact]
    public void CreateInvoice_WithVatGroupMemberAsIssuer_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Faktura wystawiona przez członka grupy VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("9999999999")
                .WithName("Grupa VAT Holding ABC"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Nabywca Sp. z o.o."))
            // Członek grupy VAT jako wystawca
            .AddRecipient(r => r
                .WithInternalId("9999999999-SPZOO1")
                .WithName("ABC Produkcja Sp. z o.o. - członek Grupy VAT")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Fabryczna 50")
                    .WithAddressLine2("41-200 Sosnowiec"))
                .WithRole(SubjectRole.VatGroupMemberIssuer))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/GVMI/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Sprzedaż produktów")
                .WithNetAmount(75000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(17250.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients![0].Role.Should().Be(SubjectRole.VatGroupMemberIssuer);
    }

    [Fact]
    public void CreateInvoice_WithVatGroupMemberAsRecipient_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Faktura z członkiem grupy VAT jako odbiorcą
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Dostawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithTaxId("9999999999")
                .WithName("Grupa VAT Holding ABC")
                .AsVatGroup())
            // Członek grupy VAT jako odbiorca
            .AddRecipient(r => r
                .WithInternalId("9999999999-DYSTR")
                .WithName("ABC Dystrybucja Sp. z o.o. - członek Grupy VAT")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Magazynowa 30")
                    .WithAddressLine2("40-200 Katowice"))
                .WithRole(SubjectRole.VatGroupMemberRecipient))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/GVMR/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Dostawa towarów do dystrybucji")
                .WithNetAmount(100000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23000.00m))
            .Build();

        // Assert
        invoice.Buyer.IsVatGroup.Should().Be(1);
        invoice.Recipients![0].Role.Should().Be(SubjectRole.VatGroupMemberRecipient);
    }

    [Fact]
    public void CreateInvoice_WithVatGroupInternalTransaction_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Transakcja wewnątrz grupy VAT (między członkami)
        // Uwaga: Transakcje wewnątrz grupy VAT są neutralne podatkowo
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("9999999999")
                .WithName("Grupa VAT Holding XYZ"))
            .WithBuyer(b => b
                .WithTaxId("9999999999")
                .WithName("Grupa VAT Holding XYZ")
                .AsVatGroup())
            // Członek wystawiający
            .AddRecipient(r => r
                .WithInternalId("9999999999-PROD")
                .WithName("XYZ Produkcja Sp. z o.o.")
                .WithRole(SubjectRole.VatGroupMemberIssuer))
            // Członek odbierający
            .AddRecipient(r => r
                .WithInternalId("9999999999-HANDEL")
                .WithName("XYZ Handel Sp. z o.o.")
                .WithRole(SubjectRole.VatGroupMemberRecipient))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/GVWEW/001/2024")
                .AddDescription("Uwaga", "Transakcja wewnątrz grupy VAT - neutralna podatkowo"))
            .AddLineItem(l => l
                .WithProductName("Przekazanie wyrobów do sprzedaży")
                .WithNetAmount(200000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(46000.00m))
            .Build();

        // Assert
        invoice.Recipients.Should().HaveCount(2);
        invoice.Recipients!.Should().Contain(r => r.Role == SubjectRole.VatGroupMemberIssuer);
        invoice.Recipients.Should().Contain(r => r.Role == SubjectRole.VatGroupMemberRecipient);
    }

    #endregion

    #region Combined Scenarios

    [Fact]
    public void CreateInvoice_FromJstToVatGroup_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Faktura z JST do grupy VAT
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("5252248481")
                .WithName("Gmina Miasto Warszawa"))
            .WithBuyer(b => b
                .WithTaxId("9999999999")
                .WithName("Grupa VAT Deweloper Holding")
                .AsVatGroup())
            .AddRecipient(r => r
                .WithInternalId("5252248481-ZGN")
                .WithName("Zakład Gospodarowania Nieruchomościami")
                .WithRole(SubjectRole.LocalGovernmentIssuer))
            .AddRecipient(r => r
                .WithInternalId("9999999999-DEV1")
                .WithName("Developer 1 Sp. z o.o. - członek Grupy VAT")
                .WithRole(SubjectRole.VatGroupMemberRecipient))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/JST-GV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Sprzedaż działki gruntowej")
                .WithNetAmount(1000000.00m)
                .WithVatRate(VatRate.Exempt)
                .WithVatAmount(0.00m))
            .Build();

        // Assert
        invoice.Buyer.IsVatGroup.Should().Be(1);
        invoice.Recipients.Should().HaveCount(2);
        invoice.Recipients!.Should().Contain(r => r.Role == SubjectRole.LocalGovernmentIssuer);
        invoice.Recipients.Should().Contain(r => r.Role == SubjectRole.VatGroupMemberRecipient);
    }

    [Fact]
    public void CreateInvoice_FromVatGroupToJst_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Faktura z grupy VAT do JST
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("9999999999")
                .WithName("Grupa VAT IT Services"))
            .WithBuyer(b => b
                .WithTaxId("5252248481")
                .WithName("Gmina Miasto Warszawa")
                .AsLocalGovernmentUnit())
            .AddRecipient(r => r
                .WithInternalId("9999999999-SOFT")
                .WithName("IT Software Sp. z o.o. - członek Grupy VAT")
                .WithRole(SubjectRole.VatGroupMemberIssuer))
            .AddRecipient(r => r
                .WithInternalId("5252248481-UM")
                .WithName("Urząd Miasta Stołecznego Warszawy")
                .WithRole(SubjectRole.LocalGovernmentRecipient))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/GV-JST/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Wdrożenie systemu informatycznego")
                .WithNetAmount(500000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(115000.00m))
            .Build();

        // Assert
        invoice.Buyer.IsLocalGovernmentUnit.Should().Be(1);
        invoice.Recipients.Should().HaveCount(2);
        invoice.Recipients!.Should().Contain(r => r.Role == SubjectRole.VatGroupMemberIssuer);
        invoice.Recipients.Should().Contain(r => r.Role == SubjectRole.LocalGovernmentRecipient);
    }

    #endregion

    #region Employee Scenario

    [Fact]
    public void CreateInvoice_WithEmployee_ShouldConfigureCorrectly()
    {
        // Arrange & Act - Faktura z pracownikiem (np. refakturowanie kosztów)
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Pracodawca Sp. z o.o."))
            .WithBuyer(b => b
                .WithNoIdentifier()
                .WithName("Jan Kowalski"))
            .AddRecipient(r => r
                .WithNoIdentifier()
                .WithName("Jan Kowalski")
                .WithRole(SubjectRole.Employee)
                .WithRoleDescription("Pracownik - refakturowanie kosztów delegacji"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/PRAC/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Refakturowanie kosztów noclegu - delegacja służbowa")
                .WithNetAmount(400.00m)
                .WithVatRate(VatRate.Rate8)
                .WithVatAmount(32.00m))
            .Build();

        // Assert
        invoice.HasRecipients.Should().BeTrue();
        invoice.Recipients![0].Role.Should().Be(SubjectRole.Employee);
        invoice.Recipients[0].RoleDescription.Should().Contain("Pracownik");
    }

    #endregion

    #region XML Serialization Tests

    [Fact]
    public void CreateInvoice_WithJst_ShouldSerializeToValidXml()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("5252248481")
                .WithName("Gmina Warszawa")
                .AsLocalGovernmentUnit())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/JST/001/2024"))
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
        xml.Should().Contain("Gmina");
    }

    [Fact]
    public void CreateInvoice_WithVatGroup_ShouldSerializeToValidXml()
    {
        // Arrange
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("9999999999")
                .WithName("Grupa VAT XYZ")
                .AsVatGroup())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/GV/001/2024"))
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
        xml.Should().Contain("Grupa VAT");
    }

    [Fact]
    public void CreateInvoice_WithJst_ShouldRoundTripSerialize()
    {
        // Arrange
        var originalInvoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("5252248481")
                .WithName("Gmina")
                .AsLocalGovernmentUnit())
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
        deserializedInvoice!.Buyer.IsLocalGovernmentUnit.Should().Be(1);
    }

    [Fact]
    public void CreateInvoice_WithVatGroup_ShouldRoundTripSerialize()
    {
        // Arrange
        var originalInvoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Sprzedawca"))
            .WithBuyer(b => b
                .WithTaxId("9999999999")
                .WithName("Grupa VAT")
                .AsVatGroup())
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
        deserializedInvoice!.Buyer.IsVatGroup.Should().Be(1);
    }

    #endregion
}
