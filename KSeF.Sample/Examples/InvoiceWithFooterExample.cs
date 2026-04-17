/*=====================================================================================

    KSeF.Sample
    Przykład tworzenia faktury VAT z pełną stopką (Stopka).
    Demonstruje użycie FooterBuilder do dodania informacji tekstowych
    (StopkaFaktury) oraz danych rejestrowych podmiotu (KRS, REGON, BDO).
    Pokazuje zarówno wygodną metodę AddRegistryData jak i nested
    RegistryDataBuilder dla bardziej złożonych przypadków.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Sample.Examples;

/// <summary>
/// Przyklad tworzenia faktury VAT z pelna stopka
/// </summary>
public class InvoiceWithFooterExample
{
    /// <summary>
    /// Demonstracja tworzenia faktury z uzyciem FooterBuilder
    /// </summary>
    public static void Run()
    {
        // 1. Konfiguracja DI
        var services = new ServiceCollection();
        services.AddKsefInvoiceServices(options =>
        {
            options.SchemaVersion = SchemaVersion.FA3;
            options.ValidateBeforeSerialize = false;  // Wylaczone dla przykladowych danych
            options.ValidateAgainstXsd = false;       // Wylaczone dla przykladowych danych
        });

        var serviceProvider = services.BuildServiceProvider();
        var invoiceService = serviceProvider.GetRequiredService<IKsefInvoiceService>();

        // 2. Budowanie faktury z pelna stopka
        var invoice = invoiceService.CreateInvoice()
            // Sprzedawca (Podmiot1)
            .WithSeller(seller => seller
                .WithTaxId("1234567890")
                .WithName("Acme Sp. z o.o.")
                .WithAddress(address => address
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Przemyslowa 15")
                    .WithAddressLine2("00-850 Warszawa"))
                .WithContactData("kontakt@acme.pl", "+48 22 555 66 77"))

            // Nabywca (Podmiot2)
            .WithBuyer(buyer => buyer
                .WithTaxId("9876543210")
                .WithName("Klient Sp. z o.o.")
                .WithAddress(address => address
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Handlowa 99")
                    .WithAddressLine2("31-500 Krakow")))

            // Dane faktury (Fa)
            .WithInvoiceDetails(details => details
                .WithInvoiceNumber("FV/2024/100")
                .WithIssueDate(2024, 3, 15)
                .WithSaleDate(2024, 3, 15)
                .WithCurrency(CurrencyCode.PLN))

            // Pozycje faktury (FaWiersz)
            .AddLineItem(item => item
                .WithProductName("Usluga wdrozenia systemu CRM")
                .WithUnit("szt.")
                .WithQuantity(1)
                .WithUnitNetPrice(15000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithNetAmount(15000.00m)
                .WithVatAmount(3450.00m))
            .AddLineItem(item => item
                .WithProductName("Licencja roczna - 10 uzytkownikow")
                .WithUnit("szt.")
                .WithQuantity(1)
                .WithUnitNetPrice(5000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithNetAmount(5000.00m)
                .WithVatAmount(1150.00m))

            // Platnosc
            .WithPayment(payment => payment
                .AddPaymentTerm(2024, 4, 15)
                .AsBankTransfer("PL61109010140000071219812874"))

            // STOPKA - demonstracja FooterBuilder
            .WithFooter(footer => footer
                // Informacje tekstowe (StopkaFaktury) - max 3 wpisy
                .AddFooterText("Faktura wygenerowana automatycznie przez system KSeF.Invoice")
                .AddFooterText("Termin reklamacji: 14 dni od daty otrzymania faktury")

                // Dane rejestrowe - metoda z parametrami (wygodna dla pelnych danych)
                .AddRegistryData(
                    fullName: "Acme Sp. z o.o.",
                    krs: "0000123456",
                    regon: "123456789",
                    bdo: "000123456")

                // Dane rejestrowe - nested builder (elastyczny dla czesc danych)
                .AddRegistry(r => r
                    .WithFullName("Oddzial Acme Krakow")
                    .WithKRS("0000654321")
                    .WithREGON("987654321")))

            .Build();

        // 3. Informacje o stopce
        Console.WriteLine("=== Faktura z pelna stopka ===");
        Console.WriteLine($"Numer faktury: {invoice.InvoiceData.InvoiceNumber}");
        Console.WriteLine($"Sprzedawca: {invoice.Seller.Name}");
        Console.WriteLine($"Nabywca: {invoice.Buyer.Name}");
        Console.WriteLine($"Kwota netto: {invoice.InvoiceData.TotalNetAmount:N2} PLN");
        Console.WriteLine($"Kwota VAT: {invoice.InvoiceData.TotalVatAmount:N2} PLN");
        Console.WriteLine($"Kwota brutto: {invoice.InvoiceData.TotalAmount:N2} PLN");

        // Informacje o stopce
        if (invoice.Footer != null)
        {
            if (invoice.Footer.HasAdditionalInfo)
            {
                Console.WriteLine($"Informacje dodatkowe: {invoice.Footer.AdditionalInfo?.Count ?? 0} wpis(ow)");
                foreach (var info in invoice.Footer.AdditionalInfo!)
                {
                    Console.WriteLine($"  - {info.FooterText}");
                }
            }

            if (invoice.Footer.HasRegistries)
            {
                Console.WriteLine($"Dane rejestrowe: {invoice.Footer.Registries?.Count ?? 0} wpis(ow)");
                foreach (var registry in invoice.Footer.Registries!)
                {
                    Console.WriteLine($"  - {registry.FullName} (KRS: {registry.KRS}, REGON: {registry.REGON}, BDO: {registry.BDO})");
                }
            }
        }

        // 4. Walidacja
        var validationResult = invoiceService.Validate(invoice);
        Console.WriteLine($"Walidacja: {(validationResult.IsValid ? "POPRAWNA" : "BLEDY")}");

        if (!validationResult.IsValid)
        {
            Console.WriteLine($"Bledy walidacji ({validationResult.Errors.Count}):");
            foreach (var error in validationResult.Errors.Take(5))
            {
                Console.WriteLine($"  - [{error.Code}] {error.Message}");
            }
        }

        // 5. Serializacja do XML (ValidateBeforeSerialize=false umozliwia to nawet z bledami walidacji)
        string xml = invoiceService.ToXml(invoice);
        Console.WriteLine($"Rozmiar XML: {xml.Length} znakow");

        // Pokaz fragment XML ze stopka
        if (xml.Contains("Stopka>"))  // Szukamy bez namespace prefix (moze byc tns:Stopka lub Stopka)
        {
            var stopkaIndex = xml.IndexOf("Stopka>");
            if (stopkaIndex > 0)
            {
                stopkaIndex = xml.LastIndexOf('<', stopkaIndex);  // Cofnij sie do '<'
                var stopkaEndIndex = xml.IndexOf("</", stopkaIndex);
                stopkaEndIndex = xml.IndexOf("Stopka>", stopkaEndIndex) + "Stopka>".Length;

                if (stopkaIndex >= 0 && stopkaEndIndex > stopkaIndex)
                {
                    var stopkaXml = xml.Substring(stopkaIndex, stopkaEndIndex - stopkaIndex);
                    Console.WriteLine("\nFragment XML - Stopka:");
                    Console.WriteLine(stopkaXml);
                }
            }
        }

        // Zapis do pliku
        File.WriteAllText("faktura_z_stopka.xml", xml);
        Console.WriteLine("\nZapisano do: faktura_z_stopka.xml");
    }
}
