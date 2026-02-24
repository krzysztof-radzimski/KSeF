# KSeF.Invoice - Przyklady uzycia

Ten folder zawiera przyklady uzycia biblioteki KSeF.Invoice.

## Struktura

```
Samples/
├── Program.cs                     # Glowny plik z przykladami
├── Samples.csproj                 # Projekt przykladow
├── README.md                      # Ten plik
├── Examples/                      # Szczegolowe przyklady w osobnych plikach
│   ├── SimpleInvoiceExample.cs    # Prosta faktura VAT
│   ├── CorrectionInvoiceExample.cs# Faktura korygujaca
│   ├── ValidationExample.cs       # Walidacja faktur
│   └── DependencyInjectionExample.cs # Integracja z DI
└── XmlExamples/                   # Przykladowe pliki XML
    ├── prosta_faktura_vat.xml     # Minimalna faktura VAT
    ├── faktura_korygujaca.xml     # Faktura korygujaca
    ├── faktura_wielopozycyjna.xml # Faktura z wieloma pozycjami
    └── faktura_zaliczkowa.xml     # Faktura zaliczkowa
```

## Uruchomienie przykladow

```bash
cd Samples
dotnet run
```

## Przyklady w Program.cs

1. **Prosta faktura VAT** - Tworzenie podstawowej faktury z jedną pozycją
2. **Faktura z wieloma pozycjami** - Faktura z różnymi produktami i usługami
3. **Faktura korygująca** - Korekta faktury z odniesieniem do dokumentu pierwotnego
4. **Faktura zaliczkowa** - Dokumentowanie otrzymanej zaliczki
5. **Deserializacja XML** - Odczyt faktury z pliku XML

## Opis plików XML

### prosta_faktura_vat.xml
Minimalna struktura faktury VAT zgodna ze schematem FA(3).
Zawiera: nagłówek, sprzedawcę, nabywcę, jedną pozycję, płatność.

### faktura_korygujaca.xml
Faktura korygująca z odniesieniem do faktury pierwotnej.
Zawiera: dane faktury korygowanej, przyczynę korekty, ujemne wartości.

### faktura_wielopozycyjna.xml
Faktura z wieloma pozycjami i różnymi stawkami VAT.
Zawiera: produkty (VAT 23%), książki (VAT 8%), usługi, kody GTIN/PKWiU.

### faktura_zaliczkowa.xml
Faktura dokumentująca otrzymanie zaliczki.
Zawiera: oznaczenie typu ZAL, dodatkowe opisy z numerem zamówienia.

## Uzycie w swoim projekcie

```csharp
// 1. Dodaj referencję do KSeF.Invoice
// 2. Skonfiguruj DI
var services = new ServiceCollection();
services.AddKsefInvoiceServices(options =>
{
    options.SchemaVersion = SchemaVersion.FA3;
    options.ValidateBeforeSerialize = true;
});

var provider = services.BuildServiceProvider();
var invoiceService = provider.GetRequiredService<IKsefInvoiceService>();

// 3. Twórz faktury
var invoice = invoiceService.CreateInvoice()
    .WithSeller(/* ... */)
    .WithBuyer(/* ... */)
    .WithInvoiceDetails(/* ... */)
    .AddLineItem(/* ... */)
    .Build();

// 4. Waliduj i serializuj
var result = invoiceService.Validate(invoice);
if (result.IsValid)
{
    var xml = invoiceService.ToXml(invoice);
}
```
