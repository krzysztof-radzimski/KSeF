using KSeF.Api.Models;

namespace KSeF.Api.Services;

/// <summary>
/// Serwis zarządzania sesją interaktywną KSeF
/// </summary>
public interface IKsefSessionService
{
    /// <summary>
    /// Otwiera sesję interaktywną KSeF z autoryzacją skonfigurowaną w opcjach
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Informacje o sesji</returns>
    Task<SessionInfo> OpenSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Zamyka sesję interaktywną KSeF
    /// </summary>
    /// <param name="sessionInfo">Informacje o sesji do zamknięcia</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task CloseSessionAsync(SessionInfo sessionInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Odświeża token dostępowy sesji
    /// </summary>
    /// <param name="sessionInfo">Aktualne informacje o sesji</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Zaktualizowane informacje o sesji</returns>
    Task<SessionInfo> RefreshSessionAsync(SessionInfo sessionInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera status sesji
    /// </summary>
    /// <param name="sessionReference">Numer referencyjny sesji</param>
    /// <param name="accessToken">Token dostępowy</param>
    /// <param name="cancellationToken">Token anulowania</param>
    Task<KsefOperationResult> GetSessionStatusAsync(string sessionReference, string accessToken, CancellationToken cancellationToken = default);
}
