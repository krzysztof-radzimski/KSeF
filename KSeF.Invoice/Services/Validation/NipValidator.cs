namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Walidator numeru NIP (Numer Identyfikacji Podatkowej)
/// Implementuje walidację zgodną z polskim algorytmem NIP
/// </summary>
public class NipValidator : INipValidator
{
    /// <summary>
    /// Wagi dla poszczególnych cyfr NIP używane w algorytmie sumy kontrolnej
    /// </summary>
    private static readonly int[] Weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };

    /// <inheritdoc />
    public ValidationResult Validate(string? nip)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(nip))
        {
            result.AddError("NIP_EMPTY", "Numer NIP jest wymagany", "NIP");
            return result;
        }

        // Usuń myślniki i spacje
        var cleanNip = CleanNip(nip);

        // Sprawdź długość
        if (cleanNip.Length != 10)
        {
            result.AddError("NIP_INVALID_LENGTH",
                $"NIP musi mieć dokładnie 10 cyfr. Podany ma {cleanNip.Length} znaków.", "NIP");
            return result;
        }

        // Sprawdź czy zawiera tylko cyfry
        if (!cleanNip.All(char.IsDigit))
        {
            result.AddError("NIP_INVALID_CHARACTERS",
                "NIP może zawierać tylko cyfry", "NIP");
            return result;
        }

        // Sprawdź sumę kontrolną
        if (!ValidateChecksum(cleanNip))
        {
            result.AddError("NIP_INVALID_CHECKSUM",
                "NIP ma nieprawidłową sumę kontrolną", "NIP");
        }

        return result;
    }

    /// <inheritdoc />
    public bool IsValid(string? nip)
    {
        if (string.IsNullOrWhiteSpace(nip))
            return false;

        var cleanNip = CleanNip(nip);

        if (cleanNip.Length != 10)
            return false;

        if (!cleanNip.All(char.IsDigit))
            return false;

        return ValidateChecksum(cleanNip);
    }

    /// <summary>
    /// Usuwa myślniki i spacje z numeru NIP
    /// </summary>
    /// <param name="nip">Surowy numer NIP</param>
    /// <returns>Oczyszczony numer NIP</returns>
    private static string CleanNip(string nip)
    {
        return nip.Replace("-", "").Replace(" ", "");
    }

    /// <summary>
    /// Waliduje sumę kontrolną NIP zgodnie z algorytmem:
    /// suma = sum(cyfra[i] * waga[i]) dla i = 0..8
    /// cyfra_kontrolna = suma mod 11
    /// Jeśli reszta = 10, NIP jest nieprawidłowy
    /// </summary>
    /// <param name="nip">Oczyszczony numer NIP (10 cyfr)</param>
    /// <returns>True jeśli suma kontrolna jest prawidłowa</returns>
    private static bool ValidateChecksum(string nip)
    {
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += (nip[i] - '0') * Weights[i];
        }

        var checkDigit = sum % 11;

        // Jeśli reszta = 10, NIP jest nieprawidłowy
        if (checkDigit == 10)
            return false;

        var lastDigit = nip[9] - '0';
        return checkDigit == lastDigit;
    }
}
