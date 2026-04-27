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

/// <summary>
/// Builder do budowania danych identyfikacyjnych nabywcy (TPodmiot2)
/// Obsługuje 4 warianty identyfikacji: NIP, VAT UE, identyfikator zagraniczny, brak ID
/// </summary>
public class BuyerIdentificationBuilder
{
    private readonly BuyerIdentification _identification;

    /// <summary>
    /// Inicjalizuje builder z istniejącym obiektem BuyerIdentification
    /// </summary>
    /// <param name="identification">Obiekt danych identyfikacyjnych nabywcy</param>
    public BuyerIdentificationBuilder(BuyerIdentification identification)
    {
#if NETSTANDARD2_0
        ThrowHelper.ThrowIfNull(identification);
#else
        ArgumentNullException.ThrowIfNull(identification);
#endif
        _identification = identification;
    }

    /// <summary>
    /// Ustawia identyfikację nabywcy przez polski NIP (wariant a: Nazwa + NIP)
    /// Automatycznie usuwa myślniki z numeru NIP
    /// </summary>
    /// <param name="nip">Numer NIP (10 cyfr)</param>
    /// <returns>Builder dla fluent chaining</returns>
    public BuyerIdentificationBuilder WithNip(string nip)
    {
        _identification.Nip = nip != null ? nip.Trim().Replace("-", string.Empty) : nip;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikację nabywcy przez VAT UE (wariant b: KodUE + NrVatUE)
    /// </summary>
    /// <param name="countryCode">Kod kraju UE</param>
    /// <param name="vatId">Numer VAT UE</param>
    /// <returns>Builder dla fluent chaining</returns>
    public BuyerIdentificationBuilder WithEuVatId(EUCountryCode countryCode, string vatId)
    {
        _identification.EUCountryCode = countryCode;
        _identification.VatNumberEU = vatId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikację nabywcy przez zagraniczny identyfikator (wariant c: KodKraju + ID)
    /// Dla podmiotów spoza UE lub bez VAT (np. paszport)
    /// </summary>
    /// <param name="countryCode">Kod kraju ISO (2 znaki)</param>
    /// <param name="id">Identyfikator (np. numer paszportu)</param>
    /// <returns>Builder dla fluent chaining</returns>
    public BuyerIdentificationBuilder WithForeignId(string countryCode, string id)
    {
        _identification.CountryCode = countryCode;
        _identification.OtherTaxId = id;
        return this;
    }

    /// <summary>
    /// Ustawia brak identyfikatora nabywcy (wariant d: tylko Nazwa)
    /// Dla podmiotów niebędących płatnikami VAT
    /// </summary>
    /// <returns>Builder dla fluent chaining</returns>
    public BuyerIdentificationBuilder WithNoIdentifier()
    {
        _identification.NoIdentifier = 1;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę nabywcy (Nazwa)
    /// Wymagana we wszystkich wariantach
    /// </summary>
    /// <param name="name">Nazwa nabywcy</param>
    /// <returns>Builder dla fluent chaining</returns>
    public BuyerIdentificationBuilder WithName(string name)
    {
        _identification.Name = name;
        return this;
    }

    /// <summary>
    /// Buduje obiekt BuyerIdentification (TPodmiot2)
    /// </summary>
    /// <returns>Skonfigurowany obiekt danych identyfikacyjnych nabywcy</returns>
    public BuyerIdentification Build() => _identification;
}
