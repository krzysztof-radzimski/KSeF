using KSeF.Invoice.Models.Common;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania danych kontaktowych
/// </summary>
public class ContactDataBuilder
{
    private readonly ContactData _contactData = new();

    /// <summary>
    /// Ustawia adres e-mail
    /// </summary>
    public ContactDataBuilder WithEmail(string email)
    {
        _contactData.Email = email;
        return this;
    }

    /// <summary>
    /// Ustawia numer telefonu
    /// </summary>
    public ContactDataBuilder WithPhone(string phone)
    {
        _contactData.Phone = phone;
        return this;
    }

    /// <summary>
    /// Buduje dane kontaktowe
    /// </summary>
    public ContactData Build() => _contactData;
}
