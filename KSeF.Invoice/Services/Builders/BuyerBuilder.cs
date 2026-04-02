/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych nabywcy (Podmiot2) z wykorzystaniem
    wzorca Fluent API. Obsługuje identyfikację przez NIP, VAT UE
    lub identyfikator zagraniczny, a także konfigurację adresu,
    danych kontaktowych, adresu korespondencyjnego oraz oznaczenia
    jako jednostka samorządu terytorialnego (JST) lub członek
    grupy VAT (GV).

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania danych nabywcy (Podmiot2)
/// </summary>
public class BuyerBuilder {
    private readonly Buyer _buyer = new();

    /// <summary>
    /// Ustawia NIP nabywcy (dla podmiotów polskich)
    /// </summary>
    public BuyerBuilder WithTaxId(string taxId) {
        _buyer.TaxId = taxId != null ? taxId.Trim().Replace("-", String.Empty) : taxId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator VAT UE (dla podmiotów z UE)
    /// </summary>
    public BuyerBuilder WithEuVatId(EUCountryCode countryCode, string vatId) {
        _buyer.EuCountryCode = countryCode;
        _buyer.EuVatId = vatId;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator zagraniczny (dla podmiotów spoza UE)
    /// </summary>
    public BuyerBuilder WithForeignId(string countryCode, string id) {
        _buyer.OtherIdCountryCode = countryCode;
        _buyer.OtherId = id;
        return this;
    }

    /// <summary>
    /// Oznacza że nabywca nie posiada identyfikatora podatkowego
    /// </summary>
    public BuyerBuilder WithNoIdentifier() {
        _buyer.NoIdentifier = 1;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę nabywcy
    /// </summary>
    public BuyerBuilder WithName(string name) {
        _buyer.Name = name;
        return this;
    }

    /// <summary>
    /// Ustawia adres nabywcy
    /// </summary>
    public BuyerBuilder WithAddress(Action<AddressBuilder> configure) {
        var builder = new AddressBuilder();
        configure(builder);
        _buyer.Address = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia adres nabywcy
    /// </summary>
    public BuyerBuilder WithAddress(Address address) {
        _buyer.Address = address;
        return this;
    }

    /// <summary>
    /// Ustawia adres korespondencyjny nabywcy
    /// </summary>
    public BuyerBuilder WithCorrespondenceAddress(Action<AddressBuilder> configure) {
        var builder = new AddressBuilder();
        configure(builder);
        _buyer.CorrespondenceAddress = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia dane kontaktowe nabywcy
    /// </summary>
    public BuyerBuilder WithContactData(Action<ContactDataBuilder> configure) {
        var builder = new ContactDataBuilder();
        configure(builder);
        _buyer.ContactData = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia dane kontaktowe nabywcy
    /// </summary>
    public BuyerBuilder WithContactData(string? email = null, string? phone = null) {
        _buyer.ContactData = new ContactData {
            Email = email,
            Phone = phone
        };
        return this;
    }

    /// <summary>
    /// Ustawia numer klienta nadany przez sprzedawcę
    /// </summary>
    public BuyerBuilder WithCustomerNumber(string customerNumber) {
        _buyer.CustomerNumber = customerNumber;
        return this;
    }

    /// <summary>
    /// Oznacza nabywcę jako jednostkę samorządu terytorialnego (JST)
    /// </summary>
    public BuyerBuilder AsLocalGovernmentUnit() {
        _buyer.Jst = 1;
        return this;
    }

    /// <summary>
    /// Oznacza nabywcę jako członka grupy VAT (GV=1)
    /// Wymaga wypełnienia Podmiot3 z rolą 10
    /// </summary>
    public BuyerBuilder AsVatGroupMember() {
        _buyer.Gv = 1;
        return this;
    }

    /// <summary>
    /// Ustawia unikalny klucz powiązania danych nabywcy (IDNabywcy)
    /// Używany gdy dane nabywcy na fakturze korygującej zmieniły się
    /// </summary>
    public BuyerBuilder WithBuyerIdentityKey(string identityKey) {
        _buyer.BuyerIdentityKey = identityKey;
        return this;
    }

    /// <summary>
    /// Buduje obiekt nabywcy
    /// </summary>
    public Buyer Build() => _buyer;
}
