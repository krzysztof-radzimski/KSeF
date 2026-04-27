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

/// <summary>
/// Builder do budowania danych nabywcy z faktury korygowanej (Podmiot2K)
/// Umożliwia konfigurację danych identyfikacyjnych, adresu i identyfikatora nabywcy
/// </summary>
public class CorrectedBuyerDataBuilder
{
    private readonly CorrectedBuyerData _data = new();

    /// <summary>
    /// Ustawia identyfikację nabywcy przez polski NIP (wariant a: Nazwa + NIP)
    /// Automatycznie usuwa myślniki z numeru NIP
    /// </summary>
    /// <param name="nip">Numer NIP (10 cyfr)</param>
    /// <returns>Builder dla fluent chaining</returns>
    public CorrectedBuyerDataBuilder WithNip(string nip)
    {
        _data.IdentificationData.Nip = nip != null ? nip.Trim().Replace("-", string.Empty) : nip;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikację nabywcy przez VAT UE (wariant b: KodUE + NrVatUE)
    /// </summary>
    /// <param name="countryCode">Kod kraju UE</param>
    /// <param name="vatId">Numer VAT UE</param>
    /// <returns>Builder dla fluent chaining</returns>
    public CorrectedBuyerDataBuilder WithEuVatId(EUCountryCode countryCode, string vatId)
    {
        _data.IdentificationData.EUCountryCode = countryCode;
        _data.IdentificationData.VatNumberEU = vatId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikację nabywcy przez zagraniczny identyfikator (wariant c: KodKraju + ID)
    /// Dla podmiotów spoza UE lub bez VAT (np. paszport)
    /// </summary>
    /// <param name="countryCode">Kod kraju ISO (2 znaki)</param>
    /// <param name="id">Identyfikator (np. numer paszportu)</param>
    /// <returns>Builder dla fluent chaining</returns>
    public CorrectedBuyerDataBuilder WithForeignId(string countryCode, string id)
    {
        _data.IdentificationData.CountryCode = countryCode;
        _data.IdentificationData.OtherTaxId = id;
        return this;
    }

    /// <summary>
    /// Ustawia brak identyfikatora nabywcy (wariant d: tylko Nazwa)
    /// Dla podmiotów niebędących płatnikami VAT
    /// </summary>
    /// <returns>Builder dla fluent chaining</returns>
    public CorrectedBuyerDataBuilder WithNoIdentifier()
    {
        _data.IdentificationData.NoIdentifier = 1;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę nabywcy (Nazwa)
    /// Wymagana we wszystkich wariantach identyfikacji
    /// </summary>
    /// <param name="name">Nazwa nabywcy</param>
    /// <returns>Builder dla fluent chaining</returns>
    public CorrectedBuyerDataBuilder WithName(string name)
    {
        _data.IdentificationData.Name = name;
        return this;
    }

    /// <summary>
    /// Konfiguruje dane identyfikacyjne nabywcy przez nested builder
    /// </summary>
    /// <param name="configure">Akcja konfigurująca BuyerIdentificationBuilder</param>
    /// <returns>Builder dla fluent chaining</returns>
    public CorrectedBuyerDataBuilder WithIdentification(Action<BuyerIdentificationBuilder> configure)
    {
        var builder = new BuyerIdentificationBuilder(_data.IdentificationData);
        configure(builder);
        return this;
    }

    /// <summary>
    /// Konfiguruje adres nabywcy przez nested builder
    /// </summary>
    /// <param name="configure">Akcja konfigurująca AddressBuilder</param>
    /// <returns>Builder dla fluent chaining</returns>
    public CorrectedBuyerDataBuilder WithAddress(Action<AddressBuilder> configure)
    {
        var builder = new AddressBuilder();
        configure(builder);
        _data.Address = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia adres nabywcy przez gotowy obiekt Address
    /// </summary>
    /// <param name="address">Obiekt adresu</param>
    /// <returns>Builder dla fluent chaining</returns>
    public CorrectedBuyerDataBuilder WithAddress(Address address)
    {
        _data.Address = address;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator nabywcy (IDNabywcy)
    /// Opcjonalny, maksymalnie 32 znaki
    /// </summary>
    /// <param name="identifier">Identyfikator nabywcy</param>
    /// <returns>Builder dla fluent chaining</returns>
    /// <exception cref="ArgumentException">Gdy długość identyfikatora przekracza 32 znaki</exception>
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

    /// <summary>
    /// Buduje obiekt CorrectedBuyerData (Podmiot2K)
    /// </summary>
    /// <returns>Skonfigurowany obiekt danych nabywcy z faktury korygowanej</returns>
    public CorrectedBuyerData Build() => _data;
}
