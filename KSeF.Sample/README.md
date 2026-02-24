# KSeF.Sample - Przyklady uzycia

Ten folder zawiera przyklady uzycia bibliotek **KSeF.Invoice** i **KSeF.Api**.

## Struktura

```
KSeF.Sample/
├── Program.cs                          # Glowny plik z przykladami KSeF.Invoice
├── KSeF.Sample.csproj                  # Projekt przykladow
├── README.md                           # Ten plik
├── Examples/                           # Szczegolowe przyklady w osobnych plikach
│   ├── SimpleInvoiceExample.cs         # Prosta faktura VAT
│   ├── CorrectionInvoiceExample.cs     # Faktura korygujaca
│   ├── ValidationExample.cs            # Walidacja faktur
│   ├── DependencyInjectionExample.cs   # Integracja z DI
│   ├── ApiConfigurationExample.cs      # Konfiguracja KSeF.Api
│   ├── SendInvoiceExample.cs           # Wysylanie faktur do KSeF
│   ├── ReceiveInvoiceExample.cs        # Pobieranie faktur z KSeF
│   └── SessionManagementExample.cs     # Zarzadzanie sesjami KSeF
└── XmlExamples/                        # Przykladowe pliki XML
    ├── prosta_faktura_vat.xml          # Minimalna faktura VAT
    ├── faktura_korygujaca.xml          # Faktura korygujaca
    ├── faktura_wielopozycyjna.xml      # Faktura z wieloma pozycjami
    └── faktura_zaliczkowa.xml          # Faktura zaliczkowa
```

## Uruchomienie przykladow

```bash
cd KSeF.Sample
dotnet run
```

## Przyklady KSeF.Invoice (Program.cs)

1. **Prosta faktura VAT** - Tworzenie podstawowej faktury z jedna pozycja
2. **Faktura z wieloma pozycjami** - Faktura z roznymi produktami i uslugami
3. **Faktura korygujaca** - Korekta faktury z odniesieniem do dokumentu pierwotnego
4. **Faktura zaliczkowa** - Dokumentowanie otrzymanej zaliczki
5. **Deserializacja XML** - Odczyt faktury z pliku XML

## Przyklady KSeF.Api (Examples/)

6. **Konfiguracja API** (`ApiConfigurationExample.cs`) - Konfiguracja polaczenia z KSeF, rejestracja serwisow, srodowiska
7. **Wysylanie faktur** (`SendInvoiceExample.cs`) - Wysylanie pojedynczych i wielu faktur, typy wysylania
8. **Pobieranie faktur** (`ReceiveInvoiceExample.cs`) - Wyszukiwanie i pobieranie faktur zakupowych, UPO
9. **Zarzadzanie sesjami** (`SessionManagementExample.cs`) - Cykl zycia sesji, odswiezanie tokenow, statusy

> **UWAGA:** Przyklady KSeF.Api wymagaja poprawnego tokenu KSeF i polaczenia z API.
> Bez konfiguracji wyswietlaja jedynie strukture kodu.

## Opis plikow XML

### prosta_faktura_vat.xml
Minimalna struktura faktury VAT zgodna ze schematem FA(3).
Zawiera: naglowek, sprzedawce, nabywce, jedna pozycje, platnosc.

### faktura_korygujaca.xml
Faktura korygujaca z odniesieniem do faktury pierwotnej.
Zawiera: dane faktury korygowanej, przyczyne korekty, ujemne wartosci.

### faktura_wielopozycyjna.xml
Faktura z wieloma pozycjami i roznymi stawkami VAT.
Zawiera: produkty (VAT 23%), ksiazki (VAT 8%), uslugi, kody GTIN/PKWiU.

### faktura_zaliczkowa.xml
Faktura dokumentujaca otrzymanie zaliczki.
Zawiera: oznaczenie typu ZAL, dodatkowe opisy z numerem zamowienia.
