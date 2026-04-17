using FluentAssertions;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using Xunit;

namespace KSeF.Invoice.Tests.Builders;

public class CorrectedSellerDataBuilderTests
{
    [Fact]
    public void Build_Default_ShouldCreateObjectWithDefaultIdentificationAndAddress()
    {
        // Act
        var result = new CorrectedSellerDataBuilder().Build();

        // Assert
        result.IdentificationData.Should().NotBeNull();
        result.Address.Should().NotBeNull();
        result.TaxpayerPrefix.Should().BeNull();
    }

    [Fact]
    public void WithTaxpayerPrefix_ShouldSetPrefix()
    {
        // Act
        var result = new CorrectedSellerDataBuilder()
            .WithTaxpayerPrefix(EUCountryCode.DE)
            .Build();

        // Assert
        result.TaxpayerPrefix.Should().Be(EUCountryCode.DE);
    }

    [Fact]
    public void WithNip_ShouldSetNipOnIdentificationData()
    {
        // Act
        var result = new CorrectedSellerDataBuilder()
            .WithNip("526-104-08-28")
            .Build();

        // Assert
        result.IdentificationData.Nip.Should().Be("5261040828");
    }

    [Fact]
    public void WithName_ShouldSetNameOnIdentificationData()
    {
        // Act
        var result = new CorrectedSellerDataBuilder()
            .WithName("Firma Testowa Sp. z o.o.")
            .Build();

        // Assert
        result.IdentificationData.Name.Should().Be("Firma Testowa Sp. z o.o.");
    }

    [Fact]
    public void WithIdentification_ShouldConfigureIdentificationData()
    {
        // Act
        var result = new CorrectedSellerDataBuilder()
            .WithIdentification(id => id
                .WithNip("5261040828")
                .WithName("Firma Testowa"))
            .Build();

        // Assert
        result.IdentificationData.Nip.Should().Be("5261040828");
        result.IdentificationData.Name.Should().Be("Firma Testowa");
    }

    [Fact]
    public void WithAddress_Action_ShouldConfigureAddress()
    {
        // Act
        var result = new CorrectedSellerDataBuilder()
            .WithAddress(a => a
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Testowa 1")
                .WithAddressLine2("00-001 Warszawa"))
            .Build();

        // Assert
        result.Address.CountryCode.Should().Be("PL");
        result.Address.AddressLine1.Should().Be("ul. Testowa 1");
        result.Address.AddressLine2.Should().Be("00-001 Warszawa");
    }

    [Fact]
    public void WithAddress_Object_ShouldSetAddress()
    {
        // Arrange
        var address = new Address
        {
            CountryCode = "DE",
            AddressLine1 = "Berliner Str. 10"
        };

        // Act
        var result = new CorrectedSellerDataBuilder()
            .WithAddress(address)
            .Build();

        // Assert
        result.Address.Should().BeSameAs(address);
    }

    [Fact]
    public void FluentChaining_ShouldSetAllProperties()
    {
        // Act
        var result = new CorrectedSellerDataBuilder()
            .WithTaxpayerPrefix(EUCountryCode.PL)
            .WithNip("5261040828")
            .WithName("Test Sp. z o.o.")
            .WithAddress(a => a
                .WithAddressLine1("ul. Testowa 1")
                .WithAddressLine2("00-001 Warszawa"))
            .Build();

        // Assert
        result.TaxpayerPrefix.Should().Be(EUCountryCode.PL);
        result.IdentificationData.Nip.Should().Be("5261040828");
        result.IdentificationData.Name.Should().Be("Test Sp. z o.o.");
        result.Address.AddressLine1.Should().Be("ul. Testowa 1");
        result.Address.AddressLine2.Should().Be("00-001 Warszawa");
    }

    [Fact]
    public void InvoiceCorrectionSectionBuilder_WithCorrectedSeller_ShouldIntegrate()
    {
        // Arrange
        var section = new KSeF.Invoice.Models.Corrections.InvoiceCorrectionSection();
        var sectionBuilder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        sectionBuilder.WithCorrectedSeller(s => s
            .WithNip("5261040828")
            .WithName("Firma Testowa")
            .WithAddress(a => a.WithAddressLine1("ul. Testowa 1")));

        // Assert
        section.CorrectedSeller.Should().NotBeNull();
        section.CorrectedSeller!.IdentificationData.Nip.Should().Be("5261040828");
        section.CorrectedSeller.IdentificationData.Name.Should().Be("Firma Testowa");
        section.CorrectedSeller.Address.AddressLine1.Should().Be("ul. Testowa 1");
    }
}
