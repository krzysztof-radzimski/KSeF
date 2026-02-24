using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Samples.Examples;

/// <summary>
/// Przyklad tworzenia prostej faktury VAT
/// </summary>
public class SimpleInvoiceExample
{
    /// <summary>
    /// Demonstracja tworzenia i walidacji prostej faktury VAT
    /// </summary>
    public static void Run()
    {
        // 1. Konfiguracja DI
        var services = new ServiceCollection();
        services.AddKsefInvoiceServices(options =>
        {
            options.SchemaVersion = SchemaVersion.FA3;
            options.ValidateBeforeSerialize = true;
        });

        var serviceProvider = services.BuildServiceProvider();
        var invoiceService = serviceProvider.GetRequiredService<IKsefInvoiceService>();

        // 2. Budowanie faktury przy uzyciu fluent API
        var invoice = invoiceService.CreateInvoice()
            // Sprzedawca (Podmiot1)
            .WithSeller(seller => seller
                .WithTaxId("1234567890")          // NIP sprzedawcy
                .WithName("Firma ABC Sp. z o.o.") // Nazwa firmy
                .WithAddress(address => address
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Przykladowa 1")
                    .WithAddressLine2("00-001 Warszawa"))
                .WithContactData("biuro@firma-abc.pl", "+48 22 111 22 33"))

            // Nabywca (Podmiot2)
            .WithBuyer(buyer => buyer
                .WithTaxId("0987654321")         // NIP nabywcy
                .WithName("Klient XYZ S.A.")     // Nazwa firmy
                .WithAddress(address => address
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Kliencka 99")
                    .WithAddressLine2("30-001 Krakow")))

            // Dane faktury (Fa)
            .WithInvoiceDetails(details => details
                .WithInvoiceNumber("FV/2024/001")
                .WithIssueDate(2024, 1, 15)      // Data wystawienia
                .WithSaleDate(2024, 1, 15)       // Data sprzedazy
                .WithCurrency(CurrencyCode.PLN)) // Waluta

            // Pozycja faktury (FaWiersz)
            .AddLineItem(item => item
                .WithProductName("Usluga konsultingowa IT")
                .WithUnit("szt.")
                .WithQuantity(1)
                .WithUnitNetPrice(1000.00m)      // Cena jednostkowa netto
                .WithVatRate(VatRate.Rate23)     // Stawka VAT 23%
                .WithNetAmount(1000.00m)         // Wartosc netto
                .WithVatAmount(230.00m))         // Kwota VAT

            // Platnosc
            .WithPayment(payment => payment
                .AddPaymentTerm(2024, 1, 30)     // Termin platnosci
                .AsBankTransfer("PL61109010140000071219812874"))

            .Build();

        // 3. Walidacja faktury
        var validationResult = invoiceService.Validate(invoice);

        Console.WriteLine("=== Prosta faktura VAT ===");
        Console.WriteLine($"Numer faktury: {invoice.InvoiceData.InvoiceNumber}");
        Console.WriteLine($"Sprzedawca: {invoice.Seller.Name}");
        Console.WriteLine($"Nabywca: {invoice.Buyer.Name}");
        Console.WriteLine($"Kwota netto: {invoice.InvoiceData.TotalNetAmount:N2} PLN");
        Console.WriteLine($"Kwota VAT: {invoice.InvoiceData.TotalVatAmount:N2} PLN");
        Console.WriteLine($"Kwota brutto: {invoice.InvoiceData.TotalAmount:N2} PLN");
        Console.WriteLine($"Walidacja: {(validationResult.IsValid ? "POPRAWNA" : "BLEDY")}");

        // 4. Serializacja do XML
        if (validationResult.IsValid)
        {
            string xml = invoiceService.ToXml(invoice);
            Console.WriteLine($"Rozmiar XML: {xml.Length} znakow");

            // Zapis do pliku
            File.WriteAllText("prosta_faktura.xml", xml);
            Console.WriteLine("Zapisano do: prosta_faktura.xml");
        }
    }
}
