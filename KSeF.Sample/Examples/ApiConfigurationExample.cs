using KSeF.Api;
using KSeF.Api.Configuration;
using KSeF.Api.Services;
using KSeF.Invoice;
using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Sample.Examples;

/// <summary>
/// Przyklad konfiguracji KSeF.Api z Dependency Injection
/// </summary>
public class ApiConfigurationExample
{
    /// <summary>
    /// Demonstracja roznych sposobow konfiguracji KSeF.Api
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("=== Konfiguracja KSeF.Api ===\n");

        // 1. Konfiguracja z delegatem
        Console.WriteLine("1. Konfiguracja z delegatem:");
        ConfigureWithDelegate();

        // 2. Wyswietlenie dostepnych srodowisk
        Console.WriteLine("\n2. Dostepne srodowiska KSeF:");
        ShowEnvironments();

        // 3. Demonstracja zarejestrowanych serwisow
        Console.WriteLine("\n3. Zarejestrowane serwisy:");
        ShowRegisteredServices();
    }

    private static void ConfigureWithDelegate()
    {
        var services = new ServiceCollection();

        // Konfiguracja KSeF.Api z delegatem - rejestruje wszystkie serwisy
        services.AddKsefApiServices(options =>
        {
            options.BaseUrl = KsefEnvironment.Test;      // Srodowisko testowe
            options.Nip = "1234567890";                   // NIP podmiotu
            options.AuthMethod = KsefAuthMethod.Token;    // Autoryzacja tokenem
            options.KsefToken = "PRZYKLADOWY_TOKEN";      // Token KSeF
            options.TimeoutSeconds = 120;                 // Timeout 120s
            options.MaxRetries = 3;                       // Maks. 3 proby
            options.SystemInfo = "KSeF.Sample 1.0";       // Info o systemie
        });

        var provider = services.BuildServiceProvider();

        // Sprawdzenie czy serwisy sa zarejestrowane
        var sessionService = provider.GetRequiredService<IKsefSessionService>();
        var sendService = provider.GetRequiredService<IKsefInvoiceSendService>();

        Console.WriteLine($"   IKsefSessionService: {sessionService.GetType().Name}");
        Console.WriteLine($"   IKsefInvoiceSendService: {sendService.GetType().Name}");
        Console.WriteLine("   Konfiguracja poprawna!");
    }

    private static void ShowEnvironments()
    {
        Console.WriteLine($"   Test:        {KsefEnvironment.Test}");
        Console.WriteLine($"   Demo:        {KsefEnvironment.Demo}");
        Console.WriteLine($"   Produkcja:   {KsefEnvironment.Production}");
    }

    private static void ShowRegisteredServices()
    {
        var services = new ServiceCollection();
        services.AddKsefApiServices(options =>
        {
            options.BaseUrl = KsefEnvironment.Test;
            options.Nip = "1234567890";
            options.AuthMethod = KsefAuthMethod.Token;
            options.KsefToken = "TOKEN";
        });

        var provider = services.BuildServiceProvider();

        // KSeF.Api rejestruje rowniez KSeF.Invoice
        var invoiceService = provider.GetRequiredService<IKsefInvoiceService>();
        var sessionService = provider.GetRequiredService<IKsefSessionService>();
        var sendService = provider.GetRequiredService<IKsefInvoiceSendService>();
        var receiveService = provider.GetRequiredService<IKsefInvoiceReceiveService>();
        var statusService = provider.GetRequiredService<IKsefInvoiceStatusService>();

        Console.WriteLine($"   IKsefInvoiceService:        {invoiceService.GetType().Name}");
        Console.WriteLine($"   IKsefSessionService:        {sessionService.GetType().Name}");
        Console.WriteLine($"   IKsefInvoiceSendService:    {sendService.GetType().Name}");
        Console.WriteLine($"   IKsefInvoiceReceiveService: {receiveService.GetType().Name}");
        Console.WriteLine($"   IKsefInvoiceStatusService:  {statusService.GetType().Name}");
    }
}
