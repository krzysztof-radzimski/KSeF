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

/// <summary>
/// Builder do budowania danych sprzedawcy z faktury korygowanej (Podmiot1K)
/// </summary>
public class CorrectedSellerDataBuilder
{
    private readonly CorrectedSellerData _data = new();

    /// <summary>
    /// Ustawia prefiks podatnika (PrefiksPodatnika)
    /// </summary>
    /// <param name="prefix">Dwuznakowy kod kraju EU, np. PL, DE</param>
    /// <returns>Instancję buildera do łańcuchowania wywołań</returns>
    public CorrectedSellerDataBuilder WithTaxpayerPrefix(EUCountryCode prefix)
    {
        _data.TaxpayerPrefix = prefix;
        return this;
    }

    /// <summary>
    /// Ustawia NIP sprzedawcy (normalizuje format: usuwa myślniki i whitespace)
    /// </summary>
    /// <param name="nip">Numer NIP sprzedawcy</param>
    /// <returns>Instancję buildera do łańcuchowania wywołań</returns>
    public CorrectedSellerDataBuilder WithNip(string nip)
    {
        _data.IdentificationData.Nip = nip != null ? nip.Trim().Replace("-", string.Empty) : nip;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę sprzedawcy
    /// </summary>
    /// <param name="name">Nazwa sprzedawcy</param>
    /// <returns>Instancję buildera do łańcuchowania wywołań</returns>
    public CorrectedSellerDataBuilder WithName(string name)
    {
        _data.IdentificationData.Name = name;
        return this;
    }

    /// <summary>
    /// Konfiguruje dane identyfikacyjne sprzedawcy przez nested builder
    /// </summary>
    /// <param name="configure">Akcja konfigurująca SellerIdentificationBuilder</param>
    /// <returns>Instancję buildera do łańcuchowania wywołań</returns>
    public CorrectedSellerDataBuilder WithIdentification(Action<SellerIdentificationBuilder> configure)
    {
        var builder = new SellerIdentificationBuilder(_data.IdentificationData);
        configure(builder);
        return this;
    }

    /// <summary>
    /// Konfiguruje adres sprzedawcy przez nested builder
    /// </summary>
    /// <param name="configure">Akcja konfigurująca AddressBuilder</param>
    /// <returns>Instancję buildera do łańcuchowania wywołań</returns>
    public CorrectedSellerDataBuilder WithAddress(Action<AddressBuilder> configure)
    {
        var builder = new AddressBuilder();
        configure(builder);
        _data.Address = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia gotowy obiekt Address
    /// </summary>
    /// <param name="address">Obiekt adresu</param>
    /// <returns>Instancję buildera do łańcuchowania wywołań</returns>
    public CorrectedSellerDataBuilder WithAddress(Address address)
    {
        _data.Address = address;
        return this;
    }

    /// <summary>
    /// Buduje obiekt CorrectedSellerData
    /// </summary>
    /// <returns>Zbudowany obiekt CorrectedSellerData</returns>
    public CorrectedSellerData Build() => _data;
}
