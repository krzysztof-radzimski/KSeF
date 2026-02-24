# Pierwsze kroki z KSeF

Przewodnik szybkiego startu dla bibliotek KSeF.Invoice i KSeF.Api.

## Wymagania

- .NET 9.0 SDK
- Pakiety NuGet z GitHub Packages CIRFMF (dla KSeF.Api)

## Instalacja

### 1. Dodanie referencji do KSeF.Invoice

```bash
dotnet add package KSeF.Invoice
```

### 2. Dodanie referencji do KSeF.Api (opcjonalnie)

Pakiety `KSeF.Client` sa hostowane w GitHub Packages. Wymagany jest Personal Access Token (PAT) z uprawnieniem `read:packages`.

```bash
# Dodanie zrodla NuGet
dotnet nuget add source "https://nuget.pkg.github.com/CIRFMF/index.json" \
  --name github-cirf \
  --username token \
  --password TWOJ_PAT_TOKEN \
  --store-password-in-clear-text

# Instalacja pakietu
dotnet add package KSeF.Api
```

## Szybki start - tworzenie faktury

```csharp
using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using Microsoft.Extensions.DependencyInjection;

// 1. Konfiguracja DI
var services = new ServiceCollection();
services.AddKsefInvoiceServices(options =>
{
    options.SchemaVersion = SchemaVersion.FA3;
    options.ValidateBeforeSerialize = true;
});

var provider = services.BuildServiceProvider();
var invoiceService = provider.GetRequiredService<IKsefInvoiceService>();

// 2. Tworzenie faktury
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
        .WithUnitNetPrice(1000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(1000.00m)
        .WithVatAmount(230.00m))
    .WithPayment(p => p
        .AddPaymentTerm(2025, 3, 31)
        .AsBankTransfer("PL61109010140000071219812874"))
    .Build();

// 3. Walidacja
var result = invoiceService.Validate(invoice);
Console.WriteLine(result.IsValid ? "Faktura poprawna!" : "Bledy walidacji");

// 4. Serializacja do XML
string xml = invoiceService.ToXml(invoice);
```

## Szybki start - integracja z API KSeF

```csharp
using KSeF.Api;
using KSeF.Api.Configuration;
using KSeF.Api.Services;

// 1. Konfiguracja DI z KSeF.Api
var services = new ServiceCollection();
services.AddKsefApiServices(options =>
{
    options.BaseUrl = KsefEnvironment.Test;
    options.Nip = "1234567890";
    options.AuthMethod = KsefAuthMethod.Token;
    options.KsefToken = "TWOJ_TOKEN_KSEF";
    options.SystemInfo = "MojSystem 1.0";
});

var provider = services.BuildServiceProvider();
var sendService = provider.GetRequiredService<IKsefInvoiceSendService>();

// 2. Wyslanie faktury
var result = await sendService.SendVatInvoiceAsync(invoice);
if (result.Success)
{
    Console.WriteLine($"Wyslano! KSeF: {result.KsefNumber}");
}
```

## Nastepne kroki

- [Architektura projektu](architecture.md) - Szczegolowy opis architektury
- [API KSeF](ksef-api.md) - Integracja z API KSeF
- [Tworzenie faktur](invoice-types.md) - Wszystkie typy faktur
- [Walidacja](validation.md) - Walidacja biznesowa i XSD
