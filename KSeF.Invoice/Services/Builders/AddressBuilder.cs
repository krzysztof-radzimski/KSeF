using KSeF.Invoice.Models.Common;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania adresu
/// </summary>
public class AddressBuilder
{
    private readonly Address _address = new();

    /// <summary>
    /// Ustawia kod kraju (domyślnie PL)
    /// </summary>
    public AddressBuilder WithCountryCode(string countryCode)
    {
        _address.CountryCode = countryCode;
        return this;
    }

    /// <summary>
    /// Ustawia pierwszą linię adresu (ulica, numer domu/lokalu lub miejscowość)
    /// </summary>
    public AddressBuilder WithAddressLine1(string addressLine1)
    {
        _address.AddressLine1 = addressLine1;
        return this;
    }

    /// <summary>
    /// Ustawia drugą linię adresu (kod pocztowy, miejscowość)
    /// </summary>
    public AddressBuilder WithAddressLine2(string addressLine2)
    {
        _address.AddressLine2 = addressLine2;
        return this;
    }

    /// <summary>
    /// Ustawia Globalny Numer Lokalizacyjny (GLN)
    /// </summary>
    public AddressBuilder WithGln(string gln)
    {
        _address.Gln = gln;
        return this;
    }

    /// <summary>
    /// Buduje adres z podanych danych
    /// </summary>
    public Address Build() => _address;
}
