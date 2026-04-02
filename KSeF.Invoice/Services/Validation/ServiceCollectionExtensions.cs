/*=====================================================================================

    KSeF.Invoice
    Rozszerzenia IServiceCollection do rejestracji serwisów
    walidacji faktur KSeF w kontenerze Dependency Injection.
    Rejestruje walidatory szczegółowe (NIP, IBAN, dat), główny
    walidator biznesowy (InvoiceValidator) oraz walidator XSD
    (XsdValidator).

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

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
