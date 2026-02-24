using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania danych nabywcy (Podmiot2)
/// </summary>
public class BuyerBuilder
{
    private readonly Buyer _buyer = new();

    /// <summary>
    /// Ustawia NIP nabywcy (dla podmiotów polskich)
    /// </summary>
    public BuyerBuilder WithTaxId(string taxId)
    {
        _buyer.TaxId = taxId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator VAT UE (dla podmiotów z UE)
    /// </summary>
    public BuyerBuilder WithEuVatId(EUCountryCode countryCode, string vatId)
    {
        _buyer.EuCountryCode = countryCode;
        _buyer.EuVatId = vatId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator zagraniczny (dla podmiotów spoza UE)
    /// </summary>
    public BuyerBuilder WithForeignId(string countryCode, string id)
    {
        _buyer.OtherIdCountryCode = countryCode;
        _buyer.OtherId = id;
        return this;
    }

    /// <summary>
    /// Oznacza że nabywca nie posiada identyfikatora podatkowego
    /// </summary>
    public BuyerBuilder WithNoIdentifier()
    {
        _buyer.NoIdentifier = 1;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę nabywcy
    /// </summary>
    public BuyerBuilder WithName(string name)
    {
        _buyer.Name = name;
        return this;
    }

    /// <summary>
    /// Ustawia adres nabywcy
    /// </summary>
    public BuyerBuilder WithAddress(Action<AddressBuilder> configure)
    {
        var builder = new AddressBuilder();
        configure(builder);
        _buyer.Address = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia adres nabywcy
    /// </summary>
    public BuyerBuilder WithAddress(Address address)
    {
        _buyer.Address = address;
        return this;
    }

    /// <summary>
    /// Ustawia adres korespondencyjny nabywcy
    /// </summary>
    public BuyerBuilder WithCorrespondenceAddress(Action<AddressBuilder> configure)
    {
        var builder = new AddressBuilder();
        configure(builder);
        _buyer.CorrespondenceAddress = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia dane kontaktowe nabywcy
    /// </summary>
    public BuyerBuilder WithContactData(Action<ContactDataBuilder> configure)
    {
        var builder = new ContactDataBuilder();
        configure(builder);
        _buyer.ContactData = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia dane kontaktowe nabywcy
    /// </summary>
    public BuyerBuilder WithContactData(string? email = null, string? phone = null)
    {
        _buyer.ContactData = new ContactData
        {
            Email = email,
            Phone = phone
        };
        return this;
    }

    /// <summary>
    /// Ustawia numer klienta nadany przez sprzedawcę
    /// </summary>
    public BuyerBuilder WithCustomerNumber(string customerNumber)
    {
        _buyer.CustomerNumber = customerNumber;
        return this;
    }

    /// <summary>
    /// Oznacza nabywcę jako jednostkę samorządu terytorialnego (JST)
    /// </summary>
    public BuyerBuilder AsLocalGovernmentUnit()
    {
        _buyer.IsLocalGovernmentUnit = 1;
        return this;
    }

    /// <summary>
    /// Oznacza nabywcę jako grupę VAT
    /// </summary>
    public BuyerBuilder AsVatGroup()
    {
        _buyer.IsVatGroup = 1;
        return this;
    }

    /// <summary>
    /// Buduje obiekt nabywcy
    /// </summary>
    public Buyer Build() => _buyer;
}
