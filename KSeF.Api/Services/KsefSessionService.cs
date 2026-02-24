using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Client.Core.Interfaces;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Client.Core.Interfaces.Services;
using KSeF.Client.Core.Models.Authorization;
using KSeF.Client.Core.Models.Sessions.OnlineSession;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KSeF.Api.Services;

/// <summary>
/// Implementacja serwisu zarządzania sesją interaktywną KSeF.
/// Używa IAuthCoordinator do autoryzacji i IKSeFClient do operacji sesji.
/// </summary>
public class KsefSessionService : IKsefSessionService
{
    private readonly IKSeFClient _ksefClient;
    private readonly ICryptographyService _cryptographyService;
    private readonly IAuthCoordinator _authCoordinator;
    private readonly KsefApiOptions _options;
    private readonly ILogger<KsefSessionService> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję serwisu zarządzania sesją KSeF.
    /// </summary>
    /// <param name="ksefClient">Klient KSeF do wykonywania operacji API.</param>
    /// <param name="cryptographyService">Serwis kryptograficzny do szyfrowania i deszyfrowania danych.</param>
    /// <param name="authCoordinator">Koordynator autoryzacji do zarządzania tokenami dostępowymi.</param>
    /// <param name="options">Opcje konfiguracyjne KSeF API.</param>
    /// <param name="logger">Logger do rejestrowania operacji.</param>
    public KsefSessionService(
        IKSeFClient ksefClient,
        ICryptographyService cryptographyService,
        IAuthCoordinator authCoordinator,
        IOptions<KsefApiOptions> options,
        ILogger<KsefSessionService> logger)
    {
        _ksefClient = ksefClient;
        _cryptographyService = cryptographyService;
        _authCoordinator = authCoordinator;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<SessionInfo> OpenSessionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Otwieranie sesji interaktywnej KSeF dla NIP: {Nip}", _options.Nip);

        try
        {
            // 1. Autoryzacja - uzyskanie tokenu dostępowego
            var authResponse = _options.AuthMethod switch
            {
                KsefAuthMethod.Token => await AuthenticateWithTokenAsync(cancellationToken),
                KsefAuthMethod.Certificate => await AuthenticateWithCertificateAsync(cancellationToken),
                _ => throw new InvalidOperationException($"Nieobsługiwana metoda autoryzacji: {_options.AuthMethod}")
            };

            var accessTokenInfo = authResponse.AccessToken;
            var refreshTokenInfo = authResponse.RefreshToken;

            // 2. Otwarcie sesji interaktywnej
            var openSessionRequest = new OpenOnlineSessionRequest();
            var sessionResponse = await _ksefClient.OpenOnlineSessionAsync(
                openSessionRequest,
                accessTokenInfo.Token,
                cancellationToken: cancellationToken);

            var sessionInfo = new SessionInfo
            {
                SessionReference = sessionResponse.ReferenceNumber,
                AccessToken = accessTokenInfo.Token,
                RefreshToken = refreshTokenInfo?.Token
            };

            _logger.LogInformation("Sesja interaktywna otwarta. Ref: {SessionRef}", sessionInfo.SessionReference);
            return sessionInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas otwierania sesji KSeF");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CloseSessionAsync(SessionInfo sessionInfo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Zamykanie sesji interaktywnej KSeF. Ref: {SessionRef}", sessionInfo.SessionReference);

        try
        {
            await _ksefClient.CloseOnlineSessionAsync(
                sessionInfo.SessionReference,
                sessionInfo.AccessToken,
                cancellationToken);

            _logger.LogInformation("Sesja zamknięta pomyślnie");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas zamykania sesji KSeF");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<SessionInfo> RefreshSessionAsync(SessionInfo sessionInfo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(sessionInfo.RefreshToken))
        {
            throw new InvalidOperationException("Brak refresh tokenu - nie można odświeżyć sesji");
        }

        var tokenInfo = await _authCoordinator.RefreshAccessTokenAsync(
            sessionInfo.RefreshToken,
            cancellationToken);

        return new SessionInfo
        {
            SessionReference = sessionInfo.SessionReference,
            AccessToken = tokenInfo.Token,
            RefreshToken = sessionInfo.RefreshToken
        };
    }

    /// <inheritdoc />
    public async Task<KsefOperationResult> GetSessionStatusAsync(
        string sessionReference,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _ksefClient.GetSessionStatusAsync(
                sessionReference,
                accessToken,
                cancellationToken);

            return KsefOperationResult.Ok($"Status sesji: {status.Status?.Code}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas sprawdzania statusu sesji");
            return KsefOperationResult.Fail(ex.Message);
        }
    }

    private async Task<AuthenticationOperationStatusResponse> AuthenticateWithTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_options.KsefToken))
        {
            throw new InvalidOperationException(
                "Token KSeF nie został skonfigurowany. Ustaw wartość w konfiguracji: KSeF:KsefToken");
        }

        return await _authCoordinator.AuthKsefTokenAsync(
            contextIdentifierType: AuthenticationTokenContextIdentifierType.Nip,
            contextIdentifierValue: _options.Nip,
            tokenKsef: _options.KsefToken,
            cryptographyService: _cryptographyService,
            encryptionMethod: EncryptionMethodEnum.ECDsa,
            authorizationPolicy: null!,
            cancellationToken: cancellationToken);
    }

    private async Task<AuthenticationOperationStatusResponse> AuthenticateWithCertificateAsync(CancellationToken cancellationToken)
    {
        // Certyfikat jest konfigurowany przez ICertificateFetcher zarejestrowany w DI
        // (AddCryptographyClient z pemCertificatesFetcher w ServiceCollectionExtensions)
        return await _authCoordinator.AuthAsync(
            contextIdentifierType: AuthenticationTokenContextIdentifierType.Nip,
            contextIdentifierValue: _options.Nip,
            identifierType: AuthenticationTokenSubjectIdentifierTypeEnum.CertificateSubject,
            xmlSigner: null!,
            authorizationPolicy: null!,
            verifyCertificateChain: true,
            cancellationToken: cancellationToken);
    }
}
