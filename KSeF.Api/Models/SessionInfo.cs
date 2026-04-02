/*=====================================================================================

    KSeF.Api
    Model przechowujący informacje o sesji interaktywnej KSeF.
    Zawiera numer referencyjny sesji, token dostępowy, token
    odświeżający, datę wygaśnięcia tokenu oraz dane szyfrowania
    (klucz AES + IV) używane do szyfrowania faktur w ramach
    sesji. Udostępnia właściwość IsActive do sprawdzenia
    aktywności sesji.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Client.Core.Models.Sessions;

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
    /// Dane szyfrowania sesji (klucz AES + IV) używane do szyfrowania faktur w ramach sesji
    /// </summary>
    public EncryptionData? EncryptionData { get; set; }

    /// <summary>
    /// Czy sesja jest aktywna
    /// </summary>
    public bool IsActive => ExpiresAt == null || ExpiresAt > DateTime.UtcNow;
}
