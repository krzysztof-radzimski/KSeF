namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Walidator dat na fakturze
/// </summary>
public class DateValidator : IDateValidator
{
    /// <summary>
    /// Maksymalny okres wsteczny dla daty wystawienia (w dniach)
    /// Faktury mogą być wystawiane z datą wsteczną, ale nie za daleko
    /// </summary>
    private const int MaxDaysBack = 365 * 5; // 5 lat

    /// <inheritdoc />
    public ValidationResult ValidateIssueDate(DateOnly issueDate)
    {
        var result = new ValidationResult();
        var today = DateOnly.FromDateTime(DateTime.Today);

        // Data wystawienia nie może być w przyszłości
        if (issueDate > today)
        {
            result.AddError("DATE_ISSUE_FUTURE",
                $"Data wystawienia ({issueDate:yyyy-MM-dd}) nie może być w przyszłości",
                "IssueDate");
        }

        // Ostrzeżenie dla bardzo starych dat
        var minDate = today.AddDays(-MaxDaysBack);
        if (issueDate < minDate)
        {
            result.AddWarning("DATE_ISSUE_OLD",
                $"Data wystawienia ({issueDate:yyyy-MM-dd}) jest bardzo stara (ponad 5 lat)",
                "IssueDate");
        }

        return result;
    }

    /// <inheritdoc />
    public ValidationResult ValidateSaleDate(DateOnly? saleDate, DateOnly? issueDate = null)
    {
        var result = new ValidationResult();

        if (!saleDate.HasValue)
            return result;

        var today = DateOnly.FromDateTime(DateTime.Today);

        // Data sprzedaży nie może być w przyszłości
        if (saleDate.Value > today)
        {
            result.AddError("DATE_SALE_FUTURE",
                $"Data sprzedaży ({saleDate.Value:yyyy-MM-dd}) nie może być w przyszłości",
                "SaleDate");
        }

        // Sprawdź zgodność z datą wystawienia
        if (issueDate.HasValue)
        {
            // Data sprzedaży nie powinna być znacznie po dacie wystawienia
            // (dopuszczalne jest kilka dni różnicy dla faktur wystawianych przed dostawą)
            var maxFutureDays = 30;
            if (saleDate.Value > issueDate.Value.AddDays(maxFutureDays))
            {
                result.AddWarning("DATE_SALE_AFTER_ISSUE",
                    $"Data sprzedaży ({saleDate.Value:yyyy-MM-dd}) jest znacznie późniejsza " +
                    $"niż data wystawienia ({issueDate.Value:yyyy-MM-dd})",
                    "SaleDate");
            }
        }

        return result;
    }

    /// <inheritdoc />
    public ValidationResult ValidatePeriod(DateOnly startDate, DateOnly endDate)
    {
        var result = new ValidationResult();
        var today = DateOnly.FromDateTime(DateTime.Today);

        // Data końcowa nie może być przed datą początkową
        if (endDate < startDate)
        {
            result.AddError("DATE_PERIOD_INVALID",
                $"Data końcowa okresu ({endDate:yyyy-MM-dd}) nie może być wcześniejsza " +
                $"niż data początkowa ({startDate:yyyy-MM-dd})",
                "SalePeriod");
        }

        // Data końcowa nie powinna być w dalekiej przyszłości
        var maxFutureDays = 60; // Dopuszczalne dla faktur za usługi ciągłe
        if (endDate > today.AddDays(maxFutureDays))
        {
            result.AddWarning("DATE_PERIOD_FUTURE",
                $"Data końcowa okresu ({endDate:yyyy-MM-dd}) jest w dalekiej przyszłości",
                "SalePeriod");
        }

        // Sprawdź czy okres nie jest zbyt długi (ponad rok)
        var periodDays = endDate.DayNumber - startDate.DayNumber;
        if (periodDays > 366)
        {
            result.AddWarning("DATE_PERIOD_TOO_LONG",
                $"Okres rozliczeniowy ({periodDays} dni) jest dłuższy niż rok",
                "SalePeriod");
        }

        return result;
    }
}
