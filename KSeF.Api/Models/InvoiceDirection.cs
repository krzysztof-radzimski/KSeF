/*=====================================================================================

    KSeF.Api
    Typ wyliczeniowy określający kierunek wyszukiwanych faktur
    w KSeF: zakupowe (Purchase - faktury otrzymane) lub
    sprzedażowe (Sales - faktury wystawione).

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

namespace KSeF.Api.Models;

/// <summary>
/// Kierunek wyszukiwanych faktur
/// </summary>
public enum InvoiceDirection
{
    /// <summary>
    /// Faktury zakupowe (otrzymane)
    /// </summary>
    Purchase,

    /// <summary>
    /// Faktury sprzedażowe (wystawione)
    /// </summary>
    Sales
}
