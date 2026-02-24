using System.ComponentModel;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using Xunit;

namespace KSeF.Invoice.Tests.Models.Enums;

public class BankAccountTypeTests
{
    [Theory]
    [InlineData(BankAccountType.ReceivablesSettlement, "1", 1)]
    [InlineData(BankAccountType.CollectionAndTransfer, "2", 2)]
    [InlineData(BankAccountType.InternalOperations, "3", 3)]
    public void BankAccountType_ShouldHaveCorrectXmlEnumAttributeAndValue(BankAccountType type, string expectedXmlValue, int expectedIntValue)
    {
        // Arrange
        var memberInfo = typeof(BankAccountType).GetMember(type.ToString())[0];
        var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .Cast<XmlEnumAttribute>()
            .FirstOrDefault();

        // Assert
        xmlEnumAttribute.Should().NotBeNull();
        xmlEnumAttribute!.Name.Should().Be(expectedXmlValue);
        ((int)type).Should().Be(expectedIntValue);
    }

    [Theory]
    [InlineData(BankAccountType.ReceivablesSettlement, "Rachunek banku/SKOK do rozliczeń z tytułu nabywanych wierzytelności")]
    [InlineData(BankAccountType.CollectionAndTransfer, "Rachunek banku/SKOK do pobierania należności i przekazywania dostawcy")]
    [InlineData(BankAccountType.InternalOperations, "Rachunek banku/SKOK w ramach gospodarki własnej")]
    public void BankAccountType_ShouldHaveCorrectDescriptionAttribute(BankAccountType type, string expectedDescription)
    {
        // Arrange
        var memberInfo = typeof(BankAccountType).GetMember(type.ToString())[0];
        var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .FirstOrDefault();

        // Assert
        descriptionAttribute.Should().NotBeNull();
        descriptionAttribute!.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void BankAccountType_ShouldHaveThreeValues()
    {
        // Assert
        Enum.GetValues<BankAccountType>().Should().HaveCount(3);
    }

    [Fact]
    public void BankAccountType_AllValuesShouldHaveXmlEnumAndDescriptionAttributes()
    {
        // Arrange
        var allValues = Enum.GetValues<BankAccountType>();

        // Assert
        foreach (var value in allValues)
        {
            var memberInfo = typeof(BankAccountType).GetMember(value.ToString())[0];

            var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false).FirstOrDefault();
            xmlEnumAttribute.Should().NotBeNull($"Value {value} should have XmlEnumAttribute");

            var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            descriptionAttribute.Should().NotBeNull($"Value {value} should have DescriptionAttribute");
        }
    }
}
