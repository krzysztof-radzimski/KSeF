using System.ComponentModel;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using Xunit;

namespace KSeF.Invoice.Tests.Models.Enums;

public class CorrectionTypeTests
{
    [Theory]
    [InlineData(CorrectionType.OriginalInvoiceDate, "1", 1)]
    [InlineData(CorrectionType.CorrectionInvoiceDate, "2", 2)]
    [InlineData(CorrectionType.OtherDate, "3", 3)]
    public void CorrectionType_ShouldHaveCorrectXmlEnumAttributeAndValue(CorrectionType type, string expectedXmlValue, int expectedIntValue)
    {
        // Arrange
        var memberInfo = typeof(CorrectionType).GetMember(type.ToString())[0];
        var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .Cast<XmlEnumAttribute>()
            .FirstOrDefault();

        // Assert
        xmlEnumAttribute.Should().NotBeNull();
        xmlEnumAttribute!.Name.Should().Be(expectedXmlValue);
        ((int)type).Should().Be(expectedIntValue);
    }

    [Theory]
    [InlineData(CorrectionType.OriginalInvoiceDate, "Korekta skutkująca w dacie ujęcia faktury pierwotnej")]
    [InlineData(CorrectionType.CorrectionInvoiceDate, "Korekta skutkująca w dacie wystawienia faktury korygującej")]
    [InlineData(CorrectionType.OtherDate, "Korekta skutkująca w innej dacie")]
    public void CorrectionType_ShouldHaveCorrectDescriptionAttribute(CorrectionType type, string expectedDescription)
    {
        // Arrange
        var memberInfo = typeof(CorrectionType).GetMember(type.ToString())[0];
        var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .FirstOrDefault();

        // Assert
        descriptionAttribute.Should().NotBeNull();
        descriptionAttribute!.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void CorrectionType_ShouldHaveThreeValues()
    {
        // Assert
        Enum.GetValues<CorrectionType>().Should().HaveCount(3);
    }

    [Fact]
    public void CorrectionType_AllValuesShouldHaveXmlEnumAttribute()
    {
        // Arrange
        var allValues = Enum.GetValues<CorrectionType>();

        // Assert
        foreach (var value in allValues)
        {
            var memberInfo = typeof(CorrectionType).GetMember(value.ToString())[0];
            var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false).FirstOrDefault();
            xmlEnumAttribute.Should().NotBeNull($"Value {value} should have XmlEnumAttribute");
        }
    }
}
