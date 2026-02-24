using KSeF.Api;
using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Sample.Examples;

/// <summary>
/// Przyklad zarzadzania sesjami KSeF
/// </summary>
public class SessionManagementExample
{
    /// <summary>
    /// Demonstracja zarzadzania sesjami KSeF.
    /// UWAGA: Ten przyklad wymaga polaczenia z API KSeF.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("=== Zarzadzanie sesjami KSeF (przyklad kodu) ===\n");

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
        var sessionService = provider.GetRequiredService<IKsefSessionService>();
        var statusService = provider.GetRequiredService<IKsefInvoiceStatusService>();

        // Przyklad 1: Otwieranie i zamykanie sesji
        Console.WriteLine("--- Przyklad 1: Cykl zycia sesji ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var session = await sessionService.OpenSessionAsync();");
        Console.WriteLine("   Console.WriteLine($\"Sesja: {session.SessionReference}\");");
        Console.WriteLine("   Console.WriteLine($\"Token: {session.AccessToken}\");");
        Console.WriteLine("   Console.WriteLine($\"Aktywna: {session.IsActive}\");");
        Console.WriteLine("   // ... operacje na fakturach ...");
        Console.WriteLine("   await sessionService.CloseSessionAsync(session);");

        // Przyklad 2: Odswiezanie tokenu sesji
        Console.WriteLine("\n--- Przyklad 2: Odswiezanie tokenu ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var refreshed = await sessionService.RefreshSessionAsync(session);");
        Console.WriteLine("   // refreshed.AccessToken - nowy token");
        Console.WriteLine("   // refreshed.ExpiresAt - nowa data wygasniecia");

        // Przyklad 3: Sprawdzanie statusu sesji
        Console.WriteLine("\n--- Przyklad 3: Status sesji ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var status = await sessionService.GetSessionStatusAsync(");
        Console.WriteLine("       session.SessionReference, session.AccessToken);");

        // Przyklad 4: Sprawdzanie statusu faktury
        Console.WriteLine("\n--- Przyklad 4: Status faktury ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var invoiceStatus = await statusService.GetInvoiceStatusAsync(");
        Console.WriteLine("       referenceNumber, session.AccessToken);");
        Console.WriteLine("   Console.WriteLine($\"Status: {invoiceStatus.Status}\");");
        Console.WriteLine("   Console.WriteLine($\"Numer KSeF: {invoiceStatus.KsefNumber}\");");

        // Przyklad 5: Statusy faktur w sesji
        Console.WriteLine("\n--- Przyklad 5: Statusy faktur w sesji ---");
        Console.WriteLine("   Kod:");
        Console.WriteLine("   var sessionInvoices = await statusService.GetSessionInvoicesStatusAsync(");
        Console.WriteLine("       session.SessionReference, session.AccessToken);");
        Console.WriteLine("   Console.WriteLine($\"Przetworzone: {sessionInvoices.ProcessedCount}\");");
        Console.WriteLine("   Console.WriteLine($\"Odrzucone: {sessionInvoices.RejectedCount}\");");

        // Model SessionInfo
        Console.WriteLine("\n--- Model SessionInfo ---");
        var sessionInfo = new SessionInfo
        {
            SessionReference = "REF-20250301-ABC123",
            AccessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6...",
            RefreshToken = "refresh_token_value",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        Console.WriteLine($"   SessionReference: {sessionInfo.SessionReference}");
        Console.WriteLine($"   IsActive: {sessionInfo.IsActive}");
        Console.WriteLine($"   ExpiresAt: {sessionInfo.ExpiresAt:yyyy-MM-dd HH:mm:ss}");
    }
}
