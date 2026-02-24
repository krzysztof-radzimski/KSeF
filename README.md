# KSeF - Krajowy System e-Faktur

Kompletne rozwiazanie .NET do integracji z **Krajowym Systemem e-Faktur (KSeF)** - tworzenie, walidacja, wysylanie i odbieranie faktur ustrukturyzowanych.

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

## Projekty w rozwiazaniu

| Projekt | Opis |
|---------|------|
| **KSeF.Invoice** | Modele danych, Fluent API, walidacja biznesowa/XSD, serializacja XML |
| **KSeF.Api** | Serwisy integracji z API KSeF - wysylanie i odbieranie faktur |
| **KSeF.Invoice.Tests** | Testy jednostkowe dla KSeF.Invoice |
| **KSeF.Api.Tests** | Testy jednostkowe dla KSeF.Api |
| **Samples** | Przyklady uzycia |

---

## Spis tresci

- [Instalacja](#instalacja)
- [Konfiguracja KSeF API](#konfiguracja-ksef-api)
- [Tworzenie faktur](#tworzenie-faktur)
  - [Standardowa faktura VAT](#standardowa-faktura-vat)
  - [Korekta faktury VAT](#korekta-faktury-vat)
  - [Faktura zaliczkowa](#faktura-zaliczkowa)
  - [Rozliczenie zaliczek](#rozliczenie-zaliczek)
  - [Faktura uproszczona](#faktura-uproszczona)
  - [Korekta faktury zaliczkowej](#korekta-faktury-zaliczkowej)
  - [Korekta faktury rozliczeniowej](#korekta-faktury-rozliczeniowej)
- [Wysylanie faktur do KSeF](#wysylanie-faktur-do-ksef)
- [Pobieranie faktur zakupowych](#pobieranie-faktur-zakupowych)
- [Sprawdzanie statusu faktur](#sprawdzanie-statusu-faktur)
- [Walidacja i serializacja](#walidacja-i-serializacja)
- [Testy jednostkowe](#testy-jednostkowe)
- [Architektura](#architektura)

---

## Instalacja

### 1. Konfiguracja zrodla pakietow GitHub Packages

Pakiety `KSeF.Client` sa dostepne w GitHub Packages organizacji CIRFMF. Wymagany jest Personal Access Token (PAT) z uprawnieniem `read:packages`.

```bash
dotnet nuget add source "https://nuget.pkg.github.com/CIRFMF/index.json" ^
  --name github-cirf ^
  --username token ^
  --password TWOJ_PAT_TOKEN ^
  --store-password-in-clear-text
```

### 2. Instalacja pakietow

```bash
# Modele faktur i walidacja
dotnet add package KSeF.Invoice

# Integracja z API KSeF (automatycznie instaluje KSeF.Invoice)
dotnet add package KSeF.Api
```

Lub recznie w pliku `.csproj`:

```xml
<PackageReference Include="KSeF.Api" Version="1.0.0" />
```

---

## Konfiguracja KSeF API

### appsettings.json

```json
{
  "KSeF": {
    "BaseUrl": "https://ksef-test.mf.gov.pl/api",
    "Nip": "1234567890",
    "AuthMethod": "Token",
    "KsefToken": "TWOJ_TOKEN_KSEF",
    "TimeoutSeconds": 120,
    "MaxRetries": 3,
    "SystemInfo": "MojSystem ERP 1.0"
  }
}
```

### Autoryzacja certyfikatem

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

Alternatywnie, certyfikat moze byc ladowany z magazynu Windows po thumbprincie:

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

### Srodowiska KSeF

| Srodowisko | Adres |
|------------|-------|
| Testowe | `https://ksef-test.mf.gov.pl/api` |
| Demo | `https://ksef-demo.mf.gov.pl/api` |
| Produkcyjne | `https://ksef.mf.gov.pl/api` |

### Rejestracja serwisow w ASP.NET

```csharp
using KSeF.Api;

var builder = WebApplication.CreateBuilder(args);

// Sposob 1: Z konfiguracji appsettings.json
builder.Services.AddKsefApiServices(builder.Configuration);

// Sposob 2: Z delegatem konfiguracji
builder.Services.AddKsefApiServices(options =>
{
    options.BaseUrl = KsefEnvironment.Test;
    options.Nip = "1234567890";
    options.AuthMethod = KsefAuthMethod.Token;
    options.KsefToken = "TWOJ_TOKEN";
    options.SystemInfo = "MojSystem 1.0";
});
```

Metoda `AddKsefApiServices` rejestruje automatycznie:
- **KSeF.Invoice** - modele, walidacja, serializacja
- **KSeF.Client** - komunikacja HTTP z API
- **IKsefSessionService** - zarzadzanie sesjami
- **IKsefInvoiceSendService** - wysylanie faktur
- **IKsefInvoiceReceiveService** - pobieranie faktur
- **IKsefInvoiceStatusService** - sprawdzanie statusow

---

## Tworzenie faktur

### Standardowa faktura VAT

```csharp
using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;

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

    public async Task<InvoiceSendResult> CreateAndSendVatInvoice()
    {
        var invoice = _invoiceService.CreateInvoice()
            .WithSeller(s => s
                .WithName("Firma ABC Sp. z o.o.")
                .WithTaxId("1234567890")
                .WithAddress(a => a
                    .WithStreet("ul. Przykladowa 1")
                    .WithCity("Warszawa")
                    .WithPostalCode("00-001")
                    .WithCountry(CountryCode.PL)))
            .WithBuyer(b => b
                .WithName("Klient XYZ S.A.")
                .WithTaxId("0987654321")
                .WithAddress(a => a
                    .WithStreet("ul. Kliencka 99")
                    .WithCity("Krakow")
                    .WithPostalCode("30-001")
                    .WithCountry(CountryCode.PL)))
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
                .WithPaymentMethod(PaymentMethod.BankTransfer)
                .WithPaymentDeadline(new DateOnly(2025, 3, 31))
                .WithBankAccount("PL61 1090 1014 0000 0712 1981 2874"))
            .Build();

        return await _sendService.SendVatInvoiceAsync(invoice);
    }
}
```

### Korekta faktury VAT

```csharp
var correction = _invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890")
        .WithAddress(a => a
            .WithStreet("ul. Przykladowa 1")
            .WithCity("Warszawa")
            .WithPostalCode("00-001")
            .WithCountry(CountryCode.PL)))
    .WithBuyer(b => b
        .WithName("Klient XYZ S.A.")
        .WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/001/KOR")
        .WithIssueDate(2025, 3, 15)
        .AsCorrection("Blad w cenie jednostkowej", corrected => corrected
            .WithOriginalInvoiceNumber("FV/2025/001")
            .WithOriginalIssueDate(new DateOnly(2025, 3, 1))))
    .AddLineItem(i => i
        .WithProductName("Usluga konsultingowa")
        .WithUnit("szt.")
        .WithQuantity(1)
        .WithUnitNetPrice(8000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(8000.00m)
        .WithVatAmount(1840.00m))
    .Build();

var result = await _sendService.SendCorrectionInvoiceAsync(correction);
```

### Faktura zaliczkowa

```csharp
var advance = _invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890")
        .WithAddress(a => a
            .WithStreet("ul. Przykladowa 1")
            .WithCity("Warszawa")
            .WithPostalCode("00-001")
            .WithCountry(CountryCode.PL)))
    .WithBuyer(b => b
        .WithName("Klient XYZ S.A.")
        .WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/ZAL/001")
        .WithIssueDate(2025, 2, 1)
        .AsAdvancePayment())
    .AddLineItem(i => i
        .WithProductName("Zaliczka na dostawa towaru wg zamowienia ZAM/2025/001")
        .WithUnit("szt.")
        .WithQuantity(1)
        .WithUnitNetPrice(5000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(5000.00m)
        .WithVatAmount(1150.00m))
    .WithPayment(p => p
        .WithPaymentMethod(PaymentMethod.BankTransfer)
        .WithPaymentDeadline(new DateOnly(2025, 2, 15)))
    .Build();

var result = await _sendService.SendAdvancePaymentInvoiceAsync(advance);
```

### Rozliczenie zaliczek

```csharp
var settlement = _invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890")
        .WithAddress(a => a
            .WithStreet("ul. Przykladowa 1")
            .WithCity("Warszawa")
            .WithPostalCode("00-001")
            .WithCountry(CountryCode.PL)))
    .WithBuyer(b => b
        .WithName("Klient XYZ S.A.")
        .WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/ROZ/001")
        .WithIssueDate(2025, 4, 1)
        .WithSaleDate(2025, 4, 1)
        .AsSettlement())
    .AddLineItem(i => i
        .WithProductName("Dostawa towaru wg zamowienia ZAM/2025/001 (rozliczenie zaliczki FV/2025/ZAL/001)")
        .WithUnit("szt.")
        .WithQuantity(10)
        .WithUnitNetPrice(2000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(20000.00m)
        .WithVatAmount(4600.00m))
    .Build();

var result = await _sendService.SendSettlementInvoiceAsync(settlement);
```

### Faktura uproszczona

```csharp
var simplified = _invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890")
        .WithAddress(a => a
            .WithStreet("ul. Przykladowa 1")
            .WithCity("Warszawa")
            .WithPostalCode("00-001")
            .WithCountry(CountryCode.PL)))
    .WithBuyer(b => b
        .WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/UPR/001")
        .WithIssueDate(2025, 3, 1)
        .AsSimplified())
    .AddLineItem(i => i
        .WithProductName("Usluga IT")
        .WithVatRate(VatRate.Rate23)
        .WithGrossAmount(123.00m))
    .Build();

var result = await _sendService.SendSimplifiedInvoiceAsync(simplified);
```

### Korekta faktury zaliczkowej

```csharp
var advanceCorrection = _invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890")
        .WithAddress(a => a
            .WithStreet("ul. Przykladowa 1")
            .WithCity("Warszawa")
            .WithPostalCode("00-001")
            .WithCountry(CountryCode.PL)))
    .WithBuyer(b => b
        .WithName("Klient XYZ S.A.")
        .WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/ZAL/001/KOR")
        .WithIssueDate(2025, 3, 1)
        .AsCorrection("Zmiana kwoty zaliczki", corrected => corrected
            .WithOriginalInvoiceNumber("FV/2025/ZAL/001")
            .WithOriginalIssueDate(new DateOnly(2025, 2, 1))))
    .AddLineItem(i => i
        .WithProductName("Korekta zaliczki - zmiana kwoty")
        .WithUnit("szt.")
        .WithQuantity(1)
        .WithUnitNetPrice(3000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(3000.00m)
        .WithVatAmount(690.00m))
    .Build();

var result = await _sendService.SendAdvancePaymentCorrectionAsync(advanceCorrection);
```

### Korekta faktury rozliczeniowej

```csharp
var settlementCorrection = _invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890")
        .WithAddress(a => a
            .WithStreet("ul. Przykladowa 1")
            .WithCity("Warszawa")
            .WithPostalCode("00-001")
            .WithCountry(CountryCode.PL)))
    .WithBuyer(b => b
        .WithName("Klient XYZ S.A.")
        .WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/ROZ/001/KOR")
        .WithIssueDate(2025, 5, 1)
        .AsCorrection("Korekta ilosci towaru", corrected => corrected
            .WithOriginalInvoiceNumber("FV/2025/ROZ/001")
            .WithOriginalIssueDate(new DateOnly(2025, 4, 1))))
    .AddLineItem(i => i
        .WithProductName("Dostawa towaru - korekta ilosci")
        .WithUnit("szt.")
        .WithQuantity(8)
        .WithUnitNetPrice(2000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(16000.00m)
        .WithVatAmount(3680.00m))
    .Build();

var result = await _sendService.SendSettlementCorrectionAsync(settlementCorrection);
```

---

## Wysylanie faktur do KSeF

### Wysylanie pojedynczej faktury

```csharp
public class InvoiceSendingService
{
    private readonly IKsefInvoiceSendService _sendService;

    public InvoiceSendingService(IKsefInvoiceSendService sendService)
    {
        _sendService = sendService;
    }

    public async Task SendSingleInvoice(Invoice invoice)
    {
        // Automatycznie otwiera i zamyka sesje KSeF
        var result = await _sendService.SendInvoiceAsync(invoice);

        if (result.Success)
        {
            Console.WriteLine($"Faktura wyslana! Ref: {result.ReferenceNumber}");
            Console.WriteLine($"Numer KSeF: {result.KsefNumber}");
        }
        else
        {
            Console.WriteLine($"Bledy: {string.Join(", ", result.Errors)}");
        }
    }
}
```

### Wysylanie wielu faktur w jednej sesji

```csharp
public async Task SendBatchInvoices(List<Invoice> invoices)
{
    // Otwiera jedna sesje dla wszystkich faktur
    var results = await _sendService.SendInvoicesAsync(invoices);

    foreach (var result in results)
    {
        if (result.Success)
            Console.WriteLine($"OK: {result.ReferenceNumber}");
        else
            Console.WriteLine($"BLAD: {string.Join(", ", result.Errors)}");
    }

    var successCount = results.Count(r => r.Success);
    Console.WriteLine($"Wyslano {successCount}/{results.Count} faktur");
}
```

### Wysylanie w istniejącej sesji (zaawansowane)

```csharp
public async Task SendWithExistingSession()
{
    var sessionService = /* IKsefSessionService z DI */;
    var sendService = /* IKsefInvoiceSendService z DI */;

    // Reczne zarzadzanie sesja
    var session = await sessionService.OpenSessionAsync();
    try
    {
        var result1 = await sendService.SendInvoiceAsync(invoice1, session);
        var result2 = await sendService.SendInvoiceAsync(invoice2, session);
    }
    finally
    {
        await sessionService.CloseSessionAsync(session);
    }
}
```

---

## Pobieranie faktur zakupowych

### Wyszukiwanie faktur po kryteriach

```csharp
public class PurchaseInvoiceService
{
    private readonly IKsefInvoiceReceiveService _receiveService;

    public PurchaseInvoiceService(IKsefInvoiceReceiveService receiveService)
    {
        _receiveService = receiveService;
    }

    public async Task SearchPurchaseInvoices()
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
}
```

### Pobieranie konkretnej faktury

```csharp
public async Task DownloadInvoice(string ksefNumber)
{
    var result = await _receiveService.GetInvoiceAsync(ksefNumber);

    if (result.Success)
    {
        // XML faktury
        Console.WriteLine(result.InvoiceXml);

        // Zdeserializowany model (jezeli deserializacja sie powiodla)
        if (result.Invoice != null)
        {
            Console.WriteLine($"Numer: {result.Invoice.InvoiceData.InvoiceNumber}");
            Console.WriteLine($"Kwota: {result.Invoice.InvoiceData.TotalAmount:C}");
        }
    }
}
```

### Pobieranie wszystkich nowych faktur zakupowych

```csharp
public async Task DownloadAllNewInvoices()
{
    // Pobiera wszystkie faktury zakupowe od podanej daty
    // Automatycznie iteruje po stronach wynikow
    var invoices = await _receiveService.DownloadPurchaseInvoicesAsync(
        fromDate: new DateTime(2025, 1, 1),
        toDate: DateTime.UtcNow);

    foreach (var inv in invoices.Where(i => i.Success))
    {
        Console.WriteLine($"Pobrano: {inv.KsefNumber}");
        // Zapisz XML lub przetworz model...
    }
}
```

### Pobieranie UPO (Urzedowe Poswiadczenie Odbioru)

```csharp
var upoResult = await _receiveService.GetUpoAsync("1234567890-20250301-ABCDEF-01");
if (upoResult.Success)
{
    Console.WriteLine("UPO pobrane pomyslnie");
}
```

---

## Sprawdzanie statusu faktur

### Status pojedynczej faktury

```csharp
public class StatusCheckService
{
    private readonly IKsefInvoiceStatusService _statusService;
    private readonly IKsefSessionService _sessionService;

    public StatusCheckService(
        IKsefInvoiceStatusService statusService,
        IKsefSessionService sessionService)
    {
        _statusService = statusService;
        _sessionService = sessionService;
    }

    public async Task CheckInvoiceStatus(string referenceNumber)
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
}
```

### Status faktur w sesji

```csharp
public async Task CheckSessionInvoices(string sessionReference, string accessToken)
{
    var result = await _statusService.GetSessionInvoicesStatusAsync(
        sessionReference, accessToken);

    if (result.Success)
    {
        Console.WriteLine($"Przetworzone: {result.ProcessedCount}");
        Console.WriteLine($"Odrzucone: {result.RejectedCount}");

        foreach (var inv in result.InvoiceStatuses)
        {
            Console.WriteLine($"  {inv.ReferenceNumber}: {inv.Status}");
        }
    }
}
```

### Uwaga o oznaczaniu faktur jako zaksiegowane

> **UWAGA:** KSeF API w wersji 2.0 nie udostepnia dedykowanego endpointu do oznaczania
> faktur zakupowych jako zaksiegowane. Ta funkcjonalnosc jest planowana w przyszlych
> wersjach API. Status ksiegowania nalezy sledzic w systemie ERP/ksiegowym uzytkownika.
> Serwis `IKsefInvoiceStatusService` pozwala na sprawdzanie statusu przetwarzania
> faktur w KSeF.

---

## Walidacja i serializacja

### Walidacja faktury

```csharp
var validationResult = _invoiceService.Validate(invoice);

if (validationResult.IsValid)
{
    Console.WriteLine("Faktura jest poprawna!");
}
else
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"Blad [{error.Code}]: {error.Message} (Pole: {error.FieldName})");
    }
}
```

### Serializacja do XML

```csharp
string xml = _invoiceService.ToXml(invoice);
byte[] bytes = _invoiceService.ToBytes(invoice);
```

### Deserializacja z XML

```csharp
var loadedInvoice = _invoiceService.FromXml(xml);
var loadedInvoice2 = _invoiceService.FromBytes(bytes);
```

### Walidacja XSD

```csharp
var xsdValidator = serviceProvider.GetRequiredService<IXsdValidator>();
var xsdResult = xsdValidator.Validate(invoice);
var xmlResult = xsdValidator.ValidateXml(xmlString, SchemaVersion.FA3);
```

---

## Typy faktur

| Typ | Enum | Metoda wysylania |
|-----|------|------------------|
| Faktura VAT | `InvoiceType.VAT` | `SendVatInvoiceAsync()` |
| Korekta VAT | `InvoiceType.KOR` | `SendCorrectionInvoiceAsync()` |
| Faktura zaliczkowa | `InvoiceType.ZAL` | `SendAdvancePaymentInvoiceAsync()` |
| Rozliczenie zaliczek | `InvoiceType.ROZ` | `SendSettlementInvoiceAsync()` |
| Faktura uproszczona | `InvoiceType.UPR` | `SendSimplifiedInvoiceAsync()` |
| Korekta zaliczki | `InvoiceType.KOR_ZAL` | `SendAdvancePaymentCorrectionAsync()` |
| Korekta rozliczenia | `InvoiceType.KOR_ROZ` | `SendSettlementCorrectionAsync()` |

## Stawki VAT

```csharp
// Stawki procentowe
VatRate.Rate23    // 23%
VatRate.Rate22    // 22%
VatRate.Rate8     // 8%
VatRate.Rate7     // 7%
VatRate.Rate5     // 5%
VatRate.Rate4     // 4%
VatRate.Rate3     // 3%

// Stawki zerowe
VatRate.Rate0Domestic              // 0% - dostawa krajowa
VatRate.Rate0IntraCommunitySupply  // 0% - WDT
VatRate.Rate0Export                // 0% - eksport

// Specjalne
VatRate.Exempt            // zw - zwolniony
VatRate.ReverseCharge     // np - odwrotne obciazenie
VatRate.NotSubjectToTaxI  // oo - niepodlegajacy I
VatRate.NotSubjectToTaxII // oo - niepodlegajacy II
```

---

## Testy jednostkowe

Projekt zawiera kompleksowe testy jednostkowe dla obu głównych bibliotek.

### Uruchomienie testów

```bash
# Wszystkie testy
dotnet test

# Testy dla KSeF.Invoice
dotnet test Tests/KSeF.Invoice.Tests

# Testy dla KSeF.Api
dotnet test Tests/KSeF.Api.Tests

# Testy z pokryciem kodu
dotnet test --collect:"XPlat Code Coverage"
```

### Struktura testów

#### KSeF.Invoice.Tests
- **Builders/** - testy Fluent API builders
- **Validation/** - testy walidacji biznesowej i XSD
- **Serialization/** - testy serializacji/deserializacji XML
- **Models/** - testy modeli danych

#### KSeF.Api.Tests
- **Services/** - testy serwisów integracyjnych
  - `KsefSessionServiceTests.cs` - zarządzanie sesjami
  - `KsefInvoiceSendServiceTests.cs` - wysyłanie faktur
  - `KsefInvoiceReceiveServiceTests.cs` - odbieranie faktur
  - `KsefInvoiceStatusServiceTests.cs` - statusy faktur

### Framework testowy

- **xUnit 2.9.3** - framework testowy
- **Moq 4.20.72** - mockowanie interfejsów (IKSeFClient, IAuthCoordinator, ICryptographyService)
- **FluentAssertions 8.0.1** - czytelne asercje
- Testy są izolowane i nie wymagają połączenia z API KSeF

### Przykład testu

```csharp
[Fact]
public async Task SendVatInvoiceAsync_WhenInvoiceIsValid_ReturnsSuccess()
{
    // Arrange
    var invoice = CreateValidInvoice();
    _mockClient.Setup(x => x.OpenOnlineSessionAsync(
        It.IsAny<OpenOnlineSessionRequest>(),
        It.IsAny<string>(),
        cancellationToken: It.IsAny<CancellationToken>()))
        .ReturnsAsync(new OpenOnlineSessionResponse
        {
            ReferenceNumber = "REF123"
        });

    // Act
    var result = await _sendService.SendVatInvoiceAsync(invoice);

    // Assert
    result.Success.Should().BeTrue();
    result.ReferenceNumber.Should().Be("REF123");
    _mockClient.Verify(x => x.CloseOnlineSessionAsync(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<CancellationToken>()), Times.Once);
}
```

---

## Architektura

### Struktura projektu

```
KSeF.sln
├── KSeF.Invoice/                      # Modele danych i serializacja
│   ├── IKsefInvoiceService.cs         # Glowny interfejs (fasada)
│   ├── Models/                        # Modele faktur
│   ├── Services/Builders/             # Fluent API builders
│   ├── Services/Serialization/        # Serializacja XML
│   ├── Services/Validation/           # Walidacja biznesowa i XSD
│   └── Schemas/                       # Schematy XSD (FA3)
│
├── KSeF.Api/                          # Integracja z API KSeF
│   ├── Configuration/                 # Konfiguracja (opcje, certyfikaty)
│   │   ├── KsefApiOptions.cs          # Opcje polaczenia
│   │   ├── KsefAuthMethod.cs          # Metody autoryzacji
│   │   ├── KsefCertificateOptions.cs  # Konfiguracja certyfikatu
│   │   └── KsefEnvironment.cs         # Adresy srodowisk
│   ├── Models/                        # Modele wynikow API
│   │   ├── InvoiceSendResult.cs       # Wynik wysylania
│   │   ├── InvoiceDownloadResult.cs   # Wynik pobierania
│   │   ├── InvoiceQueryResult.cs      # Wynik wyszukiwania
│   │   ├── InvoiceStatusResult.cs     # Status faktury
│   │   └── SessionInfo.cs             # Informacje o sesji
│   ├── Services/                      # Serwisy biznesowe
│   │   ├── IKsefSessionService.cs     # Zarzadzanie sesjami
│   │   ├── IKsefInvoiceSendService.cs # Wysylanie faktur
│   │   ├── IKsefInvoiceReceiveService.cs # Pobieranie faktur
│   │   └── IKsefInvoiceStatusService.cs  # Statusy faktur
│   └── ServiceCollectionExtensions.cs # Rejestracja DI
│
├── KSeF.Invoice.Tests/                # Testy jednostkowe dla KSeF.Invoice
├── KSeF.Api.Tests/                    # Testy jednostkowe dla KSeF.Api
└── Samples/                           # Przyklady uzycia
```

### Glowne interfejsy

```
IKsefInvoiceService          - Tworzenie, walidacja, serializacja faktur
│
IKsefInvoiceSendService      - Wysylanie faktur do KSeF (7 typow)
├── SendVatInvoiceAsync()
├── SendCorrectionInvoiceAsync()
├── SendAdvancePaymentInvoiceAsync()
├── SendSettlementInvoiceAsync()
├── SendSimplifiedInvoiceAsync()
├── SendAdvancePaymentCorrectionAsync()
└── SendSettlementCorrectionAsync()
│
IKsefInvoiceReceiveService   - Pobieranie faktur zakupowych
├── GetInvoiceAsync()
├── QueryPurchaseInvoicesAsync()
├── DownloadPurchaseInvoicesAsync()
└── GetUpoAsync()
│
IKsefSessionService          - Zarzadzanie sesjami KSeF
├── OpenSessionAsync()
├── CloseSessionAsync()
└── RefreshSessionAsync()
│
IKsefInvoiceStatusService    - Statusy faktur
├── GetInvoiceStatusAsync()
└── GetSessionInvoicesStatusAsync()
```

### Wzorce projektowe

- **Dependency Injection** - Wszystkie serwisy wstrzykiwane przez DI
- **Builder Pattern** - Fluent API do budowania faktur
- **Strategy Pattern** - Rozne strategie walidacji i serializacji
- **Facade Pattern** - `IKsefInvoiceService` jako punkt wejscia

---

## Wymagania

- .NET 8.0 / 9.0 lub nowszy
- Pakiety NuGet:
  - `KSeF.Client` (z GitHub Packages CIRFMF)
  - `KSeF.Client.ClientFactory` (z GitHub Packages CIRFMF)
  - `Microsoft.Extensions.DependencyInjection` 9.0+

## Licencja

Projekt udostepniony na licencji MIT. Zobacz plik [LICENSE](LICENSE).

## Linki

- [Dokumentacja KSeF](https://www.podatki.gov.pl/ksef/)
- [Schematy XSD KSeF](https://www.podatki.gov.pl/ksef/struktury-dokumentow/)
- [API KSeF](https://ksef.mf.gov.pl/)
- [KSeF Client C# (CIRFMF)](https://github.com/CIRFMF/ksef-client-csharp)
- [Dokumentacja dla integratorow](https://github.com/CIRFMF/ksef-docs)
