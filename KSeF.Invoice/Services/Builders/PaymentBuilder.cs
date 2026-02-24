using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Payments;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania informacji o płatnościach
/// </summary>
public class PaymentBuilder
{
    private readonly Payment _payment = new();

    /// <summary>
    /// Dodaje termin płatności (jako datę)
    /// </summary>
    public PaymentBuilder AddPaymentTerm(DateOnly dueDate)
    {
        _payment.PaymentTerms ??= new List<PaymentTerm>();
        _payment.PaymentTerms.Add(new PaymentTerm { DueDate = dueDate });
        return this;
    }

    /// <summary>
    /// Dodaje termin płatności (jako datę)
    /// </summary>
    public PaymentBuilder AddPaymentTerm(int year, int month, int day)
    {
        return AddPaymentTerm(new DateOnly(year, month, day));
    }

    /// <summary>
    /// Dodaje termin płatności (jako opis)
    /// </summary>
    public PaymentBuilder AddPaymentTermDescription(string description)
    {
        _payment.PaymentTerms ??= new List<PaymentTerm>();
        _payment.PaymentTerms.Add(new PaymentTerm { DueDateDescription = description });
        return this;
    }

    /// <summary>
    /// Dodaje formę płatności
    /// </summary>
    public PaymentBuilder AddPaymentMethod(PaymentMethod method, decimal? amount = null)
    {
        _payment.PaymentMethods ??= new List<PaymentMethodInfo>();
        _payment.PaymentMethods.Add(new PaymentMethodInfo
        {
            Method = method,
            Amount = amount
        });
        return this;
    }

    /// <summary>
    /// Dodaje rachunek bankowy
    /// </summary>
    public PaymentBuilder AddBankAccount(Action<BankAccountBuilder> configure)
    {
        var builder = new BankAccountBuilder();
        configure(builder);

        _payment.BankAccounts ??= new List<Models.Common.BankAccount>();
        _payment.BankAccounts.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Dodaje rachunek bankowy (prostsza wersja)
    /// </summary>
    public PaymentBuilder AddBankAccount(string accountNumber, string? bankName = null, string? swiftCode = null)
    {
        _payment.BankAccounts ??= new List<Models.Common.BankAccount>();
        _payment.BankAccounts.Add(new Models.Common.BankAccount
        {
            AccountNumber = accountNumber,
            BankName = bankName,
            SwiftCode = swiftCode
        });
        return this;
    }

    /// <summary>
    /// Ustawia rachunek bankowy faktoringowy
    /// </summary>
    public PaymentBuilder WithFactoringBankAccount(Action<BankAccountBuilder> configure)
    {
        var builder = new BankAccountBuilder();
        configure(builder);
        _payment.FactoringBankAccount = builder.Build();
        return this;
    }

    /// <summary>
    /// Ustawia warunki skonta
    /// </summary>
    public PaymentBuilder WithDiscountTerms(string discountTerms)
    {
        _payment.DiscountTerms = discountTerms;
        return this;
    }

    /// <summary>
    /// Konfiguruje płatność gotówką
    /// </summary>
    public PaymentBuilder AsCash(decimal? amount = null)
    {
        return AddPaymentMethod(PaymentMethod.Cash, amount);
    }

    /// <summary>
    /// Konfiguruje płatność przelewem bankowym
    /// </summary>
    public PaymentBuilder AsBankTransfer(string accountNumber, decimal? amount = null, string? bankName = null)
    {
        AddPaymentMethod(PaymentMethod.BankTransfer, amount);
        return AddBankAccount(accountNumber, bankName);
    }

    /// <summary>
    /// Konfiguruje płatność kartą
    /// </summary>
    public PaymentBuilder AsCard(decimal? amount = null)
    {
        return AddPaymentMethod(PaymentMethod.Card, amount);
    }

    /// <summary>
    /// Buduje informacje o płatnościach
    /// </summary>
    public Payment Build() => _payment;
}

/// <summary>
/// Builder do budowania rachunku bankowego
/// </summary>
public class BankAccountBuilder
{
    private readonly Models.Common.BankAccount _bankAccount = new();

    /// <summary>
    /// Ustawia numer rachunku bankowego (IBAN)
    /// </summary>
    public BankAccountBuilder WithAccountNumber(string accountNumber)
    {
        _bankAccount.AccountNumber = accountNumber;
        return this;
    }

    /// <summary>
    /// Ustawia kod SWIFT banku
    /// </summary>
    public BankAccountBuilder WithSwiftCode(string swiftCode)
    {
        _bankAccount.SwiftCode = swiftCode;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę banku
    /// </summary>
    public BankAccountBuilder WithBankName(string bankName)
    {
        _bankAccount.BankName = bankName;
        return this;
    }

    /// <summary>
    /// Ustawia opis rachunku
    /// </summary>
    public BankAccountBuilder WithDescription(string description)
    {
        _bankAccount.AccountDescription = description;
        return this;
    }

    /// <summary>
    /// Ustawia typ rachunku własnego banku
    /// </summary>
    public BankAccountBuilder WithBankOwnAccountType(BankAccountType accountType)
    {
        _bankAccount.BankOwnAccountType = accountType;
        return this;
    }

    /// <summary>
    /// Buduje rachunek bankowy
    /// </summary>
    public Models.Common.BankAccount Build() => _bankAccount;
}
