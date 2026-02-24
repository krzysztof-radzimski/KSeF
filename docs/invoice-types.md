# Typy faktur

KSeF obsluguje 7 typow faktur. Kazdy typ ma dedykowana metode wysylania w `IKsefInvoiceSendService`.

## Przegląd typow

| Typ | Enum | Metoda wysylania | Opis |
|-----|------|------------------|------|
| Faktura VAT | `InvoiceType.VAT` | `SendVatInvoiceAsync()` | Standardowa faktura sprzedazy |
| Korekta VAT | `InvoiceType.KOR` | `SendCorrectionInvoiceAsync()` | Korekta faktury VAT |
| Zaliczkowa | `InvoiceType.ZAL` | `SendAdvancePaymentInvoiceAsync()` | Faktura zaliczkowa |
| Rozliczeniowa | `InvoiceType.ROZ` | `SendSettlementInvoiceAsync()` | Rozliczenie zaliczek |
| Uproszczona | `InvoiceType.UPR` | `SendSimplifiedInvoiceAsync()` | Faktura uproszczona |
| Korekta zaliczkowej | `InvoiceType.KOR_ZAL` | `SendAdvancePaymentCorrectionAsync()` | Korekta faktury zaliczkowej |
| Korekta rozliczeniowej | `InvoiceType.KOR_ROZ` | `SendSettlementCorrectionAsync()` | Korekta faktury rozliczeniowej |

## Standardowa faktura VAT

Podstawowy typ faktury - dokumentuje sprzedaz towarow lub uslug.

```csharp
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
        .WithName("Klient XYZ S.A."))
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
        .AddPaymentTerm(2025, 3, 31)
        .AsBankTransfer("PL61109010140000071219812874"))
    .Build();
```

## Korekta faktury VAT

Korekta wymaga wskazania faktury korygowanej i przyczyny korekty.

```csharp
var correction = invoiceService.CreateInvoice()
    .WithSeller(/* ... */)
    .WithBuyer(/* ... */)
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/001/KOR")
        .WithIssueDate(2025, 3, 15)
        .AsCorrection("Blad w cenie jednostkowej", corrected => corrected
            .WithInvoiceNumber("FV/2025/001")
            .WithIssueDate(new DateOnly(2025, 3, 1))
            .WithKSeFNumber("1234567890-20250301-ABC123")))
    .AddLineItem(i => i
        .WithProductName("Usluga konsultingowa - korekta")
        .WithUnit("szt.")
        .WithQuantity(1)
        .WithUnitNetPrice(-200.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(-200.00m)
        .WithVatAmount(-46.00m))
    .Build();
```

## Faktura zaliczkowa

Dokumentuje otrzymanie zaliczki. Ustawia typ faktury na ZAL.

```csharp
var advance = invoiceService.CreateInvoice()
    .WithSeller(/* ... */)
    .WithBuyer(/* ... */)
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/ZAL/001")
        .WithIssueDate(2025, 2, 1)
        .AsAdvancePayment()
        .AddDescription("Zamowienie", "ZAM/2025/001"))
    .AddLineItem(i => i
        .WithProductName("Zaliczka na dostawa towaru")
        .WithUnit("szt.").WithQuantity(1)
        .WithUnitNetPrice(5000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(5000.00m)
        .WithVatAmount(1150.00m))
    .Build();
```

## Rozliczenie zaliczek

Faktura koncowa rozliczajaca wczesniej otrzymane zaliczki.

```csharp
var settlement = invoiceService.CreateInvoice()
    .WithSeller(/* ... */)
    .WithBuyer(/* ... */)
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/ROZ/001")
        .WithIssueDate(2025, 4, 1)
        .WithSaleDate(2025, 4, 1)
        .AsSettlement())
    .AddLineItem(i => i
        .WithProductName("Dostawa towaru (rozliczenie zaliczki)")
        .WithUnit("szt.").WithQuantity(10)
        .WithUnitNetPrice(2000.00m)
        .WithVatRate(VatRate.Rate23)
        .WithNetAmount(20000.00m)
        .WithVatAmount(4600.00m))
    .Build();
```

## Faktura uproszczona

Faktura bez pelnych danych nabywcy (kwota brutto do 450 PLN).

```csharp
var simplified = invoiceService.CreateInvoice()
    .WithSeller(/* ... */)
    .WithBuyer(b => b.WithTaxId("0987654321"))
    .WithInvoiceDetails(d => d
        .WithInvoiceNumber("FV/2025/UPR/001")
        .WithIssueDate(2025, 3, 1)
        .AsSimplified())
    .AddLineItem(i => i
        .WithProductName("Usluga IT")
        .WithVatRate(VatRate.Rate23)
        .WithGrossAmount(123.00m))
    .Build();
```

## Stawki VAT

```csharp
// Stawki procentowe
VatRate.Rate23                    // 23%
VatRate.Rate22                    // 22%
VatRate.Rate8                     // 8%
VatRate.Rate7                     // 7%
VatRate.Rate5                     // 5%
VatRate.Rate4                     // 4%
VatRate.Rate3                     // 3%

// Stawki zerowe
VatRate.Rate0Domestic             // 0% - dostawa krajowa
VatRate.Rate0IntraCommunitySupply // 0% - WDT
VatRate.Rate0Export               // 0% - eksport

// Specjalne
VatRate.Exempt                    // zw - zwolniony
VatRate.ReverseCharge             // np - odwrotne obciazenie
VatRate.NotSubjectToTaxI          // oo - niepodlegajacy I
VatRate.NotSubjectToTaxII         // oo - niepodlegajacy II
```
