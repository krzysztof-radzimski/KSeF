/*=====================================================================================

    KSeF.Invoice
    Builder do budowania stopki faktury (Stopka) zgodnej ze schematem
    KSeF FA(3). Zawiera metody do dodawania informacji dodatkowych
    (FooterInfo) oraz danych rejestrowych (RegistryData).
    Wykorzystuje wzorzec Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Attachments;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania stopki faktury (InvoiceFooter)
/// </summary>
public class FooterBuilder
{
    private readonly InvoiceFooter _footer = new();

    /// <summary>
    /// Dodaje informację tekstową w stopce (FooterInfo z FooterText/StopkaFaktury).
    /// Uwaga: schemat FA(3) dopuszcza max 3 wpisy Informacje.
    /// </summary>
    public FooterBuilder AddFooterText(string text)
    {
        _footer.AdditionalInfo ??= new List<FooterInfo>();
        _footer.AdditionalInfo.Add(new FooterInfo { FooterText = text });
        return this;
    }

    /// <summary>
    /// Dodaje gotowy obiekt FooterInfo.
    /// </summary>
    public FooterBuilder AddFooterInfo(FooterInfo info)
    {
        _footer.AdditionalInfo ??= new List<FooterInfo>();
        _footer.AdditionalInfo.Add(info);
        return this;
    }

    /// <summary>
    /// Dodaje rekord danych rejestrowych (wygodna metoda z parametrami opcjonalnymi).
    /// </summary>
    public FooterBuilder AddRegistryData(
        string? fullName = null,
        string? krs = null,
        string? regon = null,
        string? bdo = null)
    {
        _footer.Registries ??= new List<RegistryData>();
        _footer.Registries.Add(new RegistryData
        {
            FullName = fullName,
            KRS = krs,
            REGON = regon,
            BDO = bdo
        });
        return this;
    }

    /// <summary>
    /// Dodaje rekord rejestru przez nested builder.
    /// </summary>
    public FooterBuilder AddRegistry(Action<RegistryDataBuilder> configure)
    {
        var builder = new RegistryDataBuilder();
        configure(builder);
        _footer.Registries ??= new List<RegistryData>();
        _footer.Registries.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Dodaje gotowy rekord rejestru.
    /// </summary>
    public FooterBuilder AddRegistry(RegistryData data)
    {
        _footer.Registries ??= new List<RegistryData>();
        _footer.Registries.Add(data);
        return this;
    }

    /// <summary>
    /// Buduje obiekt InvoiceFooter.
    /// </summary>
    public InvoiceFooter Build() => _footer;
}
