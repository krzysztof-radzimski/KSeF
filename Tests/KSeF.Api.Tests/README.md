# KSeF.Api.Tests

Testy jednostkowe i integracyjne dla projektu **KSeF.Api**.

## Struktura testów

```
KSeF.Api.Tests/
├── Services/
│   ├── KsefSessionServiceTests.cs         - Testy zarządzania sesją (unit)
│   ├── KsefInvoiceSendServiceTests.cs     - Testy wysyłania faktur (unit)
│   ├── KsefInvoiceReceiveServiceTests.cs  - Testy pobierania faktur (unit)
│   ├── KsefInvoiceStatusServiceTests.cs   - Testy sprawdzania statusów (unit)
│   └── KsefAuthorizationTests.cs          - Testy autoryzacji dla 3 środowisk (integration)
├── .env.example                           - Szablon zmiennych środowiskowych
└── .env                                   - Lokalne dane uwierzytelniające (gitignored)
```

## Technologie

- **xUnit** 2.9.3 - framework testowy
- **Moq** 4.20.72 - biblioteka do mockowania
- **FluentAssertions** 8.0.1 - asercje fluent
- **.NET 9.0** - platforma docelowa

## Uruchamianie testów

### Wszystkie testy (unit + integration)

```bash
dotnet test
```

### Tylko testy jednostkowe (szybkie, bez połączenia z API)

```bash
dotnet test --filter "Category!=Integration"
```

### Tylko testy integracyjne (wymagają internetu i credentials)

```bash
dotnet test --filter "Category=Integration"
```

### Konkretny projekt

```bash
dotnet test Tests/KSeF.Api.Tests/KSeF.Api.Tests.csproj
```

### Z szczegółowymi logami

```bash
dotnet test --verbosity detailed
```

### Z pokryciem kodu

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Testowane serwisy

### KsefSessionService (unit)

- ✅ Otwieranie sesji z autoryzacją tokenem
- ✅ Zamykanie sesji
- ✅ Odświeżanie tokenu dostępowego
- ✅ Walidacja braku tokenu KSeF

### KsefInvoiceSendService (unit)

- ✅ Wysyłanie faktury z walidacją
- ✅ Walidacja typu faktury (VAT, KOR, ZAL, ROZ, UPR)
- ✅ Obsługa błędów walidacji

### KsefInvoiceReceiveService (unit)

- ✅ Pobieranie faktury po numerze KSeF
- ✅ Używanie istniejącej sesji
- ✅ Obsługa błędów deserializacji XML

### KsefInvoiceStatusService (unit)

- ✅ Sprawdzanie statusu faktury
- ✅ Używanie tokenu z sesji

### KsefAuthorizationTests (integration)

- ✅ Autoryzacja tokenem KSeF dla 3 środowisk (test, demo, prod)
- ✅ Pobieranie challenge dla 3 środowisk
- ✅ Szyfrowanie tokenu dla 3 środowisk
- ✅ Otwieranie sesji online dla 3 środowisk
- ✅ Walidacja konfiguracji i credentials

## Wzorce testowe

### Dependency Injection z mockami

```csharp
var ksefClientMock = new Mock<IKSeFClient>();
var sessionServiceMock = new Mock<IKsefSessionService>();

var service = new KsefInvoiceSendService(
    ksefClientMock.Object,
    // ... inne zależności
);
```

### Konfiguracja mocków

```csharp
_sessionServiceMock
    .Setup(x => x.OpenSessionAsync(It.IsAny<CancellationToken>()))
    .ReturnsAsync(new SessionInfo { SessionReference = "session-123" });
```

### Asercje z FluentAssertions

```csharp
result.Should().NotBeNull();
result.Success.Should().BeTrue();
result.SessionReference.Should().Be("session-123");
```

### Weryfikacja wywołań

```csharp
_ksefClientMock.Verify(
    x => x.OpenOnlineSessionAsync(
        It.IsAny<OpenOnlineSessionRequest>(),
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<CancellationToken>()),
    Times.Once);
```

## Konwencje

- Nazwy testów: `MethodName_Scenario_ExpectedBehavior`
- Struktura AAA: Arrange, Act, Assert
- Mocki poprzez Dependency Injection
- FluentAssertions dla czytelnych asercji
- Izolacja testów - każdy test niezależny
- Verify dla sprawdzenia wywołań zewnętrznych zależności

## Dodawanie nowych testów

1. Utwórz klasę testową w folderze `Services/`
2. Zdefiniuj mocki zależności w konstruktorze
3. Napisz testy w formacie `[Fact]` lub `[Theory]`
4. Użyj FluentAssertions do asercji
5. Zweryfikuj mocki jeśli wymagane

## Konfiguracja testów integracyjnych

### Zmienne środowiskowe

Testy integracyjne wymagają danych uwierzytelniających dla trzech środowisk KSeF:

- `KSEF_TEST_NIP` / `KSEF_TEST_TOKEN` - środowisko testowe (https://ksef-test.mf.gov.pl/api)
- `KSEF_DEMO_NIP` / `KSEF_DEMO_TOKEN` - środowisko demo (https://ksef-demo.mf.gov.pl/api)
- `KSEF_PROD_NIP` / `KSEF_PROD_TOKEN` - środowisko produkcyjne (https://ksef.mf.gov.pl/api)

### Konfiguracja lokalna

1. Skopiuj `.env.example` jako `.env` w katalogu `Tests/KSeF.Api.Tests/`
2. Wypełnij plik `.env` własnymi danymi uwierzytelniającymi
3. Plik `.env` jest automatycznie ignorowany przez git

```bash
# Przykład .env
KSEF_TEST_NIP=9999999999
KSEF_TEST_TOKEN=20260331-XX-XXXXXXXXXX-YYYYYYYYYY-ZZ|nip-9999999999|...
KSEF_DEMO_NIP=9999999999
KSEF_DEMO_TOKEN=20260331-XX-XXXXXXXXXX-YYYYYYYYYY-ZZ|nip-9999999999|...
KSEF_PROD_NIP=9999999999
KSEF_PROD_TOKEN=20260331-XX-XXXXXXXXXX-YYYYYYYYYY-ZZ|nip-9999999999|...
```

### Fallback do .env.example

Gdy zmienne środowiskowe nie są ustawione, testy automatycznie ładują wartości z `.env.example`. Umożliwia to uruchomienie testów bez dodatkowej konfiguracji (z fikcyjnymi danymi).

## Uwagi

### Testy jednostkowe
- Używają mocków IKSeFClient z pakietów CIRFMF/ksef-client-csharp
- Nie wymagają rzeczywistego połączenia z KSeF API
- Szybkie wykonanie (< 1s)
- 100% niezależne od środowiska zewnętrznego

### Testy integracyjne
- Oznaczone `[Trait("Category", "Integration")]`
- Wymagają połączenia z internetem i poprawnych credentials
- Trwają ~3-4 sekundy
- Testują rzeczywistą komunikację z KSeF API
- **UWAGA:** Błąd autoryzacji oznacza test failed (nie są łapane wyjątki)
