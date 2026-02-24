using FluentAssertions;
using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Api.Services;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Invoice;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace KSeF.Api.Tests.Services;

public class KsefInvoiceReceiveServiceTests
{
    private readonly Mock<IKSeFClient> _ksefClientMock;
    private readonly Mock<IKsefSessionService> _sessionServiceMock;
    private readonly Mock<IKsefInvoiceService> _invoiceServiceMock;
    private readonly Mock<IOptions<KsefApiOptions>> _optionsMock;
    private readonly Mock<ILogger<KsefInvoiceReceiveService>> _loggerMock;
    private readonly KsefInvoiceReceiveService _service;

    public KsefInvoiceReceiveServiceTests()
    {
        _ksefClientMock = new Mock<IKSeFClient>();
        _sessionServiceMock = new Mock<IKsefSessionService>();
        _invoiceServiceMock = new Mock<IKsefInvoiceService>();
        _optionsMock = new Mock<IOptions<KsefApiOptions>>();
        _loggerMock = new Mock<ILogger<KsefInvoiceReceiveService>>();

        var options = new KsefApiOptions
        {
            Nip = "1234567890",
            BaseUrl = KsefEnvironment.Test
        };

        _optionsMock.Setup(x => x.Value).Returns(options);

        _service = new KsefInvoiceReceiveService(
            _ksefClientMock.Object,
            _sessionServiceMock.Object,
            _invoiceServiceMock.Object,
            _optionsMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetInvoiceAsync_WithValidKsefNumber_ReturnsInvoice()
    {
        // Arrange
        var ksefNumber = "1234567890-20260101-ABCD1234-EF";
        var invoiceXml = "<Faktura><DaneKontrahenta>test</DaneKontrahenta></Faktura>";
        var invoice = new Invoice.Models.Invoice();

        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-123",
            AccessToken = "access-token"
        };

        _sessionServiceMock
            .Setup(x => x.OpenSessionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionInfo);

        _ksefClientMock
            .Setup(x => x.GetInvoiceAsync(ksefNumber, sessionInfo.AccessToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceXml);

        _invoiceServiceMock
            .Setup(x => x.FromXml(invoiceXml))
            .Returns(invoice);

        _sessionServiceMock
            .Setup(x => x.CloseSessionAsync(sessionInfo, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetInvoiceAsync(ksefNumber);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.KsefNumber.Should().Be(ksefNumber);
        result.InvoiceXml.Should().Be(invoiceXml);
        result.Invoice.Should().NotBeNull();

        _ksefClientMock.Verify(x => x.GetInvoiceAsync(
            ksefNumber, sessionInfo.AccessToken, It.IsAny<CancellationToken>()), Times.Once);
        _sessionServiceMock.Verify(x => x.OpenSessionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _sessionServiceMock.Verify(x => x.CloseSessionAsync(sessionInfo, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInvoiceAsync_WithExistingSession_UsesProvidedSession()
    {
        // Arrange
        var ksefNumber = "1234567890-20260101-ABCD1234-EF";
        var invoiceXml = "<Faktura>test</Faktura>";

        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-456",
            AccessToken = "access-token-456"
        };

        _ksefClientMock
            .Setup(x => x.GetInvoiceAsync(ksefNumber, sessionInfo.AccessToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceXml);

        _invoiceServiceMock
            .Setup(x => x.FromXml(invoiceXml))
            .Returns(new Invoice.Models.Invoice());

        // Act
        var result = await _service.GetInvoiceAsync(ksefNumber, sessionInfo);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        _sessionServiceMock.Verify(x => x.OpenSessionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _sessionServiceMock.Verify(x => x.CloseSessionAsync(It.IsAny<SessionInfo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetInvoiceAsync_WhenDeserializationFails_ReturnsXmlOnly()
    {
        // Arrange
        var ksefNumber = "1234567890-20260101-ABCD1234-EF";
        var invoiceXml = "<InvalidXml>";

        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-123",
            AccessToken = "access-token"
        };

        _sessionServiceMock
            .Setup(x => x.OpenSessionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionInfo);

        _ksefClientMock
            .Setup(x => x.GetInvoiceAsync(ksefNumber, sessionInfo.AccessToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceXml);

        _invoiceServiceMock
            .Setup(x => x.FromXml(invoiceXml))
            .Throws(new InvalidOperationException("Invalid XML"));

        _sessionServiceMock
            .Setup(x => x.CloseSessionAsync(sessionInfo, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetInvoiceAsync(ksefNumber);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.InvoiceXml.Should().Be(invoiceXml);
        result.Invoice.Should().BeNull();
    }
}
