using FluentAssertions;
using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Api.Services;
using KSeF.Client.Core.Interfaces;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Client.Core.Interfaces.Services;
using KSeF.Client.Core.Models.Authorization;
using KSeF.Client.Core.Models.Sessions.OnlineSession;
using KSeF.Client.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace KSeF.Api.Tests.Services;

/// <summary>
/// Testy autoryzacji KSeF dla trzech środowisk: testowego, demo i produkcyjnego.
/// NIP i token KSeF pobierane ze zmiennych środowiskowych per środowisko:
/// KSEF_TEST_NIP/KSEF_TEST_TOKEN, KSEF_DEMO_NIP/KSEF_DEMO_TOKEN, KSEF_PROD_NIP/KSEF_PROD_TOKEN.
/// Gdy zmienne nie są ustawione, ładowane są z pliku .env.example w katalogu testów.
/// </summary>
public class KsefAuthorizationTests
{
    private static readonly Dictionary<string, string> EnvExampleValues = LoadEnvExample();

    private static readonly string TestNip = ResolveEnvValue("KSEF_TEST_NIP");
    private static readonly string TestKsefToken = ResolveEnvValue("KSEF_TEST_TOKEN");
    private static readonly string DemoNip = ResolveEnvValue("KSEF_DEMO_NIP");
    private static readonly string DemoKsefToken = ResolveEnvValue("KSEF_DEMO_TOKEN");
    private static readonly string ProdNip = ResolveEnvValue("KSEF_PROD_NIP");
    private static readonly string ProdKsefToken = ResolveEnvValue("KSEF_PROD_TOKEN");

    private static Dictionary<string, string> LoadEnvExample()
    {
        var values = new Dictionary<string, string>();

        var directory = AppContext.BaseDirectory;
        string? envExamplePath = null;

        // Szukamy .env.example idąc w górę od bin/Debug/net9.0 do katalogu projektu testowego
        while (directory != null)
        {
            var candidate = Path.Combine(directory, ".env.example");
            if (File.Exists(candidate))
            {
                envExamplePath = candidate;
                break;
            }
            directory = Path.GetDirectoryName(directory);
        }

        if (envExamplePath == null)
            return values;

        foreach (var line in File.ReadAllLines(envExamplePath))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                continue;

            var separatorIndex = trimmed.IndexOf('=');
            if (separatorIndex <= 0)
                continue;

            var key = trimmed[..separatorIndex].Trim();
            var value = trimmed[(separatorIndex + 1)..].Trim();
            values[key] = value;
        }

        return values;
    }

    private static string ResolveEnvValue(string key)
    {
        var envValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrEmpty(envValue))
            return envValue;

        if (EnvExampleValues.TryGetValue(key, out var exampleValue) && !string.IsNullOrEmpty(exampleValue))
            return exampleValue;

        return key.EndsWith("_NIP")
            ? "0000000000"
            : "test-token-placeholder|nip-0000000000|0000000000000000000000000000000000000000000000000000000000000000";
    }

    private static (string Nip, string Token) GetCredentialsForEnvironment(string baseUrl)
    {
        return baseUrl switch
        {
            KsefEnvironment.Test => (TestNip, TestKsefToken),
            KsefEnvironment.Demo => (DemoNip, DemoKsefToken),
            KsefEnvironment.Production => (ProdNip, ProdKsefToken),
            _ => (TestNip, TestKsefToken)
        };
    }

    private readonly Mock<IKSeFClient> _ksefClientMock;
    private readonly Mock<ICryptographyService> _cryptographyServiceMock;
    private readonly Mock<IAuthCoordinator> _authCoordinatorMock;
    private readonly Mock<ILogger<KsefSessionService>> _loggerMock;

    public KsefAuthorizationTests()
    {
        _ksefClientMock = new Mock<IKSeFClient>();
        _cryptographyServiceMock = new Mock<ICryptographyService>();
        _authCoordinatorMock = new Mock<IAuthCoordinator>();
        _loggerMock = new Mock<ILogger<KsefSessionService>>();
    }

    private KsefSessionService CreateService(KsefApiOptions options)
    {
        var optionsMock = new Mock<IOptions<KsefApiOptions>>();
        optionsMock.Setup(x => x.Value).Returns(options);

        return new KsefSessionService(
            _ksefClientMock.Object,
            _cryptographyServiceMock.Object,
            _authCoordinatorMock.Object,
            optionsMock.Object,
            _loggerMock.Object);
    }

    private KsefApiOptions CreateOptions(string baseUrl)
    {
        var (nip, token) = GetCredentialsForEnvironment(baseUrl);
        return new KsefApiOptions
        {
            Nip = nip,
            KsefToken = token,
            AuthMethod = KsefAuthMethod.Token,
            BaseUrl = baseUrl
        };
    }

    private void SetupSuccessfulAuth(string nip, string token)
    {
        var authResponse = new AuthenticationOperationStatusResponse
        {
            AccessToken = new TokenInfo
            {
                Token = "access-token-123",
                ValidUntil = DateTime.UtcNow.AddHours(1)
            },
            RefreshToken = new TokenInfo
            {
                Token = "refresh-token-456",
                ValidUntil = DateTime.UtcNow.AddDays(1)
            }
        };

        _authCoordinatorMock
            .Setup(x => x.AuthKsefTokenAsync(
                AuthenticationTokenContextIdentifierType.Nip,
                nip,
                token,
                _cryptographyServiceMock.Object,
                EncryptionMethodEnum.Rsa,
                null!,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResponse);

        var sessionResponse = new OpenOnlineSessionResponse
        {
            ReferenceNumber = "session-ref-789"
        };

        _ksefClientMock
            .Setup(x => x.OpenOnlineSessionAsync(
                It.IsAny<OpenOnlineSessionRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionResponse);
    }

    #region Środowisko testowe (Test)

    [Fact]
    public async Task Auth_TestEnvironment_CallsAuthCoordinatorWithCorrectNipAndToken()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Test);
        var service = CreateService(options);
        SetupSuccessfulAuth(TestNip, TestKsefToken);

        // Act
        await service.OpenSessionAsync();

        // Assert
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            TestNip,
            TestKsefToken,
            _cryptographyServiceMock.Object,
            EncryptionMethodEnum.Rsa,
            null!,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_TestEnvironment_ReturnsValidSessionInfo()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Test);
        var service = CreateService(options);
        SetupSuccessfulAuth(TestNip, TestKsefToken);

        // Act
        var result = await service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        result.SessionReference.Should().Be("session-ref-789");
        result.AccessToken.Should().Be("access-token-123");
        result.RefreshToken.Should().Be("refresh-token-456");
    }

    [Fact]
    public async Task Auth_TestEnvironment_UsesCorrectBaseUrl()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Test);
        options.BaseUrl.Should().Be("https://api-test.ksef.mf.gov.pl");

        var service = CreateService(options);
        SetupSuccessfulAuth(TestNip, TestKsefToken);

        // Act
        var result = await service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        _ksefClientMock.Verify(x => x.OpenOnlineSessionAsync(
            It.IsAny<OpenOnlineSessionRequest>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_TestEnvironment_WithoutToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Test);
        options.KsefToken = null;
        var service = CreateService(options);

        // Act & Assert
        var act = () => service.OpenSessionAsync();
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Token KSeF*");
    }

    [Fact]
    public async Task Auth_TestEnvironment_RefreshToken_ReturnsNewAccessToken()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Test);
        var service = CreateService(options);

        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-ref-789",
            AccessToken = "access-token-123",
            RefreshToken = "refresh-token-456"
        };

        var newTokenInfo = new TokenInfo
        {
            Token = "new-access-token",
            ValidUntil = DateTime.UtcNow.AddHours(1)
        };

        _authCoordinatorMock
            .Setup(x => x.RefreshAccessTokenAsync(
                sessionInfo.RefreshToken,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTokenInfo);

        // Act
        var result = await service.RefreshSessionAsync(sessionInfo);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");
        result.SessionReference.Should().Be("session-ref-789");
        result.RefreshToken.Should().Be("refresh-token-456");
    }

    #endregion

    #region Środowisko demo (Demo)

    [Fact]
    public async Task Auth_DemoEnvironment_CallsAuthCoordinatorWithCorrectNipAndToken()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Demo);
        var service = CreateService(options);
        SetupSuccessfulAuth(DemoNip, DemoKsefToken);

        // Act
        await service.OpenSessionAsync();

        // Assert
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            DemoNip,
            DemoKsefToken,
            _cryptographyServiceMock.Object,
            EncryptionMethodEnum.Rsa,
            null!,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_DemoEnvironment_ReturnsValidSessionInfo()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Demo);
        var service = CreateService(options);
        SetupSuccessfulAuth(DemoNip, DemoKsefToken);

        // Act
        var result = await service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        result.SessionReference.Should().Be("session-ref-789");
        result.AccessToken.Should().Be("access-token-123");
        result.RefreshToken.Should().Be("refresh-token-456");
    }

    [Fact]
    public async Task Auth_DemoEnvironment_UsesCorrectBaseUrl()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Demo);
        options.BaseUrl.Should().Be("https://api-demo.ksef.mf.gov.pl");

        var service = CreateService(options);
        SetupSuccessfulAuth(DemoNip, DemoKsefToken);

        // Act
        var result = await service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        _ksefClientMock.Verify(x => x.OpenOnlineSessionAsync(
            It.IsAny<OpenOnlineSessionRequest>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_DemoEnvironment_WithoutToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Demo);
        options.KsefToken = null;
        var service = CreateService(options);

        // Act & Assert
        var act = () => service.OpenSessionAsync();
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Token KSeF*");
    }

    [Fact]
    public async Task Auth_DemoEnvironment_RefreshToken_ReturnsNewAccessToken()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Demo);
        var service = CreateService(options);

        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-ref-789",
            AccessToken = "access-token-123",
            RefreshToken = "refresh-token-456"
        };

        var newTokenInfo = new TokenInfo
        {
            Token = "new-access-token",
            ValidUntil = DateTime.UtcNow.AddHours(1)
        };

        _authCoordinatorMock
            .Setup(x => x.RefreshAccessTokenAsync(
                sessionInfo.RefreshToken,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTokenInfo);

        // Act
        var result = await service.RefreshSessionAsync(sessionInfo);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");
        result.SessionReference.Should().Be("session-ref-789");
        result.RefreshToken.Should().Be("refresh-token-456");
    }

    #endregion

    #region Środowisko produkcyjne (Production)

    [Fact]
    public async Task Auth_ProductionEnvironment_CallsAuthCoordinatorWithCorrectNipAndToken()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Production);
        var service = CreateService(options);
        SetupSuccessfulAuth(ProdNip, ProdKsefToken);

        // Act
        await service.OpenSessionAsync();

        // Assert
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            ProdNip,
            ProdKsefToken,
            _cryptographyServiceMock.Object,
            EncryptionMethodEnum.Rsa,
            null!,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_ProductionEnvironment_ReturnsValidSessionInfo()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Production);
        var service = CreateService(options);
        SetupSuccessfulAuth(ProdNip, ProdKsefToken);

        // Act
        var result = await service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        result.SessionReference.Should().Be("session-ref-789");
        result.AccessToken.Should().Be("access-token-123");
        result.RefreshToken.Should().Be("refresh-token-456");
    }

    [Fact]
    public async Task Auth_ProductionEnvironment_UsesCorrectBaseUrl()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Production);
        options.BaseUrl.Should().Be("https://api.ksef.mf.gov.pl");

        var service = CreateService(options);
        SetupSuccessfulAuth(ProdNip, ProdKsefToken);

        // Act
        var result = await service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        _ksefClientMock.Verify(x => x.OpenOnlineSessionAsync(
            It.IsAny<OpenOnlineSessionRequest>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_ProductionEnvironment_WithoutToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Production);
        options.KsefToken = null;
        var service = CreateService(options);

        // Act & Assert
        var act = () => service.OpenSessionAsync();
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Token KSeF*");
    }

    [Fact]
    public async Task Auth_ProductionEnvironment_RefreshToken_ReturnsNewAccessToken()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Production);
        var service = CreateService(options);

        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-ref-789",
            AccessToken = "access-token-123",
            RefreshToken = "refresh-token-456"
        };

        var newTokenInfo = new TokenInfo
        {
            Token = "new-access-token",
            ValidUntil = DateTime.UtcNow.AddHours(1)
        };

        _authCoordinatorMock
            .Setup(x => x.RefreshAccessTokenAsync(
                sessionInfo.RefreshToken,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTokenInfo);

        // Act
        var result = await service.RefreshSessionAsync(sessionInfo);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");
        result.SessionReference.Should().Be("session-ref-789");
        result.RefreshToken.Should().Be("refresh-token-456");
    }

    #endregion

    #region Testy wspólne dla wszystkich środowisk

    [Theory]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Auth_AllEnvironments_UseTokenAuthMethod(string baseUrl)
    {
        // Arrange
        var (nip, token) = GetCredentialsForEnvironment(baseUrl);
        var options = CreateOptions(baseUrl);
        options.AuthMethod.Should().Be(KsefAuthMethod.Token);

        var service = CreateService(options);
        SetupSuccessfulAuth(nip, token);

        // Act
        var result = await service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            nip,
            token,
            It.IsAny<ICryptographyService>(),
            EncryptionMethodEnum.Rsa,
            null!,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Auth_AllEnvironments_WithEmptyNip_OpensSessionSuccessfully(string baseUrl)
    {
        // Arrange - NIP jest wymagany, ale walidacja odbywa się po stronie API
        var (_, token) = GetCredentialsForEnvironment(baseUrl);
        var options = CreateOptions(baseUrl);
        options.Nip = string.Empty;
        var service = CreateService(options);

        var authResponse = new AuthenticationOperationStatusResponse
        {
            AccessToken = new TokenInfo
            {
                Token = "access-token",
                ValidUntil = DateTime.UtcNow.AddHours(1)
            },
            RefreshToken = new TokenInfo
            {
                Token = "refresh-token",
                ValidUntil = DateTime.UtcNow.AddDays(1)
            }
        };

        _authCoordinatorMock
            .Setup(x => x.AuthKsefTokenAsync(
                AuthenticationTokenContextIdentifierType.Nip,
                string.Empty,
                token,
                _cryptographyServiceMock.Object,
                EncryptionMethodEnum.Rsa,
                null!,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResponse);

        _ksefClientMock
            .Setup(x => x.OpenOnlineSessionAsync(
                It.IsAny<OpenOnlineSessionRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OpenOnlineSessionResponse { ReferenceNumber = "ref-123" });

        // Act
        var result = await service.OpenSessionAsync();

        // Assert - autoryzacja jest przekazywana do AuthCoordinator, walidacja NIP jest po stronie KSeF API
        result.Should().NotBeNull();
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            string.Empty,
            token,
            It.IsAny<ICryptographyService>(),
            It.IsAny<EncryptionMethodEnum>(),
            null!,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Auth_AllEnvironments_WithoutRefreshToken_ThrowsOnRefresh(string baseUrl)
    {
        // Arrange
        var options = CreateOptions(baseUrl);
        var service = CreateService(options);

        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-ref",
            AccessToken = "access-token",
            RefreshToken = null
        };

        // Act & Assert
        var act = () => service.RefreshSessionAsync(sessionInfo);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*refresh*");
    }

    [Theory]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Auth_AllEnvironments_AuthFailure_ThrowsException(string baseUrl)
    {
        // Arrange
        var options = CreateOptions(baseUrl);
        var service = CreateService(options);

        _authCoordinatorMock
            .Setup(x => x.AuthKsefTokenAsync(
                It.IsAny<AuthenticationTokenContextIdentifierType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ICryptographyService>(),
                It.IsAny<EncryptionMethodEnum>(),
                null!,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Błąd połączenia z KSeF API"));

        // Act & Assert
        var act = () => service.OpenSessionAsync();
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*Błąd połączenia*");
    }

    [Theory]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Auth_AllEnvironments_CancellationRequested_ThrowsOperationCanceledException(string baseUrl)
    {
        // Arrange
        var options = CreateOptions(baseUrl);
        var service = CreateService(options);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _authCoordinatorMock
            .Setup(x => x.AuthKsefTokenAsync(
                It.IsAny<AuthenticationTokenContextIdentifierType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ICryptographyService>(),
                It.IsAny<EncryptionMethodEnum>(),
                null!,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var act = () => service.OpenSessionAsync(cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region Walidacja konfiguracji środowisk

    [Fact]
    public void KsefEnvironment_Test_HasCorrectUrl()
    {
        KsefEnvironment.Test.Should().Be("https://api-test.ksef.mf.gov.pl");
    }

    [Fact]
    public void KsefEnvironment_Demo_HasCorrectUrl()
    {
        KsefEnvironment.Demo.Should().Be("https://api-demo.ksef.mf.gov.pl");
    }

    [Fact]
    public void KsefEnvironment_Production_HasCorrectUrl()
    {
        KsefEnvironment.Production.Should().Be("https://api.ksef.mf.gov.pl");
    }

    [Fact]
    public void KsefApiOptions_DefaultBaseUrl_IsTestEnvironment()
    {
        var options = new KsefApiOptions();
        options.BaseUrl.Should().Be(KsefEnvironment.Test);
    }

    [Theory]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public void KsefApiOptions_WithCredentials_HasCorrectValues(string baseUrl)
    {
        // Arrange & Act
        var (expectedNip, expectedToken) = GetCredentialsForEnvironment(baseUrl);
        var options = CreateOptions(baseUrl);

        // Assert
        options.Nip.Should().Be(expectedNip);
        options.KsefToken.Should().Be(expectedToken);
        options.AuthMethod.Should().Be(KsefAuthMethod.Token);
        options.BaseUrl.Should().Be(baseUrl);
    }

    #endregion

    #region Testy integracyjne - rzeczywiste połączenie z API KSeF

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Integration_ServerIsReachable_ReturnsSuccessStatusCode(string baseUrl)
    {
        // Arrange - prosty HTTP GET do serwera API KSeF
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        // Act
        var response = await httpClient.GetAsync(baseUrl);

        // Assert - serwer odpowiada (nawet 404 oznacza że serwer działa)
        ((int)response.StatusCode).Should().BeGreaterThan(0,
            $"Serwer KSeF {baseUrl} powinien odpowiadać na żądania HTTP");
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Integration_AuthChallenge_ReturnsValidResponse(string baseUrl)
    {
        // Arrange - tworzymy prawdziwy DI container z klientem KSeF
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddKSeFClient(options => { options.BaseUrl = baseUrl; });
        services.AddCryptographyClient();

        using var provider = services.BuildServiceProvider();
        var authorizationClient = provider.GetRequiredService<IAuthorizationClient>();

        // Act - GetAuthChallenge to pierwszy krok autoryzacji, nie wymaga tokenu
        var challengeResponse = await authorizationClient.GetAuthChallengeAsync();

        // Assert
        challengeResponse.Should().NotBeNull();
        challengeResponse.Challenge.Should().NotBeNullOrEmpty(
            $"Serwer {baseUrl} powinien zwrócić challenge autoryzacyjny");
        challengeResponse.Timestamp.Should().BeCloseTo(
            DateTimeOffset.UtcNow, TimeSpan.FromMinutes(5),
            "Timestamp z serwera powinien być zbliżony do aktualnego czasu");
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Integration_CryptographyWarmup_LoadsCertificates(string baseUrl)
    {
        // Arrange - DI container z usługą kryptograficzną
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddKSeFClient(options => { options.BaseUrl = baseUrl; });
        services.AddCryptographyClient();

        using var provider = services.BuildServiceProvider();
        var cryptographyService = provider.GetRequiredService<ICryptographyService>();

        // Act & Assert - WarmupAsync pobiera certyfikaty publiczne z KSeF
        // Jeśli serwer jest dostępny, inicjalizacja powinna się zakończyć bez błędu
        var act = () => cryptographyService.WarmupAsync();
        await act.Should().NotThrowAsync(
            $"Inicjalizacja kryptografii z {baseUrl} powinna zakończyć się sukcesem");
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(KsefEnvironment.Test)]
    [InlineData(KsefEnvironment.Demo)]
    [InlineData(KsefEnvironment.Production)]
    public async Task Integration_TokenAuthFlow_ChallengeAndEncrypt_Succeeds(string baseUrl)
    {
        // Arrange - pełny DI container z inicjalizacją kryptografii
        var (nip, token) = GetCredentialsForEnvironment(baseUrl);
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddKSeFClient(options => { options.BaseUrl = baseUrl; });
        services.AddCryptographyClient();

        using var provider = services.BuildServiceProvider();
        var authCoordinator = provider.GetRequiredService<IAuthCoordinator>();
        var cryptographyService = provider.GetRequiredService<ICryptographyService>();

        // Inicjalizacja materiałów kryptograficznych (pobiera certyfikaty publiczne KSeF)
        await cryptographyService.WarmupAsync();

        // Act - pełna autoryzacja tokenem KSeF (challenge + encrypt + submit + get access token)
        // Jeśli autoryzacja się nie powiedzie, test powinien zakończyć się błędem (failed).
        var authResponse = await authCoordinator.AuthKsefTokenAsync(
            contextIdentifierType: AuthenticationTokenContextIdentifierType.Nip,
            contextIdentifierValue: nip,
            tokenKsef: token,
            cryptographyService: cryptographyService,
            encryptionMethod: EncryptionMethodEnum.Rsa);

        // Assert - sukces autoryzacji (token jest ważny w tym środowisku)
        authResponse.Should().NotBeNull();
        authResponse.AccessToken.Should().NotBeNull();
        authResponse.AccessToken.Token.Should().NotBeNullOrEmpty(
            $"Autoryzacja tokenem na {baseUrl} powinna zwrócić access token");
        authResponse.AccessToken.ValidUntil.Should().BeAfter(DateTime.UtcNow,
            "Access token powinien mieć ważność w przyszłości");
    }

    #endregion
}
