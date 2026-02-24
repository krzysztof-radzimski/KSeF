using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Główny walidator faktur KSeF
/// Implementuje kompleksową walidację zgodnie z wymaganiami KSeF
/// </summary>
public class InvoiceValidator : IInvoiceValidator
{
    private readonly INipValidator _nipValidator;
    private readonly IIbanValidator _ibanValidator;
    private readonly IDateValidator _dateValidator;

    /// <summary>
    /// Maksymalna długość pól tekstowych (nazwy, opisy)
    /// </summary>
    private const int MaxTextFieldLength = 512;

    /// <summary>
    /// Maksymalna długość pola numeru faktury
    /// </summary>
    private const int MaxInvoiceNumberLength = 256;

    /// <summary>
    /// Maksymalna liczba pozycji na fakturze
    /// </summary>
    private const int MaxLineItems = 10000;

    /// <summary>
    /// Tolerancja dla porównywania kwot (zaokrąglenia)
    /// </summary>
    private const decimal AmountTolerance = 0.02m;

    /// <summary>
    /// Inicjalizuje nową instancję walidatora faktur
    /// </summary>
    /// <param name="nipValidator">Walidator NIP</param>
    /// <param name="ibanValidator">Walidator IBAN</param>
    /// <param name="dateValidator">Walidator dat</param>
    public InvoiceValidator(
        INipValidator nipValidator,
        IIbanValidator ibanValidator,
        IDateValidator dateValidator)
    {
        _nipValidator = nipValidator;
        _ibanValidator = ibanValidator;
        _dateValidator = dateValidator;
    }

    /// <inheritdoc />
    public ValidationResult Validate(Models.Invoice invoice)
    {
        var result = new ValidationResult();

        // Walidacja głównej struktury
        ValidateMainStructure(invoice, result);

        // Walidacja sprzedawcy
        ValidateSeller(invoice.Seller, result);

        // Walidacja nabywcy
        ValidateBuyer(invoice.Buyer, result);

        // Walidacja danych faktury
        ValidateInvoiceData(invoice.InvoiceData, result);

        // Walidacja pozycji faktury
        ValidateLineItems(invoice.InvoiceData?.LineItems, result);

        // Walidacja spójności sum
        ValidateAmountConsistency(invoice.InvoiceData, result);

        // Walidacja płatności
        ValidatePayment(invoice.InvoiceData?.Payment, result);

        return result;
    }

    /// <summary>
    /// Waliduje główną strukturę faktury
    /// </summary>
    private void ValidateMainStructure(Models.Invoice invoice, ValidationResult result)
    {
        if (invoice.Seller == null)
        {
            result.AddError("INV_SELLER_MISSING", "Sprzedawca jest wymagany", "Seller");
        }

        if (invoice.Buyer == null)
        {
            result.AddError("INV_BUYER_MISSING", "Nabywca jest wymagany", "Buyer");
        }

        if (invoice.InvoiceData == null)
        {
            result.AddError("INV_DATA_MISSING", "Dane faktury są wymagane", "InvoiceData");
        }
    }

    /// <summary>
    /// Waliduje dane sprzedawcy
    /// </summary>
    private void ValidateSeller(Models.Entities.Seller? seller, ValidationResult result)
    {
        if (seller == null)
            return;

        // NIP sprzedawcy - wymagany
        if (string.IsNullOrWhiteSpace(seller.TaxId))
        {
            result.AddError("SELLER_NIP_MISSING", "NIP sprzedawcy jest wymagany", "Seller.TaxId");
        }
        else
        {
            var nipResult = _nipValidator.Validate(seller.TaxId);
            foreach (var error in nipResult.Errors)
            {
                result.AddError(error.Code, $"Sprzedawca: {error.Message}", $"Seller.{error.FieldName}");
            }
            foreach (var warning in nipResult.Warnings)
            {
                result.AddWarning(warning.Code, $"Sprzedawca: {warning.Message}", $"Seller.{warning.FieldName}");
            }
        }

        // Nazwa sprzedawcy - wymagana
        if (string.IsNullOrWhiteSpace(seller.Name))
        {
            result.AddError("SELLER_NAME_MISSING", "Nazwa sprzedawcy jest wymagana", "Seller.Name");
        }
        else if (seller.Name.Length > MaxTextFieldLength)
        {
            result.AddError("SELLER_NAME_TOO_LONG",
                $"Nazwa sprzedawcy jest za długa (max {MaxTextFieldLength} znaków, ma {seller.Name.Length})",
                "Seller.Name");
        }
    }

    /// <summary>
    /// Waliduje dane nabywcy
    /// </summary>
    private void ValidateBuyer(Models.Entities.Buyer? buyer, ValidationResult result)
    {
        if (buyer == null)
            return;

        // Nabywca musi mieć przynajmniej jeden identyfikator
        var hasAnyId = buyer.HasPolishTaxId || buyer.HasEuVatId || buyer.HasOtherId || buyer.HasNoIdentifier;
        if (!hasAnyId)
        {
            result.AddError("BUYER_ID_MISSING",
                "Nabywca musi mieć przynajmniej jeden identyfikator (NIP, VAT UE, inny ID lub oznaczenie braku ID)",
                "Buyer");
        }

        // Walidacja NIP nabywcy (jeśli podany)
        if (buyer.HasPolishTaxId)
        {
            var nipResult = _nipValidator.Validate(buyer.TaxId);
            foreach (var error in nipResult.Errors)
            {
                result.AddError(error.Code, $"Nabywca: {error.Message}", $"Buyer.{error.FieldName}");
            }
            foreach (var warning in nipResult.Warnings)
            {
                result.AddWarning(warning.Code, $"Nabywca: {warning.Message}", $"Buyer.{warning.FieldName}");
            }
        }

        // Walidacja długości nazwy
        if (!string.IsNullOrEmpty(buyer.Name) && buyer.Name.Length > MaxTextFieldLength)
        {
            result.AddError("BUYER_NAME_TOO_LONG",
                $"Nazwa nabywcy jest za długa (max {MaxTextFieldLength} znaków, ma {buyer.Name.Length})",
                "Buyer.Name");
        }
    }

    /// <summary>
    /// Waliduje dane merytoryczne faktury
    /// </summary>
    private void ValidateInvoiceData(InvoiceData? invoiceData, ValidationResult result)
    {
        if (invoiceData == null)
            return;

        // Numer faktury - wymagany
        if (string.IsNullOrWhiteSpace(invoiceData.InvoiceNumber))
        {
            result.AddError("INV_NUMBER_MISSING", "Numer faktury jest wymagany", "InvoiceData.InvoiceNumber");
        }
        else if (invoiceData.InvoiceNumber.Length > MaxInvoiceNumberLength)
        {
            result.AddError("INV_NUMBER_TOO_LONG",
                $"Numer faktury jest za długi (max {MaxInvoiceNumberLength} znaków, ma {invoiceData.InvoiceNumber.Length})",
                "InvoiceData.InvoiceNumber");
        }

        // Data wystawienia - wymagana
        if (invoiceData.IssueDate == default)
        {
            result.AddError("INV_DATE_MISSING", "Data wystawienia jest wymagana", "InvoiceData.IssueDate");
        }
        else
        {
            var dateResult = _dateValidator.ValidateIssueDate(invoiceData.IssueDate);
            result.Merge(dateResult);
        }

        // Data sprzedaży (jeśli podana)
        if (invoiceData.HasSaleDate)
        {
            var saleDateResult = _dateValidator.ValidateSaleDate(invoiceData.SaleDate, invoiceData.IssueDate);
            result.Merge(saleDateResult);
        }

        // Okres rozliczeniowy (jeśli podany)
        if (invoiceData.HasSalePeriod && invoiceData.SalePeriod != null)
        {
            var periodResult = _dateValidator.ValidatePeriod(
                invoiceData.SalePeriod.PeriodFrom,
                invoiceData.SalePeriod.PeriodTo);
            result.Merge(periodResult);
        }

        // Nie można mieć jednocześnie daty sprzedaży i okresu
        if (invoiceData.HasSaleDate && invoiceData.HasSalePeriod)
        {
            result.AddWarning("INV_DATE_AND_PERIOD",
                "Faktura ma jednocześnie datę sprzedaży i okres rozliczeniowy. Powinno być jedno z nich.",
                "InvoiceData");
        }

        // Walidacja miejsca wystawienia
        if (!string.IsNullOrEmpty(invoiceData.IssuePlace) && invoiceData.IssuePlace.Length > MaxInvoiceNumberLength)
        {
            result.AddError("INV_PLACE_TOO_LONG",
                $"Miejsce wystawienia jest za długie (max {MaxInvoiceNumberLength} znaków)",
                "InvoiceData.IssuePlace");
        }

        // Walidacja dla faktury korygującej
        if (invoiceData.IsCorrection)
        {
            ValidateCorrectionData(invoiceData, result);
        }
    }

    /// <summary>
    /// Waliduje dane korekty faktury
    /// </summary>
    private void ValidateCorrectionData(InvoiceData invoiceData, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(invoiceData.CorrectionReason))
        {
            result.AddError("INV_CORR_REASON_MISSING",
                "Przyczyna korekty jest wymagana dla faktury korygującej",
                "InvoiceData.CorrectionReason");
        }
        else if (invoiceData.CorrectionReason.Length > MaxInvoiceNumberLength)
        {
            result.AddError("INV_CORR_REASON_TOO_LONG",
                $"Przyczyna korekty jest za długa (max {MaxInvoiceNumberLength} znaków)",
                "InvoiceData.CorrectionReason");
        }

        if (invoiceData.CorrectedInvoiceData == null)
        {
            result.AddError("INV_CORR_DATA_MISSING",
                "Dane faktury korygowanej są wymagane dla faktury korygującej",
                "InvoiceData.CorrectedInvoiceData");
        }
    }

    /// <summary>
    /// Waliduje pozycje faktury
    /// </summary>
    private void ValidateLineItems(List<InvoiceLineItem>? lineItems, ValidationResult result)
    {
        if (lineItems == null || lineItems.Count == 0)
        {
            result.AddWarning("INV_NO_ITEMS",
                "Faktura nie ma pozycji. Większość faktur powinna mieć przynajmniej jedną pozycję.",
                "InvoiceData.LineItems");
            return;
        }

        if (lineItems.Count > MaxLineItems)
        {
            result.AddError("INV_TOO_MANY_ITEMS",
                $"Faktura ma za dużo pozycji (max {MaxLineItems}, ma {lineItems.Count})",
                "InvoiceData.LineItems");
        }

        var lineNumbers = new HashSet<int>();
        for (var i = 0; i < lineItems.Count; i++)
        {
            var item = lineItems[i];
            var fieldPrefix = $"InvoiceData.LineItems[{i}]";

            // Numer wiersza
            if (item.LineNumber <= 0)
            {
                result.AddError("ITEM_LINE_NUMBER_INVALID",
                    $"Pozycja {i + 1}: Numer wiersza musi być większy od 0",
                    $"{fieldPrefix}.LineNumber");
            }
            else if (!lineNumbers.Add(item.LineNumber))
            {
                result.AddError("ITEM_LINE_NUMBER_DUPLICATE",
                    $"Pozycja {i + 1}: Zduplikowany numer wiersza {item.LineNumber}",
                    $"{fieldPrefix}.LineNumber");
            }

            // Nazwa produktu
            if (string.IsNullOrWhiteSpace(item.ProductName))
            {
                result.AddError("ITEM_NAME_MISSING",
                    $"Pozycja {i + 1}: Nazwa towaru/usługi jest wymagana",
                    $"{fieldPrefix}.ProductName");
            }
            else if (item.ProductName.Length > MaxTextFieldLength)
            {
                result.AddError("ITEM_NAME_TOO_LONG",
                    $"Pozycja {i + 1}: Nazwa towaru/usługi jest za długa (max {MaxTextFieldLength} znaków)",
                    $"{fieldPrefix}.ProductName");
            }

            // Walidacja jednostki miary
            if (!string.IsNullOrEmpty(item.Unit) && item.Unit.Length > MaxInvoiceNumberLength)
            {
                result.AddError("ITEM_UNIT_TOO_LONG",
                    $"Pozycja {i + 1}: Jednostka miary jest za długa (max {MaxInvoiceNumberLength} znaków)",
                    $"{fieldPrefix}.Unit");
            }

            // Spójność wartości
            ValidateLineItemAmounts(item, i + 1, fieldPrefix, result);
        }
    }

    /// <summary>
    /// Waliduje spójność kwot na pozycji faktury
    /// </summary>
    private void ValidateLineItemAmounts(InvoiceLineItem item, int lineNumber, string fieldPrefix, ValidationResult result)
    {
        // Sprawdź czy podano wartość netto lub brutto
        if (!item.NetAmount.HasValue && !item.GrossAmount.HasValue)
        {
            result.AddWarning("ITEM_NO_AMOUNT",
                $"Pozycja {lineNumber}: Brak wartości netto (P_11) lub brutto (P_11Vat)",
                $"{fieldPrefix}");
        }

        // Jeśli podano ilość i cenę jednostkową, sprawdź zgodność z wartością
        if (item.Quantity.HasValue && item.UnitNetPrice.HasValue && item.NetAmount.HasValue)
        {
            var calculatedNet = item.Quantity.Value * item.UnitNetPrice.Value;
            var difference = Math.Abs(calculatedNet - item.NetAmount.Value);
            if (difference > AmountTolerance)
            {
                result.AddWarning("ITEM_NET_MISMATCH",
                    $"Pozycja {lineNumber}: Wartość netto ({item.NetAmount.Value:N2}) nie zgadza się z " +
                    $"ilością × ceną ({calculatedNet:N2})",
                    $"{fieldPrefix}.NetAmount");
            }
        }

        // Walidacja zgodności stawki VAT z kwotami
        if (item.NetAmount.HasValue && item.VatAmount.HasValue)
        {
            ValidateVatAmount(item.VatRate, item.NetAmount.Value, item.VatAmount.Value, lineNumber, fieldPrefix, result);
        }
    }

    /// <summary>
    /// Waliduje zgodność kwoty VAT ze stawką
    /// </summary>
    private void ValidateVatAmount(VatRate vatRate, decimal netAmount, decimal vatAmount,
        int lineNumber, string fieldPrefix, ValidationResult result)
    {
        var expectedRate = GetVatRatePercentage(vatRate);
        if (!expectedRate.HasValue)
            return; // Dla stawek specjalnych nie walidujemy kwoty

        var expectedVat = Math.Round(netAmount * expectedRate.Value / 100m, 2);
        var difference = Math.Abs(expectedVat - vatAmount);

        if (difference > AmountTolerance)
        {
            result.AddWarning("ITEM_VAT_MISMATCH",
                $"Pozycja {lineNumber}: Kwota VAT ({vatAmount:N2}) nie zgadza się z oczekiwaną " +
                $"({expectedVat:N2}) dla stawki {GetVatRateDisplay(vatRate)}",
                $"{fieldPrefix}.VatAmount");
        }
    }

    /// <summary>
    /// Zwraca wartość procentową stawki VAT
    /// </summary>
    private static decimal? GetVatRatePercentage(VatRate vatRate)
    {
        return vatRate switch
        {
            VatRate.Rate23 => 23m,
            VatRate.Rate22 => 22m,
            VatRate.Rate8 => 8m,
            VatRate.Rate7 => 7m,
            VatRate.Rate5 => 5m,
            VatRate.Rate4 => 4m,
            VatRate.Rate3 => 3m,
            VatRate.Rate0Domestic => 0m,
            VatRate.Rate0IntraCommunitySupply => 0m,
            VatRate.Rate0Export => 0m,
            _ => null // Dla zwolnionych, oo, np nie walidujemy
        };
    }

    /// <summary>
    /// Zwraca tekstową reprezentację stawki VAT
    /// </summary>
    private static string GetVatRateDisplay(VatRate vatRate)
    {
        return vatRate switch
        {
            VatRate.Rate23 => "23%",
            VatRate.Rate22 => "22%",
            VatRate.Rate8 => "8%",
            VatRate.Rate7 => "7%",
            VatRate.Rate5 => "5%",
            VatRate.Rate4 => "4%",
            VatRate.Rate3 => "3%",
            VatRate.Rate0Domestic => "0%",
            VatRate.Rate0IntraCommunitySupply => "0% WDT",
            VatRate.Rate0Export => "0% eksport",
            VatRate.Exempt => "zw",
            VatRate.ReverseCharge => "oo",
            VatRate.NotSubjectToTaxI => "np I",
            VatRate.NotSubjectToTaxII => "np II",
            _ => vatRate.ToString()
        };
    }

    /// <summary>
    /// Waliduje spójność sum na fakturze
    /// </summary>
    private void ValidateAmountConsistency(InvoiceData? invoiceData, ValidationResult result)
    {
        if (invoiceData == null || invoiceData.LineItems == null || invoiceData.LineItems.Count == 0)
            return;

        // Oblicz sumy z pozycji wg stawek VAT
        var sumsByRate = new Dictionary<VatRate, (decimal Net, decimal Vat)>();
        foreach (var item in invoiceData.LineItems)
        {
            var net = item.NetAmount ?? 0;
            var vat = item.VatAmount ?? 0;

            if (sumsByRate.TryGetValue(item.VatRate, out var existing))
            {
                sumsByRate[item.VatRate] = (existing.Net + net, existing.Vat + vat);
            }
            else
            {
                sumsByRate[item.VatRate] = (net, vat);
            }
        }

        // Porównaj z podsumowaniami na fakturze
        ValidateRateSummary(sumsByRate, VatRate.Rate23, invoiceData.NetAmount23, invoiceData.VatAmount23, "23%", result);
        ValidateRateSummary(sumsByRate, VatRate.Rate8, invoiceData.NetAmount8, invoiceData.VatAmount8, "8%", result);
        ValidateRateSummary(sumsByRate, VatRate.Rate5, invoiceData.NetAmount5, invoiceData.VatAmount5, "5%", result);
        ValidateRateSummary(sumsByRate, VatRate.Rate4, invoiceData.NetAmount4, invoiceData.VatAmount4, "4%", result);

        // Sprawdź sumę całkowitą
        var calculatedTotal = invoiceData.TotalNetAmount + invoiceData.TotalVatAmount;
        var difference = Math.Abs(calculatedTotal - invoiceData.TotalAmount);
        if (difference > AmountTolerance)
        {
            result.AddWarning("INV_TOTAL_MISMATCH",
                $"Kwota należności ({invoiceData.TotalAmount:N2}) nie zgadza się z sumą netto + VAT ({calculatedTotal:N2})",
                "InvoiceData.TotalAmount");
        }
    }

    /// <summary>
    /// Waliduje podsumowanie dla pojedynczej stawki VAT
    /// </summary>
    private void ValidateRateSummary(Dictionary<VatRate, (decimal Net, decimal Vat)> sumsByRate,
        VatRate rate, decimal? expectedNet, decimal? expectedVat, string rateDisplay, ValidationResult result)
    {
        if (!sumsByRate.TryGetValue(rate, out var sums))
            return;

        if (expectedNet.HasValue)
        {
            var netDiff = Math.Abs(sums.Net - expectedNet.Value);
            if (netDiff > AmountTolerance)
            {
                result.AddWarning("INV_NET_SUM_MISMATCH",
                    $"Suma netto dla stawki {rateDisplay} ({expectedNet.Value:N2}) nie zgadza się " +
                    $"z sumą pozycji ({sums.Net:N2})",
                    $"InvoiceData.NetAmount{rateDisplay.Replace("%", "")}");
            }
        }

        if (expectedVat.HasValue)
        {
            var vatDiff = Math.Abs(sums.Vat - expectedVat.Value);
            if (vatDiff > AmountTolerance)
            {
                result.AddWarning("INV_VAT_SUM_MISMATCH",
                    $"Suma VAT dla stawki {rateDisplay} ({expectedVat.Value:N2}) nie zgadza się " +
                    $"z sumą pozycji ({sums.Vat:N2})",
                    $"InvoiceData.VatAmount{rateDisplay.Replace("%", "")}");
            }
        }
    }

    /// <summary>
    /// Waliduje dane płatności
    /// </summary>
    private void ValidatePayment(Models.Payments.Payment? payment, ValidationResult result)
    {
        if (payment == null)
            return;

        // Walidacja rachunków bankowych
        if (payment.HasBankAccounts && payment.BankAccounts != null)
        {
            for (var i = 0; i < payment.BankAccounts.Count; i++)
            {
                var account = payment.BankAccounts[i];
                var fieldPrefix = $"InvoiceData.Payment.BankAccounts[{i}]";

                if (string.IsNullOrWhiteSpace(account.AccountNumber))
                {
                    result.AddError("PAYMENT_ACCOUNT_EMPTY",
                        $"Rachunek bankowy {i + 1}: Numer rachunku jest wymagany",
                        $"{fieldPrefix}.AccountNumber");
                }
                else
                {
                    var ibanResult = _ibanValidator.Validate(account.AccountNumber);
                    foreach (var error in ibanResult.Errors)
                    {
                        result.AddError(error.Code, $"Rachunek bankowy {i + 1}: {error.Message}",
                            $"{fieldPrefix}.{error.FieldName}");
                    }
                    foreach (var warning in ibanResult.Warnings)
                    {
                        result.AddWarning(warning.Code, $"Rachunek bankowy {i + 1}: {warning.Message}",
                            $"{fieldPrefix}.{warning.FieldName}");
                    }
                }
            }
        }

        // Walidacja rachunku faktoringowego
        if (payment.HasFactoringBankAccount && payment.FactoringBankAccount != null)
        {
            if (string.IsNullOrWhiteSpace(payment.FactoringBankAccount.AccountNumber))
            {
                result.AddError("PAYMENT_FACTORING_EMPTY",
                    "Rachunek faktoringowy: Numer rachunku jest wymagany",
                    "InvoiceData.Payment.FactoringBankAccount.AccountNumber");
            }
            else
            {
                var ibanResult = _ibanValidator.Validate(payment.FactoringBankAccount.AccountNumber);
                foreach (var error in ibanResult.Errors)
                {
                    result.AddError(error.Code, $"Rachunek faktoringowy: {error.Message}",
                        $"InvoiceData.Payment.FactoringBankAccount.{error.FieldName}");
                }
                foreach (var warning in ibanResult.Warnings)
                {
                    result.AddWarning(warning.Code, $"Rachunek faktoringowy: {warning.Message}",
                        $"InvoiceData.Payment.FactoringBankAccount.{warning.FieldName}");
                }
            }
        }
    }
}
