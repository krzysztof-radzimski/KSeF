/*=====================================================================================

    KSeF.Invoice
    Value object grupujący wszystkie pola sekcji korekty faktury.
    Nie serializuje się bezpośrednio do XML — pola są eksponowane
    jako proxy properties w InvoiceData z atrybutami [XmlElement].

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;

namespace KSeF.Invoice.Models.Corrections;

public class InvoiceCorrectionSection
{
    public string? CorrectionReason { get; set; }

    public int? CorrectionType { get; set; }

    public CorrectedInvoiceData? CorrectedInvoiceData { get; set; }

    public SalePeriod? CorrectedInvoicePeriod { get; set; }

    public string? CorrectedInvoiceNumberAmended { get; set; }

    public decimal? AmountBeforeCorrection { get; set; }

    public decimal? ExchangeRateBeforeCorrection { get; set; }

    public bool HasAnyData =>
        !string.IsNullOrEmpty(CorrectionReason) ||
        CorrectionType.HasValue ||
        CorrectedInvoiceData != null ||
        CorrectedInvoicePeriod != null ||
        !string.IsNullOrEmpty(CorrectedInvoiceNumberAmended) ||
        AmountBeforeCorrection.HasValue ||
        ExchangeRateBeforeCorrection.HasValue;
}
