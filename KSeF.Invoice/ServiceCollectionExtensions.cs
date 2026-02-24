using KSeF.Invoice.Services.Serialization;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Invoice;

/// <summary>
/// Rozszerzenia do rejestracji serwisów KSeF Invoice w kontenerze DI
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Dodaje wszystkie serwisy biblioteki KSeF Invoice do kontenera DI
    /// z domyślnymi opcjami konfiguracji
    /// </summary>
    /// <param name="services">Kolekcja serwisów</param>
    /// <returns>Kolekcja serwisów z dodanymi serwisami KSeF</returns>
    public static IServiceCollection AddKsefInvoiceServices(this IServiceCollection services)
    {
        return services.AddKsefInvoiceServices(_ => { });
    }

    /// <summary>
    /// Dodaje wszystkie serwisy biblioteki KSeF Invoice do kontenera DI
    /// z możliwością konfiguracji opcji
    /// </summary>
    /// <param name="services">Kolekcja serwisów</param>
    /// <param name="configureOptions">Delegat konfigurujący opcje serwisu</param>
    /// <returns>Kolekcja serwisów z dodanymi serwisami KSeF</returns>
    public static IServiceCollection AddKsefInvoiceServices(
        this IServiceCollection services,
        Action<KsefInvoiceServiceOptions> configureOptions)
    {
        // Konfiguracja opcji
        services.Configure(configureOptions);

        // Serwisy walidacji
        services.AddKsefInvoiceValidation();

        // Serwisy serializacji
        services.AddKsefInvoiceSerialization();

        // Główny serwis fasady
        services.AddSingleton<IKsefInvoiceService, KsefInvoiceService>();

        return services;
    }
}
