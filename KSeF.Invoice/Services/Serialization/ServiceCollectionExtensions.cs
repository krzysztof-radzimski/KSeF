using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Invoice.Services.Serialization;

/// <summary>
/// Rozszerzenia do rejestracji serwisów serializacji w kontenerze DI
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Dodaje serwis serializacji faktur KSeF do kontenera DI
    /// </summary>
    /// <param name="services">Kolekcja serwisów</param>
    /// <returns>Kolekcja serwisów z dodanym serializatorem</returns>
    public static IServiceCollection AddKsefInvoiceSerialization(this IServiceCollection services)
    {
        services.AddSingleton<IInvoiceSerializer, KsefInvoiceSerializer>();
        return services;
    }
}
