namespace KSeF.Api.Models;

/// <summary>
/// Informacje o sesji KSeF
/// </summary>
public class SessionInfo
{
    /// <summary>
    /// Numer referencyjny sesji
    /// </summary>
    public string SessionReference { get; set; } = string.Empty;

    /// <summary>
    /// Token dostępowy do sesji
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token odświeżający
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Data wygaśnięcia tokenu dostępowego
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Czy sesja jest aktywna
    /// </summary>
    public bool IsActive => ExpiresAt == null || ExpiresAt > DateTime.UtcNow;
}
