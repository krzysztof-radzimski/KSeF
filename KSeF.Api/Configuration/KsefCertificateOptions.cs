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
