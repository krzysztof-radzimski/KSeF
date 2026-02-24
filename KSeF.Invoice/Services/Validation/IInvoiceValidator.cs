using KSeF.Invoice.Models;

namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Interfejs walidatora faktur KSeF
/// </summary>
public interface IInvoiceValidator
{
    /// <summary>
    /// Waliduje fakturę
    /// </summary>
    /// <param name="invoice">Faktura do walidacji</param>
    /// <returns>Wynik walidacji z listą błędów i ostrzeżeń</returns>
    ValidationResult Validate(Models.Invoice invoice);
}
