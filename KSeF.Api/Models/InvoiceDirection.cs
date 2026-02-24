namespace KSeF.Api.Models;

/// <summary>
/// Kierunek wyszukiwanych faktur
/// </summary>
public enum InvoiceDirection
{
    /// <summary>
    /// Faktury zakupowe (otrzymane)
    /// </summary>
    Purchase,

    /// <summary>
    /// Faktury sprzedażowe (wystawione)
    /// </summary>
    Sales
}
