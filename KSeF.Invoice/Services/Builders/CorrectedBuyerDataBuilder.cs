/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych nabywcy z faktury korygowanej (Podmiot2K)
    z wykorzystaniem wzorca Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

public class CorrectedBuyerDataBuilder
{
    private readonly CorrectedBuyerData _data = new();

    public CorrectedBuyerDataBuilder WithNip(string nip)
    {
        _data.IdentificationData.Nip = nip != null ? nip.Trim().Replace("-", string.Empty) : nip;
        return this;
    }

    public CorrectedBuyerDataBuilder WithEuVatId(EUCountryCode countryCode, string vatId)
    {
        _data.IdentificationData.EUCountryCode = countryCode;
        _data.IdentificationData.VatNumberEU = vatId;
        return this;
    }

    public CorrectedBuyerDataBuilder WithForeignId(string countryCode, string id)
    {
        _data.IdentificationData.CountryCode = countryCode;
        _data.IdentificationData.OtherTaxId = id;
        return this;
    }

    public CorrectedBuyerDataBuilder WithNoIdentifier()
    {
        _data.IdentificationData.NoIdentifier = 1;
        return this;
    }

    public CorrectedBuyerDataBuilder WithName(string name)
    {
        _data.IdentificationData.Name = name;
        return this;
    }

    public CorrectedBuyerDataBuilder WithIdentification(Action<BuyerIdentificationBuilder> configure)
    {
        var builder = new BuyerIdentificationBuilder(_data.IdentificationData);
        configure(builder);
        return this;
    }

    public CorrectedBuyerDataBuilder WithAddress(Action<AddressBuilder> configure)
    {
        var builder = new AddressBuilder();
        configure(builder);
        _data.Address = builder.Build();
        return this;
    }

    public CorrectedBuyerDataBuilder WithAddress(Address address)
    {
        _data.Address = address;
        return this;
    }

    public CorrectedBuyerDataBuilder WithBuyerIdentifier(string identifier)
    {
#if NETSTANDARD2_0
        ThrowHelper.ThrowIfNullOrWhiteSpace(identifier);
#else
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
#endif
        if (identifier.Length > 32)
            throw new ArgumentException("IDNabywcy cannot exceed 32 characters per XSD FA(3).", nameof(identifier));

        _data.BuyerIdentifier = identifier;
        return this;
    }

    public CorrectedBuyerData Build() => _data;
}
