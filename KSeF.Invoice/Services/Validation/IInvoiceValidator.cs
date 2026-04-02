/*=====================================================================================

    KSeF.Invoice
    Interfejs walidatora faktur KSeF. Definiuje kontrakt walidacji
    obiektu faktury, zwracając wynik zawierający listę błędów
    i ostrzeżeń.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

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
