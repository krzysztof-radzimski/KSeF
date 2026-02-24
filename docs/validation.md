# Walidacja faktur

KSeF.Invoice oferuje dwa poziomy walidacji: biznesowa i XSD.

## Konfiguracja walidacji

```csharp
services.AddKsefInvoiceServices(options =>
{
    options.ValidateBeforeSerialize = true;  // Walidacja przed serializacja
    options.ValidateAgainstXsd = true;       // Walidacja XSD
});
```

## Walidacja przez IKsefInvoiceService

Fasada `IKsefInvoiceService.Validate()` uruchamia walidacje biznesowa:

```csharp
var result = invoiceService.Validate(invoice);

if (result.IsValid)
{
    Console.WriteLine("Faktura poprawna!");
}
else
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"[{error.Code}] {error.Message} (Pole: {error.FieldName})");
    }
}

if (result.HasWarnings)
{
    foreach (var warning in result.Warnings)
    {
        Console.WriteLine($"Ostrzezenie: [{warning.Code}] {warning.Message}");
    }
}
```

## Walidacja biznesowa

Dostepna przez `IInvoiceValidator`:

```csharp
var validator = provider.GetRequiredService<IInvoiceValidator>();
var result = validator.Validate(invoice);
```

Sprawdza m.in.:
- Wymagane pola (numer faktury, data, sprzedawca, nabywca)
- Poprawnosc NIP (algorytm walidacji)
- Poprawnosc IBAN
- Zgodnosc dat
- Obecnosc pozycji faktury
- Poprawnosc kwot

## Walidacja XSD

Dostepna przez `IXsdValidator`:

```csharp
var xsdValidator = provider.GetRequiredService<IXsdValidator>();

// Walidacja modelu
var result = xsdValidator.Validate(invoice);

// Walidacja XML
var xmlResult = xsdValidator.ValidateXml(xmlString, SchemaVersion.FA3);
```

Wlasciwosci:

```csharp
xsdValidator.AreSchemasLoaded  // Czy schematy sa zaladowane
xsdValidator.AvailableSchemas  // Lista dostepnych schematow
```

## Walidatory pomocnicze

### NIP

```csharp
var nipValidator = provider.GetRequiredService<INipValidator>();
bool isValid = nipValidator.IsValid("1234567890");
```

### IBAN

```csharp
var ibanValidator = provider.GetRequiredService<IIbanValidator>();
bool isValid = ibanValidator.IsValid("PL61109010140000071219812874");
```

### Daty

```csharp
var dateValidator = provider.GetRequiredService<IDateValidator>();
```

## Automatyczna walidacja przy serializacji

Gdy `ValidateBeforeSerialize = true`, proba serializacji niepoprawnej faktury rzuci `InvalidOperationException`:

```csharp
try
{
    string xml = invoiceService.ToXml(invalidInvoice);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Blad walidacji: {ex.Message}");
}
```

## Wynik walidacji (ValidationResult)

| Pole | Typ | Opis |
|------|-----|------|
| `IsValid` | `bool` | Czy faktura jest poprawna |
| `Errors` | `List<ValidationError>` | Lista bledow |
| `Warnings` | `List<ValidationError>` | Lista ostrzezen |
| `HasWarnings` | `bool` | Czy sa ostrzezenia |

### ValidationError

| Pole | Typ | Opis |
|------|-----|------|
| `Code` | `string` | Kod bledu |
| `Message` | `string` | Opis bledu |
| `FieldName` | `string?` | Nazwa pola z bledem |
