using System.ComponentModel;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using Xunit;

namespace KSeF.Invoice.Tests.Models.Enums;

public class VatRateTests
{
    [Theory]
    [InlineData(VatRate.Rate23, "23")]
    [InlineData(VatRate.Rate22, "22")]
    [InlineData(VatRate.Rate8, "8")]
    [InlineData(VatRate.Rate7, "7")]
    [InlineData(VatRate.Rate5, "5")]
    [InlineData(VatRate.Rate4, "4")]
    [InlineData(VatRate.Rate3, "3")]
    [InlineData(VatRate.Rate0Domestic, "0 KR")]
    [InlineData(VatRate.Rate0IntraCommunitySupply, "0 WDT")]
    [InlineData(VatRate.Rate0Export, "0 EX")]
    [InlineData(VatRate.Exempt, "zw")]
    [InlineData(VatRate.ReverseCharge, "oo")]
    [InlineData(VatRate.NotSubjectToTaxI, "np I")]
    [InlineData(VatRate.NotSubjectToTaxII, "np II")]
    public void VatRate_ShouldHaveCorrectXmlEnumAttribute(VatRate vatRate, string expectedXmlValue)
    {
        // Arrange
        var memberInfo = typeof(VatRate).GetMember(vatRate.ToString())[0];
        var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .Cast<XmlEnumAttribute>()
            .FirstOrDefault();

        // Assert
        xmlEnumAttribute.Should().NotBeNull();
        xmlEnumAttribute!.Name.Should().Be(expectedXmlValue);
    }

    [Fact]
    public void VatRate_ShouldHaveFourteenValues()
    {
        // Assert
        Enum.GetValues<VatRate>().Should().HaveCount(14);
    }

    [Fact]
    public void VatRate_AllValuesShouldHaveXmlEnumAttribute()
    {
        // Arrange
        var allValues = Enum.GetValues<VatRate>();

        // Assert
        foreach (var value in allValues)
        {
            var memberInfo = typeof(VatRate).GetMember(value.ToString())[0];
            var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false).FirstOrDefault();
            xmlEnumAttribute.Should().NotBeNull($"Value {value} should have XmlEnumAttribute");
        }
    }

    [Fact]
    public void VatRate_AllValuesShouldHaveDescriptionAttribute()
    {
        // Arrange
        var allValues = Enum.GetValues<VatRate>();

        // Assert
        foreach (var value in allValues)
        {
            var memberInfo = typeof(VatRate).GetMember(value.ToString())[0];
            var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            descriptionAttribute.Should().NotBeNull($"Value {value} should have DescriptionAttribute");
        }
    }

    [Theory]
    [InlineData(VatRate.Rate0Domestic)]
    [InlineData(VatRate.Rate0IntraCommunitySupply)]
    [InlineData(VatRate.Rate0Export)]
    public void VatRate_ZeroRates_ShouldHaveZeroInName(VatRate vatRate)
    {
        // Assert
        vatRate.ToString().Should().StartWith("Rate0");
    }

    [Theory]
    [InlineData(VatRate.NotSubjectToTaxI)]
    [InlineData(VatRate.NotSubjectToTaxII)]
    public void VatRate_NotSubjectToTax_ShouldHaveCorrectPrefix(VatRate vatRate)
    {
        // Assert
        vatRate.ToString().Should().StartWith("NotSubjectToTax");
    }
}
