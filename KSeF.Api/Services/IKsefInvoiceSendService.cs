using KSeF.Api.Models;
using KSeF.Invoice.Models;

namespace KSeF.Api.Services;

/// <summary>
/// Serwis wysyłania faktur sprzedażowych do KSeF.
/// Obsługuje wszystkie typy faktur: VAT, korekta, zaliczkowa, rozliczeniowa, uproszczona.
/// </summary>
public interface IKsefInvoiceSendService
{
    /// <summary>
    /// Wysyła fakturę do KSeF w ramach sesji interaktywnej.
    /// Automatycznie otwiera i zamyka sesję.
    /// </summary>
    /// <param name="invoice">Faktura do wysłania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Wynik wysłania</returns>
    Task<InvoiceSendResult> SendInvoiceAsync(Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła fakturę do KSeF w ramach istniejącej sesji interaktywnej.
    /// </summary>
    /// <param name="invoice">Faktura do wysłania</param>
    /// <param name="sessionInfo">Istniejąca sesja</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Wynik wysłania</returns>
    Task<InvoiceSendResult> SendInvoiceAsync(Invoice.Models.Invoice invoice, SessionInfo sessionInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła wiele faktur do KSeF w jednej sesji interaktywnej.
    /// Automatycznie otwiera i zamyka sesję.
    /// </summary>
    /// <param name="invoices">Faktury do wysłania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Wyniki wysłania dla każdej faktury</returns>
    Task<List<InvoiceSendResult>> SendInvoicesAsync(IEnumerable<Invoice.Models.Invoice> invoices, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła standardową fakturę VAT do KSeF
    /// </summary>
    Task<InvoiceSendResult> SendVatInvoiceAsync(Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła korektę faktury VAT do KSeF
    /// </summary>
    Task<InvoiceSendResult> SendCorrectionInvoiceAsync(Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła fakturę zaliczkową do KSeF
    /// </summary>
    Task<InvoiceSendResult> SendAdvancePaymentInvoiceAsync(Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła fakturę rozliczeniową (rozliczenie zaliczek) do KSeF
    /// </summary>
    Task<InvoiceSendResult> SendSettlementInvoiceAsync(Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła fakturę uproszczoną do KSeF
    /// </summary>
    Task<InvoiceSendResult> SendSimplifiedInvoiceAsync(Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła korektę faktury zaliczkowej do KSeF
    /// </summary>
    Task<InvoiceSendResult> SendAdvancePaymentCorrectionAsync(Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła korektę faktury rozliczeniowej do KSeF
    /// </summary>
    Task<InvoiceSendResult> SendSettlementCorrectionAsync(Invoice.Models.Invoice invoice, CancellationToken cancellationToken = default);
}
