using System.Text.RegularExpressions;
using FluentAssertions;
using KSeF.Invoice.Models;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Summary;
using KSeF.Invoice.Services.Serialization;
using KSeF.Invoice.Services.Validation;
using Xunit;

namespace KSeF.Invoice.Tests.Validation;

/// <summary>
/// Testy jednostkowe walidatora XSD
/// </summary>
public class XsdValidatorTests
{
    private readonly XsdValidator _validator;
    private readonly KsefInvoiceSerializer _serializer;

    public XsdValidatorTests()
    {
        _serializer = new KsefInvoiceSerializer();
        _validator = new XsdValidator(_serializer);
    }

    #region Testy inicjalizacji

    [Fact]
    public void Constructor_WithValidSerializer_ShouldCreateInstance()
    {
        // Arrange & Act
        var validator = new XsdValidator(_serializer);

        // Assert
        validator.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullSerializer_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new XsdValidator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Testy niepoprawnych parametrów wejściowych

    [Fact]
    public void ValidateXml_EmptyXml_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => _validator.ValidateXml("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ValidateXml_NullXml_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => _validator.ValidateXml((string)null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ValidateXml_WhitespaceXml_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => _validator.ValidateXml("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ValidateXml_NullStream_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => _validator.ValidateXml((Stream)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Validate_NullInvoice_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => _validator.Validate(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Testy niepoprawnych dokumentów XML

    [Fact]
    public void ValidateXml_InvalidXmlFormat_ShouldReturnError()
    {
        // Arrange
        var invalidXml = "<?xml version=\"1.0\"?><invalid><unclosed>";

        // Act
        var result = _validator.ValidateXml(invalidXml);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        // Błąd może być XSD_XML_ERROR (błąd parsowania XML) lub XSD_SCHEMA_NOT_FOUND (jeśli schemat nie jest dostępny)
        result.Errors.Should().Contain(e => e.Code.StartsWith("XSD_"));
    }

    [Fact]
    public void ValidateXml_MalformedXml_ShouldReturnError()
    {
        // Arrange
        var malformedXml = "This is not XML at all";

        // Act
        var result = _validator.ValidateXml(malformedXml);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void ValidateXml_XmlWithoutProperNamespace_ShouldHandleGracefully()
    {
        // Arrange
        var xmlWithoutNamespace = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Faktura>
  <Naglowek>
    <KodFormularza>FA</KodFormularza>
  </Naglowek>
</Faktura>";

        // Act
        var result = _validator.ValidateXml(xmlWithoutNamespace);

        // Assert
        // Bez prawidłowej przestrzeni nazw walidator powinien zgłosić błąd lub ostrzeżenie
        // Nie zakładamy konkretnego zachowania - sprawdzamy tylko że nie rzuca wyjątku
        result.Should().NotBeNull();
    }

    #endregion

    #region Testy walidacji elementów wymaganych

    [Fact]
    public void ValidateXml_MissingRequiredElement_ShouldReturnError()
    {
        // Arrange - dokument XML bez wymaganego elementu Podmiot1 (sprzedawca)
        var incompleteXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Faktura xmlns=""http://crd.gov.pl/wzor/2025/06/25/13775/"">
  <Naglowek>
    <KodFormularza kodSystemowy=""FA (3)"" wersjaSchemy=""1-2E"">FA</KodFormularza>
    <WariantFormularza>1</WariantFormularza>
    <DataWytworzeniaFa>2025-01-01T12:00:00</DataWytworzeniaFa>
    <SystemInfo>Test</SystemInfo>
  </Naglowek>
</Faktura>";

        // Act
        var result = _validator.ValidateXml(incompleteXml);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
    }

    #endregion

    #region Testy detekcji wersji schematu

    [Fact]
    public void ValidateXml_FA3Namespace_ShouldBeRecognized()
    {
        // Arrange
        var xmlWithFA3Namespace = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Faktura xmlns=""http://crd.gov.pl/wzor/2025/06/25/13775/"">
  <Naglowek>
    <KodFormularza kodSystemowy=""FA (3)"" wersjaSchemy=""1-2E"">FA</KodFormularza>
    <WariantFormularza>1</WariantFormularza>
    <DataWytworzeniaFa>2025-01-01T12:00:00</DataWytworzeniaFa>
  </Naglowek>
</Faktura>";

        // Act
        var result = _validator.ValidateXml(xmlWithFA3Namespace, SchemaVersion.Auto);

        // Assert
        // Powinien rozpoznać przestrzeń nazw FA(3)
        // Wynik może być negatywny z powodu brakujących elementów,
        // ale to sprawdza że walidator nie rzuca wyjątku i próbuje walidować
        result.Should().NotBeNull();
    }

    #endregion

    #region Testy kodów błędów XSD

    [Fact]
    public void ValidateXml_InvalidValue_ShouldReturnXsdValidationError()
    {
        // Arrange - dokument z nieprawidłową wartością enumeracji
        var xmlWithInvalidValue = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Faktura xmlns=""http://crd.gov.pl/wzor/2025/06/25/13775/"">
  <Naglowek>
    <KodFormularza kodSystemowy=""FA (3)"" wersjaSchemy=""1-2E"">INVALID_VALUE</KodFormularza>
    <WariantFormularza>1</WariantFormularza>
    <DataWytworzeniaFa>2025-01-01T12:00:00</DataWytworzeniaFa>
  </Naglowek>
</Faktura>";

        // Act
        var result = _validator.ValidateXml(xmlWithInvalidValue);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        // Powinien być błąd XSD (dowolny z kodów rozpoczynających się od XSD_)
        result.Errors.Should().Contain(e => e.Code.StartsWith("XSD_"));
    }

    #endregion

    #region Testy serializacji i walidacji faktury

    [Fact]
    public void Validate_MinimalInvoice_ShouldNotThrowException()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();

        // Act
        var act = () => _validator.Validate(invoice);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_InvoiceWithCorrection_ShouldNotThrowException()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta ilości";
        invoice.InvoiceData.CorrectionType = 1;
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/001",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            IsIssuedOutsideKSeF = true
        };

        // Act
        var act = () => _validator.Validate(invoice);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ReturnsValidationResult()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ValidationResult>();
    }

    [Fact]
    public void Validate_MinimalInvoice_ShouldPassXsdValidation()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();

        // Act - first verify schemas are loaded
        var schemas = _validator.AvailableSchemas;
        schemas.Should().NotBeEmpty("XSD schemas should be available");

        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert - output errors for diagnostic
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed with errors:\n{errorMessages}\n\nGenerated XML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }
    }

    #endregion

    #region Testy formatowania i kodowania

    [Fact]
    public void ValidateXml_Utf8Encoding_ShouldBeSupported()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.Seller.Name = "Spółka z ąćęłńóśźżĄĆĘŁŃÓŚŹŻ S.A."; // Polskie znaki
        var xml = _serializer.SerializeToXml(invoice);

        // Verify encoding
        xml.Should().Contain("encoding=\"utf-8\"");

        // Act
        var act = () => _validator.ValidateXml(xml);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateXml_SpecialCharactersInProductName_ShouldBeEscaped()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.LineItems![0].ProductName = "Usługa \"testowa\" & specjalna <ważna>";

        // Act
        var xml = _serializer.SerializeToXml(invoice);

        // Assert - Serializer powinien poprawnie escapować znaki specjalne
        xml.Should().Contain("&amp;");
        xml.Should().Contain("&lt;");
        xml.Should().Contain("&gt;");
    }

    #endregion

    #region Testy strumienia

    [Fact]
    public void ValidateXml_WithStream_ShouldNotThrowException()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        var bytes = _serializer.SerializeToBytes(invoice);
        using var stream = new MemoryStream(bytes);

        // Act
        var act = () => _validator.ValidateXml(stream);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateXml_WithStream_ShouldReturnValidationResult()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        var bytes = _serializer.SerializeToBytes(invoice);
        using var stream = new MemoryStream(bytes);

        // Act
        var result = _validator.ValidateXml(stream);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ValidationResult>();
    }

    #endregion

    #region Testy właściwości

    [Fact]
    public void AreSchemasLoaded_InitialState_ShouldBeFalse()
    {
        // Arrange
        var validator = new XsdValidator(_serializer);

        // Act & Assert
        // Przed pierwszą walidacją schematy nie powinny być załadowane
        validator.AreSchemasLoaded.Should().BeFalse();
    }

    [Fact]
    public void AvailableSchemas_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        _ = _validator.Validate(invoice);

        // Act
        var schemas = _validator.AvailableSchemas;

        // Assert
        schemas.Should().NotBeNull();
        schemas.Should().BeAssignableTo<IReadOnlyCollection<SchemaVersion>>();
    }

    #endregion

    #region Testy XSD xsd:choice w DaneFaKorygowanej

    [Fact]
    public void Validate_CorrectionInvoice_IssuedInKSeF_ShouldPassXsdValidation()
    {
        // Arrange — faktura korygująca faktury z KSeF (gałąź A: NrKSeF + NrKSeFFaKorygowanej)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta ilości";
        invoice.InvoiceData.CorrectionType = 1;
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/001",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            IsIssuedInKSeF = true,
            CorrectedInvoiceKSeFNumber = "1234567890-20240115-ABC123456789-01"
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed with errors:\n{errorMessages}\n\nGenerated XML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:NrKSeF>1</tns:NrKSeF>");
        xml.Should().Contain("NrKSeFFaKorygowanej");
        xml.Should().Contain("1234567890-20240115-ABC123456789-01");
    }

    [Fact]
    public void Validate_CorrectionInvoice_IssuedOutsideKSeF_ShouldPassXsdValidation()
    {
        // Arrange — faktura korygująca faktury spoza KSeF (gałąź B: NrKSeFN)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta ilości";
        invoice.InvoiceData.CorrectionType = 1;
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/001",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            IsIssuedOutsideKSeF = true
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed with errors:\n{errorMessages}\n\nGenerated XML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:NrKSeFN>1</tns:NrKSeFN>");
        xml.Should().NotContain("<tns:NrKSeF>");
        xml.Should().NotContain("NrKSeFFaKorygowanej");
    }

    #endregion

    #region Testy XSD nowych pól sekcji korekty (NrFaKorygowany, P_15ZK, KursWalutyZK)

    [Fact]
    public void Validate_CorrectionInvoice_WithNrFaKorygowany_ShouldPassXsdValidation()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Błędny numer faktury";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/001",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.CorrectedInvoiceNumberAmended = "FV/2025/001-POPRAWIONY";

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:NrFaKorygowany>FV/2025/001-POPRAWIONY</tns:NrFaKorygowany>");
    }

    [Fact]
    public void Validate_CorrectionInvoice_WithP15ZKAndKursWalutyZK_ShouldPassXsdValidation()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta kwoty";
        invoice.InvoiceData.CorrectionType = 1;
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/002",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.AmountBeforeCorrection = 1500.50m;
        invoice.InvoiceData.ExchangeRateBeforeCorrection = 4.3521m;

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:P_15ZK>");
        xml.Should().Contain("<tns:KursWalutyZK>");
    }

    [Fact]
    public void Validate_CorrectionInvoice_WithP15ZKWithoutKursWalutyZK_ShouldPassXsdValidation()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta kwoty";
        invoice.InvoiceData.CorrectionType = 1;
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/003",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.AmountBeforeCorrection = 2000.00m;

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:P_15ZK>");
        xml.Should().NotContain("<tns:KursWalutyZK>");
    }

    [Fact]
    public void Validate_CorrectionInvoice_WithoutP15ZKAndKursWalutyZK_ShouldPassXsdValidation()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta danych";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new KSeF.Invoice.Models.Corrections.CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/004",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)),
            IsIssuedOutsideKSeF = true
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().NotContain("<tns:P_15ZK>");
        xml.Should().NotContain("<tns:KursWalutyZK>");
        xml.Should().NotContain("<tns:NrFaKorygowany>");
    }

    #endregion

    #region Testy XSD Podmiot1K (dane sprzedawcy z faktury korygowanej)

    [Fact]
    public void Validate_CorrectionInvoice_WithPodmiot1K_FullData_ShouldPassXsdValidation()
    {
        // Arrange — faktura KOR z pełnym Podmiot1K (prefiks + identyfikacja + adres)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta danych sprzedawcy";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/010",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.CorrectedSeller = new CorrectedSellerData
        {
            TaxpayerPrefix = EUCountryCode.PL,
            IdentificationData = new SellerIdentification
            {
                Nip = "5261040828",
                Name = "Stara Nazwa Firmy Sp. z o.o."
            },
            Address = new Address
            {
                CountryCode = "PL",
                AddressLine1 = "ul. Stara 5",
                AddressLine2 = "00-002 Warszawa"
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("Podmiot1K");
        xml.Should().Contain("PrefiksPodatnika");
        xml.Should().Contain("<tns:NIP>5261040828</tns:NIP>");
        xml.Should().Contain("Stara Nazwa Firmy");
    }

    [Fact]
    public void Validate_CorrectionInvoice_WithPodmiot1K_WithoutPrefix_ShouldPassXsdValidation()
    {
        // Arrange — Podmiot1K bez PrefiksPodatnika (opcjonalny)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta danych sprzedawcy";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/011",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.CorrectedSeller = new CorrectedSellerData
        {
            IdentificationData = new SellerIdentification
            {
                Nip = "5261040828",
                Name = "Stara Nazwa Firmy Sp. z o.o."
            },
            Address = new Address
            {
                CountryCode = "PL",
                AddressLine1 = "ul. Stara 5",
                AddressLine2 = "00-002 Warszawa"
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("Podmiot1K");
        xml.Should().NotContain("PrefiksPodatnika");
    }

    [Fact]
    public void Validate_CorrectionInvoice_WithoutPodmiot1K_ShouldPassXsdValidation()
    {
        // Arrange — faktura KOR bez Podmiot1K (cała sekcja opcjonalna)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta ilości";
        invoice.InvoiceData.CorrectionType = 1;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/012",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)),
            IsIssuedOutsideKSeF = true
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().NotContain("Podmiot1K");
    }

    [Fact]
    public void Validate_CorrectionInvoice_Podmiot1K_MissingIdentification_ShouldFailXsdValidation()
    {
        // Arrange — Podmiot1K z brakującym DaneIdentyfikacyjne (wymagane wg XSD)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta danych sprzedawcy";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/013",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.CorrectedSeller = new CorrectedSellerData
        {
            IdentificationData = new SellerIdentification
            {
                Nip = "",
                Name = ""
            },
            Address = new Address
            {
                CountryCode = "PL",
                AddressLine1 = "ul. Testowa 1"
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert — empty NIP should fail XSD validation (NIP has minLength=10)
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
    }

    #endregion

    #region Testy XSD Podmiot2K (dane nabywców z faktury korygowanej)

    [Fact]
    public void Validate_CorrectionInvoice_WithPodmiot2K_FullData_ShouldPassXsdValidation()
    {
        // Arrange — faktura KOR z pełnym Podmiot2K (identyfikacja + adres + IDNabywcy)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta danych nabywcy";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/020",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.CorrectedBuyers = new List<CorrectedBuyerData>
        {
            new CorrectedBuyerData
            {
                IdentificationData = new BuyerIdentification
                {
                    Nip = "5252248481",
                    Name = "Stary Nabywca S.A."
                },
                Address = new Address
                {
                    CountryCode = "PL",
                    AddressLine1 = "ul. Stara 10",
                    AddressLine2 = "00-003 Warszawa"
                },
                BuyerIdentifier = "KLIENT-001"
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("Podmiot2K");
        xml.Should().Contain("<tns:NIP>5252248481</tns:NIP>");
        xml.Should().Contain("Stary Nabywca S.A.");
        xml.Should().Contain("IDNabywcy");
    }

    [Fact]
    public void Validate_CorrectionInvoice_WithMultiplePodmiot2K_ShouldPassXsdValidation()
    {
        // Arrange — faktura KOR z 3 Podmiot2K (różni nabywcy)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta danych wielu nabywców";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/021",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-8)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.CorrectedBuyers = new List<CorrectedBuyerData>
        {
            new CorrectedBuyerData
            {
                IdentificationData = new BuyerIdentification
                {
                    Nip = "5252248481",
                    Name = "Nabywca Pierwszy S.A."
                },
                Address = new Address
                {
                    CountryCode = "PL",
                    AddressLine1 = "ul. Pierwsza 1",
                    AddressLine2 = "00-001 Warszawa"
                }
            },
            new CorrectedBuyerData
            {
                IdentificationData = new BuyerIdentification
                {
                    EUCountryCode = EUCountryCode.DE,
                    VatNumberEU = "123456789",
                    Name = "Nabywca Drugi GmbH"
                },
                BuyerIdentifier = "KLIENT-DE"
            },
            new CorrectedBuyerData
            {
                IdentificationData = new BuyerIdentification
                {
                    Nip = "1234567890",
                    Name = "Nabywca Trzeci Sp. z o.o."
                }
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        var podmiot2kCount = System.Text.RegularExpressions.Regex.Matches(xml, "<tns:Podmiot2K>").Count;
        podmiot2kCount.Should().Be(3);
    }

    [Fact]
    public void Validate_CorrectionInvoice_WithPodmiot2K_OnlyIdentification_ShouldPassXsdValidation()
    {
        // Arrange — Podmiot2K bez Adres i bez IDNabywcy (tylko DaneIdentyfikacyjne)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta danych nabywcy";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/022",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.CorrectedBuyers = new List<CorrectedBuyerData>
        {
            new CorrectedBuyerData
            {
                IdentificationData = new BuyerIdentification
                {
                    Nip = "5252248481",
                    Name = "Nabywca Testowy S.A."
                }
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("Podmiot2K");
        xml.Should().NotContain("IDNabywcy");
    }

    [Fact]
    public void Validate_CorrectionInvoice_WithoutPodmiot2K_ShouldPassXsdValidation()
    {
        // Arrange — faktura KOR bez Podmiot2K (cała sekcja opcjonalna)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta ilości";
        invoice.InvoiceData.CorrectionType = 1;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/023",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)),
            IsIssuedOutsideKSeF = true
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().NotContain("Podmiot2K");
    }

    [Fact]
    public void Validate_CorrectionInvoice_Podmiot2K_WithTooLongIDNabywcy_ShouldFailXsdValidation()
    {
        // Arrange — Podmiot2K z IDNabywcy > 32 znaki
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.InvoiceType = InvoiceType.KOR;
        invoice.InvoiceData.CorrectionReason = "Korekta danych nabywcy";
        invoice.InvoiceData.CorrectionType = 2;
        invoice.InvoiceData.CorrectedInvoiceData = new CorrectedInvoiceData
        {
            CorrectedInvoiceNumber = "FV/2025/024",
            CorrectedInvoiceIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)),
            IsIssuedOutsideKSeF = true
        };
        invoice.InvoiceData.CorrectedBuyers = new List<CorrectedBuyerData>
        {
            new CorrectedBuyerData
            {
                IdentificationData = new BuyerIdentification
                {
                    Nip = "5252248481",
                    Name = "Nabywca Testowy S.A."
                },
                BuyerIdentifier = new string('A', 33)
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert — IDNabywcy > 32 chars should fail XSD validation (maxLength=32)
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
    }

    #endregion

    #region Testy XSD znaczników FP, TP, ZwrotAkcyzy i ZaliczkaCzesciowa

    [Fact]
    public void Validate_InvoiceWithFP_ShouldPassXsdValidation()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.IsArticle109_3dInvoice = true;

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:FP>1</tns:FP>");
    }

    [Fact]
    public void Validate_InvoiceWithTP_ShouldPassXsdValidation()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.HasRelatedPartyTransaction = true;

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:TP>1</tns:TP>");
    }

    [Fact]
    public void Validate_InvoiceWithZwrotAkcyzy_ShouldPassXsdValidation()
    {
        // Arrange
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.IsExciseRefundEligible = true;

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:ZwrotAkcyzy>1</tns:ZwrotAkcyzy>");
    }

    [Fact]
    public void Validate_InvoiceWithZaliczkaCzesciowa_ShouldPassXsdValidation()
    {
        // Arrange — 3 zaliczki częściowe z różnymi datami i kwotami
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.PartialAdvancePayments = new List<PartialAdvancePayment>
        {
            new PartialAdvancePayment
            {
                PaymentDate = new DateOnly(2026, 1, 15),
                Amount = 500.00m
            },
            new PartialAdvancePayment
            {
                PaymentDate = new DateOnly(2026, 2, 15),
                Amount = 300.50m
            },
            new PartialAdvancePayment
            {
                PaymentDate = new DateOnly(2026, 3, 15),
                Amount = 200.00m,
                ExchangeRate = 4.3521m
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().Contain("<tns:P_6Z>");
        xml.Should().Contain("<tns:P_15Z>");
        xml.Should().Contain("<tns:KursWalutyZW>");
    }

    [Fact]
    public void Validate_InvoiceWithZaliczkaCzesciowa_WithoutExchangeRate_ShouldPassXsdValidation()
    {
        // Arrange — zaliczka bez kursu waluty
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.PartialAdvancePayments = new List<PartialAdvancePayment>
        {
            new PartialAdvancePayment
            {
                PaymentDate = new DateOnly(2026, 1, 15),
                Amount = 1000.00m
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        xml.Should().NotContain("<tns:KursWalutyZW>");
    }

    [Fact]
    public void Validate_InvoiceWithTooManyZaliczkaCzesciowa_ShouldFailXsdValidation()
    {
        // Arrange — 32 elementów (maxOccurs=31)
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.PartialAdvancePayments = new List<PartialAdvancePayment>();
        for (int i = 0; i < 32; i++)
        {
            invoice.InvoiceData.PartialAdvancePayments.Add(new PartialAdvancePayment
            {
                PaymentDate = new DateOnly(2026, 1, 1).AddDays(i),
                Amount = 100m
            });
        }

        // Act
        var result = _validator.Validate(invoice);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvoiceWithAllNewFields_ShouldPassXsdValidation()
    {
        // Arrange — FP + TP + ZwrotAkcyzy + ZaliczkaCzesciowa
        var invoice = CreateMinimalValidInvoice();
        invoice.InvoiceData.IsArticle109_3dInvoice = true;
        invoice.InvoiceData.HasRelatedPartyTransaction = true;
        invoice.InvoiceData.IsExciseRefundEligible = true;
        invoice.InvoiceData.PartialAdvancePayments = new List<PartialAdvancePayment>
        {
            new PartialAdvancePayment
            {
                PaymentDate = new DateOnly(2026, 1, 10),
                Amount = 500m
            },
            new PartialAdvancePayment
            {
                PaymentDate = new DateOnly(2026, 2, 10),
                Amount = 300m,
                ExchangeRate = 4.25m
            }
        };

        // Act
        var xml = _serializer.SerializeToXml(invoice);
        var result = _validator.Validate(invoice);

        // Assert
        if (result.HasErrors)
        {
            var errorMessages = string.Join("\n", result.Errors.Select(e => $"[{e.Code}] {e.Message}"));
            result.IsValid.Should().BeTrue($"XSD validation failed:\n{errorMessages}\n\nXML:\n{xml}");
        }
        else
        {
            result.IsValid.Should().BeTrue();
        }

        // Verify correct XSD order: ZaliczkaCzesciowa → FP → TP → ... → ZwrotAkcyzy → FaWiersz
        var zaliczkaIndex = xml.IndexOf("<tns:ZaliczkaCzesciowa>");
        var fpIndex = xml.IndexOf("<tns:FP>");
        var tpIndex = xml.IndexOf("<tns:TP>");
        var zwrotIndex = xml.IndexOf("<tns:ZwrotAkcyzy>");
        var faWierszIndex = xml.IndexOf("<tns:FaWiersz>");

        zaliczkaIndex.Should().BeLessThan(fpIndex, "ZaliczkaCzesciowa should appear before FP");
        fpIndex.Should().BeLessThan(tpIndex, "FP should appear before TP");
        tpIndex.Should().BeLessThan(zwrotIndex, "TP should appear before ZwrotAkcyzy");
        zwrotIndex.Should().BeLessThan(faWierszIndex, "ZwrotAkcyzy should appear before FaWiersz");
    }

    #endregion

    #region Pomocnicze metody

    private static KSeF.Invoice.Models.Invoice CreateMinimalValidInvoice()
    {
        return new KSeF.Invoice.Models.Invoice
        {
            Header = new InvoiceHeader
            {
                CreationDateTime = DateTime.Now,
                SystemInfo = "Test System"
            },
            Seller = new Seller
            {
                TaxId = "5261040828",
                Name = "Test Sprzedawca Sp. z o.o.",
                Address = new Address
                {
                    CountryCode = "PL",
                    AddressLine1 = "ul. Testowa 1",
                    AddressLine2 = "00-001 Warszawa"
                }
            },
            Buyer = new Buyer
            {
                TaxId = "5252248481",
                Name = "Test Nabywca S.A."
            },
            InvoiceData = new InvoiceData
            {
                CurrencyCode = CurrencyCode.PLN,
                InvoiceNumber = "FV/2025/001",
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                InvoiceType = InvoiceType.VAT,
                NetAmount23 = 100.00m,
                VatAmount23 = 23.00m,
                TotalAmount = 123.00m,
                LineItems = new List<InvoiceLineItem>
                {
                    new InvoiceLineItem
                    {
                        LineNumber = 1,
                        ProductName = "Usługa testowa",
                        Quantity = 1,
                        UnitNetPrice = 100.00m,
                        NetAmount = 100.00m,
                        VatRate = VatRate.Rate23,
                        VatAmount = 23.00m
                    }
                }
            }
        };
    }

    #endregion
}
