# KSeF.Api

Biblioteka .NET do integracji z **Krajowym Systemem e-Faktur (KSeF)** - wysyłanie, odbieranie i zarządzanie fakturami ustrukturyzowanymi przez API.

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](../LICENSE)
[![Target Framework](https://img.shields.io/badge/Framework-net8.0%20%7C%20net9.0-purple)](https://dotnet.microsoft.com/)

## Opis

**KSeF.Api** to biblioteka oferująca pełną integrację z API Krajowego Systemu e-Faktur:
- **Wysyłanie faktur** - wszystkie 7 typów faktur do KSeF
- **Odbieranie faktur** - pobieranie faktur zakupowych z KSeF
- **Zarządzanie sesjami** - automatyczne otwieranie i zamykanie sesji
- **Autoryzacja** - token KSeF lub certyfikat X.509
- **Statusy faktur** - sprawdzanie statusu przetwarzania
- **Automatyczna walidacja** - przed wysłaniem do KSeF

## Instalacja

### 1. Konfiguracja źródła GitHub Packages

Pakiety `KSeF.Client` są dostępne w GitHub Packages organizacji CIRFMF. Wymagany jest Personal Access Token (PAT) z uprawnieniem `read:packages`.

**Windows (CMD):**
```cmd
dotnet nuget add source "https://nuget.pkg.github.com/CIRFMF/index.json" ^
  --name github-cirf ^
  --username token ^
  --password TWOJ_PAT_TOKEN ^
  --store-password-in-clear-text
```

**Linux/macOS (Bash):**
```bash
dotnet nuget add source "https://nuget.pkg.github.com/CIRFMF/index.json" \
  --name github-cirf \
  --username token \
  --password TWOJ_PAT_TOKEN \
  --store-password-in-clear-text
```

> **Alternatywa:** Możesz skopiować plik `nuget.config` z głównego katalogu repozytorium.

### 2. Instalacja pakietu

```bash
dotnet add package KSeF.Api
```

Lub w pliku `.csproj`:

```xml
<PackageReference Include="KSeF.Api" Version="1.0.0" />
```

> **Uwaga:** Instalacja `KSeF.Api` automatycznie zainstaluje również `KSeF.Invoice` (modele i walidacja faktur).

## Szybki start

### 1. Konfiguracja w appsettings.json

```json
{
  "KSeF": {
    "BaseUrl": "https://ksef-test.mf.gov.pl/api",
    "Nip": "1234567890",
    "AuthMethod": "Token",
    "KsefToken": "TWOJ_TOKEN_KSEF",
    "TimeoutSeconds": 120,
    "SystemInfo": "MojSystem ERP 1.0"
  }
}
```

### 2. Rejestracja serwisów

```csharp
using KSeF.Api;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Sposób 1: Z konfiguracji appsettings.json
builder.Services.AddKsefApiServices(builder.Configuration);

// Sposób 2: Z delegatem konfiguracji
builder.Services.AddKsefApiServices(options =>
{
    options.BaseUrl = KsefEnvironment.Test;
    options.Nip = "1234567890";
    options.AuthMethod = KsefAuthMethod.Token;
    options.KsefToken = "TWOJ_TOKEN";
});
```

Metoda `AddKsefApiServices` rejestruje automatycznie:
- **KSeF.Invoice** - modele, walidacja, serializacja
- **KSeF.Client** - komunikacja HTTP z API
- **IKsefSessionService** - zarządzanie sesjami
- **IKsefInvoiceSendService** - wysyłanie faktur
- **IKsefInvoiceReceiveService** - pobieranie faktur
- **IKsefInvoiceStatusService** - sprawdzanie statusów

### 3. Wysyłanie faktury

```csharp
public class InvoiceController
{
    private readonly IKsefInvoiceService _invoiceService;
    private readonly IKsefInvoiceSendService _sendService;

    public InvoiceController(
        IKsefInvoiceService invoiceService,
        IKsefInvoiceSendService sendService)
    {
        _invoiceService = invoiceService;
        _sendService = sendService;
    }

    public async Task<IActionResult> SendInvoice()
    {
        // 1. Utwórz fakturę
        var invoice = _invoiceService.CreateInvoice()
            .WithSeller(s => s
                .WithName("Firma ABC Sp. z o.o.")
                .WithTaxId("1234567890")
                .WithAddress(a => a
                    .WithCity("Warszawa")
                    .WithPostalCode("00-001")))
            .WithBuyer(b => b
                .WithName("Klient XYZ S.A.")
                .WithTaxId("0987654321"))
            .WithInvoiceDetails(d => d
                .WithInvoiceNumber("FV/2025/001")
                .WithIssueDate(2025, 3, 1))
            .AddLineItem(i => i
                .WithProductName("Usługa konsultingowa")
                .WithUnitNetPrice(10000.00m)
                .WithVatRate(VatRate.Rate23))
            .Build();

        // 2. Wyślij do KSeF
        var result = await _sendService.SendVatInvoiceAsync(invoice);

        if (result.Success)
        {
            return Ok(new
            {
                ReferenceNumber = result.ReferenceNumber,
                KsefNumber = result.KsefNumber
            });
        }
        else
        {
            return BadRequest(new { Errors = result.Errors });
        }
    }
}
```

## Funkcjonalności

### Wysyłanie faktur

Biblioteka obsługuje wszystkie 7 typów faktur KSeF:

```csharp
public class InvoiceSendService
{
    private readonly IKsefInvoiceSendService _sendService;

    // Faktura VAT
    var result = await _sendService.SendVatInvoiceAsync(invoice);

    // Korekta VAT
    var result = await _sendService.SendCorrectionInvoiceAsync(correction);

    // Faktura zaliczkowa
    var result = await _sendService.SendAdvancePaymentInvoiceAsync(advance);

    // Rozliczenie zaliczek
    var result = await _sendService.SendSettlementInvoiceAsync(settlement);

    // Faktura uproszczona
    var result = await _sendService.SendSimplifiedInvoiceAsync(simplified);

    // Korekta zaliczkowa
    var result = await _sendService.SendAdvancePaymentCorrectionAsync(correction);

    // Korekta rozliczeniowa
    var result = await _sendService.SendSettlementCorrectionAsync(correction);
}
```

### Wysyłanie wielu faktur w jednej sesji

```csharp
public async Task SendBatch(List<Invoice> invoices)
{
    // Automatycznie otwiera jedną sesję dla wszystkich faktur
    var results = await _sendService.SendInvoicesAsync(invoices);

    foreach (var result in results)
    {
        if (result.Success)
            Console.WriteLine($"✓ {result.ReferenceNumber}");
        else
            Console.WriteLine($"✗ {string.Join(", ", result.Errors)}");
    }
}
```

> **Zaleta:** Wysyłanie wielu faktur w jednej sesji jest bardziej wydajne niż otwieranie osobnej sesji dla każdej faktury.

### Pobieranie faktur zakupowych

```csharp
public class PurchaseInvoiceService
{
    private readonly IKsefInvoiceReceiveService _receiveService;

    // Wyszukiwanie faktur po kryteriach
    public async Task SearchInvoices()
    {
        var criteria = new InvoiceQueryCriteria
        {
            DateFrom = new DateTime(2025, 1, 1),
            DateTo = new DateTime(2025, 3, 31),
            Direction = InvoiceDirection.Purchase,
            PageSize = 50
        };

        var result = await _receiveService.QueryPurchaseInvoicesAsync(criteria);

        if (result.Success)
        {
            Console.WriteLine($"Znaleziono {result.TotalCount} faktur:");
            foreach (var inv in result.Invoices)
            {
                Console.WriteLine($"  {inv.KsefNumber} | {inv.InvoiceNumber} | " +
                    $"{inv.SellerNip} | {inv.GrossAmount:C}");
            }
        }
    }

    // Pobieranie konkretnej faktury
    public async Task<InvoiceDownloadResult> GetInvoice(string ksefNumber)
    {
        return await _receiveService.GetInvoiceAsync(ksefNumber);
    }

    // Pobieranie wszystkich nowych faktur
    public async Task<List<InvoiceDownloadResult>> DownloadAll()
    {
        return await _receiveService.DownloadPurchaseInvoicesAsync(
            fromDate: new DateTime(2025, 1, 1),
            toDate: DateTime.UtcNow);
    }

    // Pobieranie UPO (Urzędowe Poświadczenie Odbioru)
    public async Task<UpoResult> GetUpo(string referenceNumber)
    {
        return await _receiveService.GetUpoAsync(referenceNumber);
    }
}
```

### Sprawdzanie statusu faktur

```csharp
public class StatusService
{
    private readonly IKsefInvoiceStatusService _statusService;
    private readonly IKsefSessionService _sessionService;

    // Status pojedynczej faktury
    public async Task CheckStatus(string referenceNumber)
    {
        var session = await _sessionService.OpenSessionAsync();
        try
        {
            var status = await _statusService.GetInvoiceStatusAsync(
                referenceNumber, session);

            Console.WriteLine($"Status: {status.Status}");
            Console.WriteLine($"Numer KSeF: {status.KsefNumber}");
        }
        finally
        {
            await _sessionService.CloseSessionAsync(session);
        }
    }

    // Status wszystkich faktur w sesji
    public async Task CheckSessionInvoices(string sessionReference, string accessToken)
    {
        var result = await _statusService.GetSessionInvoicesStatusAsync(
            sessionReference, accessToken);

        Console.WriteLine($"Przetworzone: {result.ProcessedCount}");
        Console.WriteLine($"Odrzucone: {result.RejectedCount}");

        foreach (var inv in result.InvoiceStatuses)
        {
            Console.WriteLine($"  {inv.ReferenceNumber}: {inv.Status}");
        }
    }
}
```

> **⚠️ WAŻNE:** KSeF API 2.0 nie udostępnia dedykowanego endpointu do oznaczania faktur zakupowych jako zaksięgowane. Status księgowania należy śledzić w systemie ERP/księgowym użytkownika. Serwis `IKsefInvoiceStatusService` pozwala na sprawdzanie statusu przetwarzania faktur w KSeF (czy zostały przyjęte/odrzucone), ale nie statusu księgowego.

### Zarządzanie sesjami (zaawansowane)

```csharp
public class SessionService
{
    private readonly IKsefSessionService _sessionService;

    // Ręczne zarządzanie sesją
    public async Task ManualSession()
    {
        var session = await _sessionService.OpenSessionAsync();
        try
        {
            // Operacje w ramach sesji...
            // np. wysyłanie wielu faktur

            // Odświeżenie tokenu sesji
            await _sessionService.RefreshSessionAsync(session);
        }
        finally
        {
            await _sessionService.CloseSessionAsync(session);
        }
    }
}
```

> **Uwaga:** W większości przypadków nie musisz ręcznie zarządzać sesjami - metody `Send*InvoiceAsync` robią to automatycznie.

## Autoryzacja

### Token KSeF (zalecane dla testów)

```json
{
  "KSeF": {
    "BaseUrl": "https://ksef-test.mf.gov.pl/api",
    "Nip": "1234567890",
    "AuthMethod": "Token",
    "KsefToken": "TWOJ_TOKEN_KSEF"
  }
}
```

**Zalety:**
- ✅ Prosta konfiguracja
- ✅ Szybkie ustawienie
- ✅ Idealne dla środowisk testowych

**Wady:**
- ⚠️ Token wygasa
- ⚠️ Trzeba okresowo odnawiać

### Certyfikat X.509 (zalecane dla produkcji)

```json
{
  "KSeF": {
    "BaseUrl": "https://ksef.mf.gov.pl/api",
    "Nip": "1234567890",
    "AuthMethod": "Certificate",
    "Certificate": {
      "CertificatePath": "C:\\Certs\\ksef-cert.pfx",
      "CertificatePassword": "haslo-do-certyfikatu"
    }
  }
}
```

**Certyfikat z magazynu Windows (thumbprint):**

```json
{
  "KSeF": {
    "AuthMethod": "Certificate",
    "Certificate": {
      "Thumbprint": "AB12CD34EF56..."
    }
  }
}
```

**Zalety:**
- ✅ Bezpieczny
- ✅ Długi czas ważności
- ✅ Zalecane dla produkcji

**Wady:**
- ⚠️ Wymaga certyfikatu X.509
- ⚠️ Bardziej złożona konfiguracja

### Środowiska KSeF

| Środowisko | Adres | Użycie |
|------------|-------|--------|
| Testowe | `https://ksef-test.mf.gov.pl/api` | Rozwój i testy integracyjne |
| Demo | `https://ksef-demo.mf.gov.pl/api` | Demonstracje i szkolenia |
| Produkcyjne | `https://ksef.mf.gov.pl/api` | ⚠️ Środowisko produkcyjne MF |

> **Uwaga:** Środowisko produkcyjne zapisuje faktury w systemie Ministerstwa Finansów. Używaj środowiska testowego do rozwoju.

## Struktura projektu

```
KSeF.Api/
├── Configuration/                      # Konfiguracja
│   ├── KsefApiOptions.cs               # Opcje połączenia
│   ├── KsefAuthMethod.cs               # Metody autoryzacji
│   ├── KsefCertificateOptions.cs       # Konfiguracja certyfikatu
│   └── KsefEnvironment.cs              # Adresy środowisk
├── Models/                             # Modele wyników API
│   ├── InvoiceSendResult.cs            # Wynik wysyłania
│   ├── InvoiceDownloadResult.cs        # Wynik pobierania
│   ├── InvoiceQueryResult.cs           # Wynik wyszukiwania
│   ├── InvoiceStatusResult.cs          # Status faktury
│   └── SessionInfo.cs                  # Informacje o sesji
├── Services/                           # Serwisy biznesowe
│   ├── IKsefSessionService.cs          # Zarządzanie sesjami
│   ├── KsefSessionService.cs
│   ├── IKsefInvoiceSendService.cs      # Wysyłanie faktur
│   ├── KsefInvoiceSendService.cs
│   ├── IKsefInvoiceReceiveService.cs   # Pobieranie faktur
│   ├── KsefInvoiceReceiveService.cs
│   ├── IKsefInvoiceStatusService.cs    # Statusy faktur
│   └── KsefInvoiceStatusService.cs
└── ServiceCollectionExtensions.cs      # Rejestracja DI
```

## Główne interfejsy

### IKsefInvoiceSendService
Wysyłanie faktur do KSeF (wszystkie 7 typów):

```csharp
Task<InvoiceSendResult> SendVatInvoiceAsync(Invoice invoice);
Task<InvoiceSendResult> SendCorrectionInvoiceAsync(Invoice correction);
Task<InvoiceSendResult> SendAdvancePaymentInvoiceAsync(Invoice advance);
Task<InvoiceSendResult> SendSettlementInvoiceAsync(Invoice settlement);
Task<InvoiceSendResult> SendSimplifiedInvoiceAsync(Invoice simplified);
Task<InvoiceSendResult> SendAdvancePaymentCorrectionAsync(Invoice correction);
Task<InvoiceSendResult> SendSettlementCorrectionAsync(Invoice correction);

// Wysyłanie wielu faktur w jednej sesji
Task<List<InvoiceSendResult>> SendInvoicesAsync(List<Invoice> invoices);
```

### IKsefInvoiceReceiveService
Pobieranie faktur zakupowych z KSeF:

```csharp
Task<InvoiceDownloadResult> GetInvoiceAsync(string ksefNumber);
Task<InvoiceQueryResult> QueryPurchaseInvoicesAsync(InvoiceQueryCriteria criteria);
Task<List<InvoiceDownloadResult>> DownloadPurchaseInvoicesAsync(DateTime fromDate, DateTime toDate);
Task<UpoResult> GetUpoAsync(string referenceNumber);
```

### IKsefSessionService
Zarządzanie sesjami KSeF:

```csharp
Task<SessionInfo> OpenSessionAsync();
Task CloseSessionAsync(SessionInfo session);
Task RefreshSessionAsync(SessionInfo session);
```

### IKsefInvoiceStatusService
Sprawdzanie statusów faktur:

```csharp
Task<InvoiceStatusResult> GetInvoiceStatusAsync(string referenceNumber, SessionInfo session);
Task<SessionStatusResult> GetSessionInvoicesStatusAsync(string sessionReference, string accessToken);
```

## Dependency Injection

Biblioteka w pełni obsługuje Microsoft.Extensions.DependencyInjection:

```csharp
using KSeF.Api;
using Microsoft.Extensions.DependencyInjection;

// Rejestracja z konfiguracją
builder.Services.AddKsefApiServices(builder.Configuration);

// Lub z delegatem
builder.Services.AddKsefApiServices(options =>
{
    options.BaseUrl = KsefEnvironment.Test;
    options.Nip = "1234567890";
    options.AuthMethod = KsefAuthMethod.Token;
    options.KsefToken = "TWOJ_TOKEN";
    options.TimeoutSeconds = 120;
});
```

Rejestrowane serwisy:
- `IKSeFClient` - klient HTTP KSeF (z KSeF.Client)
- `IAuthCoordinator` - autoryzacja (token/certyfikat)
- `ICryptographyService` - szyfrowanie AES/RSA
- `IKsefSessionService` - zarządzanie sesjami
- `IKsefInvoiceSendService` - wysyłanie faktur
- `IKsefInvoiceReceiveService` - pobieranie faktur
- `IKsefInvoiceStatusService` - statusy faktur
- `IKsefInvoiceService` - modele i walidacja (z KSeF.Invoice)

> **Zasada:** Projekt nie używa klas statycznych - wszystko przez Dependency Injection zgodnie z konwencją CLAUDE.md.

## Testowanie

Biblioteka jest w pełni przetestowana. Testy znajdują się w projekcie **Tests/KSeF.Api.Tests**:

```bash
# Uruchomienie testów
dotnet test Tests/KSeF.Api.Tests

# Z pokryciem kodu
dotnet test Tests/KSeF.Api.Tests --collect:"XPlat Code Coverage"
```

Struktura testów:
- **Services/** - testy serwisów integracyjnych
  - `KsefSessionServiceTests.cs` - zarządzanie sesjami
  - `KsefInvoiceSendServiceTests.cs` - wysyłanie faktur
  - `KsefInvoiceReceiveServiceTests.cs` - odbieranie faktur
  - `KsefInvoiceStatusServiceTests.cs` - statusy faktur

Framework testowy:
- **xUnit 2.9.3** - framework testowy
- **Moq 4.20.72** - mockowanie interfejsów (IKSeFClient, IAuthCoordinator, ICryptographyService)
- **FluentAssertions 8.0.1** - czytelne asercje

> **Konwencja:** Testy izolowane bez połączenia z API KSeF. Każda klasa testowa mockuje IKSeFClient + specyficzne zależności.

## Dokumentacja

Pełna dokumentacja projektu KSeF dostępna w katalogu głównym:

- [README.md](../README.md) - dokumentacja główna
- [docs/getting-started.md](../docs/getting-started.md) - przewodnik dla początkujących
- [docs/ksef-api.md](../docs/ksef-api.md) - szczegóły integracji z API KSeF
- [docs/invoice-types.md](../docs/invoice-types.md) - wszystkie typy faktur
- [docs/architecture.md](../docs/architecture.md) - architektura rozwiązania

## Modele faktur

Biblioteka **KSeF.Api** automatycznie instaluje **KSeF.Invoice** (modele i walidacja). Zobacz:

- [KSeF.Invoice/README.md](../KSeF.Invoice/README.md) - dokumentacja modeli faktur
- [docs/validation.md](../docs/validation.md) - walidacja biznesowa i XSD

## Wymagania

### Runtime
- **.NET 8.0** lub **.NET 9.0**
- Windows, Linux lub macOS

### Pakiety NuGet
- `KSeF.Invoice` 1.0.0+ - modele i walidacja faktur
- `KSeF.Client` 1.0.0+ (z GitHub Packages CIRFMF) - klient HTTP KSeF
- `KSeF.Client.Core` 1.0.0+ - modele odpowiedzi API
- `KSeF.Client.ClientFactory` 1.0.0+ - fabryka klientów
- `Microsoft.Extensions.DependencyInjection` 9.0+ - DI container

### Autoryzacja
- Token KSeF (do środowisk testowych)
- Lub certyfikat X.509 w formacie PFX (do produkcji)

## Zależności

```
KSeF.Api
├── KSeF.Invoice (modele i walidacja faktur)
├── KSeF.Client (klient HTTP z GitHub Packages CIRFMF)
│   ├── IKSeFClient - agreguje 13 interfejsów API
│   ├── IAuthCoordinator - autoryzacja (token/certyfikat)
│   └── ICryptographyService - szyfrowanie AES/RSA
├── KSeF.Client.Core (modele odpowiedzi API)
└── Microsoft.Extensions.DependencyInjection
```

Pakiety `KSeF.Client.*` pochodzą z organizacji CIRFMF na GitHub Packages i wymagają PAT z uprawnieniem `read:packages`.

## Przykłady

Pełne przykłady użycia dostępne w projekcie **KSeF.Sample**:

```bash
cd KSeF.Sample
dotnet run
```

Przykłady obejmują:
- Wysyłanie faktury VAT
- Wysyłanie korekty faktury
- Wysyłanie faktury zaliczkowej i rozliczeniowej
- Pobieranie faktur zakupowych
- Sprawdzanie statusów faktur
- Autoryzacja tokenem i certyfikatem

## Licencja

Projekt udostępniony na licencji MIT. Zobacz plik [LICENSE](../LICENSE) dla szczegółów.

## Linki

### Dokumentacja projektu
- [Główna dokumentacja projektu](../README.md)
- [KSeF.Invoice - modele faktur](../KSeF.Invoice/README.md)
- [Dokumentacja dla początkujących](../docs/getting-started.md)

### Oficjalna dokumentacja KSeF
- [Portal KSeF (Ministerstwo Finansów)](https://www.podatki.gov.pl/ksef/)
- [API KSeF](https://ksef.mf.gov.pl/)
- [Schematy XSD FA(3)](https://www.podatki.gov.pl/ksef/struktury-dokumentow/)
- [FAQ KSeF](https://www.podatki.gov.pl/ksef/faq/)

### Powiązane projekty
- [KSeF Client C# (CIRFMF)](https://github.com/CIRFMF/ksef-client-csharp) - klient HTTP używany przez ten projekt
- [Dokumentacja dla integratorów](https://github.com/CIRFMF/ksef-docs) - szczegółowa dokumentacja API

---

**Część projektu KSeF** - kompletnego rozwiązania .NET do integracji z Krajowym Systemem e-Faktur.
