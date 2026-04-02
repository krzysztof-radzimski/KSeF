/*=====================================================================================

    KSeF.Invoice
    Interfejs walidatora dat na fakturze KSeF. Definiuje kontrakt
    do walidacji daty wystawienia, daty sprzedaży oraz okresu
    rozliczeniowego z uwzględnieniem wzajemnych zależności
    między datami.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Interfejs walidatora dat na fakturze
/// </summary>
public interface IDateValidator
{
    /// <summary>
    /// Waliduje datę wystawienia faktury
    /// </summary>
    /// <param name="issueDate">Data wystawienia</param>
    /// <returns>Wynik walidacji</returns>
    ValidationResult ValidateIssueDate(DateOnly issueDate);

    /// <summary>
    /// Waliduje datę sprzedaży
    /// </summary>
    /// <param name="saleDate">Data sprzedaży</param>
    /// <param name="issueDate">Data wystawienia (opcjonalna, do porównania)</param>
    /// <returns>Wynik walidacji</returns>
    ValidationResult ValidateSaleDate(DateOnly? saleDate, DateOnly? issueDate = null);

    /// <summary>
    /// Waliduje okres rozliczeniowy
    /// </summary>
    /// <param name="startDate">Data początkowa okresu</param>
    /// <param name="endDate">Data końcowa okresu</param>
    /// <returns>Wynik walidacji</returns>
    ValidationResult ValidatePeriod(DateOnly startDate, DateOnly endDate);
}
