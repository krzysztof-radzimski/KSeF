using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Serialization;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Samples.Examples;

/// <summary>
/// Przyklad integracji z Dependency Injection
/// </summary>
public class DependencyInjectionExample
{
    /// <summary>
    /// Demonstracja roznych sposobow konfiguracji DI
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("=== Przyklady integracji z Dependency Injection ===\n");

        // 1. Podstawowa konfiguracja
        Console.WriteLine("1. Podstawowa konfiguracja (domyslne opcje):");
        BasicConfiguration();

        // 2. Konfiguracja z opcjami
        Console.WriteLine("\n2. Konfiguracja z opcjami:");
        ConfigurationWithOptions();

        // 3. Uzywanie pojedynczych serwisow
        Console.WriteLine("\n3. Uzywanie pojedynczych serwisow:");
        UsingSingleServices();

        // 4. Przyklad w klasie serwisowej
        Console.WriteLine("\n4. Uzycie w klasie serwisowej:");
        UsingInServiceClass();
    }

    private static void BasicConfiguration()
    {
        // Najprostszy sposob - dodanie wszystkich serwisow z domyslnymi opcjami
        var services = new ServiceCollection();
        services.AddKsefInvoiceServices();

        var provider = services.BuildServiceProvider();
        var invoiceService = provider.GetRequiredService<IKsefInvoiceService>();

        Console.WriteLine($"   IKsefInvoiceService: {invoiceService.GetType().Name}");
    }

    private static void ConfigurationWithOptions()
    {
        var services = new ServiceCollection();

        // Konfiguracja z niestandardowymi opcjami
        services.AddKsefInvoiceServices(options =>
        {
            // Wersja schematu XSD
            options.SchemaVersion = SchemaVersion.FA3;

            // Walidacja przed serializacja
            options.ValidateBeforeSerialize = true;

            // Walidacja XSD
            options.ValidateAgainstXsd = true;

            // Domyslna informacja o systemie
            options.DefaultSystemInfo = "MojSystem ERP v2.0";
        });

        var provider = services.BuildServiceProvider();
        var invoiceService = provider.GetRequiredService<IKsefInvoiceService>();

        // Utworzona faktura bedzie miala ustawiona informacje o systemie
        var invoice = invoiceService.CreateInvoice()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Test"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Klient"))
            .WithInvoiceDetails(d => d
                .WithInvoiceNumber("TEST/001")
                .WithIssueDate(2024, 1, 1)
                .WithCurrency(CurrencyCode.PLN))
            .AddLineItem(i => i
                .WithProductName("Test")
                .WithQuantity(1)
                .WithVatRate(VatRate.Rate23)
                .WithNetAmount(100))
            .Build();

        Console.WriteLine($"   SystemInfo: {invoice.Header.SystemInfo}");
    }

    private static void UsingSingleServices()
    {
        var services = new ServiceCollection();
        services.AddKsefInvoiceServices();
        var provider = services.BuildServiceProvider();

        // Mozna pobrac pojedyncze serwisy
        var serializer = provider.GetRequiredService<IInvoiceSerializer>();
        var businessValidator = provider.GetRequiredService<IInvoiceValidator>();
        var xsdValidator = provider.GetRequiredService<IXsdValidator>();

        Console.WriteLine($"   IInvoiceSerializer: {serializer.GetType().Name}");
        Console.WriteLine($"   IInvoiceValidator: {businessValidator.GetType().Name}");
        Console.WriteLine($"   IXsdValidator: {xsdValidator.GetType().Name}");

        // Oraz walidatory pomocnicze
        var nipValidator = provider.GetRequiredService<INipValidator>();
        var ibanValidator = provider.GetRequiredService<IIbanValidator>();
        var dateValidator = provider.GetRequiredService<IDateValidator>();

        Console.WriteLine($"   INipValidator: {nipValidator.GetType().Name}");
        Console.WriteLine($"   IIbanValidator: {ibanValidator.GetType().Name}");
        Console.WriteLine($"   IDateValidator: {dateValidator.GetType().Name}");
    }

    private static void UsingInServiceClass()
    {
        var services = new ServiceCollection();
        services.AddKsefInvoiceServices();

        // Rejestracja wlasnego serwisu
        services.AddScoped<InvoiceGeneratorService>();

        var provider = services.BuildServiceProvider();
        var generator = provider.GetRequiredService<InvoiceGeneratorService>();

        var result = generator.GenerateMonthlyInvoice("1234567890", "Firma ABC", 5000.00m);
        Console.WriteLine($"   Wygenerowano fakture: {result.InvoiceNumber}");
        Console.WriteLine($"   Kwota: {result.TotalAmount:N2} PLN");
        Console.WriteLine($"   Walidacja: {(result.IsValid ? "OK" : "BLEDY")}");
    }
}

/// <summary>
/// Przykladowy serwis wykorzystujacy IKsefInvoiceService
/// </summary>
public class InvoiceGeneratorService
{
    private readonly IKsefInvoiceService _invoiceService;
    private int _invoiceCounter = 0;

    public InvoiceGeneratorService(IKsefInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public InvoiceGenerationResult GenerateMonthlyInvoice(string buyerNip, string buyerName, decimal amount)
    {
        _invoiceCounter++;

        var invoice = _invoiceService.CreateInvoice()
            .WithSeller(seller => seller
                .WithTaxId("9999999999")
                .WithName("Moja Firma Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Glowna 1")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(buyer => buyer
                .WithTaxId(buyerNip)
                .WithName(buyerName)
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Klienta 1")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithInvoiceDetails(details => details
                .WithInvoiceNumber($"FV/{DateTime.Now:yyyy/MM}/{_invoiceCounter:D3}")
                .WithIssueDate(DateOnly.FromDateTime(DateTime.Now))
                .WithSaleDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
                .WithCurrency(CurrencyCode.PLN))
            .AddLineItem(item => item
                .WithProductName("Usluga abonamentowa - miesiac")
                .WithUnit("szt.")
                .WithQuantity(1)
                .WithUnitNetPrice(amount)
                .WithVatRate(VatRate.Rate23)
                .WithNetAmount(amount)
                .WithVatAmount(Math.Round(amount * 0.23m, 2)))
            .WithPayment(payment => payment
                .AddPaymentTerm(DateOnly.FromDateTime(DateTime.Now.AddDays(14)))
                .AsBankTransfer("PL61109010140000071219812874"))
            .Build();

        var validation = _invoiceService.Validate(invoice);

        return new InvoiceGenerationResult
        {
            InvoiceNumber = invoice.InvoiceData.InvoiceNumber ?? "",
            TotalAmount = invoice.InvoiceData.TotalAmount,
            IsValid = validation.IsValid,
            Invoice = invoice
        };
    }
}

/// <summary>
/// Wynik generowania faktury
/// </summary>
public class InvoiceGenerationResult
{
    public string InvoiceNumber { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public bool IsValid { get; set; }
    public KSeF.Invoice.Models.Invoice? Invoice { get; set; }
}
