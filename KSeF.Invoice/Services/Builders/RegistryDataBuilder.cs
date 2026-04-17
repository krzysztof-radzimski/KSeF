/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych rejestrowych (RegistryData).
    Umożliwia konfigurację pełnej nazwy podmiotu oraz numerów
    KRS, REGON i BDO. Wykorzystuje wzorzec Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Attachments;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania danych rejestrowych (RegistryData)
/// </summary>
public class RegistryDataBuilder
{
    private readonly RegistryData _data = new();

    /// <summary>
    /// Ustawia pełną nazwę podmiotu (PelnaNazwa, max 256 znaków)
    /// </summary>
    public RegistryDataBuilder WithFullName(string fullName)
    {
        _data.FullName = fullName;
        return this;
    }

    /// <summary>
    /// Ustawia numer KRS (10 cyfr)
    /// </summary>
    public RegistryDataBuilder WithKRS(string krs)
    {
        _data.KRS = krs;
        return this;
    }

    /// <summary>
    /// Ustawia numer REGON (9 lub 14 cyfr)
    /// </summary>
    public RegistryDataBuilder WithREGON(string regon)
    {
        _data.REGON = regon;
        return this;
    }

    /// <summary>
    /// Ustawia numer BDO (max 9 znaków)
    /// </summary>
    public RegistryDataBuilder WithBDO(string bdo)
    {
        _data.BDO = bdo;
        return this;
    }

    /// <summary>
    /// Buduje obiekt RegistryData
    /// </summary>
    public RegistryData Build() => _data;
}
