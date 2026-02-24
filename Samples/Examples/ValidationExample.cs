using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Serialization;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Samples.Examples;

/// <summary>
/// Przyklad walidacji faktury
/// </summary>
public class ValidationExample
{
    /// <summary>
    /// Demonstracja roznych scenariuszy walidacji
    /// </summary>
    public static void Run()
    {
        // Konfiguracja DI
        var services = new ServiceCollection();
        services.AddKsefInvoiceServices(options =>
        {
            options.ValidateBeforeSerialize = true;
            options.ValidateAgainstXsd = true;
        });
        var serviceProvider = services.BuildServiceProvider();

        var invoiceService = serviceProvider.GetRequiredService<IKsefInvoiceService>();
        var xsdValidator = serviceProvider.GetRequiredService<IXsdValidator>();
        var businessValidator = serviceProvider.GetRequiredService<IInvoiceValidator>();

        Console.WriteLine("=== Przyklady walidacji faktury ===\n");

        // 1. Walidacja poprawnej faktury
        Console.WriteLine("1. Walidacja poprawnej faktury:");
        var validInvoice = CreateValidInvoice(invoiceService);
        var result1 = invoiceService.Validate(validInvoice);
        PrintValidationResult(result1);

        // 2. Walidacja faktury z brakujacymi danymi
        Console.WriteLine("\n2. Walidacja faktury z brakujacymi danymi:");
        var invalidInvoice = CreateInvalidInvoice(invoiceService);
        var result2 = invoiceService.Validate(invalidInvoice);
        PrintValidationResult(result2);

        // 3. Tylko walidacja biznesowa (bez XSD)
        Console.WriteLine("\n3. Tylko walidacja biznesowa (bez XSD):");
        var result3 = businessValidator.Validate(validInvoice);
        PrintValidationResult(result3);

        // 4. Tylko walidacja XSD
        Console.WriteLine("\n4. Tylko walidacja XSD:");
        var serializer = serviceProvider.GetRequiredService<IInvoiceSerializer>();
        var xml = serializer.SerializeToXml(validInvoice);
        var result4 = xsdValidator.ValidateXml(xml, SchemaVersion.FA3);
        PrintValidationResult(result4);

        // 5. Sprawdzenie dostepnych schematow
        Console.WriteLine("\n5. Dostepne schematy XSD:");
        Console.WriteLine($"   Schematy zaladowane: {xsdValidator.AreSchemasLoaded}");
        Console.WriteLine($"   Dostepne wersje: {string.Join(", ", xsdValidator.AvailableSchemas)}");

        // 6. Walidacja z obsluga bledow
        Console.WriteLine("\n6. Walidacja z obsluga bledow:");
        try
        {
            // Proba serializacji nieprawidlowej faktury
            var xmlOutput = invoiceService.ToXml(invalidInvoice);
            Console.WriteLine("   Serializacja pomyslna (nieoczekiwane!)");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("   Wyjatek walidacji (oczekiwane):");
            Console.WriteLine($"   {ex.Message.Substring(0, Math.Min(200, ex.Message.Length))}...");
        }
    }

    private static KSeF.Invoice.Models.Invoice CreateValidInvoice(IKsefInvoiceService service)
    {
        return service.CreateInvoice()
            .WithSeller(seller => seller
                .WithTaxId("1234567890")
                .WithName("Firma ABC Sp. z o.o.")
                .WithAddress(address => address
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Testowa 1")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(buyer => buyer
                .WithTaxId("0987654321")
                .WithName("Klient XYZ S.A.")
                .WithAddress(address => address
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Kliencka 99")
                    .WithAddressLine2("30-001 Krakow")))
            .WithInvoiceDetails(details => details
                .WithInvoiceNumber("FV/2024/TEST/001")
                .WithIssueDate(2024, 1, 15)
                .WithSaleDate(2024, 1, 15)
                .WithCurrency(CurrencyCode.PLN))
            .AddLineItem(item => item
                .WithProductName("Usluga testowa")
                .WithUnit("szt.")
                .WithQuantity(1)
                .WithUnitNetPrice(100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithNetAmount(100.00m)
                .WithVatAmount(23.00m))
            .Build();
    }

    private static KSeF.Invoice.Models.Invoice CreateInvalidInvoice(IKsefInvoiceService service)
    {
        // Faktura z celowymi bledami
        return service.CreateInvoice()
            .WithSeller(seller => seller
                .WithTaxId("123")              // Nieprawidlowy NIP (za krotki)
                .WithName("")                   // Brak nazwy
                .WithAddress(address => address
                    .WithCountryCode("PL")
                    .WithAddressLine1("")       // Brak adresu
                    .WithAddressLine2("")))
            .WithBuyer(buyer => buyer
                .WithTaxId("ABC")              // Nieprawidlowy NIP (litery)
                .WithName("Klient")
                .WithAddress(address => address
                    .WithCountryCode("XX")     // Nieprawidlowy kod kraju
                    .WithAddressLine1("ul. Test 1")
                    .WithAddressLine2("00-001 Miasto")))
            .WithInvoiceDetails(details => details
                .WithInvoiceNumber("")          // Brak numeru faktury
                .WithIssueDate(2024, 1, 15)
                .WithCurrency(CurrencyCode.PLN))
            // Brak pozycji faktury
            .Build();
    }

    private static void PrintValidationResult(ValidationResult result)
    {
        if (result.IsValid)
        {
            Console.WriteLine("   Status: POPRAWNA");
        }
        else
        {
            Console.WriteLine($"   Status: BLEDY ({result.Errors.Count})");
            foreach (var error in result.Errors.Take(5))
            {
                var fieldInfo = error.FieldName != null ? $" [{error.FieldName}]" : "";
                Console.WriteLine($"     - {error.Code}{fieldInfo}: {error.Message}");
            }
            if (result.Errors.Count > 5)
            {
                Console.WriteLine($"     ... i {result.Errors.Count - 5} wiecej bledow");
            }
        }

        if (result.HasWarnings)
        {
            Console.WriteLine($"   Ostrzezenia ({result.Warnings.Count}):");
            foreach (var warning in result.Warnings.Take(3))
            {
                Console.WriteLine($"     - {warning.Code}: {warning.Message}");
            }
        }
    }
}
