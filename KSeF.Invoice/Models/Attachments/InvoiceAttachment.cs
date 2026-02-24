using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Attachments;

/// <summary>
/// Załącznik do faktury (Zalacznik)
/// Reprezentuje plik dołączony do faktury
/// </summary>
public class InvoiceAttachment
{
    /// <summary>
    /// Nazwa pliku załącznika (NazwaPliku)
    /// Nazwa pliku wraz z rozszerzeniem
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("NazwaPliku")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Opis załącznika (OpisZalacznika)
    /// Tekstowy opis zawartości lub przeznaczenia załącznika
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("OpisZalacznika")]
    public string? Description { get; set; }

    /// <summary>
    /// Typ MIME załącznika (TypMIME)
    /// Określa format pliku (np. "application/pdf", "image/png")
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("TypMIME")]
    public string? MimeType { get; set; }

    /// <summary>
    /// Zawartość załącznika zakodowana w Base64 (DaneZalacznika)
    /// Binarne dane pliku zakodowane jako ciąg Base64
    /// </summary>
    [XmlElement("DaneZalacznika")]
    public string? ContentBase64 { get; set; }

    /// <summary>
    /// Hash pliku (HashZalacznika)
    /// Skrót kryptograficzny zawartości załącznika
    /// Pozwala na weryfikację integralności pliku
    /// </summary>
    [XmlElement("HashZalacznika")]
    public string? FileHash { get; set; }

    /// <summary>
    /// Algorytm użyty do obliczenia hash (AlgorytmHash)
    /// Np. "SHA-256", "SHA-512"
    /// </summary>
    [XmlElement("AlgorytmHash")]
    public string? HashAlgorithm { get; set; }

    #region Właściwości pomocnicze

    /// <summary>
    /// Sprawdza czy załącznik zawiera dane
    /// </summary>
    [XmlIgnore]
    public bool HasContent => !string.IsNullOrEmpty(ContentBase64);

    /// <summary>
    /// Sprawdza czy określono hash załącznika
    /// </summary>
    [XmlIgnore]
    public bool HasHash => !string.IsNullOrEmpty(FileHash);

    /// <summary>
    /// Sprawdza czy określono opis załącznika
    /// </summary>
    [XmlIgnore]
    public bool HasDescription => !string.IsNullOrEmpty(Description);

    /// <summary>
    /// Pobiera rozmiar zawartości w bajtach (przybliżony na podstawie Base64)
    /// </summary>
    [XmlIgnore]
    public long EstimatedSizeInBytes
    {
        get
        {
            if (string.IsNullOrEmpty(ContentBase64))
                return 0;

            // Base64 encoding increases size by approximately 4/3
            return (long)(ContentBase64.Length * 3 / 4);
        }
    }

    #endregion
}
