# Integracja z API KSeF

Dokumentacja modulu KSeF.Api - komunikacja z Krajowym Systemem e-Faktur.

## Konfiguracja

### Konfiguracja z delegatem

```csharp
services.AddKsefApiServices(options =>
{
    options.BaseUrl = KsefEnvironment.Test;
    options.Nip = "1234567890";
    options.AuthMethod = KsefAuthMethod.Token;
    options.KsefToken = "TWOJ_TOKEN_KSEF";
    options.TimeoutSeconds = 120;
    options.MaxRetries = 3;
    options.SystemInfo = "MojSystem ERP 1.0";
});
```

### Konfiguracja z appsettings.json

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

```csharp
services.AddKsefApiServices(builder.Configuration);
```

### Autoryzacja certyfikatem

```json
{
  "KSeF": {
    "AuthMethod": "Certificate",
    "Certificate": {
      "CertificatePath": "C:\\Certs\\ksef-cert.pfx",
      "CertificatePassword": "haslo"
    }
  }
}
```

Certyfikat moze byc ladowany z magazynu Windows po thumbprincie:

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

| Srodowisko | Stala | Adres |
|------------|-------|-------|
| Testowe | `KsefEnvironment.Test` | `https://ksef-test.mf.gov.pl/api` |
| Demo | `KsefEnvironment.Demo` | `https://ksef-demo.mf.gov.pl/api` |
| Produkcyjne | `KsefEnvironment.Production` | `https://ksef.mf.gov.pl/api` |

## Zarejestrowane serwisy

Wywolanie `AddKsefApiServices()` rejestruje:

| Serwis | Opis |
|--------|------|
| `IKsefInvoiceService` | Tworzenie, walidacja, serializacja (z KSeF.Invoice) |
| `IKsefSessionService` | Zarzadzanie sesjami KSeF |
| `IKsefInvoiceSendService` | Wysylanie faktur do KSeF |
| `IKsefInvoiceReceiveService` | Pobieranie faktur z KSeF |
| `IKsefInvoiceStatusService` | Sprawdzanie statusow |
| `IKSeFClient` | Klient HTTP (z KSeF.Client) |
| `IAuthCoordinator` | Koordynator autoryzacji |
| `ICryptographyService` | Serwis kryptograficzny (AES/RSA) |

## Sesje

### Otwieranie sesji

```csharp
var session = await sessionService.OpenSessionAsync();
// session.SessionReference - numer referencyjny
// session.AccessToken - token dostepu
// session.IsActive - czy sesja jest aktywna
```

### Zamykanie sesji

```csharp
await sessionService.CloseSessionAsync(session);
```

### Odswiezanie tokenu

```csharp
var refreshed = await sessionService.RefreshSessionAsync(session);
```

## Wysylanie faktur

### Pojedyncza faktura (automatyczna sesja)

```csharp
var result = await sendService.SendInvoiceAsync(invoice);
// lub typ-specyficznie:
var result = await sendService.SendVatInvoiceAsync(invoice);
```

### Wiele faktur w jednej sesji

```csharp
var results = await sendService.SendInvoicesAsync(invoices);
foreach (var r in results.Where(r => r.Success))
{
    Console.WriteLine($"KSeF: {r.KsefNumber}");
}
```

### Faktura w istniejacej sesji

```csharp
var session = await sessionService.OpenSessionAsync();
try
{
    await sendService.SendInvoiceAsync(invoice1, session);
    await sendService.SendInvoiceAsync(invoice2, session);
}
finally
{
    await sessionService.CloseSessionAsync(session);
}
```

### Wynik wysylania (InvoiceSendResult)

| Pole | Typ | Opis |
|------|-----|------|
| `Success` | `bool` | Czy operacja sie powiodla |
| `ReferenceNumber` | `string?` | Numer referencyjny |
| `KsefNumber` | `string?` | Numer KSeF faktury |
| `SessionReference` | `string?` | Numer sesji |
| `ProcessingTimestamp` | `DateTime?` | Czas przetworzenia |
| `Errors` | `List<string>` | Lista bledow |
| `InvoiceXml` | `string?` | XML wyslany do KSeF |

## Pobieranie faktur

### Pobranie konkretnej faktury

```csharp
var result = await receiveService.GetInvoiceAsync("1234567890-20250301-ABCDEF-01");
if (result.Success)
{
    string xml = result.InvoiceXml;
    Invoice? invoice = result.Invoice;
}
```

### Wyszukiwanie faktur zakupowych

```csharp
var criteria = new InvoiceQueryCriteria
{
    DateFrom = new DateTime(2025, 1, 1),
    DateTo = new DateTime(2025, 3, 31),
    Direction = InvoiceDirection.Purchase,
    PageSize = 50
};

var result = await receiveService.QueryPurchaseInvoicesAsync(criteria);
foreach (var inv in result.Invoices)
{
    Console.WriteLine($"{inv.KsefNumber} | {inv.SellerNip} | {inv.GrossAmount:C}");
}
```

### Pobieranie wszystkich nowych faktur

```csharp
var invoices = await receiveService.DownloadPurchaseInvoicesAsync(
    fromDate: new DateTime(2025, 1, 1),
    toDate: DateTime.UtcNow);
// Automatycznie iteruje po stronach wynikow
```

### Pobieranie UPO

```csharp
var upo = await receiveService.GetUpoAsync("1234567890-20250301-ABCDEF-01");
```

## Statusy faktur

### Status faktury

```csharp
var status = await statusService.GetInvoiceStatusAsync(referenceNumber, accessToken);
Console.WriteLine($"Status: {status.Status}");
Console.WriteLine($"KSeF: {status.KsefNumber}");
```

### Statusy faktur w sesji

```csharp
var result = await statusService.GetSessionInvoicesStatusAsync(
    sessionReference, accessToken);
Console.WriteLine($"Przetworzone: {result.ProcessedCount}");
Console.WriteLine($"Odrzucone: {result.RejectedCount}");
```

## Ograniczenia API KSeF 2.0

> **UWAGA:** KSeF API w wersji 2.0 nie udostepnia dedykowanego endpointu do oznaczania faktur
> zakupowych jako zaksiegowane. Ta funkcjonalnosc jest planowana w przyszlych wersjach API.
> Status ksiegowania nalezy sledzic w systemie ERP/ksiegowym uzytkownika.
