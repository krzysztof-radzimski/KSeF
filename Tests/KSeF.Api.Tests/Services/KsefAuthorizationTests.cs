using FluentAssertions;
using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Api.Services;
using KSeF.Client.Core.Interfaces;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Client.Core.Interfaces.Services;
using KSeF.Client.Core.Models.Authorization;
using KSeF.Client.Core.Models.Sessions.OnlineSession;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace KSeF.Api.Tests.Services;

/// <summary>
/// Testy autoryzacji KSeF dla trzech środowisk: testowego, demo i produkcyjnego.
/// NIP i token KSeF pobierane ze zmiennych środowiskowych KSEF_TEST_NIP i KSEF_TEST_TOKEN.
/// Gdy zmienne nie są ustawione, używane są fikcyjne wartości testowe (testy mockowane).
/// </summary>
public class KsefAuthorizationTests
{
    private static readonly string TestNip =
        Environment.GetEnvironmentVariable("KSEF_TEST_NIP") ?? "0000000000";
    private static readonly string TestKsefToken =
        Environment.GetEnvironmentVariable("KSEF_TEST_TOKEN") ?? "test-token-placeholder|nip-0000000000|0000000000000000000000000000000000000000000000000000000000000000";

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
        return new KsefApiOptions
        {
            Nip = TestNip,
            KsefToken = TestKsefToken,
            AuthMethod = KsefAuthMethod.Token,
            BaseUrl = baseUrl
        };
    }

    private void SetupSuccessfulAuth()
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
                TestNip,
                TestKsefToken,
                _cryptographyServiceMock.Object,
                EncryptionMethodEnum.ECDsa,
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
        SetupSuccessfulAuth();

        // Act
        await service.OpenSessionAsync();

        // Assert
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            TestNip,
            TestKsefToken,
            _cryptographyServiceMock.Object,
            EncryptionMethodEnum.ECDsa,
            null!,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_TestEnvironment_ReturnsValidSessionInfo()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Test);
        var service = CreateService(options);
        SetupSuccessfulAuth();

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
        SetupSuccessfulAuth();

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
        SetupSuccessfulAuth();

        // Act
        await service.OpenSessionAsync();

        // Assert
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            TestNip,
            TestKsefToken,
            _cryptographyServiceMock.Object,
            EncryptionMethodEnum.ECDsa,
            null!,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_DemoEnvironment_ReturnsValidSessionInfo()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Demo);
        var service = CreateService(options);
        SetupSuccessfulAuth();

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
        SetupSuccessfulAuth();

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
        SetupSuccessfulAuth();

        // Act
        await service.OpenSessionAsync();

        // Assert
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            TestNip,
            TestKsefToken,
            _cryptographyServiceMock.Object,
            EncryptionMethodEnum.ECDsa,
            null!,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Auth_ProductionEnvironment_ReturnsValidSessionInfo()
    {
        // Arrange
        var options = CreateOptions(KsefEnvironment.Production);
        var service = CreateService(options);
        SetupSuccessfulAuth();

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
        SetupSuccessfulAuth();

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
        var options = CreateOptions(baseUrl);
        options.AuthMethod.Should().Be(KsefAuthMethod.Token);

        var service = CreateService(options);
        SetupSuccessfulAuth();

        // Act
        var result = await service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            AuthenticationTokenContextIdentifierType.Nip,
            TestNip,
            TestKsefToken,
            It.IsAny<ICryptographyService>(),
            EncryptionMethodEnum.ECDsa,
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
                TestKsefToken,
                _cryptographyServiceMock.Object,
                EncryptionMethodEnum.ECDsa,
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
            TestKsefToken,
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

    [Fact]
    public void KsefApiOptions_WithTestCredentials_HasCorrectValues()
    {
        // Arrange & Act
        var options = CreateOptions(KsefEnvironment.Test);

        // Assert
        options.Nip.Should().Be(TestNip);
        options.KsefToken.Should().Be(TestKsefToken);
        options.AuthMethod.Should().Be(KsefAuthMethod.Token);
        options.BaseUrl.Should().Be(KsefEnvironment.Test);
    }

    #endregion
}
