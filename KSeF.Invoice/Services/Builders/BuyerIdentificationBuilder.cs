/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych identyfikacyjnych nabywcy (TPodmiot2)
    z wykorzystaniem wzorca Fluent API. Obsługuje identyfikację przez
    NIP, VAT UE, identyfikator zagraniczny lub brak identyfikatora.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

public class BuyerIdentificationBuilder
{
    private readonly BuyerIdentification _identification;

    public BuyerIdentificationBuilder(BuyerIdentification identification)
    {
#if NETSTANDARD2_0
        ThrowHelper.ThrowIfNull(identification);
#else
        ArgumentNullException.ThrowIfNull(identification);
#endif
        _identification = identification;
    }

    public BuyerIdentificationBuilder WithNip(string nip)
    {
        _identification.Nip = nip != null ? nip.Trim().Replace("-", string.Empty) : nip;
        return this;
    }

    public BuyerIdentificationBuilder WithEuVatId(EUCountryCode countryCode, string vatId)
    {
        _identification.EUCountryCode = countryCode;
        _identification.VatNumberEU = vatId;
        return this;
    }

    public BuyerIdentificationBuilder WithForeignId(string countryCode, string id)
    {
        _identification.CountryCode = countryCode;
        _identification.OtherTaxId = id;
        return this;
    }

    public BuyerIdentificationBuilder WithNoIdentifier()
    {
        _identification.NoIdentifier = 1;
        return this;
    }

    public BuyerIdentificationBuilder WithName(string name)
    {
        _identification.Name = name;
        return this;
    }

    public BuyerIdentification Build() => _identification;
}
