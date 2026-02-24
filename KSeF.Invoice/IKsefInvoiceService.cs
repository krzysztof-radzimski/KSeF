using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Services.Validation;

namespace KSeF.Invoice;

/// <summary>
/// Interfejs głównego serwisu biblioteki KSeF Invoice
/// Stanowi główny punkt wejścia do biblioteki z prostym API
/// </summary>
public interface IKsefInvoiceService
{
    /// <summary>
    /// Rozpoczyna budowanie nowej faktury
    /// </summary>
    /// <returns>Builder faktury z fluent API</returns>
    InvoiceBuilder CreateInvoice();

    /// <summary>
    /// Waliduje fakturę (walidacja biznesowa i opcjonalnie XSD)
    /// </summary>
    /// <param name="invoice">Faktura do walidacji</param>
    /// <returns>Wynik walidacji z listą błędów i ostrzeżeń</returns>
    ValidationResult Validate(Models.Invoice invoice);

    /// <summary>
    /// Serializuje fakturę do formatu XML
    /// </summary>
    /// <param name="invoice">Faktura do serializacji</param>
    /// <returns>Dokument XML jako string</returns>
    /// <exception cref="InvalidOperationException">Gdy walidacja jest włączona i faktura jest nieprawidłowa</exception>
    string ToXml(Models.Invoice invoice);

    /// <summary>
    /// Serializuje fakturę do tablicy bajtów (UTF-8)
    /// </summary>
    /// <param name="invoice">Faktura do serializacji</param>
    /// <returns>Dokument XML jako tablica bajtów</returns>
    /// <exception cref="InvalidOperationException">Gdy walidacja jest włączona i faktura jest nieprawidłowa</exception>
    byte[] ToBytes(Models.Invoice invoice);

    /// <summary>
    /// Deserializuje XML do obiektu faktury
    /// </summary>
    /// <param name="xml">Dokument XML jako string</param>
    /// <returns>Obiekt faktury lub null gdy deserializacja się nie powiodła</returns>
    Models.Invoice? FromXml(string xml);

    /// <summary>
    /// Deserializuje tablicę bajtów do obiektu faktury
    /// </summary>
    /// <param name="data">Dokument XML jako tablica bajtów</param>
    /// <returns>Obiekt faktury lub null gdy deserializacja się nie powiodła</returns>
    Models.Invoice? FromBytes(byte[] data);
}
