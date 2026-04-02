/*=====================================================================================

    KSeF.Invoice
    Interfejs walidatora numeru NIP (Numer Identyfikacji
    Podatkowej). Definiuje kontrakt do pełnej walidacji NIP
    z wynikiem szczegółowym oraz szybkiego sprawdzenia
    poprawności formatu.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Interfejs walidatora numeru NIP
/// </summary>
public interface INipValidator
{
    /// <summary>
    /// Waliduje numer NIP
    /// </summary>
    /// <param name="nip">Numer NIP do walidacji</param>
    /// <returns>Wynik walidacji</returns>
    ValidationResult Validate(string? nip);

    /// <summary>
    /// Sprawdza czy NIP ma prawidłowy format (szybka walidacja)
    /// </summary>
    /// <param name="nip">Numer NIP do sprawdzenia</param>
    /// <returns>True jeśli format jest prawidłowy</returns>
    bool IsValid(string? nip);
}
