/*=====================================================================================

    KSeF.Api
    Klasa opcji konfiguracyjnych certyfikatu do autoryzacji
    w KSeF. Umożliwia wskazanie pliku certyfikatu (.pfx/.p12),
    odcisku palca (thumbprint) z magazynu certyfikatów Windows
    lub klucza prywatnego PEM wraz z odpowiednimi hasłami.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

namespace KSeF.Api.Configuration;

/// <summary>
/// Konfiguracja certyfikatu do autoryzacji KSeF
/// </summary>
public class KsefCertificateOptions
{
    /// <summary>
    /// Ścieżka do pliku certyfikatu (.pfx / .p12)
    /// </summary>
    public string? CertificatePath { get; set; }

    /// <summary>
    /// Hasło do certyfikatu
    /// </summary>
    public string? CertificatePassword { get; set; }

    /// <summary>
    /// Odcisk palca (thumbprint) certyfikatu w magazynie certyfikatów Windows
    /// </summary>
    public string? Thumbprint { get; set; }

    /// <summary>
    /// Ścieżka do pliku klucza prywatnego PEM (opcjonalnie, gdy certyfikat i klucz są osobno)
    /// </summary>
    public string? PrivateKeyPemPath { get; set; }

    /// <summary>
    /// Hasło do klucza prywatnego PEM
    /// </summary>
    public string? PrivateKeyPassword { get; set; }
}
