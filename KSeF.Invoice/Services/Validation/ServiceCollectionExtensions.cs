using KSeF.Invoice.Services.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Invoice.Services.Validation;

/// <summary>
/// Rozszerzenia do rejestracji serwisów walidacji w kontenerze DI
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Dodaje serwisy walidacji faktur KSeF do kontenera DI
    /// </summary>
    /// <param name="services">Kolekcja serwisów</param>
    /// <returns>Kolekcja serwisów z dodanymi walidatorami</returns>
    public static IServiceCollection AddKsefInvoiceValidation(this IServiceCollection services)
    {
        // Walidatory szczegółowe
        services.AddSingleton<INipValidator, NipValidator>();
        services.AddSingleton<IIbanValidator, IbanValidator>();
        services.AddSingleton<IDateValidator, DateValidator>();

        // Główny walidator faktur (walidacja biznesowa)
        services.AddSingleton<InvoiceValidator>();
        services.AddSingleton<IInvoiceValidator>(sp => sp.GetRequiredService<InvoiceValidator>());

        // Walidator XSD (walidacja struktury XML)
        services.AddSingleton<XsdValidator>();
        services.AddSingleton<IXsdValidator>(sp => sp.GetRequiredService<XsdValidator>());

        return services;
    }
}
