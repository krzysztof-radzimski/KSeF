/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych sprzedawcy (Podmiot1) z wykorzystaniem
    wzorca Fluent API. Umożliwia konfigurację NIP, nazwy, adresu,
    adresu korespondencyjnego, danych kontaktowych, numeru EORI
    oraz statusu podatnika.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania danych sprzedawcy (Podmiot1)
/// </summary>
public class SellerBuilder {
    private readonly Seller _seller = new();

    /// <summary>
    /// Ustawia NIP sprzedawcy
    /// </summary>
    public SellerBuilder WithTaxId(string taxId) {
        _seller.TaxId = taxId != null ? taxId.Trim().Replace("-", String.Empty) : taxId;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę sprzedawcy
    /// </summary>
    public SellerBuilder WithName(string name) {
        _seller.Name = name;
        return this;
    }

    /// <summary>
    /// Ustawia adres sprzedawcy
    /// </summary>
    public SellerBuilder WithAddress(Action<AddressBuilder> configure) {
        var builder = new AddressBuilder();
        configure(builder);
        _seller.Address = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia adres sprzedawcy
    /// </summary>
    public SellerBuilder WithAddress(Address address) {
        _seller.Address = address;
        return this;
    }

    /// <summary>
    /// Ustawia adres korespondencyjny sprzedawcy
    /// </summary>
    public SellerBuilder WithCorrespondenceAddress(Action<AddressBuilder> configure) {
        var builder = new AddressBuilder();
        configure(builder);
        _seller.CorrespondenceAddress = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia dane kontaktowe sprzedawcy
    /// </summary>
    public SellerBuilder WithContactData(Action<ContactDataBuilder> configure) {
        var builder = new ContactDataBuilder();
        configure(builder);
        _seller.ContactData = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia dane kontaktowe sprzedawcy
    /// </summary>
    public SellerBuilder WithContactData(string? email = null, string? phone = null) {
        _seller.ContactData = new ContactData {
            Email = email,
            Phone = phone
        };
        return this;
    }

    /// <summary>
    /// Ustawia numer EORI sprzedawcy
    /// </summary>
    public SellerBuilder WithEoriNumber(string eoriNumber) {
        _seller.EoriNumber = eoriNumber;
        return this;
    }

    /// <summary>
    /// Ustawia status podatnika
    /// </summary>
    public SellerBuilder WithStatusInfo(TaxpayerStatus status) {
        _seller.StatusInfo = status;
        return this;
    }

    /// <summary>
    /// Buduje obiekt sprzedawcy
    /// </summary>
    public Seller Build() => _seller;
}
