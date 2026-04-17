using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class CorrectedBuyerDataTests
{
    [Fact]
    public void Constructor_ShouldCreateObjectWithDefaultIdentification()
    {
        // Act
        var data = new CorrectedBuyerData();

        // Assert
        data.IdentificationData.Should().NotBeNull();
        data.Address.Should().BeNull();
        data.BuyerIdentifier.Should().BeNull();
    }

    [Fact]
    public void ShouldSerializeAddress_WhenNull_ShouldReturnFalse()
    {
        // Arrange
        var data = new CorrectedBuyerData();

        // Act & Assert
        data.ShouldSerializeAddress().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeAddress_WhenSet_ShouldReturnTrue()
    {
        // Arrange
        var data = new CorrectedBuyerData
        {
            Address = new Address { CountryCode = "PL", AddressLine1 = "ul. Testowa 1" }
        };

        // Act & Assert
        data.ShouldSerializeAddress().Should().BeTrue();
    }

    [Fact]
    public void ShouldSerializeBuyerIdentifier_WhenNull_ShouldReturnFalse()
    {
        // Arrange
        var data = new CorrectedBuyerData();

        // Act & Assert
        data.ShouldSerializeBuyerIdentifier().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeBuyerIdentifier_WhenEmpty_ShouldReturnFalse()
    {
        // Arrange
        var data = new CorrectedBuyerData { BuyerIdentifier = "" };

        // Act & Assert
        data.ShouldSerializeBuyerIdentifier().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeBuyerIdentifier_WhenSet_ShouldReturnTrue()
    {
        // Arrange
        var data = new CorrectedBuyerData { BuyerIdentifier = "KLIENT-001" };

        // Act & Assert
        data.ShouldSerializeBuyerIdentifier().Should().BeTrue();
    }

    [Fact]
    public void XmlSerialization_ShouldProduceCorrectElementOrder()
    {
        // Arrange
        var data = new CorrectedBuyerData
        {
            IdentificationData = new BuyerIdentification
            {
                Nip = "5252248481",
                Name = "Nabywca Testowy S.A."
            },
            Address = new Address
            {
                CountryCode = "PL",
                AddressLine1 = "ul. Testowa 1",
                AddressLine2 = "00-001 Warszawa"
            },
            BuyerIdentifier = "KLIENT-001"
        };

        // Act
        var xml = SerializeToXml(data);

        // Assert — verify element order: DaneIdentyfikacyjne before Adres before IDNabywcy
        var identIndex = xml.IndexOf("DaneIdentyfikacyjne");
        var addressIndex = xml.IndexOf("Adres");
        var idIndex = xml.IndexOf("IDNabywcy");

        identIndex.Should().BeLessThan(addressIndex);
        addressIndex.Should().BeLessThan(idIndex);
    }

    [Fact]
    public void XmlSerialization_WithoutOptionalFields_ShouldOmitThem()
    {
        // Arrange
        var data = new CorrectedBuyerData
        {
            IdentificationData = new BuyerIdentification
            {
                Nip = "5252248481",
                Name = "Nabywca Testowy S.A."
            }
        };

        // Act
        var xml = SerializeToXml(data);

        // Assert
        xml.Should().Contain("DaneIdentyfikacyjne");
        xml.Should().NotContain("Adres");
        xml.Should().NotContain("IDNabywcy");
    }

    private static string SerializeToXml(CorrectedBuyerData data)
    {
        var serializer = new XmlSerializer(typeof(CorrectedBuyerData));
        using var writer = new StringWriter();
        serializer.Serialize(writer, data);
        return writer.ToString();
    }
}
