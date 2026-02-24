using System.Xml.Serialization;

namespace KSeF.Invoice.Models.Attachments;

/// <summary>
/// Sekcja załączników do faktury (Zalacznik)
/// Strukturalne dane dodatkowe dołączone do faktury
/// Zgodna ze schematem KSeF FA(3)
/// </summary>
[XmlType("Zalacznik")]
public class InvoiceAttachmentSection
{
    /// <summary>
    /// Lista bloków danych załącznika (BlokDanych)
    /// Szczegółowe dane załącznika - może zawierać do 1000 bloków
    /// </summary>
    [XmlElement("BlokDanych")]
    public List<AttachmentDataBlock>? DataBlocks { get; set; }

    /// <summary>
    /// Sprawdza czy sekcja zawiera dane
    /// </summary>
    [XmlIgnore]
    public bool HasData => DataBlocks != null && DataBlocks.Count > 0;
}

/// <summary>
/// Blok danych załącznika (BlokDanych)
/// Szczegółowe dane załącznika zawierające metadane, tekst i tabele
/// </summary>
[XmlType("BlokDanych")]
public class AttachmentDataBlock
{
    /// <summary>
    /// Nagłówek bloku danych (ZNaglowek)
    /// Opis nagłówka bloku
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("ZNaglowek")]
    public string? Header { get; set; }

    /// <summary>
    /// Metadane - lista par klucz-wartość (MetaDane)
    /// Dane opisowe bloku - do 1000 wpisów
    /// </summary>
    [XmlElement("MetaDane")]
    public List<AttachmentMetadata>? Metadata { get; set; }

    /// <summary>
    /// Część tekstowa bloku danych (Tekst)
    /// </summary>
    [XmlElement("Tekst")]
    public AttachmentText? Text { get; set; }

    /// <summary>
    /// Lista tabel (Tabela)
    /// Dane tabelaryczne - do 1000 tabel
    /// </summary>
    [XmlElement("Tabela")]
    public List<AttachmentTable>? Tables { get; set; }

    /// <summary>
    /// Sprawdza czy blok zawiera metadane
    /// </summary>
    [XmlIgnore]
    public bool HasMetadata => Metadata != null && Metadata.Count > 0;

    /// <summary>
    /// Sprawdza czy blok zawiera tekst
    /// </summary>
    [XmlIgnore]
    public bool HasText => Text != null && Text.Paragraphs != null && Text.Paragraphs.Count > 0;

    /// <summary>
    /// Sprawdza czy blok zawiera tabele
    /// </summary>
    [XmlIgnore]
    public bool HasTables => Tables != null && Tables.Count > 0;
}

/// <summary>
/// Metadane załącznika - para klucz-wartość (MetaDane)
/// </summary>
[XmlType("MetaDane")]
public class AttachmentMetadata
{
    /// <summary>
    /// Klucz metadanych (ZKlucz)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("ZKlucz")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Wartość metadanych (ZWartosc)
    /// Maksymalnie 256 znaków
    /// </summary>
    [XmlElement("ZWartosc")]
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Część tekstowa bloku danych załącznika (Tekst)
/// </summary>
[XmlType("Tekst")]
public class AttachmentText
{
    /// <summary>
    /// Lista akapitów tekstu (Akapit)
    /// Maksymalnie 10 akapitów, każdy do 512 znaków
    /// </summary>
    [XmlElement("Akapit")]
    public List<string>? Paragraphs { get; set; }
}

/// <summary>
/// Tabela w załączniku (Tabela)
/// </summary>
[XmlType("Tabela")]
public class AttachmentTable
{
    /// <summary>
    /// Metadane tabeli (TMetaDane)
    /// Dane opisowe tabeli - do 1000 wpisów
    /// </summary>
    [XmlElement("TMetaDane")]
    public List<TableMetadata>? Metadata { get; set; }

    /// <summary>
    /// Opis tabeli (Opis)
    /// Maksymalnie 512 znaków
    /// </summary>
    [XmlElement("Opis")]
    public string? Description { get; set; }

    /// <summary>
    /// Nagłówek tabeli (TNaglowek)
    /// Definiuje kolumny tabeli
    /// </summary>
    [XmlElement("TNaglowek")]
    public TableHeader? Header { get; set; }

    /// <summary>
    /// Wiersze tabeli (Wiersz)
    /// Dane wierszy - do 1000 wierszy
    /// </summary>
    [XmlElement("Wiersz")]
    public List<TableRow>? Rows { get; set; }

    /// <summary>
    /// Suma/podsumowanie tabeli (Suma)
    /// </summary>
    [XmlElement("Suma")]
    public TableSummary? Summary { get; set; }

    /// <summary>
    /// Sprawdza czy tabela ma nagłówek
    /// </summary>
    [XmlIgnore]
    public bool HasHeader => Header != null && Header.Columns != null && Header.Columns.Count > 0;

    /// <summary>
    /// Sprawdza czy tabela ma wiersze
    /// </summary>
    [XmlIgnore]
    public bool HasRows => Rows != null && Rows.Count > 0;
}

/// <summary>
/// Metadane tabeli - para klucz-wartość (TMetaDane)
/// </summary>
[XmlType("TMetaDane")]
public class TableMetadata
{
    /// <summary>
    /// Klucz (TKlucz)
    /// </summary>
    [XmlElement("TKlucz")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Wartość (TWartosc)
    /// </summary>
    [XmlElement("TWartosc")]
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Nagłówek tabeli (TNaglowek)
/// </summary>
[XmlType("TNaglowek")]
public class TableHeader
{
    /// <summary>
    /// Lista kolumn (Kol)
    /// Definicje kolumn - maksymalnie 20
    /// </summary>
    [XmlElement("Kol")]
    public List<TableColumn>? Columns { get; set; }
}

/// <summary>
/// Kolumna tabeli (Kol)
/// </summary>
[XmlType("Kol")]
public class TableColumn
{
    /// <summary>
    /// Nazwa kolumny (NKom)
    /// Zawartość pola nagłówka
    /// </summary>
    [XmlElement("NKom")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Typ kolumny (Typ)
    /// Możliwe wartości: date, datetime, dec, int, time, txt
    /// </summary>
    [XmlAttribute("Typ")]
    public string Type { get; set; } = "txt";
}

/// <summary>
/// Wiersz tabeli (Wiersz)
/// </summary>
[XmlType("Wiersz")]
public class TableRow
{
    /// <summary>
    /// Lista komórek wiersza (WKom)
    /// Wartości komórek - maksymalnie 20 (odpowiednio do liczby kolumn)
    /// </summary>
    [XmlElement("WKom")]
    public List<string>? Cells { get; set; }
}

/// <summary>
/// Podsumowanie tabeli (Suma)
/// </summary>
[XmlType("Suma")]
public class TableSummary
{
    /// <summary>
    /// Lista komórek podsumowania (SKom)
    /// Wartości podsumowania - maksymalnie 20 (odpowiednio do liczby kolumn)
    /// </summary>
    [XmlElement("SKom")]
    public List<string>? Cells { get; set; }
}
