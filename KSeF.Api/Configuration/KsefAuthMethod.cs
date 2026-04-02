/*=====================================================================================

    KSeF.Api
    Typ wyliczeniowy definiujący dostępne metody autoryzacji
    do Krajowego Systemu e-Faktur (KSeF). Obsługuje autoryzację
    tokenem KSeF oraz certyfikatem kwalifikowanym (XAdES).

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

namespace KSeF.Api.Configuration;

/// <summary>
/// Metoda autoryzacji do KSeF
/// </summary>
public enum KsefAuthMethod
{
    /// <summary>
    /// Autoryzacja tokenem KSeF
    /// </summary>
    Token,

    /// <summary>
    /// Autoryzacja certyfikatem kwalifikowanym (XAdES)
    /// </summary>
    Certificate
}
