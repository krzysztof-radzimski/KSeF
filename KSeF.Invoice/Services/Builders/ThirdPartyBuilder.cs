using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania danych podmiotu trzeciego (Podmiot3)
/// </summary>
public class ThirdPartyBuilder
{
    private readonly ThirdParty _thirdParty = new();

    /// <summary>
    /// Ustawia NIP podmiotu trzeciego (dla podmiotów polskich)
    /// </summary>
    public ThirdPartyBuilder WithTaxId(string taxId)
    {
        _thirdParty.TaxId = taxId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator wewnętrzny z NIP (format: NIP-XXXXX)
    /// </summary>
    public ThirdPartyBuilder WithInternalId(string internalId)
    {
        _thirdParty.InternalId = internalId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator VAT UE (dla podmiotów z UE)
    /// </summary>
    public ThirdPartyBuilder WithEuVatId(EUCountryCode countryCode, string vatId)
    {
        _thirdParty.EuCountryCode = countryCode;
        _thirdParty.EuVatId = vatId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator zagraniczny (dla podmiotów spoza UE)
    /// </summary>
    public ThirdPartyBuilder WithForeignId(string countryCode, string id)
    {
        _thirdParty.OtherIdCountryCode = countryCode;
        _thirdParty.OtherId = id;
        return this;
    }

    /// <summary>
    /// Oznacza że podmiot nie posiada identyfikatora podatkowego
    /// </summary>
    public ThirdPartyBuilder WithNoIdentifier()
    {
        _thirdParty.NoIdentifier = 1;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę podmiotu trzeciego
    /// </summary>
    public ThirdPartyBuilder WithName(string name)
    {
        _thirdParty.Name = name;
        return this;
    }

    /// <summary>
    /// Ustawia adres podmiotu trzeciego
    /// </summary>
    public ThirdPartyBuilder WithAddress(Action<AddressBuilder> configure)
    {
        var builder = new AddressBuilder();
        configure(builder);
        _thirdParty.Address = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia adres podmiotu trzeciego
    /// </summary>
    public ThirdPartyBuilder WithAddress(Address address)
    {
        _thirdParty.Address = address;
        return this;
    }

    /// <summary>
    /// Ustawia rolę podmiotu trzeciego na fakturze
    /// </summary>
    public ThirdPartyBuilder WithRole(SubjectRole role)
    {
        _thirdParty.Role = role;
        return this;
    }

    /// <summary>
    /// Ustawia opis roli (dla roli "Inny")
    /// </summary>
    public ThirdPartyBuilder WithRoleDescription(string roleDescription)
    {
        _thirdParty.RoleDescription = roleDescription;
        return this;
    }

    /// <summary>
    /// Ustawia udział procentowy podmiotu (dla roli AdditionalBuyer)
    /// </summary>
    public ThirdPartyBuilder WithSharePercentage(decimal sharePercentage)
    {
        _thirdParty.SharePercentage = sharePercentage;
        return this;
    }

    /// <summary>
    /// Konfiguruje podmiot jako faktora
    /// </summary>
    public ThirdPartyBuilder AsFactor()
    {
        _thirdParty.Role = SubjectRole.Factor;
        return this;
    }

    /// <summary>
    /// Konfiguruje podmiot jako odbiorcę
    /// </summary>
    public ThirdPartyBuilder AsRecipient()
    {
        _thirdParty.Role = SubjectRole.Recipient;
        return this;
    }

    /// <summary>
    /// Konfiguruje podmiot jako dodatkowego nabywcę
    /// </summary>
    public ThirdPartyBuilder AsAdditionalBuyer(decimal? sharePercentage = null)
    {
        _thirdParty.Role = SubjectRole.AdditionalBuyer;
        if (sharePercentage.HasValue)
        {
            _thirdParty.SharePercentage = sharePercentage.Value;
        }
        return this;
    }

    /// <summary>
    /// Konfiguruje podmiot jako płatnika
    /// </summary>
    public ThirdPartyBuilder AsPayer()
    {
        _thirdParty.Role = SubjectRole.Payer;
        return this;
    }

    /// <summary>
    /// Buduje obiekt podmiotu trzeciego
    /// </summary>
    public ThirdParty Build() => _thirdParty;
}
