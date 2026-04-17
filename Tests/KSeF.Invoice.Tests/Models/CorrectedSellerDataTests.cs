using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Common;
using KSeF.Invoice.Models.Corrections;
using KSeF.Invoice.Models.Enums;
using Xunit;

namespace KSeF.Invoice.Tests.Models;

public class CorrectedSellerDataTests
{
    [Fact]
    public void Constructor_ShouldCreateObjectWithDefaultIdentificationAndAddress()
    {
        // Act
        var data = new CorrectedSellerData();

        // Assert
        data.IdentificationData.Should().NotBeNull();
        data.Address.Should().NotBeNull();
        data.TaxpayerPrefix.Should().BeNull();
    }

    [Fact]
    public void ShouldSerializeTaxpayerPrefix_WhenNull_ShouldReturnFalse()
    {
        // Arrange
        var data = new CorrectedSellerData();

        // Act & Assert
        data.ShouldSerializeTaxpayerPrefix().Should().BeFalse();
    }

    [Fact]
    public void ShouldSerializeTaxpayerPrefix_WhenSet_ShouldReturnTrue()
    {
        // Arrange
        var data = new CorrectedSellerData { TaxpayerPrefix = EUCountryCode.PL };

        // Act & Assert
        data.ShouldSerializeTaxpayerPrefix().Should().BeTrue();
    }

    [Fact]
    public void XmlSerialization_ShouldProduceCorrectElementOrder()
    {
        // Arrange
        var data = new CorrectedSellerData
        {
            TaxpayerPrefix = EUCountryCode.PL,
            IdentificationData = new SellerIdentification
            {
                Nip = "5261040828",
                Name = "Test Sp. z o.o."
            },
            Address = new Address
            {
                CountryCode = "PL",
                AddressLine1 = "ul. Testowa 1",
                AddressLine2 = "00-001 Warszawa"
            }
        };

        // Act
        var xml = SerializeToXml(data);

        // Assert — verify element order: PrefiksPodatnika before DaneIdentyfikacyjne before Adres
        var prefixIndex = xml.IndexOf("PrefiksPodatnika");
        var identIndex = xml.IndexOf("DaneIdentyfikacyjne");
        var addressIndex = xml.IndexOf("Adres");

        prefixIndex.Should().BeLessThan(identIndex);
        identIndex.Should().BeLessThan(addressIndex);
    }

    [Fact]
    public void XmlSerialization_WithoutPrefix_ShouldOmitPrefiksPodatnika()
    {
        // Arrange
        var data = new CorrectedSellerData
        {
            IdentificationData = new SellerIdentification
            {
                Nip = "5261040828",
                Name = "Test Sp. z o.o."
            },
            Address = new Address
            {
                CountryCode = "PL",
                AddressLine1 = "ul. Testowa 1"
            }
        };

        // Act
        var xml = SerializeToXml(data);

        // Assert
        xml.Should().NotContain("PrefiksPodatnika");
        xml.Should().Contain("DaneIdentyfikacyjne");
        xml.Should().Contain("Adres");
    }

    private static string SerializeToXml(CorrectedSellerData data)
    {
        var serializer = new XmlSerializer(typeof(CorrectedSellerData));
        using var writer = new StringWriter();
        serializer.Serialize(writer, data);
        return writer.ToString();
    }
}
