namespace KSeF.Api.Configuration;

/// <summary>
/// Adresy środowisk KSeF API
/// </summary>
public static class KsefEnvironment
{
    /// <summary>
    /// Środowisko testowe (test)
    /// </summary>
    public const string Test = "https://ksef-test.mf.gov.pl/api";

    /// <summary>
    /// Środowisko demo
    /// </summary>
    public const string Demo = "https://ksef-demo.mf.gov.pl/api";

    /// <summary>
    /// Środowisko produkcyjne
    /// </summary>
    public const string Production = "https://ksef.mf.gov.pl/api";
}
