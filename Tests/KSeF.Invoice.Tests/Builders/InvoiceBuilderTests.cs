using FluentAssertions;
using KSeF.Invoice.Models;
using Xunit;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Entities;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Models.Payments;
using KSeF.Invoice.Services.Builders;

namespace KSeF.Invoice.Tests.Builders;

/// <summary>
/// Testy jednostkowe dla InvoiceBuilder - głównego fluent API do budowania faktur
/// </summary>
public class InvoiceBuilderTests
{
    #region Create Tests

    [Fact]
    public void Create_ShouldReturnNewInvoiceBuilderInstance()
    {
        // Act
        var builder = InvoiceBuilder.Create();

        // Assert
        builder.Should().NotBeNull();
        builder.Should().BeOfType<InvoiceBuilder>();
    }

    [Fact]
    public void Create_ShouldReturnDifferentInstancesEachTime()
    {
        // Act
        var builder1 = InvoiceBuilder.Create();
        var builder2 = InvoiceBuilder.Create();

        // Assert
        builder1.Should().NotBeSameAs(builder2);
    }

    #endregion

    #region WithSeller Tests

    [Fact]
    public void WithSeller_WithAction_ShouldConfigureSellerCorrectly()
    {
        // Arrange
        var builder = InvoiceBuilder.Create();

        // Act
        var invoice = builder
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Test Seller Sp. z o.o."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Seller.Should().NotBeNull();
        invoice.Seller.TaxId.Should().Be("1234567890");
        invoice.Seller.Name.Should().Be("Test Seller Sp. z o.o.");
    }

    [Fact]
    public void WithSeller_WithObject_ShouldAssignSellerDirectly()
    {
        // Arrange
        var seller = new Seller
        {
            TaxId = "9876543210",
            Name = "Direct Seller Sp. z o.o."
        };

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(seller)
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Seller.Should().BeSameAs(seller);
        invoice.Seller.TaxId.Should().Be("9876543210");
    }

    [Fact]
    public void WithSeller_WithFullAddress_ShouldConfigureAddressCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Test Seller")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Testowa 123")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Seller.Address.Should().NotBeNull();
        invoice.Seller.Address!.CountryCode.Should().Be("PL");
        invoice.Seller.Address.AddressLine1.Should().Be("ul. Testowa 123");
        invoice.Seller.Address.AddressLine2.Should().Be("00-001 Warszawa");
    }

    [Fact]
    public void WithSeller_WithContactData_ShouldConfigureContactCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Test Seller")
                .WithContactData("test@example.com", "+48123456789"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Seller.ContactData.Should().NotBeNull();
        invoice.Seller.ContactData!.Email.Should().Be("test@example.com");
        invoice.Seller.ContactData.Phone.Should().Be("+48123456789");
    }

    #endregion

    #region WithBuyer Tests

    [Fact]
    public void WithBuyer_WithAction_ShouldConfigureBuyerCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Test Buyer Sp. z o.o."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Buyer.Should().NotBeNull();
        invoice.Buyer.TaxId.Should().Be("0987654321");
        invoice.Buyer.Name.Should().Be("Test Buyer Sp. z o.o.");
    }

    [Fact]
    public void WithBuyer_WithObject_ShouldAssignBuyerDirectly()
    {
        // Arrange
        var buyer = new Buyer
        {
            TaxId = "1111111111",
            Name = "Direct Buyer"
        };

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(buyer)
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Buyer.Should().BeSameAs(buyer);
    }

    [Fact]
    public void WithBuyer_WithEuVatId_ShouldConfigureEuBuyerCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b
                .WithEuVatId(EUCountryCode.DE, "123456789")
                .WithName("German Company GmbH"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Buyer.EuCountryCode.Should().Be(EUCountryCode.DE);
        invoice.Buyer.EuVatId.Should().Be("123456789");
    }

    [Fact]
    public void WithBuyer_WithForeignId_ShouldConfigureForeignBuyerCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b
                .WithForeignId("US", "123-45-6789")
                .WithName("US Company Inc."))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Buyer.OtherIdCountryCode.Should().Be("US");
        invoice.Buyer.OtherId.Should().Be("123-45-6789");
    }

    [Fact]
    public void WithBuyer_WithNoIdentifier_ShouldSetNoIdentifierFlag()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b
                .WithNoIdentifier()
                .WithName("Anonymous Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Buyer.NoIdentifier.Should().Be(1);
    }

    [Fact]
    public void WithBuyer_AsLocalGovernmentUnit_ShouldSetJstFlag()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Gmina Testowa")
                .AsLocalGovernmentUnit())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Buyer.IsLocalGovernmentUnit.Should().Be(1);
    }

    [Fact]
    public void WithBuyer_AsVatGroup_ShouldSetVatGroupFlag()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Grupa VAT Test")
                .AsVatGroup())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Buyer.IsVatGroup.Should().Be(1);
    }

    #endregion

    #region AddRecipient Tests

    [Fact]
    public void AddRecipient_WithAction_ShouldAddRecipientToList()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .AddRecipient(r => r
                .WithTaxId("1111111111")
                .WithName("Recipient Company")
                .AsRecipient())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Recipients.Should().NotBeNull();
        invoice.Recipients.Should().HaveCount(1);
        invoice.Recipients![0].TaxId.Should().Be("1111111111");
        invoice.Recipients[0].Name.Should().Be("Recipient Company");
        invoice.Recipients[0].Role.Should().Be(SubjectRole.Recipient);
    }

    [Fact]
    public void AddRecipient_WithObject_ShouldAddRecipientDirectly()
    {
        // Arrange
        var thirdParty = new ThirdParty
        {
            TaxId = "2222222222",
            Name = "Direct Recipient",
            Role = SubjectRole.Factor
        };

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .AddRecipient(thirdParty)
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Recipients.Should().NotBeNull();
        invoice.Recipients.Should().Contain(thirdParty);
    }

    [Fact]
    public void AddRecipient_MultipleRecipients_ShouldAddAllToList()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .AddRecipient(r => r.WithTaxId("1111111111").WithName("Recipient 1").AsRecipient())
            .AddRecipient(r => r.WithTaxId("2222222222").WithName("Recipient 2").AsFactor())
            .AddRecipient(r => r.WithTaxId("3333333333").WithName("Recipient 3").AsPayer())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Recipients.Should().HaveCount(3);
        invoice.HasRecipients.Should().BeTrue();
    }

    [Fact]
    public void AddRecipient_AsFactor_ShouldSetFactorRole()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .AddRecipient(r => r
                .WithTaxId("1111111111")
                .WithName("Faktoring Sp. z o.o.")
                .AsFactor())
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Recipients![0].Role.Should().Be(SubjectRole.Factor);
    }

    [Fact]
    public void AddRecipient_AsAdditionalBuyer_WithSharePercentage_ShouldSetBothValues()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .AddRecipient(r => r
                .WithTaxId("1111111111")
                .WithName("Additional Buyer")
                .AsAdditionalBuyer(50.00m))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        // Assert
        invoice.Recipients![0].Role.Should().Be(SubjectRole.AdditionalBuyer);
        invoice.Recipients[0].SharePercentage.Should().Be(50.00m);
    }

    #endregion

    #region WithInvoiceDetails Tests

    [Fact]
    public void WithInvoiceDetails_WithAction_ShouldConfigureDetailsCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024")
                .WithIssuePlace("Warszawa")
                .WithSaleDate(2024, 1, 14))
            .Build();

        // Assert
        invoice.InvoiceData.Should().NotBeNull();
        invoice.InvoiceData.IssueDate.Should().Be(new DateOnly(2024, 1, 15));
        invoice.InvoiceData.InvoiceNumber.Should().Be("FV/001/2024");
        invoice.InvoiceData.IssuePlace.Should().Be("Warszawa");
        invoice.InvoiceData.SaleDate.Should().Be(new DateOnly(2024, 1, 14));
    }

    [Fact]
    public void WithInvoiceDetails_WithObject_ShouldAssignDetailsDirectly()
    {
        // Arrange
        var invoiceData = new InvoiceData
        {
            IssueDate = new DateOnly(2024, 2, 20),
            InvoiceNumber = "FV/002/2024"
        };

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(invoiceData)
            .Build();

        // Assert
        invoice.InvoiceData.Should().BeSameAs(invoiceData);
    }

    [Fact]
    public void WithInvoiceDetails_WithSalePeriod_ShouldConfigurePeriodCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 31)
                .WithInvoiceNumber("FV/001/2024")
                .WithSalePeriod(2024, 1, 1, 2024, 1, 31))
            .Build();

        // Assert
        invoice.InvoiceData.SalePeriod.Should().NotBeNull();
        invoice.InvoiceData.SalePeriod!.PeriodFrom.Should().Be(new DateOnly(2024, 1, 1));
        invoice.InvoiceData.SalePeriod.PeriodTo.Should().Be(new DateOnly(2024, 1, 31));
    }

    [Fact]
    public void WithInvoiceDetails_WithCurrency_ShouldSetCurrencyCode()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024")
                .WithCurrency(CurrencyCode.EUR))
            .Build();

        // Assert
        invoice.InvoiceData.CurrencyCode.Should().Be(CurrencyCode.EUR);
    }

    [Fact]
    public void WithInvoiceDetails_AsCorrection_ShouldConfigureCorrectionData()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 20)
                .WithInvoiceNumber("FV/001/2024/KOR")
                .AsCorrection("Błędna ilość na fakturze pierwotnej", c => c
                    .WithInvoiceNumber("FV/001/2024")
                    .WithIssueDate(new DateOnly(2024, 1, 15))))
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.KOR);
        invoice.InvoiceData.CorrectionReason.Should().Be("Błędna ilość na fakturze pierwotnej");
        invoice.InvoiceData.CorrectedInvoiceData.Should().NotBeNull();
        invoice.InvoiceData.CorrectedInvoiceData!.CorrectedInvoiceNumber.Should().Be("FV/001/2024");
        invoice.IsCorrection.Should().BeTrue();
    }

    [Fact]
    public void WithInvoiceDetails_AsAdvancePayment_ShouldSetZalType()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/ZAL/001/2024")
                .AsAdvancePayment())
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ZAL);
        invoice.IsAdvancePayment.Should().BeTrue();
    }

    [Fact]
    public void WithInvoiceDetails_AsSettlement_ShouldSetRozType()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/ROZ/001/2024")
                .AsSettlement())
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.ROZ);
        invoice.IsSettlement.Should().BeTrue();
    }

    [Fact]
    public void WithInvoiceDetails_AsSimplified_ShouldSetUprType()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/UPR/001/2024")
                .AsSimplified())
            .Build();

        // Assert
        invoice.InvoiceData.InvoiceType.Should().Be(InvoiceType.UPR);
        invoice.IsSimplified.Should().BeTrue();
    }

    #endregion

    #region AddLineItem Tests

    [Fact]
    public void AddLineItem_WithAction_ShouldAddLineItemAndAutoNumber()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Usługa konsultingowa")
                .WithQuantity(10)
                .WithUnit("godz.")
                .WithUnitNetPrice(100.00m)
                .WithNetAmount(1000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(230.00m))
            .Build();

        // Assert
        invoice.InvoiceData.LineItems.Should().NotBeNull();
        invoice.InvoiceData.LineItems.Should().HaveCount(1);
        invoice.InvoiceData.LineItems![0].LineNumber.Should().Be(1);
        invoice.InvoiceData.LineItems[0].ProductName.Should().Be("Usługa konsultingowa");
    }

    [Fact]
    public void AddLineItem_WithObject_ShouldAutoNumberLineItem()
    {
        // Arrange
        var lineItem = new InvoiceLineItem
        {
            ProductName = "Direct Line Item",
            Quantity = 5,
            UnitNetPrice = 50.00m,
            NetAmount = 250.00m,
            VatRate = VatRate.Rate8,
            VatAmount = 20.00m
        };

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(lineItem)
            .Build();

        // Assert
        invoice.InvoiceData.LineItems![0].LineNumber.Should().Be(1);
    }

    [Fact]
    public void AddLineItem_MultipleItems_ShouldAutoNumberSequentially()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l.WithProductName("Item 1").WithNetAmount(100).WithVatRate(VatRate.Rate23).WithVatAmount(23))
            .AddLineItem(l => l.WithProductName("Item 2").WithNetAmount(200).WithVatRate(VatRate.Rate23).WithVatAmount(46))
            .AddLineItem(l => l.WithProductName("Item 3").WithNetAmount(300).WithVatRate(VatRate.Rate23).WithVatAmount(69))
            .Build();

        // Assert
        invoice.InvoiceData.LineItems.Should().HaveCount(3);
        invoice.InvoiceData.LineItems![0].LineNumber.Should().Be(1);
        invoice.InvoiceData.LineItems[1].LineNumber.Should().Be(2);
        invoice.InvoiceData.LineItems[2].LineNumber.Should().Be(3);
    }

    [Fact]
    public void AddLineItems_WithEnumerable_ShouldAddAllItems()
    {
        // Arrange
        var itemConfigs = new List<Action<LineItemBuilder>>
        {
            l => l.WithProductName("Item A").WithNetAmount(100).WithVatRate(VatRate.Rate23).WithVatAmount(23),
            l => l.WithProductName("Item B").WithNetAmount(200).WithVatRate(VatRate.Rate23).WithVatAmount(46),
            l => l.WithProductName("Item C").WithNetAmount(300).WithVatRate(VatRate.Rate23).WithVatAmount(69)
        };

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItems(itemConfigs)
            .Build();

        // Assert
        invoice.InvoiceData.LineItems.Should().HaveCount(3);
        invoice.InvoiceData.LineItems![0].ProductName.Should().Be("Item A");
        invoice.InvoiceData.LineItems[1].ProductName.Should().Be("Item B");
        invoice.InvoiceData.LineItems[2].ProductName.Should().Be("Item C");
    }

    [Fact]
    public void AddLineItem_WithCalculateAmounts_ShouldCalculateVatCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Calculated Item")
                .WithQuantity(10)
                .WithUnitNetPrice(100.00m)
                .WithVatRate(VatRate.Rate23)
                .CalculateAmounts())
            .Build();

        // Assert
        var lineItem = invoice.InvoiceData.LineItems![0];
        lineItem.NetAmount.Should().Be(1000.00m);
        lineItem.VatAmount.Should().Be(230.00m);
    }

    [Fact]
    public void AddLineItem_WithDiscount_ShouldCalculateNetAmountWithDiscount()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Discounted Item")
                .WithQuantity(10)
                .WithUnitNetPrice(100.00m)
                .WithDiscount(100.00m)
                .WithVatRate(VatRate.Rate23)
                .CalculateAmounts())
            .Build();

        // Assert
        var lineItem = invoice.InvoiceData.LineItems![0];
        lineItem.NetAmount.Should().Be(900.00m); // 1000 - 100 discount
        lineItem.VatAmount.Should().Be(207.00m); // 900 * 0.23
    }

    #endregion

    #region WithPayment Tests

    [Fact]
    public void WithPayment_WithAction_ShouldConfigurePaymentCorrectly()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l.WithProductName("Item").WithNetAmount(100).WithVatRate(VatRate.Rate23).WithVatAmount(23))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 1, 30)
                .AddPaymentMethod(PaymentMethod.BankTransfer)
                .AddBankAccount("PL12345678901234567890123456"))
            .Build();

        // Assert
        invoice.InvoiceData.Payment.Should().NotBeNull();
        invoice.InvoiceData.Payment!.PaymentTerms.Should().HaveCount(1);
        invoice.InvoiceData.Payment.PaymentTerms![0].DueDate.Should().Be(new DateOnly(2024, 1, 30));
        invoice.InvoiceData.Payment.PaymentMethods.Should().HaveCount(1);
        invoice.InvoiceData.Payment.BankAccounts.Should().HaveCount(1);
    }

    [Fact]
    public void WithPayment_WithObject_ShouldAssignPaymentDirectly()
    {
        // Arrange
        var payment = new Payment
        {
            PaymentTerms = new List<PaymentTerm>
            {
                new() { DueDate = new DateOnly(2024, 2, 15) }
            },
            PaymentMethods = new List<PaymentMethodInfo>
            {
                new() { Method = PaymentMethod.Cash }
            }
        };

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l.WithProductName("Item").WithNetAmount(100).WithVatRate(VatRate.Rate23).WithVatAmount(23))
            .WithPayment(payment)
            .Build();

        // Assert
        invoice.InvoiceData.Payment.Should().BeSameAs(payment);
    }

    [Fact]
    public void WithPayment_AsBankTransfer_ShouldConfigureBankPayment()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l.WithProductName("Item").WithNetAmount(100).WithVatRate(VatRate.Rate23).WithVatAmount(23))
            .WithPayment(p => p.AsBankTransfer("PL12345678901234567890123456", bankName: "Test Bank"))
            .Build();

        // Assert
        invoice.InvoiceData.Payment!.PaymentMethods![0].Method.Should().Be(PaymentMethod.BankTransfer);
        invoice.InvoiceData.Payment.BankAccounts![0].AccountNumber.Should().Be("PL12345678901234567890123456");
        invoice.InvoiceData.Payment.BankAccounts[0].BankName.Should().Be("Test Bank");
    }

    [Fact]
    public void WithPayment_AsCash_ShouldConfigureCashPayment()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l.WithProductName("Item").WithNetAmount(100).WithVatRate(VatRate.Rate23).WithVatAmount(23))
            .WithPayment(p => p.AsCash(123.00m))
            .Build();

        // Assert
        invoice.InvoiceData.Payment!.PaymentMethods![0].Method.Should().Be(PaymentMethod.Cash);
        invoice.InvoiceData.Payment.PaymentMethods[0].Amount.Should().Be(123.00m);
    }

    #endregion

    #region DisableAutoCalculation Tests

    [Fact]
    public void DisableAutoCalculation_ShouldPreventVatSummaryCalculation()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .AddLineItem(l => l
                .WithProductName("Item")
                .WithNetAmount(100.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23.00m))
            .DisableAutoCalculation()
            .Build();

        // Assert - gdy wyłączone auto-obliczanie, wartości pozostają domyślne (null dla nullable decimal, 0 dla decimal)
        invoice.InvoiceData.NetAmount23.Should().BeNull();
        invoice.InvoiceData.VatAmount23.Should().BeNull();
        invoice.InvoiceData.TotalAmount.Should().Be(0);
    }

    #endregion

    #region WithCreationDateTime Tests

    [Fact]
    public void WithCreationDateTime_ShouldSetHeaderCreationDateTime()
    {
        // Arrange
        var creationDateTime = new DateTime(2024, 1, 15, 10, 30, 0);

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .WithCreationDateTime(creationDateTime)
            .Build();

        // Assert
        invoice.Header.CreationDateTime.Should().Be(creationDateTime);
    }

    [Fact]
    public void Build_WithoutCreationDateTime_ShouldSetDefaultDateTime()
    {
        // Arrange
        var beforeBuild = DateTime.Now;

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .Build();

        var afterBuild = DateTime.Now;

        // Assert
        invoice.Header.CreationDateTime.Should().BeOnOrAfter(beforeBuild);
        invoice.Header.CreationDateTime.Should().BeOnOrBefore(afterBuild);
    }

    #endregion

    #region WithSystemInfo Tests

    [Fact]
    public void WithSystemInfo_ShouldSetHeaderSystemInfo()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"))
            .WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024"))
            .WithSystemInfo("Test System v1.0")
            .Build();

        // Assert
        invoice.Header.SystemInfo.Should().Be("Test System v1.0");
    }

    #endregion

    #region Fluent API Chain Tests

    [Fact]
    public void FluentApi_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = InvoiceBuilder.Create();

        // Act
        var result1 = builder.WithSeller(s => s.WithTaxId("1234567890").WithName("Seller"));
        var result2 = result1.WithBuyer(b => b.WithTaxId("0987654321").WithName("Buyer"));
        var result3 = result2.WithInvoiceDetails(d => d.WithIssueDate(2024, 1, 15).WithInvoiceNumber("FV/001/2024"));
        var result4 = result3.AddLineItem(l => l.WithProductName("Item").WithNetAmount(100).WithVatRate(VatRate.Rate23).WithVatAmount(23));
        var result5 = result4.WithPayment(p => p.AsCash());
        var result6 = result5.DisableAutoCalculation();
        var result7 = result6.WithCreationDateTime(DateTime.Now);
        var result8 = result7.WithSystemInfo("Test");

        // Assert
        result1.Should().BeSameAs(builder);
        result2.Should().BeSameAs(builder);
        result3.Should().BeSameAs(builder);
        result4.Should().BeSameAs(builder);
        result5.Should().BeSameAs(builder);
        result6.Should().BeSameAs(builder);
        result7.Should().BeSameAs(builder);
        result8.Should().BeSameAs(builder);
    }

    [Fact]
    public void Build_ShouldReturnCompleteInvoice()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Seller Company Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Sprzedawcy 10")
                    .WithAddressLine2("00-001 Warszawa")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Buyer Company Sp. z o.o.")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("ul. Nabywcy 20")
                    .WithAddressLine2("00-002 Kraków")))
            .WithInvoiceDetails(d => d
                .WithIssueDate(2024, 1, 15)
                .WithInvoiceNumber("FV/001/2024")
                .WithSaleDate(2024, 1, 14)
                .WithCurrency(CurrencyCode.PLN))
            .AddLineItem(l => l
                .WithProductName("Usługa programistyczna")
                .WithQuantity(160)
                .WithUnit("godz.")
                .WithUnitNetPrice(150.00m)
                .WithNetAmount(24000.00m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(5520.00m))
            .WithPayment(p => p
                .AddPaymentTerm(2024, 2, 14)
                .AsBankTransfer("PL12345678901234567890123456"))
            .WithSystemInfo("KSeF Test System v1.0")
            .Build();

        // Assert
        invoice.Should().NotBeNull();
        invoice.Seller.TaxId.Should().Be("1234567890");
        invoice.Buyer.TaxId.Should().Be("0987654321");
        invoice.InvoiceData.InvoiceNumber.Should().Be("FV/001/2024");
        invoice.InvoiceData.LineItems.Should().HaveCount(1);
        invoice.InvoiceData.TotalAmount.Should().Be(29520.00m); // 24000 + 5520
        invoice.Header.SystemInfo.Should().Be("KSeF Test System v1.0");
    }

    #endregion
}
