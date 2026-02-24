// Przyklady uzycia biblioteki KSeF.Invoice
// Uruchom z: dotnet run

using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== KSeF.Invoice - Przyklady uzycia ===\n");

// 1. Konfiguracja Dependency Injection
// UWAGA: W przykladach uzywamy fikcyjnych numerow NIP i IBAN,
// dlatego wylaczamy walidacje przed serializacja.
// W produkcji nalezy ustawic ValidateBeforeSerialize = true
var services = new ServiceCollection();
services.AddKsefInvoiceServices(options =>
{
    options.SchemaVersion = SchemaVersion.FA3;
    options.ValidateBeforeSerialize = false;  // Wylaczone dla przykladowych danych
    options.ValidateAgainstXsd = false;       // Wylaczone dla przykladowych danych
    options.DefaultSystemInfo = "KSeF.Invoice.Samples 1.0";
});

var serviceProvider = services.BuildServiceProvider();
var invoiceService = serviceProvider.GetRequiredService<IKsefInvoiceService>();

// 2. Przyklad: Prosta faktura VAT
Console.WriteLine("1. Tworzenie prostej faktury VAT...");
var simpleInvoice = CreateSimpleInvoice(invoiceService);
ValidateAndSerialize(invoiceService, simpleInvoice, "Prosta faktura VAT");

// 3. Przyklad: Faktura z wieloma pozycjami
Console.WriteLine("\n2. Tworzenie faktury z wieloma pozycjami...");
var multiItemInvoice = CreateMultiItemInvoice(invoiceService);
ValidateAndSerialize(invoiceService, multiItemInvoice, "Faktura z wieloma pozycjami");

// 4. Przyklad: Faktura korygujaca
Console.WriteLine("\n3. Tworzenie faktury korygujacej...");
var correctionInvoice = CreateCorrectionInvoice(invoiceService);
ValidateAndSerialize(invoiceService, correctionInvoice, "Faktura korygujaca");

// 5. Przyklad: Faktura zaliczkowa
Console.WriteLine("\n4. Tworzenie faktury zaliczkowej...");
var advanceInvoice = CreateAdvanceInvoice(invoiceService);
ValidateAndSerialize(invoiceService, advanceInvoice, "Faktura zaliczkowa");

// 6. Przyklad: Deserializacja faktury z XML
Console.WriteLine("\n5. Deserializacja faktury z XML...");
var xml = invoiceService.ToXml(simpleInvoice);
var deserializedInvoice = invoiceService.FromXml(xml);
if (deserializedInvoice != null)
{
    Console.WriteLine($"   Deserializacja pomyslna!");
    Console.WriteLine($"   Numer faktury: {deserializedInvoice.InvoiceData.InvoiceNumber}");
    Console.WriteLine($"   Sprzedawca: {deserializedInvoice.Seller.Name}");
    Console.WriteLine($"   Nabywca: {deserializedInvoice.Buyer.Name}");
}

Console.WriteLine("\n=== Koniec przykladow ===");


// --- Funkcje pomocnicze ---

static KSeF.Invoice.Models.Invoice CreateSimpleInvoice(IKsefInvoiceService service)
{
    return service.CreateInvoice()
        .WithSeller(seller => seller
            .WithTaxId("1234567890")
            .WithName("Firma ABC Sp. z o.o.")
            .WithAddress(address => address
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Przykladowa 1")
                .WithAddressLine2("00-001 Warszawa")))
        .WithBuyer(buyer => buyer
            .WithTaxId("0987654321")
            .WithName("Klient XYZ S.A.")
            .WithAddress(address => address
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Kliencka 99")
                .WithAddressLine2("30-001 Krakow")))
        .WithInvoiceDetails(details => details
            .WithInvoiceNumber("FV/2024/001")
            .WithIssueDate(2024, 1, 15)
            .WithSaleDate(2024, 1, 15)
            .WithCurrency(CurrencyCode.PLN))
        .AddLineItem(item => item
            .WithProductName("Usluga konsultingowa")
            .WithUnit("szt.")
            .WithQuantity(1)
            .WithUnitNetPrice(1000.00m)
            .WithVatRate(VatRate.Rate23)
            .WithNetAmount(1000.00m)
            .WithVatAmount(230.00m))
        .WithPayment(payment => payment
            .AddPaymentTerm(2024, 1, 30)
            .AsBankTransfer("PL61109010140000071219812874"))
        .Build();
}

static KSeF.Invoice.Models.Invoice CreateMultiItemInvoice(IKsefInvoiceService service)
{
    return service.CreateInvoice()
        .WithSeller(seller => seller
            .WithTaxId("1234567890")
            .WithName("Sklep Komputerowy ABC")
            .WithAddress(address => address
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Techniczna 15")
                .WithAddressLine2("02-677 Warszawa"))
            .WithContactData("biuro@abc.pl", "+48 22 123 45 67"))
        .WithBuyer(buyer => buyer
            .WithTaxId("9876543210")
            .WithName("Firma IT Solutions")
            .WithAddress(address => address
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Biznesowa 42")
                .WithAddressLine2("31-564 Krakow")))
        .WithInvoiceDetails(details => details
            .WithInvoiceNumber("FV/2024/002")
            .WithIssueDate(2024, 1, 20)
            .WithSaleDate(2024, 1, 20)
            .WithCurrency(CurrencyCode.PLN))
        .AddLineItem(item => item
            .WithProductName("Laptop Dell XPS 15")
            .WithUnit("szt.")
            .WithQuantity(2)
            .WithUnitNetPrice(5000.00m)
            .WithVatRate(VatRate.Rate23)
            .WithNetAmount(10000.00m)
            .WithVatAmount(2300.00m)
            .WithGtinCode("5901234123457"))
        .AddLineItem(item => item
            .WithProductName("Monitor 27\" 4K")
            .WithUnit("szt.")
            .WithQuantity(2)
            .WithUnitNetPrice(1500.00m)
            .WithVatRate(VatRate.Rate23)
            .WithNetAmount(3000.00m)
            .WithVatAmount(690.00m))
        .AddLineItem(item => item
            .WithProductName("Klawiatura mechaniczna")
            .WithUnit("szt.")
            .WithQuantity(4)
            .WithUnitNetPrice(300.00m)
            .WithVatRate(VatRate.Rate23)
            .WithNetAmount(1200.00m)
            .WithVatAmount(276.00m))
        .AddLineItem(item => item
            .WithProductName("Usluga konfiguracji")
            .WithUnit("h")
            .WithQuantity(8)
            .WithUnitNetPrice(150.00m)
            .WithVatRate(VatRate.Rate23)
            .WithNetAmount(1200.00m)
            .WithVatAmount(276.00m)
            .WithPkwiuCode("62.02.30.0"))
        .WithPayment(payment => payment
            .AddPaymentTerm(2024, 2, 20)
            .AsBankTransfer("PL61109010140000071219812874", null, "PKO BP"))
        .Build();
}

static KSeF.Invoice.Models.Invoice CreateCorrectionInvoice(IKsefInvoiceService service)
{
    return service.CreateInvoice()
        .WithSeller(seller => seller
            .WithTaxId("1234567890")
            .WithName("Firma ABC Sp. z o.o.")
            .WithAddress(address => address
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Przykladowa 1")
                .WithAddressLine2("00-001 Warszawa")))
        .WithBuyer(buyer => buyer
            .WithTaxId("0987654321")
            .WithName("Klient XYZ S.A.")
            .WithAddress(address => address
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Kliencka 99")
                .WithAddressLine2("30-001 Krakow")))
        .WithInvoiceDetails(details => details
            .WithInvoiceNumber("FV/2024/001/KOR")
            .WithIssueDate(2024, 2, 1)
            .WithSaleDate(2024, 1, 15)
            .WithCurrency(CurrencyCode.PLN)
            .AsCorrection("Blad w cenie jednostkowej", corrected => corrected
                .WithInvoiceNumber("FV/2024/001")
                .WithIssueDate(new DateOnly(2024, 1, 15))))
        .AddLineItem(item => item
            .WithProductName("Usluga konsultingowa - korekta")
            .WithUnit("szt.")
            .WithQuantity(1)
            .WithUnitNetPrice(-100.00m)  // Zmniejszenie o 100 PLN
            .WithVatRate(VatRate.Rate23)
            .WithNetAmount(-100.00m)
            .WithVatAmount(-23.00m))
        .Build();
}

static KSeF.Invoice.Models.Invoice CreateAdvanceInvoice(IKsefInvoiceService service)
{
    return service.CreateInvoice()
        .WithSeller(seller => seller
            .WithTaxId("1234567890")
            .WithName("Firma Budowlana ABC")
            .WithAddress(address => address
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Budowlana 10")
                .WithAddressLine2("00-001 Warszawa")))
        .WithBuyer(buyer => buyer
            .WithTaxId("5555555555")
            .WithName("Inwestor DEF")
            .WithAddress(address => address
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Inwestorska 50")
                .WithAddressLine2("02-222 Warszawa")))
        .WithInvoiceDetails(details => details
            .WithInvoiceNumber("FV/2024/ZAL/001")
            .WithIssueDate(2024, 1, 10)
            .WithCurrency(CurrencyCode.PLN)
            .AsAdvancePayment()
            .AddDescription("Zamowienie", "ZAM/2024/001")
            .AddDescription("Opis", "Zaliczka na prace budowlane - etap I"))
        .AddLineItem(item => item
            .WithProductName("Zaliczka na prace budowlane - etap I")
            .WithUnit("szt.")
            .WithQuantity(1)
            .WithUnitNetPrice(50000.00m)
            .WithVatRate(VatRate.Rate23)
            .WithNetAmount(50000.00m)
            .WithVatAmount(11500.00m))
        .WithPayment(payment => payment
            .AddPaymentTerm(2024, 1, 20)
            .AsBankTransfer("PL27114020040000320218427362"))
        .Build();
}

static void ValidateAndSerialize(IKsefInvoiceService service, KSeF.Invoice.Models.Invoice invoice, string name)
{
    Console.WriteLine($"   Walidacja faktury '{name}'...");

    var validationResult = service.Validate(invoice);

    if (validationResult.IsValid)
    {
        Console.WriteLine("   Faktura jest poprawna!");

        try
        {
            var xml = service.ToXml(invoice);
            Console.WriteLine($"   Serializacja XML pomyslna ({xml.Length} znakow)");

            // Zapisz do pliku (opcjonalnie)
            var fileName = $"{invoice.InvoiceData.InvoiceNumber?.Replace("/", "_") ?? "faktura"}.xml";
            File.WriteAllText(fileName, xml);
            Console.WriteLine($"   Zapisano do pliku: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Blad serializacji: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine($"   Faktura zawiera bledy ({validationResult.Errors.Count}):");
        foreach (var error in validationResult.Errors.Take(5))
        {
            Console.WriteLine($"     - [{error.Code}] {error.Message}");
        }
        if (validationResult.Errors.Count > 5)
        {
            Console.WriteLine($"     ... i {validationResult.Errors.Count - 5} wiecej bledow");
        }
    }

    if (validationResult.HasWarnings)
    {
        Console.WriteLine($"   Ostrzezenia ({validationResult.Warnings.Count}):");
        foreach (var warning in validationResult.Warnings.Take(3))
        {
            Console.WriteLine($"     - [{warning.Code}] {warning.Message}");
        }
    }
}
