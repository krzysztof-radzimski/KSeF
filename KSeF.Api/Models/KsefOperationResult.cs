namespace KSeF.Api.Models;

/// <summary>
/// Ogólny wynik operacji KSeF
/// </summary>
public class KsefOperationResult
{
    /// <summary>
    /// Czy operacja zakończyła się sukcesem
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Komunikat
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Lista błędów
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Tworzy wynik sukcesu
    /// </summary>
    public static KsefOperationResult Ok(string? message = null) => new()
    {
        Success = true,
        Message = message
    };

    /// <summary>
    /// Tworzy wynik błędu
    /// </summary>
    public static KsefOperationResult Fail(params string[] errors) => new()
    {
        Success = false,
        Errors = [.. errors]
    };
}
