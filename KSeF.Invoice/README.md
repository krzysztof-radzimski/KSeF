# KSeF.Invoice

Biblioteka .NET do tworzenia, walidacji i serializacji faktur ustrukturyzowanych zgodnych z **Krajowym Systemem e-Faktur (KSeF)** oraz schematem FA(3).

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](../LICENSE)
[![Target Framework](https://img.shields.io/badge/Framework-net8.0%20%7C%20net9.0-purple)](https://dotnet.microsoft.com/)

## Opis

**KSeF.Invoice** to biblioteka oferująca:
- **Modele danych** - kompleksowe modele wszystkich typów faktur KSeF
- **Fluent API** - intuicyjne tworzenie faktur za pomocą builderów
- **Walidacja** - biznesowa i XSD przed wysłaniem do KSeF
- **Serializacja XML** - pełna obsługa konwersji między modelem a XML FA(3)
- **7 typów faktur** - VAT, korekty, zaliczki, rozliczenia, uproszczone

## Instalacja

```bash
dotnet add package KSeF.Invoice
```

Lub w pliku `.csproj`:

```xml
<PackageReference Include="KSeF.Invoice" Version="1.0.0" />
```

## Szybki start

### 1. Rejestracja serwisów

```csharp
using KSeF.Invoice;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddKSeFInvoice();
var provider = services.BuildServiceProvider();

var invoiceService = provider.GetRequiredService<IKsefInvoiceService>();
```

### 2. Tworzenie faktury VAT

```csharp
var invoice = invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890")
        .WithAddress(a => a
            .WithCity("Warszawa")
            .WithPostalCode("00-001")
            .WithStreet("ul. Przykładowa 1")))
    .WithBuyer(b => b
        .WithName("Klient XYZ S.A.")
        .WithTaxId("0987654321")
        .WithAddress(a => a
            .WithCity("Kraków")
            .WithPostalCode("30-001")))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/001")
        .WithIssueDate(2025, 3, 1)
        .WithSaleDate(2025, 3, 1)
        .WithCurrency(CurrencyCode.PLN))
    .AddLineItem(i => i
        .WithProductName("Usługa konsultingowa")
        .WithUnit("szt.")
        .WithQuantity(1)
        .WithUnitNetPrice(10000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(10000.00m)
        .WithVatAmount(2300.00m))
    .Build();
```

### 3. Walidacja

```csharp
var validationResult = invoiceService.Validate(invoice);

if (validationResult.IsValid)
{
    Console.WriteLine("Faktura jest poprawna!");
}
else
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"Błąd [{error.Code}]: {error.Message}");
    }
}
```

### 4. Serializacja do XML

```csharp
string xml = invoiceService.ToXml(invoice);
byte[] bytes = invoiceService.ToBytes(invoice);

// Zapis do pliku
File.WriteAllText("faktura.xml", xml);
```

### 5. Deserializacja z XML

```csharp
string xml = File.ReadAllText("faktura.xml");
var loadedInvoice = invoiceService.FromXml(xml);
```

## Typy faktur

Biblioteka obsługuje wszystkie 7 typów faktur zgodnie ze schematem FA(3):

| Typ faktury | Builder method | Opis |
|-------------|---------------|------|
| **Faktura VAT** | `.Build()` | Standardowa faktura sprzedażowa |
| **Korekta VAT** | `.AsCorrection(...)` | Korekta do faktury VAT |
| **Faktura zaliczkowa** | `.AsAdvancePayment()` | Faktura za zaliczkę |
| **Rozliczenie zaliczek** | `.AsSettlement()` | Rozliczenie wcześniejszych zaliczek |
| **Faktura uproszczona** | `.AsSimplified()` | Faktura uproszczona (do 450 PLN) |
| **Korekta zaliczkowa** | `.AsCorrection(...)` + `.AsAdvancePayment()` | Korekta faktury zaliczkowej |
| **Korekta rozliczeniowa** | `.AsCorrection(...)` + `.AsSettlement()` | Korekta faktury rozliczeniowej |

### Przykład korekty faktury

```csharp
var correction = invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890"))
    .WithBuyer(b => b
        .WithName("Klient XYZ S.A.")
        .WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/001/KOR")
        .WithIssueDate(2025, 3, 15)
        .AsCorrection("Błąd w cenie jednostkowej", corrected => corrected
            .WithOriginalInvoiceNumber("FV/2025/001")
            .WithOriginalIssueDate(new DateOnly(2025, 3, 1))))
    .AddLineItem(i => i
        .WithProductName("Usługa konsultingowa - korekta")
        .WithUnitNetPrice(8000.00m)
        .WithVatRate(VatRate.Rate23))
    .Build();
```

### Przykład faktury zaliczkowej

```csharp
var advance = invoiceService.CreateInvoice()
    .WithSeller(s => s
        .WithName("Firma ABC Sp. z o.o.")
        .WithTaxId("1234567890"))
    .WithBuyer(b => b
        .WithName("Klient XYZ S.A.")
        .WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/ZAL/001")
        .WithIssueDate(2025, 2, 1)
        .AsAdvancePayment())
    .AddLineItem(i => i
        .WithProductName("Zaliczka na dostawę towaru")
        .WithUnitNetPrice(5000.00m)
        .WithVatRate(VatRate.Rate23))
    .Build();
```

## Walidacja

Biblioteka oferuje dwa poziomy walidacji:

### 1. Walidacja biznesowa

Sprawdza logiczną poprawność faktury:
- Wymagane pola (NIP, nazwa, numer faktury, daty)
- Formaty (NIP, kody pocztowe, numery rachunków)
- Kwoty (zgodność netto, VAT, brutto)
- Daty (data wystawienia nie może być z przyszłości)
- Pozycje faktury (co najmniej jedna pozycja)

```csharp
var validationResult = invoiceService.Validate(invoice);

if (!validationResult.IsValid)
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"[{error.Code}] {error.FieldName}: {error.Message}");
    }
}
```

### 2. Walidacja XSD

Sprawdza zgodność XML ze schematem FA(3):

```csharp
var xsdValidator = provider.GetRequiredService<IXsdValidator>();

// Walidacja modelu
var xsdResult = xsdValidator.Validate(invoice);

// Walidacja XML
string xml = invoiceService.ToXml(invoice);
var xmlResult = xsdValidator.ValidateXml(xml, SchemaVersion.FA3);

if (!xmlResult.IsValid)
{
    foreach (var error in xmlResult.Errors)
    {
        Console.WriteLine($"Błąd XSD: {error}");
    }
}
```

## Serializacja

### XML

```csharp
// Model -> XML
string xml = invoiceService.ToXml(invoice);

// XML -> Model
var invoice = invoiceService.FromXml(xml);
```

### Bytes

```csharp
// Model -> Bytes (UTF-8)
byte[] bytes = invoiceService.ToBytes(invoice);

// Bytes -> Model
var invoice = invoiceService.FromBytes(bytes);
```

### Zapis do pliku

```csharp
string xml = invoiceService.ToXml(invoice);
File.WriteAllText("faktura_FV_2025_001.xml", xml);
```

## Stawki VAT

Biblioteka definiuje wszystkie stawki VAT zgodne z polskim prawem:

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
VatRate.Rate0IntraCommunitySupply  // 0% - WDT (wewnątrzwspólnotowa dostawa towarów)
VatRate.Rate0Export                // 0% - eksport

// Specjalne
VatRate.Exempt            // zw - zwolniony z VAT
VatRate.ReverseCharge     // np - odwrotne obciążenie
VatRate.NotSubjectToTaxI  // oo - niepodlegający I
VatRate.NotSubjectToTaxII // oo - niepodlegający II
```

## Struktura projektu

```
KSeF.Invoice/
├── IKsefInvoiceService.cs          # Główny interfejs (fasada)
├── KsefInvoiceService.cs           # Implementacja
├── ServiceCollectionExtensions.cs  # Rejestracja DI (AddKSeFInvoice)
├── Models/                         # Modele danych faktur
│   ├── Invoice.cs                  # Model główny faktury
│   ├── InvoiceHeader.cs            # Nagłówek faktury
│   ├── Seller.cs                   # Sprzedawca
│   ├── Buyer.cs                    # Nabywca
│   ├── InvoiceLineItem.cs          # Pozycja faktury
│   ├── Payment.cs                  # Informacje o płatności
│   └── Enums/                      # Enumy (VatRate, CurrencyCode, etc.)
├── Services/
│   ├── Builders/                   # Fluent API builders
│   │   ├── InvoiceBuilder.cs
│   │   ├── SellerBuilder.cs
│   │   ├── BuyerBuilder.cs
│   │   └── LineItemBuilder.cs
│   ├── Serialization/              # Serializacja/deserializacja XML
│   │   ├── IInvoiceSerializer.cs
│   │   └── InvoiceXmlSerializer.cs
│   └── Validation/                 # Walidacja
│       ├── IInvoiceValidator.cs
│       ├── BusinessValidator.cs
│       ├── IXsdValidator.cs
│       └── XsdValidator.cs
└── Schemas/                        # Schematy XSD
    └── FA_3_0.xsd
```

## Dependency Injection

Biblioteka w pełni obsługuje Microsoft.Extensions.DependencyInjection:

```csharp
using KSeF.Invoice;
using Microsoft.Extensions.DependencyInjection;

// Rejestracja serwisów
services.AddKSeFInvoice();

// Opcjonalna konfiguracja
services.AddKSeFInvoice(options =>
{
    options.EnableXsdValidation = true;
    options.SchemaVersion = SchemaVersion.FA3;
});
```

Rejestrowane serwisy:
- `IKsefInvoiceService` - główny serwis (fasada)
- `IInvoiceBuilder` - builder faktur
- `IInvoiceSerializer` - serializacja XML
- `IInvoiceValidator` - walidacja biznesowa
- `IXsdValidator` - walidacja XSD

> **Zasada:** Projekt nie używa klas statycznych - wszystko przez Dependency Injection zgodnie z konwencją CLAUDE.md.

## Testowanie

Biblioteka jest w pełni przetestowana. Testy znajdują się w projekcie **Tests/KSeF.Invoice.Tests**:

```bash
# Uruchomienie testów
dotnet test Tests/KSeF.Invoice.Tests

# Z pokryciem kodu
dotnet test Tests/KSeF.Invoice.Tests --collect:"XPlat Code Coverage"
```

Struktura testów:
- **Builders/** - testy Fluent API builders
- **Validation/** - testy walidacji biznesowej i XSD
- **Serialization/** - testy serializacji/deserializacji XML
- **Models/** - testy modeli danych

Framework testowy:
- **xUnit 2.9.3** - framework testowy
- **FluentAssertions 8.0.1** - czytelne asercje

## Dokumentacja

Pełna dokumentacja projektu KSeF dostępna w katalogu głównym:

- [README.md](../README.md) - dokumentacja główna
- [docs/getting-started.md](../docs/getting-started.md) - przewodnik dla początkujących
- [docs/invoice-types.md](../docs/invoice-types.md) - szczegóły wszystkich typów faktur
- [docs/validation.md](../docs/validation.md) - walidacja biznesowa i XSD
- [docs/architecture.md](../docs/architecture.md) - architektura rozwiązania

## Integracja z API KSeF

Ta biblioteka zawiera tylko modele i walidację. Do wysyłania i odbierania faktur użyj biblioteki **KSeF.Api**:

```bash
dotnet add package KSeF.Api
```

Biblioteka **KSeF.Api** automatycznie referencuje **KSeF.Invoice** i dodaje:
- Wysyłanie faktur do KSeF (wszystkie 7 typów)
- Pobieranie faktur zakupowych
- Zarządzanie sesjami KSeF
- Sprawdzanie statusów faktur

Zobacz [KSeF.Api/README.md](../KSeF.Api/README.md) dla szczegółów.

## Wymagania

- **.NET 8.0** lub **.NET 9.0**
- `Microsoft.Extensions.DependencyInjection` 9.0+

## Licencja

Projekt udostępniony na licencji MIT. Zobacz plik [LICENSE](../LICENSE) dla szczegółów.

## Linki

- [Główna dokumentacja projektu](../README.md)
- [KSeF.Api - integracja z API](../KSeF.Api/README.md)
- [Dokumentacja KSeF (Ministerstwo Finansów)](https://www.podatki.gov.pl/ksef/)
- [Schematy XSD FA(3)](https://www.podatki.gov.pl/ksef/struktury-dokumentow/)

---

**Część projektu KSeF** - kompletnego rozwiązania .NET do integracji z Krajowym Systemem e-Faktur.
