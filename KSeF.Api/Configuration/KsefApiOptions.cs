namespace KSeF.Api.Configuration;

/// <summary>
/// Opcje konfiguracji połączenia z KSeF API
/// </summary>
public class KsefApiOptions
{
    /// <summary>
    /// Sekcja konfiguracji w appsettings.json
    /// </summary>
    public const string SectionName = "KSeF";

    /// <summary>
    /// Adres bazowy API KSeF. Domyślnie środowisko testowe.
    /// Dostępne wartości: <see cref="KsefEnvironment"/>
    /// </summary>
    public string BaseUrl { get; set; } = KsefEnvironment.Test;

    /// <summary>
    /// NIP podmiotu (10 cyfr bez myślników)
    /// </summary>
    public string Nip { get; set; } = string.Empty;

    /// <summary>
    /// Metoda autoryzacji do KSeF
    /// </summary>
    public KsefAuthMethod AuthMethod { get; set; } = KsefAuthMethod.Token;

    /// <summary>
    /// Token KSeF do autoryzacji (wymagany gdy AuthMethod = Token)
    /// </summary>
    public string? KsefToken { get; set; }

    /// <summary>
    /// Konfiguracja certyfikatu (wymagana gdy AuthMethod = Certificate)
    /// </summary>
    public KsefCertificateOptions? Certificate { get; set; }

    /// <summary>
    /// Timeout operacji w sekundach (domyślnie 120)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Maksymalna liczba prób ponowienia nieudanych operacji (domyślnie 3)
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Informacja o systemie nadawczym (przekazywana w nagłówku faktury)
    /// </summary>
    public string? SystemInfo { get; set; }
}
