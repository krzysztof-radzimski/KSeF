using KSeF.Api;
using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Api.Services;
using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Sample.Examples;

/// <summary>
/// Przyklad wysylania faktur do KSeF (wymaga poprawnej konfiguracji i polaczenia z API)
/// </summary>
public class SendInvoiceExample
{
    /// <summary>
    /// Demonstracja wysylania faktur do KSeF.
    /// UWAGA: Ten przyklad wymaga polaczenia z API KSeF i poprawnych danych autoryzacyjnych.
    /// Bez polaczenia wyswietla jedynie strukture kodu do wysylania faktur.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("=== Wysylanie faktur do KSeF (przyklad kodu) ===\n");

        // Konfiguracja
        var services = new ServiceCollection();
        services.AddKsefApiServices(options =>
        {
            options.BaseUrl = KsefEnvironment.Test;
            options.Nip = "1234567890";
            options.AuthMethod = KsefAuthMethod.Token;
            options.KsefToken = "PRZYKLADOWY_TOKEN";
            options.SystemInfo = "KSeF.Sample 1.0";
        });

        var provider = services.BuildServiceProvider();
        var invoiceService = provider.GetRequiredService<IKsefInvoiceService>();
        var sendService = provider.GetRequiredService<IKsefInvoiceSendService>();

        // Tworzenie faktury do wyslania
        var invoice = invoiceService.CreateInvoice()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Firma ABC Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Przykladowa 1")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Klient XYZ S.A.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Kliencka 99")
                    .WithAddressLine2("30-001 Krakow")))
            .WithInvoiceDetails(d => d
                .WithInvoiceNumber("FV/2025/001")
                .WithIssueDate(2025, 3, 1)
                .WithSaleDate(2025, 3, 1)
                .WithCurrency(CurrencyCode.PLN))
            .AddLineItem(i => i
                .WithProductName("Usluga konsultingowa")
                .WithUnit("szt.")
                .WithQuantity(1)
                .WithUnitNetPrice(10000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithNetAmount(10000.00m)
                .WithVatAmount(2300.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2025, 3, 31)
                .AsBankTransfer("PL61109010140000071219812874"))
            .Build();

        Console.WriteLine("Faktura przygotowana do wyslania:");
        Console.WriteLine($"   Numer: {invoice.InvoiceData.InvoiceNumber}");
        Console.WriteLine($"   Kwota brutto: {invoice.InvoiceData.TotalAmount:N2} PLN");

        // Przyklad 1: Wysylanie pojedynczej faktury
        Console.WriteLine("\n--- Przyklad 1: Wysylanie pojedynczej faktury ---");
        Console.WriteLine("   Kod: await sendService.SendInvoiceAsync(invoice);");
        Console.WriteLine("   Lub: await sendService.SendVatInvoiceAsync(invoice);");

        // Przyklad 2: Wysylanie wielu faktur w jednej sesji
        Console.WriteLine("\n--- Przyklad 2: Wysylanie wielu faktur (batch) ---");
        Console.WriteLine("   Kod: await sendService.SendInvoicesAsync(new[] { invoice1, invoice2 });");

        // Przyklad 3: Wysylanie w istniejącej sesji
        Console.WriteLine("\n--- Przyklad 3: Wysylanie w istniejacej sesji ---");
        Console.WriteLine("   Kod: var session = await sessionService.OpenSessionAsync();");
        Console.WriteLine("         await sendService.SendInvoiceAsync(invoice, session);");
        Console.WriteLine("         await sessionService.CloseSessionAsync(session);");

        // Przyklad 4: Dostepne metody wysylania dla roznych typow faktur
        Console.WriteLine("\n--- Przyklad 4: Metody wysylania wg typu faktury ---");
        Console.WriteLine("   SendVatInvoiceAsync()              - faktura VAT");
        Console.WriteLine("   SendCorrectionInvoiceAsync()       - korekta VAT");
        Console.WriteLine("   SendAdvancePaymentInvoiceAsync()   - faktura zaliczkowa");
        Console.WriteLine("   SendSettlementInvoiceAsync()       - rozliczenie zaliczek");
        Console.WriteLine("   SendSimplifiedInvoiceAsync()       - faktura uproszczona");
        Console.WriteLine("   SendAdvancePaymentCorrectionAsync()- korekta zaliczkowej");
        Console.WriteLine("   SendSettlementCorrectionAsync()    - korekta rozliczeniowej");

        Console.WriteLine("\nUWAGA: Aby faktycznie wyslac fakture, ustaw poprawny token KSeF");
        Console.WriteLine("       i NIP w konfiguracji, a nastepnie odkomentuj wywolanie async.");
    }
}
