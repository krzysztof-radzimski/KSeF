/*=====================================================================================

    KSeF.Invoice.Tests
    Testy jednostkowe dla FooterBuilder. Weryfikują poprawność
    budowania stopki faktury (InvoiceFooter) z informacjami dodatkowymi
    (FooterInfo) oraz danymi rejestrowymi (RegistryData).
    Testy obejmują Fluent API oraz integrację z InvoiceBuilder.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using FluentAssertions;
using KSeF.Invoice.Models.Attachments;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using Xunit;

namespace KSeF.Invoice.Tests.Builders;

public class FooterBuilderTests
{
    #region AddFooterText Tests

    [Fact]
    public void AddFooterText_WithValidText_ShouldAddFooterInfo()
    {
        // Arrange
        var builder = new FooterBuilder();
        var text = "Test footer information";

        // Act
        builder.AddFooterText(text);
        var result = builder.Build();

        // Assert
        result.AdditionalInfo.Should().NotBeNull();
        result.AdditionalInfo.Should().HaveCount(1);
        result.AdditionalInfo![0].FooterText.Should().Be(text);
    }

    [Fact]
    public void AddFooterText_Multiple_ShouldAddAllEntries()
    {
        // Arrange
        var builder = new FooterBuilder();
        var text1 = "First footer text";
        var text2 = "Second footer text";
        var text3 = "Third footer text";

        // Act
        builder.AddFooterText(text1)
               .AddFooterText(text2)
               .AddFooterText(text3);
        var result = builder.Build();

        // Assert
        result.AdditionalInfo.Should().NotBeNull();
        result.AdditionalInfo.Should().HaveCount(3);
        result.AdditionalInfo![0].FooterText.Should().Be(text1);
        result.AdditionalInfo[1].FooterText.Should().Be(text2);
        result.AdditionalInfo[2].FooterText.Should().Be(text3);
    }

    [Fact]
    public void AddFooterText_ShouldReturnBuilderInstance()
    {
        // Arrange
        var builder = new FooterBuilder();

        // Act
        var returned = builder.AddFooterText("Test");

        // Assert
        returned.Should().BeSameAs(builder);
    }

    #endregion

    #region AddFooterInfo Tests

    [Fact]
    public void AddFooterInfo_WithObject_ShouldAddToList()
    {
        // Arrange
        var builder = new FooterBuilder();
        var footerInfo = new FooterInfo { FooterText = "Custom footer info" };

        // Act
        builder.AddFooterInfo(footerInfo);
        var result = builder.Build();

        // Assert
        result.AdditionalInfo.Should().NotBeNull();
        result.AdditionalInfo.Should().HaveCount(1);
        result.AdditionalInfo![0].Should().BeSameAs(footerInfo);
    }

    [Fact]
    public void AddFooterInfo_MixedWithAddFooterText_ShouldCombineAll()
    {
        // Arrange
        var builder = new FooterBuilder();
        var footerInfo = new FooterInfo { FooterText = "Object footer" };

        // Act
        builder.AddFooterText("Text footer 1")
               .AddFooterInfo(footerInfo)
               .AddFooterText("Text footer 2");
        var result = builder.Build();

        // Assert
        result.AdditionalInfo.Should().NotBeNull();
        result.AdditionalInfo.Should().HaveCount(3);
        result.AdditionalInfo![0].FooterText.Should().Be("Text footer 1");
        result.AdditionalInfo[1].Should().BeSameAs(footerInfo);
        result.AdditionalInfo[2].FooterText.Should().Be("Text footer 2");
    }

    #endregion

    #region AddRegistryData Tests

    [Fact]
    public void AddRegistryData_WithAllFields_ShouldSetAll()
    {
        // Arrange
        var builder = new FooterBuilder();
        var fullName = "Test Company Ltd.";
        var krs = "0000123456";
        var regon = "123456789";
        var bdo = "ABC123456";

        // Act
        builder.AddRegistryData(fullName, krs, regon, bdo);
        var result = builder.Build();

        // Assert
        result.Registries.Should().NotBeNull();
        result.Registries.Should().HaveCount(1);
        result.Registries![0].FullName.Should().Be(fullName);
        result.Registries[0].KRS.Should().Be(krs);
        result.Registries[0].REGON.Should().Be(regon);
        result.Registries[0].BDO.Should().Be(bdo);
    }

    [Fact]
    public void AddRegistryData_WithOnlyKRS_ShouldSetOnlyKRS()
    {
        // Arrange
        var builder = new FooterBuilder();
        var krs = "0000999888";

        // Act
        builder.AddRegistryData(krs: krs);
        var result = builder.Build();

        // Assert
        result.Registries.Should().NotBeNull();
        result.Registries.Should().HaveCount(1);
        result.Registries![0].FullName.Should().BeNull();
        result.Registries[0].KRS.Should().Be(krs);
        result.Registries[0].REGON.Should().BeNull();
        result.Registries[0].BDO.Should().BeNull();
    }

    [Fact]
    public void AddRegistryData_Multiple_ShouldAddAll()
    {
        // Arrange
        var builder = new FooterBuilder();

        // Act
        builder.AddRegistryData(fullName: "Company 1", krs: "1111111111")
               .AddRegistryData(fullName: "Company 2", regon: "222222222")
               .AddRegistryData(fullName: "Company 3", bdo: "BDO333");
        var result = builder.Build();

        // Assert
        result.Registries.Should().NotBeNull();
        result.Registries.Should().HaveCount(3);
        result.Registries![0].FullName.Should().Be("Company 1");
        result.Registries[0].KRS.Should().Be("1111111111");
        result.Registries[1].FullName.Should().Be("Company 2");
        result.Registries[1].REGON.Should().Be("222222222");
        result.Registries[2].FullName.Should().Be("Company 3");
        result.Registries[2].BDO.Should().Be("BDO333");
    }

    #endregion

    #region AddRegistry Tests

    [Fact]
    public void AddRegistry_WithAction_ShouldBuildAndAdd()
    {
        // Arrange
        var builder = new FooterBuilder();

        // Act
        builder.AddRegistry(r => r
            .WithFullName("Action Company")
            .WithKRS("5555555555")
            .WithREGON("555555555"));
        var result = builder.Build();

        // Assert
        result.Registries.Should().NotBeNull();
        result.Registries.Should().HaveCount(1);
        result.Registries![0].FullName.Should().Be("Action Company");
        result.Registries[0].KRS.Should().Be("5555555555");
        result.Registries[0].REGON.Should().Be("555555555");
    }

    [Fact]
    public void AddRegistry_WithObject_ShouldAddToList()
    {
        // Arrange
        var builder = new FooterBuilder();
        var registryData = new RegistryData
        {
            FullName = "Object Company",
            KRS = "7777777777"
        };

        // Act
        builder.AddRegistry(registryData);
        var result = builder.Build();

        // Assert
        result.Registries.Should().NotBeNull();
        result.Registries.Should().HaveCount(1);
        result.Registries![0].Should().BeSameAs(registryData);
    }

    #endregion

    #region Build Tests

    [Fact]
    public void Build_EmptyBuilder_ShouldReturnEmptyFooter()
    {
        // Arrange
        var builder = new FooterBuilder();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.AdditionalInfo.Should().BeNull();
        result.Registries.Should().BeNull();
    }

    [Fact]
    public void Build_WithAllData_ShouldReturnCompleteFooter()
    {
        // Arrange
        var builder = new FooterBuilder();

        // Act
        builder.AddFooterText("Footer information")
               .AddRegistryData(fullName: "Complete Company", krs: "9999999999");
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.AdditionalInfo.Should().HaveCount(1);
        result.Registries.Should().HaveCount(1);
        result.HasAdditionalInfo.Should().BeTrue();
        result.HasRegistries.Should().BeTrue();
    }

    [Fact]
    public void Build_ShouldReturnInvoiceFooterInstance()
    {
        // Arrange
        var builder = new FooterBuilder();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().BeOfType<InvoiceFooter>();
    }

    #endregion

    #region Integration Tests (InvoiceBuilder)

    [Fact]
    public void InvoiceBuilder_WithFooter_ShouldConfigureFooter()
    {
        // Arrange & Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Seller Company")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("Main St 1")
                    .WithAddressLine2("00-001 Warsaw")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Buyer Company")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("Second St 2")
                    .WithAddressLine2("30-001 Krakow")))
            .WithInvoiceDetails(d => d
                .WithInvoiceNumber("FV/2026/001")
                .WithIssueDate(2026, 4, 17))
            .AddLineItem(l => l
                .WithProductName("Test Item")
                .WithNetAmount(100m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23m))
            .WithFooter(f => f
                .AddFooterText("This is footer text")
                .AddRegistryData(fullName: "Test Registry", krs: "0000111222"))
            .Build();

        // Assert
        invoice.Footer.Should().NotBeNull();
        invoice.Footer!.AdditionalInfo.Should().HaveCount(1);
        invoice.Footer.AdditionalInfo![0].FooterText.Should().Be("This is footer text");
        invoice.Footer.Registries.Should().HaveCount(1);
        invoice.Footer.Registries![0].FullName.Should().Be("Test Registry");
        invoice.Footer.Registries[0].KRS.Should().Be("0000111222");
    }

    [Fact]
    public void InvoiceBuilder_WithFooterObject_ShouldAssignFooter()
    {
        // Arrange
        var footer = new InvoiceFooter
        {
            AdditionalInfo = new List<FooterInfo>
            {
                new FooterInfo { FooterText = "Direct footer" }
            }
        };

        // Act
        var invoice = InvoiceBuilder.Create()
            .WithSeller(s => s
                .WithTaxId("1234567890")
                .WithName("Seller Company")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("Main St 1")
                    .WithAddressLine2("00-001 Warsaw")))
            .WithBuyer(b => b
                .WithTaxId("0987654321")
                .WithName("Buyer Company")
                .WithAddress(a => a
                    .WithCountryCode("PL")
                    .WithAddressLine1("Second St 2")
                    .WithAddressLine2("30-001 Krakow")))
            .WithInvoiceDetails(d => d
                .WithInvoiceNumber("FV/2026/002")
                .WithIssueDate(2026, 4, 17))
            .AddLineItem(l => l
                .WithProductName("Test Item")
                .WithNetAmount(100m)
                .WithVatRate(VatRate.Rate23)
                .WithVatAmount(23m))
            .WithFooter(footer)
            .Build();

        // Assert
        invoice.Footer.Should().BeSameAs(footer);
        invoice.Footer!.AdditionalInfo.Should().HaveCount(1);
        invoice.Footer.AdditionalInfo![0].FooterText.Should().Be("Direct footer");
    }

    #endregion
}
