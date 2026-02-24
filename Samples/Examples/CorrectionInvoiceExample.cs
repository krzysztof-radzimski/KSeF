using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Samples.Examples;

/// <summary>
/// Przyklad tworzenia faktury korygujacej
/// </summary>
public class CorrectionInvoiceExample
{
    /// <summary>
    /// Demonstracja tworzenia faktury korygujacej
    /// </summary>
    public static void Run()
    {
        // Konfiguracja DI
        var services = new ServiceCollection();
        services.AddKsefInvoiceServices();
        var serviceProvider = services.BuildServiceProvider();
        var invoiceService = serviceProvider.GetRequiredService<IKsefInvoiceService>();

        // Faktura korygujaca - zmniejszenie wartosci
        var correctionInvoice = invoiceService.CreateInvoice()
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
                .WithInvoiceNumber("FV/2024/001/KOR")  // Numer korekty
                .WithIssueDate(2024, 2, 1)             // Data wystawienia korekty
                .WithSaleDate(2024, 1, 15)             // Data sprzedazy z faktury oryginalnej
                .WithCurrency(CurrencyCode.PLN)
                // Oznaczenie jako faktura korygujaca
                .AsCorrection("Blad w cenie jednostkowej - udzielony rabat", corrected => corrected
                    .WithInvoiceNumber("FV/2024/001")               // Numer faktury korygowanej
                    .WithIssueDate(new DateOnly(2024, 1, 15))       // Data wystawienia faktury korygowanej
                    .WithKSeFNumber("1234567890-20240115-ABC123"))) // Numer KSeF faktury korygowanej (opcjonalnie)

            // Pozycja korygujaca - ujemna wartosc (zmniejszenie)
            .AddLineItem(item => item
                .WithProductName("Usluga konsultingowa IT - korekta (rabat)")
                .WithUnit("szt.")
                .WithQuantity(1)
                .WithUnitNetPrice(-200.00m)    // Ujemna cena = zmniejszenie
                .WithVatRate(VatRate.Rate23)
                .WithNetAmount(-200.00m)       // Ujemna wartosc netto
                .WithVatAmount(-46.00m))       // Ujemna kwota VAT

            .Build();

        // Walidacja
        var validationResult = invoiceService.Validate(correctionInvoice);

        Console.WriteLine("=== Faktura korygujaca ===");
        Console.WriteLine($"Numer korekty: {correctionInvoice.InvoiceData.InvoiceNumber}");
        Console.WriteLine($"Faktura korygowana: {correctionInvoice.InvoiceData.CorrectedInvoiceData?.CorrectedInvoiceNumber}");
        Console.WriteLine($"Przyczyna korekty: {correctionInvoice.InvoiceData.CorrectionReason}");
        Console.WriteLine($"Typ faktury: {correctionInvoice.InvoiceData.InvoiceType}");
        Console.WriteLine($"Jest korekta: {correctionInvoice.IsCorrection}");
        Console.WriteLine($"Zmiana kwoty netto: {correctionInvoice.InvoiceData.TotalNetAmount:N2} PLN");
        Console.WriteLine($"Zmiana kwoty VAT: {correctionInvoice.InvoiceData.TotalVatAmount:N2} PLN");
        Console.WriteLine($"Walidacja: {(validationResult.IsValid ? "POPRAWNA" : "BLEDY")}");

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"  Blad: [{error.Code}] {error.Message}");
            }
        }
    }
}
