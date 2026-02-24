# Testy jednostkowe

Projekt zawiera kompleksowe testy jednostkowe dla obu bibliotek.

## Uruchomienie testow

```bash
# Wszystkie testy
dotnet test

# Testy dla KSeF.Invoice
dotnet test Tests/KSeF.Invoice.Tests

# Testy dla KSeF.Api
dotnet test Tests/KSeF.Api.Tests

# Testy z pokryciem kodu
dotnet test --collect:"XPlat Code Coverage"
```

## Framework testowy

| Pakiet | Wersja | Przeznaczenie |
|--------|--------|---------------|
| xUnit | 2.9.3 | Framework testowy |
| Moq | 4.20.72 | Mockowanie interfejsow |
| FluentAssertions | 8.0.1 | Czytelne asercje |

## Struktura testow

### KSeF.Invoice.Tests

```
Tests/KSeF.Invoice.Tests/
├── Builders/
│   ├── InvoiceBuilderTests.cs           # Testy Fluent API
│   └── VatSummaryCalculationTests.cs    # Obliczenia podsumowania VAT
├── Models/
│   ├── InvoiceDataTests.cs
│   ├── InvoiceLineItemTests.cs
│   ├── InvoiceSerializationTests.cs
│   ├── NullHandlingTests.cs
│   ├── XmlMappingTests.cs
│   └── XmlNamespaceTests.cs
├── Models/Enums/
│   ├── InvoiceTypeTests.cs
│   ├── VatRateTests.cs
│   ├── PaymentMethodTests.cs
│   └── ...
├── Scenarios/                           # Testy scenariuszowe
│   ├── BasicVatInvoiceScenarioTests.cs
│   ├── CorrectionInvoiceScenarioTests.cs
│   ├── AdvancePaymentInvoiceScenarioTests.cs
│   ├── SimplifiedInvoiceScenarioTests.cs
│   ├── MultiRecipientInvoiceScenarioTests.cs
│   ├── JstAndVatGroupInvoiceScenarioTests.cs
│   └── XmlStructureComparisonTests.cs
├── Validation/
│   ├── NipValidatorTests.cs
│   ├── IbanValidatorTests.cs
│   ├── RequiredFieldsValidationTests.cs
│   ├── AmountConsistencyValidationTests.cs
│   ├── InvoiceValidatorIntegrationTests.cs
│   └── XsdValidatorTests.cs
└── Helpers/
    └── XmlSerializationHelper.cs
```

### KSeF.Api.Tests

```
Tests/KSeF.Api.Tests/
└── Services/
    ├── KsefSessionServiceTests.cs
    ├── KsefInvoiceSendServiceTests.cs
    ├── KsefInvoiceReceiveServiceTests.cs
    └── KsefInvoiceStatusServiceTests.cs
```

## Konwencje testowe

### Wzorzec testow

Kazda klasa testowa:
1. Mockuje `IKSeFClient` + specyficzne zaleznosci
2. Tworzy instancje serwisu w konstruktorze
3. Uzywa wzorca Arrange-Act-Assert

```csharp
[Fact]
public async Task SendVatInvoiceAsync_WhenInvoiceIsValid_ReturnsSuccess()
{
    // Arrange
    var invoice = CreateValidInvoice();
    _mockClient.Setup(x => x.OpenOnlineSessionAsync(
        It.IsAny<OpenOnlineSessionRequest>(),
        It.IsAny<string>(),
        cancellationToken: It.IsAny<CancellationToken>()))
        .ReturnsAsync(new OpenOnlineSessionResponse
        {
            ReferenceNumber = "REF123"
        });

    // Act
    var result = await _sendService.SendVatInvoiceAsync(invoice);

    // Assert
    result.Success.Should().BeTrue();
    result.ReferenceNumber.Should().Be("REF123");
    _mockClient.Verify(x => x.CloseOnlineSessionAsync(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<CancellationToken>()), Times.Once);
}
```

### Testy sa izolowane

- Nie wymagaja polaczenia z API KSeF
- Mockuja wszystkie zaleznosci zewnetrzne
- Weryfikuja poprawne wywolania mockow (`Verify`)
- Testuja zwracane wyniki, walidacje parametrow i obsluge bledow
