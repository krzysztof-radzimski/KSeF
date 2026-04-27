/*=====================================================================================

    KSeF.Invoice
    Builder do budowania referencji do faktury zaliczkowej (FakturaZaliczkowa)
    z wykorzystaniem wzorca Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/
using KSeF.Invoice.Models;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania referencji do faktury zaliczkowej (AdvanceInvoiceReference)
/// Obsługuje xsd:choice - faktura w KSeF lub poza KSeF
/// </summary>
public class AdvanceInvoiceReferenceBuilder
{
    private readonly AdvanceInvoiceReference _data = new();

    /// <summary>
    /// Ustawia numer faktury zaliczkowej wystawionej w KSeF (NrKSeFFaZaliczkowej)
    /// </summary>
    /// <param name="ksefNumber">Numer KSeF faktury zaliczkowej</param>
    /// <returns>Ten sam builder do fluent chaining</returns>
    /// <exception cref="InvalidOperationException">Gdy już ustawiono wariant poza KSeF (xsd:choice)</exception>
    public AdvanceInvoiceReferenceBuilder WithKSeFNumber(string ksefNumber)
    {
        if (_data.IsIssuedOutsideKSeF)
            throw new InvalidOperationException("Nie można ustawić NrKSeFFaZaliczkowej i NrKSeFZN jednocześnie (xsd:choice).");

        _data.KSeFNumber = ksefNumber;
        return this;
    }

    /// <summary>
    /// Ustawia numer faktury zaliczkowej wystawionej poza KSeF (NrKSeFZN + NrFaZaliczkowej)
    /// </summary>
    /// <param name="invoiceNumber">Numer faktury zaliczkowej spoza KSeF</param>
    /// <returns>Ten sam builder do fluent chaining</returns>
    /// <exception cref="InvalidOperationException">Gdy już ustawiono wariant KSeF (xsd:choice)</exception>
    public AdvanceInvoiceReferenceBuilder IssuedOutsideKSeF(string invoiceNumber)
    {
        if (!string.IsNullOrEmpty(_data.KSeFNumber))
            throw new InvalidOperationException("Nie można ustawić NrKSeFZN i NrKSeFFaZaliczkowej jednocześnie (xsd:choice).");

        _data.IsIssuedOutsideKSeF = true;
        _data.AdvanceInvoiceNumber = invoiceNumber;
        return this;
    }

    /// <summary>
    /// Buduje obiekt AdvanceInvoiceReference
    /// </summary>
    /// <returns>Skonfigurowany obiekt referencji do faktury zaliczkowej</returns>
    /// <exception cref="InvalidOperationException">Gdy nie wybrano żadnego wariantu lub wybrano oba (xsd:choice)</exception>
    public AdvanceInvoiceReference Build()
    {
        var hasKSeF = !string.IsNullOrEmpty(_data.KSeFNumber);
        var hasOutside = _data.IsIssuedOutsideKSeF;

        if (hasKSeF && hasOutside)
            throw new InvalidOperationException(
                "FakturaZaliczkowa wymaga NrKSeFFaZaliczkowej lub (NrKSeFZN + NrFaZaliczkowej), nie obu (xsd:choice).");

        if (!hasKSeF && !hasOutside)
            throw new InvalidOperationException(
                "FakturaZaliczkowa wymaga NrKSeFFaZaliczkowej lub (NrKSeFZN + NrFaZaliczkowej) (xsd:choice).");

        return _data;
    }
}
