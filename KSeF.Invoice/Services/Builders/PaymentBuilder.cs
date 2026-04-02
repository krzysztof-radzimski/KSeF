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
    /// Dodaje termin płatności (jako opis: ilość + jednostka + zdarzenie początkowe)
    /// </summary>
    public PaymentBuilder AddPaymentTermDescription(int quantity, string unit, string startingEvent)
    {
        _payment.PaymentTerms ??= new List<PaymentTerm>();
        _payment.PaymentTerms.Add(new PaymentTerm
        {
            DueDateDescription = new PaymentTermDescription
            {
                Quantity = quantity,
                Unit = unit,
                StartingEvent = startingEvent
            }
        });
        return this;
    }

    /// <summary>
    /// Ustawia standardową formę płatności (FormaPlatnosci)
    /// </summary>
    public PaymentBuilder WithPaymentMethod(PaymentMethod method)
    {
        _payment.PaymentMethod = method;
        _payment.OtherPaymentMarker = null;
        _payment.OtherPaymentDescription = null;
        return this;
    }

    /// <summary>
    /// Ustawia inną formę płatności (PlatnoscInna + OpisPlatnosci)
    /// </summary>
    public PaymentBuilder WithOtherPaymentMethod(string description)
    {
        _payment.OtherPaymentMarker = 1;
        _payment.OtherPaymentDescription = description;
        _payment.PaymentMethod = null;
        return this;
    }

    /// <summary>
    /// Oznacza fakturę jako zapłaconą w całości (Zaplacono + DataZaplaty)
    /// </summary>
    public PaymentBuilder AsPaidInFull(DateOnly paymentDate)
    {
        _payment.PaidInFull = 1;
        _payment.PaymentDate = paymentDate;
        _payment.PartialPaymentMarker = null;
        _payment.PartialPayments = null;
        return this;
    }

    /// <summary>
    /// Ustawia znacznik zapłaty częściowej (ZnacznikZaplatyCzesciowej)
    /// </summary>
    /// <param name="fullyPaidInParts">true = zapłacono w całości w częściach (2), false = zapłacono częściowo (1)</param>
    public PaymentBuilder WithPartialPayments(bool fullyPaidInParts = false)
    {
        _payment.PartialPaymentMarker = fullyPaidInParts ? 2 : 1;
        _payment.PaidInFull = null;
        _payment.PaymentDate = null;
        _payment.PartialPayments ??= new List<PartialPayment>();
        return this;
    }

    /// <summary>
    /// Dodaje zapłatę częściową
    /// </summary>
    public PaymentBuilder AddPartialPayment(decimal amount, DateOnly date, PaymentMethod? method = null)
    {
        _payment.PartialPayments ??= new List<PartialPayment>();
        _payment.PartialPayments.Add(new PartialPayment
        {
            Amount = amount,
            Date = date,
            PaymentMethod = method
        });
        return this;
    }

    /// <summary>
    /// Dodaje zapłatę częściową z inną formą płatności
    /// </summary>
    public PaymentBuilder AddPartialPaymentOther(decimal amount, DateOnly date, string paymentDescription)
    {
        _payment.PartialPayments ??= new List<PartialPayment>();
        _payment.PartialPayments.Add(new PartialPayment
        {
            Amount = amount,
            Date = date,
            OtherPaymentMarker = 1,
            OtherPaymentDescription = paymentDescription
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

        _payment.BankAccounts ??= new List<BankAccount>();
        _payment.BankAccounts.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Dodaje rachunek bankowy (prostsza wersja)
    /// </summary>
    public PaymentBuilder AddBankAccount(string accountNumber, string? bankName = null, string? swiftCode = null, BankAccountType? bankOwnAccountType = null)
    {
        _payment.BankAccounts ??= new List<BankAccount>();
        _payment.BankAccounts.Add(new BankAccount
        {
            AccountNumber = accountNumber,
            BankOwnAccountType = bankOwnAccountType,
            BankName = bankName,
            SwiftCode = swiftCode
        });
        return this;
    }

    /// <summary>
    /// Dodaje rachunek bankowy faktora (RachunekBankowyFaktora)
    /// </summary>
    public PaymentBuilder AddFactoringBankAccount(Action<BankAccountBuilder> configure)
    {
        var builder = new BankAccountBuilder();
        configure(builder);
        _payment.FactoringBankAccounts ??= new List<BankAccount>();
        _payment.FactoringBankAccounts.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Ustawia rachunek bankowy faktoringowy (alias dla AddFactoringBankAccount)
    /// </summary>
    public PaymentBuilder WithFactoringBankAccount(Action<BankAccountBuilder> configure)
    {
        return AddFactoringBankAccount(configure);
    }

    /// <summary>
    /// Ustawia warunki skonta (Skonto)
    /// </summary>
    public PaymentBuilder WithDiscount(string conditions, string amount)
    {
        _payment.Discount = new DiscountTerms
        {
            Conditions = conditions,
            Amount = amount
        };
        return this;
    }

    /// <summary>
    /// Ustawia link do płatności (LinkDoPlatnosci)
    /// </summary>
    public PaymentBuilder WithPaymentLink(string paymentLink)
    {
        _payment.PaymentLink = paymentLink;
        return this;
    }

    /// <summary>
    /// Ustawia identyfikator płatności KSeF (IPKSeF)
    /// </summary>
    public PaymentBuilder WithKSeFPaymentId(string ksefPaymentId)
    {
        _payment.KSeFPaymentId = ksefPaymentId;
        return this;
    }

    /// <summary>
    /// Konfiguruje płatność gotówką
    /// </summary>
    public PaymentBuilder AsCash()
    {
        return WithPaymentMethod(PaymentMethod.Cash);
    }

    /// <summary>
    /// Konfiguruje płatność przelewem bankowym
    /// </summary>
    public PaymentBuilder AsBankTransfer(string accountNumber, decimal? amount = null, string? bankName = null, BankAccountType? bankOwnAccountType = null)
    {
        WithPaymentMethod(PaymentMethod.BankTransfer);
        return AddBankAccount(accountNumber, bankName, bankOwnAccountType: bankOwnAccountType);
    }

    /// <summary>
    /// Konfiguruje płatność kartą
    /// </summary>
    public PaymentBuilder AsCard()
    {
        return WithPaymentMethod(PaymentMethod.Card);
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
    private readonly BankAccount _bankAccount = new();

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
    public BankAccount Build() => _bankAccount;
}
