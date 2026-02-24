namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Wynik walidacji faktury
/// </summary>
public class ValidationResult
{
    private readonly List<ValidationError> _errors = new();
    private readonly List<ValidationWarning> _warnings = new();

    /// <summary>
    /// Lista błędów walidacji (uniemożliwiających przetworzenie faktury)
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Lista ostrzeżeń walidacji (niepowodujących odrzucenia faktury)
    /// </summary>
    public IReadOnlyList<ValidationWarning> Warnings => _warnings.AsReadOnly();

    /// <summary>
    /// Określa czy walidacja zakończyła się sukcesem (brak błędów)
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Określa czy wystąpiły jakiekolwiek błędy
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Określa czy wystąpiły jakiekolwiek ostrzeżenia
    /// </summary>
    public bool HasWarnings => _warnings.Count > 0;

    /// <summary>
    /// Dodaje błąd walidacji
    /// </summary>
    /// <param name="code">Kod błędu</param>
    /// <param name="message">Komunikat błędu</param>
    /// <param name="fieldName">Nazwa pola, którego dotyczy błąd</param>
    public void AddError(string code, string message, string? fieldName = null)
    {
        _errors.Add(new ValidationError(code, message, fieldName));
    }

    /// <summary>
    /// Dodaje błąd walidacji
    /// </summary>
    /// <param name="error">Błąd walidacji</param>
    public void AddError(ValidationError error)
    {
        _errors.Add(error);
    }

    /// <summary>
    /// Dodaje ostrzeżenie walidacji
    /// </summary>
    /// <param name="code">Kod ostrzeżenia</param>
    /// <param name="message">Komunikat ostrzeżenia</param>
    /// <param name="fieldName">Nazwa pola, którego dotyczy ostrzeżenie</param>
    public void AddWarning(string code, string message, string? fieldName = null)
    {
        _warnings.Add(new ValidationWarning(code, message, fieldName));
    }

    /// <summary>
    /// Dodaje ostrzeżenie walidacji
    /// </summary>
    /// <param name="warning">Ostrzeżenie walidacji</param>
    public void AddWarning(ValidationWarning warning)
    {
        _warnings.Add(warning);
    }

    /// <summary>
    /// Łączy wyniki z innej walidacji
    /// </summary>
    /// <param name="other">Wynik walidacji do połączenia</param>
    public void Merge(ValidationResult other)
    {
        _errors.AddRange(other.Errors);
        _warnings.AddRange(other.Warnings);
    }

    /// <summary>
    /// Tworzy wynik walidacji zakończonej sukcesem
    /// </summary>
    /// <returns>Pusty wynik walidacji (bez błędów i ostrzeżeń)</returns>
    public static ValidationResult Success() => new();

    /// <summary>
    /// Tworzy wynik walidacji z błędem
    /// </summary>
    /// <param name="code">Kod błędu</param>
    /// <param name="message">Komunikat błędu</param>
    /// <param name="fieldName">Nazwa pola, którego dotyczy błąd</param>
    /// <returns>Wynik walidacji z jednym błędem</returns>
    public static ValidationResult WithError(string code, string message, string? fieldName = null)
    {
        var result = new ValidationResult();
        result.AddError(code, message, fieldName);
        return result;
    }

    /// <summary>
    /// Tworzy wynik walidacji z ostrzeżeniem
    /// </summary>
    /// <param name="code">Kod ostrzeżenia</param>
    /// <param name="message">Komunikat ostrzeżenia</param>
    /// <param name="fieldName">Nazwa pola, którego dotyczy ostrzeżenie</param>
    /// <returns>Wynik walidacji z jednym ostrzeżeniem</returns>
    public static ValidationResult WithWarning(string code, string message, string? fieldName = null)
    {
        var result = new ValidationResult();
        result.AddWarning(code, message, fieldName);
        return result;
    }
}

/// <summary>
/// Błąd walidacji faktury
/// </summary>
/// <param name="Code">Kod błędu</param>
/// <param name="Message">Komunikat błędu</param>
/// <param name="FieldName">Nazwa pola, którego dotyczy błąd (opcjonalne)</param>
public record ValidationError(string Code, string Message, string? FieldName = null);

/// <summary>
/// Ostrzeżenie walidacji faktury
/// </summary>
/// <param name="Code">Kod ostrzeżenia</param>
/// <param name="Message">Komunikat ostrzeżenia</param>
/// <param name="FieldName">Nazwa pola, którego dotyczy ostrzeżenie (opcjonalne)</param>
public record ValidationWarning(string Code, string Message, string? FieldName = null);
