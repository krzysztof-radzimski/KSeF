/*=====================================================================================

    KSeF.Invoice
    Rozszerzenia IServiceCollection do rejestracji serwisów
    serializacji faktur KSeF w kontenerze Dependency Injection.
    Rejestruje KsefInvoiceSerializer jako singleton implementujący
    interfejs IInvoiceSerializer.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

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
