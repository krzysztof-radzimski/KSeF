/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych sprzedawcy z faktury korygowanej (Podmiot1K)
    z wykorzystaniem wzorca Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

public class CorrectedSellerDataBuilder
{
    private readonly CorrectedSellerData _data = new();

    public CorrectedSellerDataBuilder WithTaxpayerPrefix(EUCountryCode prefix)
    {
        _data.TaxpayerPrefix = prefix;
        return this;
    }

    public CorrectedSellerDataBuilder WithNip(string nip)
    {
        _data.IdentificationData.Nip = nip != null ? nip.Trim().Replace("-", string.Empty) : nip;
        return this;
    }

    public CorrectedSellerDataBuilder WithName(string name)
    {
        _data.IdentificationData.Name = name;
        return this;
    }

    public CorrectedSellerDataBuilder WithIdentification(Action<SellerIdentificationBuilder> configure)
    {
        var builder = new SellerIdentificationBuilder(_data.IdentificationData);
        configure(builder);
        return this;
    }

    public CorrectedSellerDataBuilder WithAddress(Action<AddressBuilder> configure)
    {
        var builder = new AddressBuilder();
        configure(builder);
        _data.Address = builder.Build();
        return this;
    }

    public CorrectedSellerDataBuilder WithAddress(Address address)
    {
        _data.Address = address;
        return this;
    }

    public CorrectedSellerData Build() => _data;
}
