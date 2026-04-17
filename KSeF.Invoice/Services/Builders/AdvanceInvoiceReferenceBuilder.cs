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

public class AdvanceInvoiceReferenceBuilder
{
    private readonly AdvanceInvoiceReference _data = new();

    public AdvanceInvoiceReferenceBuilder WithKSeFNumber(string ksefNumber)
    {
        if (_data.IsIssuedOutsideKSeF)
            throw new InvalidOperationException("Nie można ustawić NrKSeFFaZaliczkowej i NrKSeFZN jednocześnie (xsd:choice).");

        _data.KSeFNumber = ksefNumber;
        return this;
    }

    public AdvanceInvoiceReferenceBuilder IssuedOutsideKSeF(string invoiceNumber)
    {
        if (!string.IsNullOrEmpty(_data.KSeFNumber))
            throw new InvalidOperationException("Nie można ustawić NrKSeFZN i NrKSeFFaZaliczkowej jednocześnie (xsd:choice).");

        _data.IsIssuedOutsideKSeF = true;
        _data.AdvanceInvoiceNumber = invoiceNumber;
        return this;
    }

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
