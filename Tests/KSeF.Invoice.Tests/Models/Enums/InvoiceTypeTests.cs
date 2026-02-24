using System.ComponentModel;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using Xunit;

namespace KSeF.Invoice.Tests.Models.Enums;

public class InvoiceTypeTests
{
    [Theory]
    [InlineData(InvoiceType.VAT, "VAT")]
    [InlineData(InvoiceType.KOR, "KOR")]
    [InlineData(InvoiceType.ZAL, "ZAL")]
    [InlineData(InvoiceType.ROZ, "ROZ")]
    [InlineData(InvoiceType.UPR, "UPR")]
    [InlineData(InvoiceType.KOR_ZAL, "KOR_ZAL")]
    [InlineData(InvoiceType.KOR_ROZ, "KOR_ROZ")]
    public void InvoiceType_ShouldHaveCorrectXmlEnumAttribute(InvoiceType invoiceType, string expectedXmlValue)
    {
        // Arrange
        var memberInfo = typeof(InvoiceType).GetMember(invoiceType.ToString())[0];
        var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .Cast<XmlEnumAttribute>()
            .FirstOrDefault();

        // Assert
        xmlEnumAttribute.Should().NotBeNull();
        xmlEnumAttribute!.Name.Should().Be(expectedXmlValue);
    }

    [Theory]
    [InlineData(InvoiceType.VAT, "Faktura podstawowa")]
    [InlineData(InvoiceType.KOR, "Faktura korygująca")]
    [InlineData(InvoiceType.ZAL, "Faktura zaliczkowa - dokumentująca otrzymanie zapłaty lub jej części przed dokonaniem czynności")]
    [InlineData(InvoiceType.ROZ, "Faktura rozliczeniowa - wystawiona w związku z art. 106f ust. 3 ustawy")]
    [InlineData(InvoiceType.UPR, "Faktura uproszczona - o której mowa w art. 106e ust. 5 pkt 3 ustawy")]
    [InlineData(InvoiceType.KOR_ZAL, "Faktura korygująca fakturę zaliczkową")]
    [InlineData(InvoiceType.KOR_ROZ, "Faktura korygująca fakturę rozliczeniową")]
    public void InvoiceType_ShouldHaveCorrectDescriptionAttribute(InvoiceType invoiceType, string expectedDescription)
    {
        // Arrange
        var memberInfo = typeof(InvoiceType).GetMember(invoiceType.ToString())[0];
        var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .FirstOrDefault();

        // Assert
        descriptionAttribute.Should().NotBeNull();
        descriptionAttribute!.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void InvoiceType_ShouldHaveSevenValues()
    {
        // Assert
        Enum.GetValues<InvoiceType>().Should().HaveCount(7);
    }

    [Fact]
    public void InvoiceType_AllValuesShouldHaveXmlEnumAttribute()
    {
        // Arrange
        var allValues = Enum.GetValues<InvoiceType>();

        // Assert
        foreach (var value in allValues)
        {
            var memberInfo = typeof(InvoiceType).GetMember(value.ToString())[0];
            var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false).FirstOrDefault();
            xmlEnumAttribute.Should().NotBeNull($"Value {value} should have XmlEnumAttribute");
        }
    }

    [Fact]
    public void InvoiceType_AllValuesShouldHaveDescriptionAttribute()
    {
        // Arrange
        var allValues = Enum.GetValues<InvoiceType>();

        // Assert
        foreach (var value in allValues)
        {
            var memberInfo = typeof(InvoiceType).GetMember(value.ToString())[0];
            var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            descriptionAttribute.Should().NotBeNull($"Value {value} should have DescriptionAttribute");
        }
    }

    [Theory]
    [InlineData(InvoiceType.KOR)]
    [InlineData(InvoiceType.KOR_ZAL)]
    [InlineData(InvoiceType.KOR_ROZ)]
    public void InvoiceType_CorrectionTypes_ShouldStartWithKOR(InvoiceType invoiceType)
    {
        // Assert
        invoiceType.ToString().Should().StartWith("KOR");
    }
}
