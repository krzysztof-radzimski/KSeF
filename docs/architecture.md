# Architektura projektu KSeF

## Struktura rozwiazania

```
KSeF.sln
├── KSeF.Invoice/                         # Biblioteka modeli i serializacji
│   ├── IKsefInvoiceService.cs            # Glowny interfejs (fasada)
│   ├── KsefInvoiceService.cs             # Implementacja fasady
│   ├── KsefInvoiceServiceOptions.cs      # Opcje konfiguracji
│   ├── ServiceCollectionExtensions.cs    # Rejestracja DI: AddKsefInvoiceServices()
│   ├── Models/                           # Modele danych faktur
│   │   ├── Invoice.cs                    # Model faktury
│   │   ├── InvoiceData.cs                # Dane faktury (naglowek, typ, numer)
│   │   ├── InvoiceLineItem.cs            # Pozycja faktury
│   │   ├── Common/                       # Wspolne modele (adres, NIP, konto)
│   │   ├── Entities/                     # Podmioty (sprzedawca, nabywca, trzeci)
│   │   ├── Enums/                        # Enumeracje (stawki VAT, waluty, typy)
│   │   ├── Corrections/                  # Modele korekt
│   │   ├── Payments/                     # Modele platnosci
│   │   ├── Summary/                      # Podsumowania VAT
│   │   └── Attachments/                  # Zalaczniki i stopka
│   ├── Services/
│   │   ├── Builders/                     # Fluent API builders
│   │   │   ├── InvoiceBuilder.cs         # Glowny builder faktury
│   │   │   ├── SellerBuilder.cs          # Builder sprzedawcy
│   │   │   ├── BuyerBuilder.cs           # Builder nabywcy
│   │   │   ├── InvoiceDetailsBuilder.cs  # Builder danych faktury
│   │   │   ├── LineItemBuilder.cs        # Builder pozycji
│   │   │   └── PaymentBuilder.cs         # Builder platnosci
│   │   ├── Serialization/                # Serializacja XML
│   │   │   ├── IInvoiceSerializer.cs     # Interfejs serializatora
│   │   │   └── KsefInvoiceSerializer.cs  # Implementacja XML KSeF FA(3)
│   │   └── Validation/                   # Walidacja
│   │       ├── IInvoiceValidator.cs      # Walidacja biznesowa
│   │       ├── IXsdValidator.cs          # Walidacja XSD
│   │       ├── INipValidator.cs          # Walidacja NIP
│   │       ├── IIbanValidator.cs         # Walidacja IBAN
│   │       └── IDateValidator.cs         # Walidacja dat
│   └── Schemas/                          # Schematy XSD
│       ├── FA3.xsd                       # Schemat faktury FA(3)
│       └── StrukturyDanych.xsd           # Wspolne struktury danych
│
├── KSeF.Api/                             # Biblioteka integracji z API KSeF
│   ├── Configuration/                    # Konfiguracja polaczenia
│   │   ├── KsefApiOptions.cs             # Opcje: URL, NIP, token, timeout
│   │   ├── KsefAuthMethod.cs             # Enum: Token, Certificate
│   │   ├── KsefCertificateOptions.cs     # Konfiguracja certyfikatu
│   │   └── KsefEnvironment.cs            # Adresy srodowisk (Test/Demo/Prod)
│   ├── Models/                           # Modele wynikow API
│   │   ├── InvoiceSendResult.cs          # Wynik wysylania
│   │   ├── InvoiceDownloadResult.cs      # Wynik pobierania
│   │   ├── InvoiceQueryResult.cs         # Wynik wyszukiwania
│   │   ├── InvoiceQueryCriteria.cs       # Kryteria wyszukiwania
│   │   ├── InvoiceStatusResult.cs        # Status faktury
│   │   ├── SessionInfo.cs                # Informacje o sesji
│   │   ├── SessionInvoicesResult.cs      # Status faktur w sesji
│   │   └── KsefOperationResult.cs        # Ogolny wynik operacji
│   ├── Services/                         # Serwisy biznesowe
│   │   ├── IKsefSessionService.cs        # Zarzadzanie sesjami
│   │   ├── KsefSessionService.cs         # Implementacja sesji
│   │   ├── IKsefInvoiceSendService.cs    # Wysylanie faktur
│   │   ├── KsefInvoiceSendService.cs     # Implementacja wysylania
│   │   ├── IKsefInvoiceReceiveService.cs # Pobieranie faktur
│   │   ├── KsefInvoiceReceiveService.cs  # Implementacja pobierania
│   │   ├── IKsefInvoiceStatusService.cs  # Statusy faktur
│   │   └── KsefInvoiceStatusService.cs   # Implementacja statusow
│   └── ServiceCollectionExtensions.cs    # Rejestracja DI: AddKsefApiServices()
│
├── Tests/
│   ├── KSeF.Invoice.Tests/              # Testy KSeF.Invoice
│   └── KSeF.Api.Tests/                  # Testy KSeF.Api
│
└── KSeF.Sample/                          # Przyklady uzycia
```

## Wzorce projektowe

### Dependency Injection
Wszystkie serwisy sa rejestrowane przez extension methods na `IServiceCollection`:

```csharp
// Tylko KSeF.Invoice (modele, walidacja, serializacja)
services.AddKsefInvoiceServices(options => { ... });

// KSeF.Api + KSeF.Invoice (pelna integracja z API)
services.AddKsefApiServices(options => { ... });
```

`AddKsefApiServices()` automatycznie wywoluje `AddKsefInvoiceServices()` oraz rejestruje klienta HTTP KSeF.

### Builder Pattern (Fluent API)
Tworzenie faktur odbywa sie przez Fluent API:

```csharp
var invoice = invoiceService.CreateInvoice()
    .WithSeller(s => s.WithTaxId("...").WithName("..."))
    .WithBuyer(b => b.WithTaxId("...").WithName("..."))
    .WithInvoiceDetails(d => d.WithInvoiceNumber("..."))
    .AddLineItem(i => i.WithProductName("..."))
    .Build();
```

### Facade Pattern
`IKsefInvoiceService` jest fasada laczaca tworzenie, walidacje i serializacje faktur:

```csharp
public interface IKsefInvoiceService
{
    InvoiceBuilder CreateInvoice();
    ValidationResult Validate(Invoice invoice);
    string ToXml(Invoice invoice);
    byte[] ToBytes(Invoice invoice);
    Invoice? FromXml(string xml);
    Invoice? FromBytes(byte[] bytes);
}
```

## Glowne interfejsy

### KSeF.Invoice

| Interfejs | Opis |
|-----------|------|
| `IKsefInvoiceService` | Fasada: tworzenie, walidacja, serializacja |
| `IInvoiceSerializer` | Serializacja/deserializacja XML |
| `IInvoiceValidator` | Walidacja biznesowa |
| `IXsdValidator` | Walidacja XSD |
| `INipValidator` | Walidacja numerow NIP |
| `IIbanValidator` | Walidacja numerow IBAN |
| `IDateValidator` | Walidacja dat |

### KSeF.Api

| Interfejs | Opis |
|-----------|------|
| `IKsefSessionService` | Otwieranie, zamykanie, odswiezanie sesji |
| `IKsefInvoiceSendService` | Wysylanie faktur (7 typow) |
| `IKsefInvoiceReceiveService` | Pobieranie faktur zakupowych, UPO |
| `IKsefInvoiceStatusService` | Sprawdzanie statusow faktur i sesji |

## Zaleznosci zewnetrzne

### KSeF.Client (CIRFMF)
Pakiety z GitHub Packages realizujace komunikacje HTTP z API KSeF:

| Pakiet | Opis |
|--------|------|
| `KSeF.Client` | Klient HTTP, modele request/response |
| `KSeF.Client.Core` | Interfejsy bazowe, kryptografia |
| `KSeF.Client.ClientFactory` | Fabryka klienta, rejestracja DI |

### Microsoft.Extensions
- `Microsoft.Extensions.DependencyInjection` - Kontener DI
- `Microsoft.Extensions.Options` - Wzorzec Options
- `Microsoft.Extensions.Logging.Abstractions` - Logowanie
- `Microsoft.Extensions.Configuration` - Konfiguracja (KSeF.Api)
