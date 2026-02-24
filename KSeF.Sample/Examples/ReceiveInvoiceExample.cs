using KSeF.Api;
using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Sample.Examples;

/// <summary>
/// Przyklad pobierania faktur z KSeF (wymaga poprawnej konfiguracji i polaczenia z API)
/// </summary>
public class ReceiveInvoiceExample
{
    /// <summary>
    /// Demonstracja pobierania faktur zakupowych z KSeF.
    /// UWAGA: Ten przyklad wymaga polaczenia z API KSeF.
    /// Bez polaczenia wyswietla jedynie strukture kodu do pobierania faktur.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("=== Pobieranie faktur z KSeF (przyklad kodu) ===\n");

        // Konfiguracja
        var services = new ServiceCollection();
        services.AddKsefApiServices(options =>
        {
            options.BaseUrl = KsefEnvironment.Test;
            options.Nip = "1234567890";
            options.AuthMethod = KsefAuthMethod.Token;
            options.KsefToken = "PRZYKLADOWY_TOKEN";
        });

        var provider = services.BuildServiceProvider();
        var receiveService = provider.GetRequiredService<IKsefInvoiceReceiveService>();

        // Przyklad 1: Pobranie faktury po numerze KSeF
        Console.WriteLine("--- Przyklad 1: Pobranie konkretnej faktury ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var result = await receiveService.GetInvoiceAsync(\"1234567890-20250301-ABCDEF-01\");");
        Console.WriteLine("   if (result.Success)");
        Console.WriteLine("   {");
        Console.WriteLine("       Console.WriteLine(result.InvoiceXml);");
        Console.WriteLine("       var invoice = result.Invoice;");
        Console.WriteLine("   }");

        // Przyklad 2: Wyszukiwanie faktur zakupowych
        Console.WriteLine("\n--- Przyklad 2: Wyszukiwanie faktur zakupowych ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var criteria = new InvoiceQueryCriteria");
        Console.WriteLine("   {");
        Console.WriteLine("       DateFrom = new DateTime(2025, 1, 1),");
        Console.WriteLine("       DateTo = new DateTime(2025, 3, 31),");
        Console.WriteLine("       Direction = InvoiceDirection.Purchase,");
        Console.WriteLine("       PageSize = 50");
        Console.WriteLine("   };");
        Console.WriteLine("   var result = await receiveService.QueryPurchaseInvoicesAsync(criteria);");

        // Przyklad 3: Pobieranie wszystkich nowych faktur
        Console.WriteLine("\n--- Przyklad 3: Pobieranie wszystkich nowych faktur ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var invoices = await receiveService.DownloadPurchaseInvoicesAsync(");
        Console.WriteLine("       fromDate: new DateTime(2025, 1, 1),");
        Console.WriteLine("       toDate: DateTime.UtcNow);");
        Console.WriteLine("   // Automatycznie iteruje po stronach wynikow");

        // Przyklad 4: Pobieranie UPO
        Console.WriteLine("\n--- Przyklad 4: Pobieranie UPO ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var upo = await receiveService.GetUpoAsync(\"1234567890-20250301-ABCDEF-01\");");

        Console.WriteLine("\nUWAGA: Aby faktycznie pobrac faktury, ustaw poprawny token KSeF");
        Console.WriteLine("       i NIP w konfiguracji.");
    }
}
