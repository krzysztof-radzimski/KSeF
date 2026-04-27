/*=====================================================================================

    KSeF.Invoice
    Builder do budowania danych identyfikacyjnych sprzedawcy (TPodmiot1)
    z wykorzystaniem wzorca Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;

namespace KSeF.Invoice.Services.Builders;

/// <summary>
/// Builder do budowania danych identyfikacyjnych sprzedawcy (TPodmiot1)
/// </summary>
public class SellerIdentificationBuilder
{
    private readonly SellerIdentification _identification;

    /// <summary>
    /// Inicjalizuje nową instancję buildera z istniejącym obiektem SellerIdentification
    /// </summary>
    /// <param name="identification">Obiekt identyfikacji sprzedawcy do modyfikacji</param>
    /// <exception cref="ArgumentNullException">Gdy identification jest null</exception>
    public SellerIdentificationBuilder(SellerIdentification identification)
    {
#if NETSTANDARD2_0
        ThrowHelper.ThrowIfNull(identification);
#else
        ArgumentNullException.ThrowIfNull(identification);
#endif
        _identification = identification;
    }

    /// <summary>
    /// Ustawia NIP sprzedawcy (normalizuje format: usuwa myślniki i whitespace)
    /// </summary>
    /// <param name="nip">Numer NIP sprzedawcy</param>
    /// <returns>Instancję buildera do łańcuchowania wywołań</returns>
    public SellerIdentificationBuilder WithNip(string nip)
    {
        _identification.Nip = nip != null ? nip.Trim().Replace("-", string.Empty) : nip;
        return this;
    }

    /// <summary>
    /// Ustawia nazwę sprzedawcy
    /// </summary>
    /// <param name="name">Nazwa sprzedawcy</param>
    /// <returns>Instancję buildera do łańcuchowania wywołań</returns>
    public SellerIdentificationBuilder WithName(string name)
    {
        _identification.Name = name;
        return this;
    }

    /// <summary>
    /// Buduje obiekt SellerIdentification
    /// </summary>
    /// <returns>Zbudowany obiekt SellerIdentification</returns>
    public SellerIdentification Build() => _identification;
}
