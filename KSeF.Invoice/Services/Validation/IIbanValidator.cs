namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Interfejs walidatora numeru rachunku bankowego (IBAN)
/// </summary>
public interface IIbanValidator
{
    /// <summary>
    /// Waliduje numer rachunku bankowego
    /// </summary>
    /// <param name="accountNumber">Numer rachunku do walidacji</param>
    /// <returns>Wynik walidacji</returns>
    ValidationResult Validate(string? accountNumber);

    /// <summary>
    /// Sprawdza czy numer rachunku ma prawidłowy format (szybka walidacja)
    /// </summary>
    /// <param name="accountNumber">Numer rachunku do sprawdzenia</param>
    /// <returns>True jeśli format jest prawidłowy</returns>
    bool IsValid(string? accountNumber);
}
