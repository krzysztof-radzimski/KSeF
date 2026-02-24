using System.Numerics;

namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Walidator numeru rachunku bankowego (IBAN)
/// Implementuje walidację zgodną ze standardem ISO 13616
/// </summary>
public class IbanValidator : IIbanValidator
{
    /// <summary>
    /// Minimalna długość numeru rachunku
    /// </summary>
    private const int MinLength = 10;

    /// <summary>
    /// Maksymalna długość numeru rachunku (zgodnie z KSeF)
    /// </summary>
    private const int MaxLength = 34;

    /// <summary>
    /// Długość polskiego numeru IBAN (z kodem kraju)
    /// </summary>
    private const int PolishIbanLength = 28;

    /// <summary>
    /// Długość polskiego numeru NRB (bez kodu kraju)
    /// </summary>
    private const int PolishNrbLength = 26;

    /// <inheritdoc />
    public ValidationResult Validate(string? accountNumber)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            result.AddError("IBAN_EMPTY", "Numer rachunku bankowego jest wymagany", "AccountNumber");
            return result;
        }

        // Usuń spacje
        var cleanNumber = CleanAccountNumber(accountNumber);

        // Sprawdź długość
        if (cleanNumber.Length < MinLength)
        {
            result.AddError("IBAN_TOO_SHORT",
                $"Numer rachunku jest za krótki. Minimalna długość to {MinLength} znaków.", "AccountNumber");
            return result;
        }

        if (cleanNumber.Length > MaxLength)
        {
            result.AddError("IBAN_TOO_LONG",
                $"Numer rachunku jest za długi. Maksymalna długość to {MaxLength} znaków.", "AccountNumber");
            return result;
        }

        // Sprawdź czy to IBAN (zaczyna się od kodu kraju)
        var isIban = char.IsLetter(cleanNumber[0]) && char.IsLetter(cleanNumber[1]);

        if (isIban)
        {
            // Walidacja IBAN
            if (!ValidateIbanFormat(cleanNumber))
            {
                result.AddError("IBAN_INVALID_FORMAT",
                    "Nieprawidłowy format numeru IBAN", "AccountNumber");
                return result;
            }

            if (!ValidateIbanChecksum(cleanNumber))
            {
                result.AddError("IBAN_INVALID_CHECKSUM",
                    "Nieprawidłowa suma kontrolna IBAN", "AccountNumber");
            }

            // Sprawdź czy to polski IBAN
            if (cleanNumber.StartsWith("PL") && cleanNumber.Length != PolishIbanLength)
            {
                result.AddWarning("IBAN_POLISH_LENGTH",
                    $"Polski numer IBAN powinien mieć {PolishIbanLength} znaków", "AccountNumber");
            }
        }
        else
        {
            // Walidacja polskiego NRB (bez kodu kraju)
            if (!cleanNumber.All(char.IsDigit))
            {
                result.AddError("NRB_INVALID_CHARACTERS",
                    "Numer rachunku NRB może zawierać tylko cyfry", "AccountNumber");
                return result;
            }

            if (cleanNumber.Length == PolishNrbLength)
            {
                // Walidacja sumy kontrolnej NRB (przekształć na IBAN i sprawdź)
                var ibanFormat = "PL" + cleanNumber;
                if (!ValidateIbanChecksum(ibanFormat))
                {
                    result.AddError("NRB_INVALID_CHECKSUM",
                        "Nieprawidłowa suma kontrolna numeru NRB", "AccountNumber");
                }
            }
            else
            {
                result.AddWarning("NRB_UNUSUAL_LENGTH",
                    $"Numer rachunku ma nietypową długość ({cleanNumber.Length} znaków). " +
                    $"Polski NRB ma {PolishNrbLength} cyfr.", "AccountNumber");
            }
        }

        return result;
    }

    /// <inheritdoc />
    public bool IsValid(string? accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return false;

        var cleanNumber = CleanAccountNumber(accountNumber);

        if (cleanNumber.Length < MinLength || cleanNumber.Length > MaxLength)
            return false;

        var isIban = char.IsLetter(cleanNumber[0]) && char.IsLetter(cleanNumber[1]);

        if (isIban)
        {
            return ValidateIbanFormat(cleanNumber) && ValidateIbanChecksum(cleanNumber);
        }

        // Dla NRB - tylko sprawdzenie formatu i ewentualnie sumy kontrolnej
        if (!cleanNumber.All(char.IsDigit))
            return false;

        if (cleanNumber.Length == PolishNrbLength)
        {
            var ibanFormat = "PL" + cleanNumber;
            return ValidateIbanChecksum(ibanFormat);
        }

        return true;
    }

    /// <summary>
    /// Usuwa spacje z numeru rachunku
    /// </summary>
    private static string CleanAccountNumber(string accountNumber)
    {
        return accountNumber.Replace(" ", "").ToUpperInvariant();
    }

    /// <summary>
    /// Sprawdza format IBAN (kod kraju + cyfry)
    /// </summary>
    private static bool ValidateIbanFormat(string iban)
    {
        if (iban.Length < 4)
            return false;

        // Pierwsze 2 znaki - kod kraju (litery)
        if (!char.IsLetter(iban[0]) || !char.IsLetter(iban[1]))
            return false;

        // Znaki 3-4 - cyfry kontrolne
        if (!char.IsDigit(iban[2]) || !char.IsDigit(iban[3]))
            return false;

        // Reszta - alfanumeryczne
        for (var i = 4; i < iban.Length; i++)
        {
            if (!char.IsLetterOrDigit(iban[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Waliduje sumę kontrolną IBAN zgodnie z algorytmem ISO 13616:
    /// 1. Przenieś 4 pierwsze znaki na koniec
    /// 2. Zamień litery na cyfry (A=10, B=11, ..., Z=35)
    /// 3. Oblicz resztę z dzielenia przez 97
    /// 4. Reszta powinna być równa 1
    /// </summary>
    private static bool ValidateIbanChecksum(string iban)
    {
        // Przenieś 4 pierwsze znaki na koniec
        var rearranged = iban[4..] + iban[..4];

        // Zamień litery na cyfry
        var numericString = string.Empty;
        foreach (var c in rearranged)
        {
            if (char.IsDigit(c))
            {
                numericString += c;
            }
            else
            {
                // A=10, B=11, ..., Z=35
                numericString += (c - 'A' + 10).ToString();
            }
        }

        // Oblicz resztę z dzielenia przez 97 używając BigInteger
        var number = BigInteger.Parse(numericString);
        var remainder = number % 97;

        return remainder == 1;
    }
}
