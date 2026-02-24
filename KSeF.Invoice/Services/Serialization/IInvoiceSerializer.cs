namespace KSeF.Invoice.Services.Serialization;

/// <summary>
/// Interfejs definiujący kontrakt dla serializacji faktury do formatu XML KSeF
/// </summary>
public interface IInvoiceSerializer
{
    /// <summary>
    /// Serializuje fakturę do formatu XML jako string
    /// </summary>
    /// <param name="invoice">Faktura do serializacji</param>
    /// <returns>Dokument XML jako string w kodowaniu UTF-8</returns>
    string SerializeToXml(Models.Invoice invoice);

    /// <summary>
    /// Serializuje fakturę do tablicy bajtów (UTF-8)
    /// </summary>
    /// <param name="invoice">Faktura do serializacji</param>
    /// <returns>Dokument XML jako tablica bajtów</returns>
    byte[] SerializeToBytes(Models.Invoice invoice);

    /// <summary>
    /// Serializuje fakturę do strumienia
    /// </summary>
    /// <param name="invoice">Faktura do serializacji</param>
    /// <param name="stream">Strumień docelowy</param>
    void SerializeToStream(Models.Invoice invoice, Stream stream);

    /// <summary>
    /// Serializuje fakturę do pliku
    /// </summary>
    /// <param name="invoice">Faktura do serializacji</param>
    /// <param name="filePath">Ścieżka do pliku docelowego</param>
    void SerializeToFile(Models.Invoice invoice, string filePath);

    /// <summary>
    /// Deserializuje XML do obiektu faktury
    /// </summary>
    /// <param name="xml">Dokument XML jako string</param>
    /// <returns>Obiekt faktury</returns>
    Models.Invoice? DeserializeFromXml(string xml);

    /// <summary>
    /// Deserializuje tablicę bajtów do obiektu faktury
    /// </summary>
    /// <param name="bytes">Dokument XML jako tablica bajtów</param>
    /// <returns>Obiekt faktury</returns>
    Models.Invoice? DeserializeFromBytes(byte[] bytes);

    /// <summary>
    /// Deserializuje strumień do obiektu faktury
    /// </summary>
    /// <param name="stream">Strumień źródłowy</param>
    /// <returns>Obiekt faktury</returns>
    Models.Invoice? DeserializeFromStream(Stream stream);

    /// <summary>
    /// Deserializuje plik do obiektu faktury
    /// </summary>
    /// <param name="filePath">Ścieżka do pliku źródłowego</param>
    /// <returns>Obiekt faktury</returns>
    Models.Invoice? DeserializeFromFile(string filePath);
}
