using System;
using FluentAssertions;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;
using KSeF.Invoice.Services.Builders;
using Xunit;

namespace KSeF.Invoice.Tests.Builders;

public class CorrectedBuyerDataBuilderTests
{
    [Fact]
    public void Build_Default_ShouldCreateObjectWithDefaultIdentification()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder().Build();

        // Assert
        result.IdentificationData.Should().NotBeNull();
        result.Address.Should().BeNull();
        result.BuyerIdentifier.Should().BeNull();
    }

    [Fact]
    public void WithNip_ShouldSetNipOnIdentificationData()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithNip("525-224-84-81")
            .Build();

        // Assert
        result.IdentificationData.Nip.Should().Be("5252248481");
    }

    [Fact]
    public void WithEuVatId_ShouldSetEuVatIdOnIdentificationData()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithEuVatId(EUCountryCode.DE, "123456789")
            .Build();

        // Assert
        result.IdentificationData.EUCountryCode.Should().Be(EUCountryCode.DE);
        result.IdentificationData.VatNumberEU.Should().Be("123456789");
    }

    [Fact]
    public void WithForeignId_ShouldSetForeignIdOnIdentificationData()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithForeignId("US", "EIN-12345")
            .Build();

        // Assert
        result.IdentificationData.CountryCode.Should().Be("US");
        result.IdentificationData.OtherTaxId.Should().Be("EIN-12345");
    }

    [Fact]
    public void WithNoIdentifier_ShouldSetNoIdentifierOnIdentificationData()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithNoIdentifier()
            .Build();

        // Assert
        result.IdentificationData.NoIdentifier.Should().Be(1);
    }

    [Fact]
    public void WithName_ShouldSetNameOnIdentificationData()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithName("Nabywca Testowy S.A.")
            .Build();

        // Assert
        result.IdentificationData.Name.Should().Be("Nabywca Testowy S.A.");
    }

    [Fact]
    public void WithIdentification_ShouldConfigureIdentificationData()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithIdentification(id => id
                .WithNip("5252248481")
                .WithName("Nabywca Testowy"))
            .Build();

        // Assert
        result.IdentificationData.Nip.Should().Be("5252248481");
        result.IdentificationData.Name.Should().Be("Nabywca Testowy");
    }

    [Fact]
    public void WithAddress_Action_ShouldConfigureAddress()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithAddress(a => a
                .WithCountryCode("PL")
                .WithAddressLine1("ul. Testowa 1")
                .WithAddressLine2("00-001 Warszawa"))
            .Build();

        // Assert
        result.Address.Should().NotBeNull();
        result.Address!.CountryCode.Should().Be("PL");
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
        var result = new CorrectedBuyerDataBuilder()
            .WithAddress(address)
            .Build();

        // Assert
        result.Address.Should().BeSameAs(address);
    }

    [Fact]
    public void WithBuyerIdentifier_ShouldSetBuyerIdentifier()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithBuyerIdentifier("KLIENT-001")
            .Build();

        // Assert
        result.BuyerIdentifier.Should().Be("KLIENT-001");
    }

    [Fact]
    public void WithBuyerIdentifier_ExceedingMaxLength_ShouldThrow()
    {
        // Arrange
        var tooLong = new string('A', 33);

        // Act
        var act = () => new CorrectedBuyerDataBuilder()
            .WithBuyerIdentifier(tooLong);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*32*");
    }

    [Fact]
    public void WithBuyerIdentifier_AtMaxLength_ShouldNotThrow()
    {
        // Arrange
        var maxLength = new string('A', 32);

        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithBuyerIdentifier(maxLength)
            .Build();

        // Assert
        result.BuyerIdentifier.Should().Be(maxLength);
    }

    [Fact]
    public void FluentChaining_ShouldSetAllProperties()
    {
        // Act
        var result = new CorrectedBuyerDataBuilder()
            .WithNip("5252248481")
            .WithName("Nabywca Testowy S.A.")
            .WithAddress(a => a
                .WithAddressLine1("ul. Testowa 1")
                .WithAddressLine2("00-001 Warszawa"))
            .WithBuyerIdentifier("KLIENT-001")
            .Build();

        // Assert
        result.IdentificationData.Nip.Should().Be("5252248481");
        result.IdentificationData.Name.Should().Be("Nabywca Testowy S.A.");
        result.Address.Should().NotBeNull();
        result.Address!.AddressLine1.Should().Be("ul. Testowa 1");
        result.BuyerIdentifier.Should().Be("KLIENT-001");
    }

    [Fact]
    public void InvoiceCorrectionSectionBuilder_AddCorrectedBuyer_ShouldAddToList()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var sectionBuilder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        sectionBuilder.AddCorrectedBuyer(b => b
            .WithNip("5252248481")
            .WithName("Nabywca Testowy")
            .WithAddress(a => a.WithAddressLine1("ul. Testowa 1")));

        // Assert
        section.CorrectedBuyers.Should().NotBeNull();
        section.CorrectedBuyers.Should().HaveCount(1);
        section.CorrectedBuyers![0].IdentificationData.Nip.Should().Be("5252248481");
        section.CorrectedBuyers[0].IdentificationData.Name.Should().Be("Nabywca Testowy");
        section.CorrectedBuyers[0].Address!.AddressLine1.Should().Be("ul. Testowa 1");
    }

    [Fact]
    public void InvoiceCorrectionSectionBuilder_AddMultipleBuyers_ShouldAddAll()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var sectionBuilder = new InvoiceCorrectionSectionBuilder(section);

        // Act
        sectionBuilder
            .AddCorrectedBuyer(b => b.WithNip("5252248481").WithName("Nabywca 1"))
            .AddCorrectedBuyer(b => b.WithNip("1234567890").WithName("Nabywca 2"))
            .AddCorrectedBuyer(b => b.WithForeignId("DE", "DE123456789").WithName("Nabywca 3"));

        // Assert
        section.CorrectedBuyers.Should().HaveCount(3);
    }

    [Fact]
    public void InvoiceCorrectionSectionBuilder_AddBuyer102_ShouldThrow()
    {
        // Arrange
        var section = new InvoiceCorrectionSection();
        var sectionBuilder = new InvoiceCorrectionSectionBuilder(section);

        for (int i = 0; i < 101; i++)
        {
            sectionBuilder.AddCorrectedBuyer(b => b.WithNip("5252248481").WithName($"Nabywca {i}"));
        }

        // Act
        var act = () => sectionBuilder.AddCorrectedBuyer(b => b.WithNip("0000000000").WithName("Nabywca 102"));

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*101*");
    }
}
