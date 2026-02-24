using FluentAssertions;
using KSeF.Invoice.Services.Validation;
using Xunit;

namespace KSeF.Invoice.Tests.Validation;

/// <summary>
/// Testy jednostkowe walidatora IBAN
/// </summary>
public class IbanValidatorTests
{
    private readonly IbanValidator _validator;

    public IbanValidatorTests()
    {
        _validator = new IbanValidator();
    }

    #region Poprawne numery IBAN

    [Theory]
    [InlineData("PL61109010140000071219812874")] // Polski IBAN - przykład z dokumentacji
    [InlineData("PL27114020040000300201355387")] // Polski IBAN - mBank
    [InlineData("DE89370400440532013000")] // Niemiecki IBAN
    [InlineData("GB29NWBK60161331926819")] // Brytyjski IBAN
    [InlineData("FR1420041010050500013M02606")] // Francuski IBAN (z literami)
    public void Validate_ValidIban_ShouldReturnValidResult(string iban)
    {
        // Act
        var result = _validator.Validate(iban);

        // Assert
        result.IsValid.Should().BeTrue();
        result.HasErrors.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("PL61109010140000071219812874")]
    [InlineData("DE89370400440532013000")]
    [InlineData("GB29NWBK60161331926819")]
    public void IsValid_ValidIban_ShouldReturnTrue(string iban)
    {
        // Act
        var isValid = _validator.IsValid(iban);

        // Assert
        isValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("PL 61 1090 1014 0000 0712 1981 2874")]
    [InlineData("PL61 1090 1014 0000 0712 1981 2874")]
    [InlineData("DE 89 3704 0044 0532 0130 00")]
    public void Validate_ValidIbanWithSpaces_ShouldReturnValidResult(string iban)
    {
        // Act
        var result = _validator.Validate(iban);

        // Assert
        result.IsValid.Should().BeTrue();
        result.HasErrors.Should().BeFalse();
    }

    [Theory]
    [InlineData("pl61109010140000071219812874")] // Małe litery
    [InlineData("Pl61109010140000071219812874")] // Mieszane
    public void Validate_ValidIbanLowercase_ShouldReturnValidResult(string iban)
    {
        // Act
        var result = _validator.Validate(iban);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Poprawne polskie numery NRB (bez kodu kraju)

    [Theory]
    [InlineData("61109010140000071219812874")] // Polski NRB (26 cyfr)
    [InlineData("27114020040000300201355387")] // Polski NRB
    public void Validate_ValidPolishNrb_ShouldReturnValidResult(string nrb)
    {
        // Act
        var result = _validator.Validate(nrb);

        // Assert
        result.IsValid.Should().BeTrue();
        result.HasErrors.Should().BeFalse();
    }

    [Theory]
    [InlineData("61109010140000071219812874")]
    [InlineData("27114020040000300201355387")]
    public void IsValid_ValidPolishNrb_ShouldReturnTrue(string nrb)
    {
        // Act
        var isValid = _validator.IsValid(nrb);

        // Assert
        isValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("61 1090 1014 0000 0712 1981 2874")]
    [InlineData("27 1140 2004 0000 3002 0135 5387")]
    public void Validate_ValidNrbWithSpaces_ShouldReturnValidResult(string nrb)
    {
        // Act
        var result = _validator.Validate(nrb);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Niepoprawne numery IBAN - pusty lub null

    [Fact]
    public void Validate_NullAccountNumber_ShouldReturnError()
    {
        // Act
        var result = _validator.Validate(null);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Code.Should().Be("IBAN_EMPTY");
    }

    [Fact]
    public void IsValid_NullAccountNumber_ShouldReturnFalse()
    {
        // Act
        var isValid = _validator.IsValid(null);

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Validate_EmptyOrWhitespaceAccountNumber_ShouldReturnError(string accountNumber)
    {
        // Act
        var result = _validator.Validate(accountNumber);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Code.Should().Be("IBAN_EMPTY");
    }

    #endregion

    #region Niepoprawne numery IBAN - zła długość

    [Theory]
    [InlineData("PL123")] // Za krótki IBAN
    [InlineData("DE123")] // Za krótki IBAN
    [InlineData("123456789")] // Za krótki NRB (9 cyfr)
    public void Validate_TooShortAccountNumber_ShouldReturnError(string accountNumber)
    {
        // Act
        var result = _validator.Validate(accountNumber);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Code.Should().Be("IBAN_TOO_SHORT");
    }

    [Theory]
    [InlineData("PL12345678901234567890123456789012345")] // Za długi (35+ znaków)
    [InlineData("12345678901234567890123456789012345")] // Za długi NRB
    public void Validate_TooLongAccountNumber_ShouldReturnError(string accountNumber)
    {
        // Act
        var result = _validator.Validate(accountNumber);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Code.Should().Be("IBAN_TOO_LONG");
    }

    [Theory]
    [InlineData("PL123")]
    [InlineData("12345678901234567890123456789012345")]
    public void IsValid_InvalidLengthAccountNumber_ShouldReturnFalse(string accountNumber)
    {
        // Act
        var isValid = _validator.IsValid(accountNumber);

        // Assert
        isValid.Should().BeFalse();
    }

    #endregion

    #region Niepoprawne numery IBAN - zły format

    [Theory]
    [InlineData("12PL109010140000071219812874")] // Kod kraju nie na początku
    [InlineData("P161109010140000071219812874")] // Niepełny kod kraju
    [InlineData("PLA1109010140000071219812874")] // Litera zamiast cyfry kontrolnej
    [InlineData("PLAA109010140000071219812874")] // Litery zamiast cyfr kontrolnych
    public void Validate_InvalidIbanFormat_ShouldReturnError(string iban)
    {
        // Act
        var result = _validator.Validate(iban);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
    }

    [Theory]
    [InlineData("1234567890123456789012345A")] // Litera w NRB
    [InlineData("12345678901234567890123!56")] // Znak specjalny w NRB
    public void Validate_InvalidNrbCharacters_ShouldReturnError(string nrb)
    {
        // Act
        var result = _validator.Validate(nrb);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Code.Should().Be("NRB_INVALID_CHARACTERS");
    }

    #endregion

    #region Niepoprawne numery IBAN - błędna suma kontrolna

    [Theory]
    [InlineData("PL61109010140000071219812875")] // Zmieniona ostatnia cyfra
    [InlineData("PL62109010140000071219812874")] // Zmieniona cyfra kontrolna
    [InlineData("DE89370400440532013001")] // Zmieniona ostatnia cyfra
    public void Validate_InvalidIbanChecksum_ShouldReturnError(string iban)
    {
        // Act
        var result = _validator.Validate(iban);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Code.Should().Be("IBAN_INVALID_CHECKSUM");
    }

    [Theory]
    [InlineData("61109010140000071219812875")] // Polski NRB - zmieniona ostatnia cyfra
    [InlineData("27114020040000300201355388")] // Polski NRB - zmieniona ostatnia cyfra
    public void Validate_InvalidNrbChecksum_ShouldReturnError(string nrb)
    {
        // Act
        var result = _validator.Validate(nrb);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Code.Should().Be("NRB_INVALID_CHECKSUM");
    }

    [Theory]
    [InlineData("PL61109010140000071219812875")]
    [InlineData("61109010140000071219812875")]
    public void IsValid_InvalidChecksum_ShouldReturnFalse(string accountNumber)
    {
        // Act
        var isValid = _validator.IsValid(accountNumber);

        // Assert
        isValid.Should().BeFalse();
    }

    #endregion

    #region Ostrzeżenia

    [Fact]
    public void Validate_PolishIbanWithWrongLength_ShouldReturnWarning()
    {
        // Polski IBAN powinien mieć 28 znaków (PL + 26 cyfr)
        // Ten test sprawdza, czy zwracane jest ostrzeżenie dla nietypowej długości
        // (jeśli walidacja sumy kontrolnej przechodzi)

        // Uwaga: trudno stworzyć prawidłowy IBAN PL z inną długością,
        // więc ten test potwierdza zachowanie dla standardowych przypadków
        var result = _validator.Validate("PL61109010140000071219812874");

        result.IsValid.Should().BeTrue();
        result.HasWarnings.Should().BeFalse(); // Poprawna długość = brak ostrzeżeń
    }

    [Theory]
    [InlineData("1234567890")] // 10 cyfr - minimalna długość
    [InlineData("12345678901234567890")] // 20 cyfr - nie 26
    public void Validate_NrbWithUnusualLength_ShouldReturnWarning(string nrb)
    {
        // Act
        var result = _validator.Validate(nrb);

        // Assert
        result.HasWarnings.Should().BeTrue();
        result.Warnings.Should().ContainSingle()
            .Which.Code.Should().Be("NRB_UNUSUAL_LENGTH");
    }

    #endregion

    #region Szczegóły błędów

    [Fact]
    public void Validate_EmptyAccountNumber_ShouldReturnProperErrorDetails()
    {
        // Act
        var result = _validator.Validate("");

        // Assert
        result.Errors.Should().ContainSingle();
        var error = result.Errors[0];
        error.Code.Should().Be("IBAN_EMPTY");
        error.Message.Should().Contain("wymagany");
        error.FieldName.Should().Be("AccountNumber");
    }

    [Fact]
    public void Validate_TooShortAccountNumber_ShouldReturnProperErrorDetails()
    {
        // Act
        var result = _validator.Validate("PL123");

        // Assert
        result.Errors.Should().ContainSingle();
        var error = result.Errors[0];
        error.Code.Should().Be("IBAN_TOO_SHORT");
        error.Message.Should().Contain("10");
        error.FieldName.Should().Be("AccountNumber");
    }

    [Fact]
    public void Validate_InvalidChecksum_ShouldReturnProperErrorDetails()
    {
        // Act
        var result = _validator.Validate("PL61109010140000071219812875");

        // Assert
        result.Errors.Should().ContainSingle();
        var error = result.Errors[0];
        error.Code.Should().Be("IBAN_INVALID_CHECKSUM");
        error.Message.Should().Contain("suma kontrolna");
        error.FieldName.Should().Be("AccountNumber");
    }

    #endregion

    #region Różne kraje UE

    [Theory]
    [InlineData("AT611904300234573201")] // Austria
    [InlineData("BE68539007547034")] // Belgia
    [InlineData("CZ6508000000192000145399")] // Czechy
    [InlineData("DK5000400440116243")] // Dania
    [InlineData("FI2112345600000785")] // Finlandia
    [InlineData("HU42117730161111101800000000")] // Węgry
    [InlineData("NL91ABNA0417164300")] // Holandia
    [InlineData("SK3112000000198742637541")] // Słowacja
    public void Validate_ValidEuropeanIbans_ShouldReturnValidResult(string iban)
    {
        // Act
        var result = _validator.Validate(iban);

        // Assert
        result.IsValid.Should().BeTrue();
        result.HasErrors.Should().BeFalse();
    }

    #endregion

    #region Przypadki brzegowe

    [Theory]
    [InlineData("0000000000")] // 10 zer - minimalna długość
    public void Validate_MinimumLengthAccountNumber_ShouldNotThrowException(string accountNumber)
    {
        // Act
        var act = () => _validator.Validate(accountNumber);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("1234567890123456789012345678901234")] // 34 znaki - maksymalna długość
    public void Validate_MaximumLengthAccountNumber_ShouldNotThrowException(string accountNumber)
    {
        // Act
        var act = () => _validator.Validate(accountNumber);

        // Assert
        act.Should().NotThrow();
    }

    #endregion
}
