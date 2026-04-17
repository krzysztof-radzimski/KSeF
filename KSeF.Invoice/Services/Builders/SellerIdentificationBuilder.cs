/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych identyfikacyjnych sprzedawcy (TPodmiot1)
    z wykorzystaniem wzorca Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;

namespace KSeF.Invoice.Services.Builders;

public class SellerIdentificationBuilder
{
    private readonly SellerIdentification _identification;

    public SellerIdentificationBuilder(SellerIdentification identification)
    {
#if NETSTANDARD2_0
        ThrowHelper.ThrowIfNull(identification);
#else
        ArgumentNullException.ThrowIfNull(identification);
#endif
        _identification = identification;
    }

    public SellerIdentificationBuilder WithNip(string nip)
    {
        _identification.Nip = nip != null ? nip.Trim().Replace("-", string.Empty) : nip;
        return this;
    }

    public SellerIdentificationBuilder WithName(string name)
    {
        _identification.Name = name;
        return this;
    }

    public SellerIdentification Build() => _identification;
}
