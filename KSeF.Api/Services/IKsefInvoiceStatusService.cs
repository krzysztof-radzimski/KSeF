/*=====================================================================================

    KSeF.Api
    Interfejs serwisu sprawdzania statusów faktur w KSeF.
    Umożliwia sprawdzanie statusu przetwarzania pojedynczej
    faktury po numerze referencyjnym oraz statusu wszystkich
    faktur w ramach sesji. UWAGA: KSeF API 2.0 nie udostępnia
    endpointu do oznaczania faktur jako zaksięgowane -
    śledzenie tego stanu należy realizować w systemie ERP.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Api.Models;

namespace KSeF.Api.Services;

/// <summary>
/// Serwis zarządzania statusami faktur w KSeF (np. oznaczanie jako zaksięgowane).
///
/// UWAGA: KSeF API w wersji 2.0 nie udostępnia dedykowanego endpointu do oznaczania faktur
/// jako zaksięgowane. Ta funkcjonalność jest planowana w przyszłych wersjach API.
/// Serwis umożliwia sprawdzanie statusu faktur i sesji.
/// </summary>
public interface IKsefInvoiceStatusService
{
    /// <summary>
    /// Sprawdza status faktury po numerze referencyjnym
    /// </summary>
    /// <param name="referenceNumber">Numer referencyjny faktury</param>
    /// <param name="accessToken">Token dostępowy</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Status faktury</returns>
    Task<InvoiceStatusResult> GetInvoiceStatusAsync(
        string referenceNumber,
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza status faktury w ramach sesji
    /// </summary>
    /// <param name="referenceNumber">Numer referencyjny faktury</param>
    /// <param name="sessionInfo">Informacje o sesji</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Status faktury</returns>
    Task<InvoiceStatusResult> GetInvoiceStatusAsync(
        string referenceNumber,
        SessionInfo sessionInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza status przetwarzania faktur w sesji
    /// </summary>
    /// <param name="sessionReference">Numer referencyjny sesji</param>
    /// <param name="accessToken">Token dostępowy</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Status sesji z listą faktur</returns>
    Task<SessionInvoicesResult> GetSessionInvoicesStatusAsync(
        string sessionReference,
        string accessToken,
        CancellationToken cancellationToken = default);
}
