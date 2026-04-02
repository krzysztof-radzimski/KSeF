/*=====================================================================================

    KSeF.Api
    Klasa zawierająca stałe z adresami URL środowisk
    Krajowego Systemu e-Faktur (KSeF): testowego, demo
    oraz produkcyjnego. Używana do konfiguracji adresu
    bazowego API w opcjach KsefApiOptions.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

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
