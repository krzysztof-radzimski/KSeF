using KSeF.Invoice.Services.Builders;
using KSeF.Invoice.Services.Serialization;
using KSeF.Invoice.Services.Validation;
using Microsoft.Extensions.Options;

namespace KSeF.Invoice;

/// <summary>
/// Główny serwis biblioteki KSeF Invoice
/// Stanowi fasadę łączącą funkcjonalności budowania, walidacji i serializacji faktur
/// </summary>
public class KsefInvoiceService : IKsefInvoiceService
{
    private readonly IInvoiceSerializer _serializer;
    private readonly IInvoiceValidator _validator;
    private readonly IXsdValidator _xsdValidator;
    private readonly KsefInvoiceServiceOptions _options;

    /// <summary>
    /// Tworzy nową instancję serwisu KSeF Invoice
    /// </summary>
    /// <param name="serializer">Serwis serializacji faktur</param>
    /// <param name="validator">Walidator biznesowy faktur</param>
    /// <param name="xsdValidator">Walidator XSD faktur</param>
    /// <param name="options">Opcje konfiguracyjne</param>
    public KsefInvoiceService(
        IInvoiceSerializer serializer,
        IInvoiceValidator validator,
        IXsdValidator xsdValidator,
        IOptions<KsefInvoiceServiceOptions> options)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _xsdValidator = xsdValidator ?? throw new ArgumentNullException(nameof(xsdValidator));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public InvoiceBuilder CreateInvoice()
    {
        var builder = InvoiceBuilder.Create();

        if (!string.IsNullOrEmpty(_options.DefaultSystemInfo))
        {
            builder.WithSystemInfo(_options.DefaultSystemInfo);
        }

        return builder;
    }

    /// <inheritdoc />
    public ValidationResult Validate(Models.Invoice invoice)
    {
        if (invoice == null)
        {
            return ValidationResult.WithError("NULL_INVOICE", "Faktura nie może być null");
        }

        // Walidacja biznesowa
        var result = _validator.Validate(invoice);

        // Walidacja XSD (jeśli włączona)
        if (_options.ValidateAgainstXsd)
        {
            var xsdResult = _xsdValidator.Validate(invoice);
            result.Merge(xsdResult);
        }

        return result;
    }

    /// <inheritdoc />
    public string ToXml(Models.Invoice invoice)
    {
        if (invoice == null)
        {
            throw new ArgumentNullException(nameof(invoice));
        }

        ValidateBeforeSerializationIfEnabled(invoice);

        return _serializer.SerializeToXml(invoice);
    }

    /// <inheritdoc />
    public byte[] ToBytes(Models.Invoice invoice)
    {
        if (invoice == null)
        {
            throw new ArgumentNullException(nameof(invoice));
        }

        ValidateBeforeSerializationIfEnabled(invoice);

        return _serializer.SerializeToBytes(invoice);
    }

    /// <inheritdoc />
    public Models.Invoice? FromXml(string xml)
    {
        if (string.IsNullOrEmpty(xml))
        {
            return null;
        }

        return _serializer.DeserializeFromXml(xml);
    }

    /// <inheritdoc />
    public Models.Invoice? FromBytes(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return null;
        }

        return _serializer.DeserializeFromBytes(data);
    }

    /// <summary>
    /// Waliduje fakturę przed serializacją jeśli opcja jest włączona
    /// </summary>
    /// <param name="invoice">Faktura do walidacji</param>
    /// <exception cref="InvalidOperationException">Gdy walidacja się nie powiodła</exception>
    private void ValidateBeforeSerializationIfEnabled(Models.Invoice invoice)
    {
        if (!_options.ValidateBeforeSerialize)
        {
            return;
        }

        var validationResult = Validate(invoice);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(Environment.NewLine,
                validationResult.Errors.Select(e => $"[{e.Code}] {e.Message}" +
                    (e.FieldName != null ? $" (pole: {e.FieldName})" : "")));

            throw new InvalidOperationException(
                $"Faktura nie przeszła walidacji i nie może zostać zserializowana:{Environment.NewLine}{errors}");
        }
    }
}
