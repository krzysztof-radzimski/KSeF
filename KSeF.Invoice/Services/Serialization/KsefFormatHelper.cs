using System.Globalization;

namespace KSeF.Invoice.Services.Serialization;

/// <summary>
/// Klasa pomocnicza zawierająca metody formatowania zgodne z wymaganiami XSD KSeF
/// </summary>
public static class KsefFormatHelper
{
    /// <summary>
    /// Kultura niezmienna do formatowania liczb (kropka jako separator dziesiętny)
    /// </summary>
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Format daty zgodny z XSD (xs:date) - YYYY-MM-DD
    /// </summary>
    public const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Format daty i czasu zgodny z XSD (xs:dateTime) - YYYY-MM-DDTHH:MM:SS
    /// </summary>
    public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    /// <summary>
    /// Format daty i czasu UTC zgodny z XSD - YYYY-MM-DDTHH:MM:SSZ
    /// </summary>
    public const string DateTimeUtcFormat = "yyyy-MM-ddTHH:mm:ssZ";

    /// <summary>
    /// Maksymalna liczba miejsc dziesiętnych dla kwot w KSeF
    /// </summary>
    public const int MaxDecimalPlaces = 2;

    /// <summary>
    /// Maksymalna liczba miejsc dziesiętnych dla cen jednostkowych
    /// </summary>
    public const int MaxUnitPriceDecimalPlaces = 8;

    /// <summary>
    /// Maksymalna liczba miejsc dziesiętnych dla ilości
    /// </summary>
    public const int MaxQuantityDecimalPlaces = 8;

    /// <summary>
    /// Formatuje kwotę zgodnie z wymaganiami KSeF (2 miejsca dziesiętne)
    /// </summary>
    /// <param name="amount">Kwota do sformatowania</param>
    /// <returns>Sformatowana kwota jako string</returns>
    public static string FormatAmount(decimal amount)
    {
        return amount.ToString($"F{MaxDecimalPlaces}", InvariantCulture);
    }

    /// <summary>
    /// Formatuje kwotę z określoną liczbą miejsc dziesiętnych
    /// </summary>
    /// <param name="amount">Kwota do sformatowania</param>
    /// <param name="decimalPlaces">Liczba miejsc dziesiętnych</param>
    /// <returns>Sformatowana kwota jako string</returns>
    public static string FormatAmount(decimal amount, int decimalPlaces)
    {
        if (decimalPlaces < 0 || decimalPlaces > MaxUnitPriceDecimalPlaces)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces),
                $"Liczba miejsc dziesiętnych musi być między 0 a {MaxUnitPriceDecimalPlaces}.");
        }

        return amount.ToString($"F{decimalPlaces}", InvariantCulture);
    }

    /// <summary>
    /// Formatuje cenę jednostkową zgodnie z wymaganiami KSeF (do 8 miejsc dziesiętnych)
    /// </summary>
    /// <param name="unitPrice">Cena jednostkowa do sformatowania</param>
    /// <returns>Sformatowana cena jako string</returns>
    public static string FormatUnitPrice(decimal unitPrice)
    {
        // Usuwamy końcowe zera, ale zachowujemy przynajmniej 2 miejsca dziesiętne
        var formatted = unitPrice.ToString($"F{MaxUnitPriceDecimalPlaces}", InvariantCulture);
        return TrimTrailingZeros(formatted, MaxDecimalPlaces);
    }

    /// <summary>
    /// Formatuje ilość zgodnie z wymaganiami KSeF (do 8 miejsc dziesiętnych)
    /// </summary>
    /// <param name="quantity">Ilość do sformatowania</param>
    /// <returns>Sformatowana ilość jako string</returns>
    public static string FormatQuantity(decimal quantity)
    {
        var formatted = quantity.ToString($"F{MaxQuantityDecimalPlaces}", InvariantCulture);
        return TrimTrailingZeros(formatted, 0);
    }

    /// <summary>
    /// Formatuje datę zgodnie z XSD (xs:date) - YYYY-MM-DD
    /// </summary>
    /// <param name="date">Data do sformatowania</param>
    /// <returns>Sformatowana data jako string</returns>
    public static string FormatDate(DateOnly date)
    {
        return date.ToString(DateFormat, InvariantCulture);
    }

    /// <summary>
    /// Formatuje datę zgodnie z XSD (xs:date) - YYYY-MM-DD
    /// </summary>
    /// <param name="date">Data do sformatowania</param>
    /// <returns>Sformatowana data jako string</returns>
    public static string FormatDate(DateTime date)
    {
        return date.ToString(DateFormat, InvariantCulture);
    }

    /// <summary>
    /// Formatuje datę i czas zgodnie z XSD (xs:dateTime)
    /// </summary>
    /// <param name="dateTime">Data i czas do sformatowania</param>
    /// <returns>Sformatowana data i czas jako string</returns>
    public static string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString(DateTimeFormat, InvariantCulture);
    }

    /// <summary>
    /// Formatuje datę i czas jako UTC zgodnie z XSD (xs:dateTime)
    /// </summary>
    /// <param name="dateTime">Data i czas do sformatowania</param>
    /// <returns>Sformatowana data i czas UTC jako string</returns>
    public static string FormatDateTimeUtc(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString(DateTimeUtcFormat, InvariantCulture);
    }

    /// <summary>
    /// Parsuje kwotę z formatu XSD
    /// </summary>
    /// <param name="value">Wartość do sparsowania</param>
    /// <returns>Sparsowana kwota</returns>
    public static decimal ParseAmount(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return decimal.Parse(value, NumberStyles.Number, InvariantCulture);
    }

    /// <summary>
    /// Parsuje kwotę z formatu XSD, zwraca null dla pustych wartości
    /// </summary>
    /// <param name="value">Wartość do sparsowania</param>
    /// <returns>Sparsowana kwota lub null</returns>
    public static decimal? ParseAmountOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return decimal.Parse(value, NumberStyles.Number, InvariantCulture);
    }

    /// <summary>
    /// Parsuje datę z formatu XSD (xs:date)
    /// </summary>
    /// <param name="value">Wartość do sparsowania</param>
    /// <returns>Sparsowana data</returns>
    public static DateOnly ParseDate(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return DateOnly.ParseExact(value, DateFormat, InvariantCulture);
    }

    /// <summary>
    /// Parsuje datę z formatu XSD (xs:date), zwraca null dla pustych wartości
    /// </summary>
    /// <param name="value">Wartość do sparsowania</param>
    /// <returns>Sparsowana data lub null</returns>
    public static DateOnly? ParseDateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateOnly.ParseExact(value, DateFormat, InvariantCulture);
    }

    /// <summary>
    /// Parsuje datę i czas z formatu XSD (xs:dateTime)
    /// </summary>
    /// <param name="value">Wartość do sparsowania</param>
    /// <returns>Sparsowana data i czas</returns>
    public static DateTime ParseDateTime(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        // Obsługa formatu z Z na końcu (UTC)
        if (value.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
        {
            return DateTime.ParseExact(value, DateTimeUtcFormat, InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        return DateTime.ParseExact(value, DateTimeFormat, InvariantCulture);
    }

    /// <summary>
    /// Formatuje NIP (usuwa kreski i spacje)
    /// </summary>
    /// <param name="nip">NIP do sformatowania</param>
    /// <returns>Sformatowany NIP (10 cyfr)</returns>
    public static string FormatNip(string nip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nip);
        return new string(nip.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Waliduje NIP
    /// </summary>
    /// <param name="nip">NIP do walidacji</param>
    /// <returns>True jeśli NIP jest poprawny</returns>
    public static bool ValidateNip(string? nip)
    {
        if (string.IsNullOrWhiteSpace(nip))
        {
            return false;
        }

        var digitsOnly = FormatNip(nip);
        if (digitsOnly.Length != 10)
        {
            return false;
        }

        // Wagi dla kontroli sumy
        int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            sum += (digitsOnly[i] - '0') * weights[i];
        }

        int checkDigit = sum % 11;
        if (checkDigit == 10)
        {
            checkDigit = 0;
        }

        return checkDigit == (digitsOnly[9] - '0');
    }

    /// <summary>
    /// Zaokrągla kwotę do 2 miejsc po przecinku zgodnie z wymaganiami KSeF
    /// </summary>
    /// <param name="amount">Kwota do zaokrąglenia</param>
    /// <returns>Zaokrąglona kwota</returns>
    public static decimal RoundAmount(decimal amount)
    {
        return Math.Round(amount, MaxDecimalPlaces, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Usuwa końcowe zera z sformatowanej liczby, zachowując minimalną liczbę miejsc dziesiętnych
    /// </summary>
    private static string TrimTrailingZeros(string formatted, int minDecimalPlaces)
    {
        int decimalIndex = formatted.IndexOf('.');
        if (decimalIndex == -1)
        {
            return formatted;
        }

        int minLength = decimalIndex + 1 + minDecimalPlaces;
        int lastNonZero = formatted.Length - 1;

        while (lastNonZero > minLength - 1 && formatted[lastNonZero] == '0')
        {
            lastNonZero--;
        }

        // Jeśli wszystkie miejsca dziesiętne to zera i minDecimalPlaces = 0, usuń też kropkę
        if (lastNonZero == decimalIndex && minDecimalPlaces == 0)
        {
            return formatted[..decimalIndex];
        }

        return formatted[..(lastNonZero + 1)];
    }
}
