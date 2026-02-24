# KSeF.Api.Tests

Testy jednostkowe dla projektu **KSeF.Api**.

## Struktura testów

```
KSeF.Api.Tests/
└── Services/
    ├── KsefSessionServiceTests.cs         - Testy zarządzania sesją
    ├── KsefInvoiceSendServiceTests.cs     - Testy wysyłania faktur
    ├── KsefInvoiceReceiveServiceTests.cs  - Testy pobierania faktur
    └── KsefInvoiceStatusServiceTests.cs   - Testy sprawdzania statusów
```

## Technologie

- **xUnit** 2.9.3 - framework testowy
- **Moq** 4.20.72 - biblioteka do mockowania
- **FluentAssertions** 8.0.1 - asercje fluent
- **.NET 9.0** - platforma docelowa

## Uruchamianie testów

### Wszystkie testy

```bash
dotnet test
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

### KsefSessionService

- ✅ Otwieranie sesji z autoryzacją tokenem
- ✅ Zamykanie sesji
- ✅ Odświeżanie tokenu dostępowego
- ✅ Walidacja braku tokenu KSeF

### KsefInvoiceSendService

- ✅ Wysyłanie faktury z walidacją
- ✅ Walidacja typu faktury (VAT, KOR, ZAL, ROZ, UPR)
- ✅ Obsługa błędów walidacji

### KsefInvoiceReceiveService

- ✅ Pobieranie faktury po numerze KSeF
- ✅ Używanie istniejącej sesji
- ✅ Obsługa błędów deserializacji XML

### KsefInvoiceStatusService

- ✅ Sprawdzanie statusu faktury
- ✅ Używanie tokenu z sesji

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

## Uwagi

- Testy używają mocków IKSeFClient z pakietów CIRFMF/ksef-client-csharp
- Nie wymagają rzeczywistego połączenia z KSeF API
- Szybkie wykonanie (< 1s dla wszystkich testów)
- 100% niezależne od środowiska zewnętrznego
