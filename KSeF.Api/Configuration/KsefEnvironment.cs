namespace KSeF.Api.Configuration;

/// <summary>
/// Adresy środowisk KSeF API
/// </summary>
public static class KsefEnvironment
{
    /// <summary>
    /// Środowisko testowe (test)
    /// </summary>
    public const string Test = "https://api-test.ksef.mf.gov.pl";

    /// <summary>
    /// Środowisko demo
    /// </summary>
    public const string Demo = "https://api-demo.ksef.mf.gov.pl";

    /// <summary>
    /// Środowisko produkcyjne
    /// </summary>
    public const string Production = "https://api.ksef.mf.gov.pl";
}
