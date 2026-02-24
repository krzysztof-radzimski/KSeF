using KSeF.Api.Configuration;
using KSeF.Api.Services;
using KSeF.Client.DI;
using KSeF.Invoice;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KSeF.Api;

/// <summary>
/// Rozszerzenia do rejestracji serwisów KSeF API w kontenerze DI
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Dodaje serwisy KSeF API do kontenera DI z konfiguracją z IConfiguration.
    /// Rejestruje KSeF.Client, KSeF.Invoice oraz serwisy wysyłania i odbierania faktur.
    /// </summary>
    /// <param name="services">Kolekcja serwisów</param>
    /// <param name="configuration">Konfiguracja aplikacji (sekcja "KSeF")</param>
    /// <returns>Kolekcja serwisów</returns>
    public static IServiceCollection AddKsefApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var ksefSection = configuration.GetSection(KsefApiOptions.SectionName);
        services.Configure<KsefApiOptions>(ksefSection);

        var options = ksefSection.Get<KsefApiOptions>() ?? new KsefApiOptions();

        return services.AddKsefApiServicesInternal(options);
    }

    /// <summary>
    /// Dodaje serwisy KSeF API do kontenera DI z delegatem konfiguracji.
    /// </summary>
    /// <param name="services">Kolekcja serwisów</param>
    /// <param name="configureOptions">Delegat konfigurujący opcje</param>
    /// <returns>Kolekcja serwisów</returns>
    public static IServiceCollection AddKsefApiServices(
        this IServiceCollection services,
        Action<KsefApiOptions> configureOptions)
    {
        var options = new KsefApiOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        return services.AddKsefApiServicesInternal(options);
    }

    private static IServiceCollection AddKsefApiServicesInternal(
        this IServiceCollection services,
        KsefApiOptions options)
    {
        // 1. Rejestracja KSeF.Invoice (modele, walidacja, serializacja)
        services.AddKsefInvoiceServices(invoiceOptions =>
        {
            if (!string.IsNullOrEmpty(options.SystemInfo))
            {
                invoiceOptions.DefaultSystemInfo = options.SystemInfo;
            }
        });

        // 2. Rejestracja KSeF.Client (komunikacja HTTP z API KSeF)
        services.AddKSeFClient(clientOptions =>
        {
            clientOptions.BaseUrl = options.BaseUrl;
        });

        // 3. Rejestracja serwisu kryptograficznego
        services.AddCryptographyClient();

        // 4. Rejestracja serwisów API
        services.AddScoped<IKsefSessionService, KsefSessionService>();
        services.AddScoped<IKsefInvoiceSendService, KsefInvoiceSendService>();
        services.AddScoped<IKsefInvoiceReceiveService, KsefInvoiceReceiveService>();
        services.AddScoped<IKsefInvoiceStatusService, KsefInvoiceStatusService>();

        return services;
    }
}
