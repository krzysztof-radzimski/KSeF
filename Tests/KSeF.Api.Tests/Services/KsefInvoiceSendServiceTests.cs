using FluentAssertions;
using KSeF.Api.Configuration;
using KSeF.Api.Models;
using KSeF.Api.Services;
using KSeF.Client.Core.Interfaces.Clients;
using KSeF.Client.Core.Interfaces.Services;
using KSeF.Invoice;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace KSeF.Api.Tests.Services;

public class KsefInvoiceSendServiceTests
{
    private readonly Mock<IKSeFClient> _ksefClientMock;
    private readonly Mock<ICryptographyService> _cryptographyServiceMock;
    private readonly Mock<IKsefSessionService> _sessionServiceMock;
    private readonly Mock<IKsefInvoiceService> _invoiceServiceMock;
    private readonly Mock<IOptions<KsefApiOptions>> _optionsMock;
    private readonly Mock<ILogger<KsefInvoiceSendService>> _loggerMock;
    private readonly KsefInvoiceSendService _service;

    public KsefInvoiceSendServiceTests()
    {
        _ksefClientMock = new Mock<IKSeFClient>();
        _cryptographyServiceMock = new Mock<ICryptographyService>();
        _sessionServiceMock = new Mock<IKsefSessionService>();
        _invoiceServiceMock = new Mock<IKsefInvoiceService>();
        _optionsMock = new Mock<IOptions<KsefApiOptions>>();
        _loggerMock = new Mock<ILogger<KsefInvoiceSendService>>();

        var options = new KsefApiOptions
        {
            Nip = "1234567890",
            BaseUrl = KsefEnvironment.Test
        };

        _optionsMock.Setup(x => x.Value).Returns(options);

        _service = new KsefInvoiceSendService(
            _ksefClientMock.Object,
            _cryptographyServiceMock.Object,
            _sessionServiceMock.Object,
            _invoiceServiceMock.Object,
            _optionsMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SendInvoiceAsync_WithInvalidInvoice_ReturnsFailure()
    {
        // Arrange
        var invoice = CreateTestInvoice(InvoiceType.VAT);
        var sessionInfo = new SessionInfo
        {
            SessionReference = "session-123",
            AccessToken = "access-token"
        };

        var validationResult = ValidationResult.WithError(
            "INVOICE_NUMBER_REQUIRED",
            "Numer faktury jest wymagany",
            "InvoiceNumber");

        _sessionServiceMock
            .Setup(x => x.OpenSessionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionInfo);

        _invoiceServiceMock
            .Setup(x => x.Validate(invoice))
            .Returns(validationResult);

        _sessionServiceMock
            .Setup(x => x.CloseSessionAsync(sessionInfo, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SendInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("InvoiceNumber"));

        _invoiceServiceMock.Verify(x => x.Validate(invoice), Times.Once);
    }

    [Fact]
    public async Task SendVatInvoiceAsync_WithNonVatInvoice_ThrowsArgumentException()
    {
        // Arrange
        var invoice = CreateTestInvoice(InvoiceType.KOR);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SendVatInvoiceAsync(invoice));
    }

    [Fact]
    public async Task SendCorrectionInvoiceAsync_WithWrongType_ThrowsArgumentException()
    {
        // Arrange
        var invoice = CreateTestInvoice(InvoiceType.VAT);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SendCorrectionInvoiceAsync(invoice));
    }

    [Fact]
    public async Task SendAdvancePaymentInvoiceAsync_WithWrongType_ThrowsArgumentException()
    {
        // Arrange
        var invoice = CreateTestInvoice(InvoiceType.VAT);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SendAdvancePaymentInvoiceAsync(invoice));
    }

    [Fact]
    public async Task SendSettlementInvoiceAsync_WithWrongType_ThrowsArgumentException()
    {
        // Arrange
        var invoice = CreateTestInvoice(InvoiceType.VAT);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SendSettlementInvoiceAsync(invoice));
    }

    [Fact]
    public async Task SendSimplifiedInvoiceAsync_WithWrongType_ThrowsArgumentException()
    {
        // Arrange
        var invoice = CreateTestInvoice(InvoiceType.VAT);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SendSimplifiedInvoiceAsync(invoice));
    }

    private static Invoice.Models.Invoice CreateTestInvoice(InvoiceType type, string? invoiceNumber = null)
    {
        return new Invoice.Models.Invoice
        {
            InvoiceData = new Invoice.Models.InvoiceData
            {
                InvoiceType = type,
                InvoiceNumber = invoiceNumber ?? "TEST/001/2026"
            }
        };
    }
}
