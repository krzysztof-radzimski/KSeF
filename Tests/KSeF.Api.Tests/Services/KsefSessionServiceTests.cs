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

public class KsefSessionServiceTests
{
    private readonly Mock<IKSeFClient> _ksefClientMock;
    private readonly Mock<ICryptographyService> _cryptographyServiceMock;
    private readonly Mock<IAuthCoordinator> _authCoordinatorMock;
    private readonly Mock<IOptions<KsefApiOptions>> _optionsMock;
    private readonly Mock<ILogger<KsefSessionService>> _loggerMock;
    private readonly KsefSessionService _service;
    private readonly KsefApiOptions _options;

    public KsefSessionServiceTests()
    {
        _ksefClientMock = new Mock<IKSeFClient>();
        _cryptographyServiceMock = new Mock<ICryptographyService>();
        _authCoordinatorMock = new Mock<IAuthCoordinator>();
        _optionsMock = new Mock<IOptions<KsefApiOptions>>();
        _loggerMock = new Mock<ILogger<KsefSessionService>>();

        _options = new KsefApiOptions
        {
            Nip = "1234567890",
            KsefToken = "test-token",
            AuthMethod = KsefAuthMethod.Token,
            BaseUrl = KsefEnvironment.Test
        };

        _optionsMock.Setup(x => x.Value).Returns(_options);

        _service = new KsefSessionService(
            _ksefClientMock.Object,
            _cryptographyServiceMock.Object,
            _authCoordinatorMock.Object,
            _optionsMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task OpenSessionAsync_WithTokenAuth_ReturnsSessionInfo()
    {
        // Arrange
        var authResponse = new AuthenticationOperationStatusResponse
        {
            AccessToken = new TokenInfo { Token = "access-token", ValidUntil = DateTime.UtcNow.AddHours(1) },
            RefreshToken = new TokenInfo { Token = "refresh-token", ValidUntil = DateTime.UtcNow.AddDays(1) }
        };

        var sessionResponse = new OpenOnlineSessionResponse
        {
            ReferenceNumber = "session-123"
        };

        _authCoordinatorMock
            .Setup(x => x.AuthKsefTokenAsync(
                AuthenticationTokenContextIdentifierType.Nip,
                _options.Nip,
                _options.KsefToken,
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
            .ReturnsAsync(sessionResponse);

        // Act
        var result = await _service.OpenSessionAsync();

        // Assert
        result.Should().NotBeNull();
        result.SessionReference.Should().Be("session-123");
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");

        _authCoordinatorMock.Verify(x => x.AuthKsefTokenAsync(
            It.IsAny<AuthenticationTokenContextIdentifierType>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ICryptographyService>(),
            It.IsAny<EncryptionMethodEnum>(),
            null!,
            It.IsAny<CancellationToken>()), Times.Once);

        _ksefClientMock.Verify(x => x.OpenOnlineSessionAsync(
            It.IsAny<OpenOnlineSessionRequest>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OpenSessionAsync_WithoutKsefToken_ThrowsInvalidOperationException()
    {
        // Arrange
        _options.KsefToken = null;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.OpenSessionAsync());
    }

    [Fact]
    public async Task CloseSessionAsync_CallsKsefClient()
    {
        // Arrange
        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-123",
            AccessToken = "access-token"
        };

        _ksefClientMock
            .Setup(x => x.CloseOnlineSessionAsync(
                sessionInfo.SessionReference,
                sessionInfo.AccessToken,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.CloseSessionAsync(sessionInfo);

        // Assert
        _ksefClientMock.Verify(x => x.CloseOnlineSessionAsync(
            sessionInfo.SessionReference,
            sessionInfo.AccessToken,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshSessionAsync_WithRefreshToken_ReturnsUpdatedSessionInfo()
    {
        // Arrange
        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-123",
            AccessToken = "old-access-token",
            RefreshToken = "refresh-token"
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
        var result = await _service.RefreshSessionAsync(sessionInfo);

        // Assert
        result.Should().NotBeNull();
        result.SessionReference.Should().Be("session-123");
        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("refresh-token");

        _authCoordinatorMock.Verify(x => x.RefreshAccessTokenAsync(
            sessionInfo.RefreshToken,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshSessionAsync_WithoutRefreshToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-123",
            AccessToken = "access-token",
            RefreshToken = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RefreshSessionAsync(sessionInfo));
    }
}
