using System.ComponentModel;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using Xunit;

namespace KSeF.Invoice.Tests.Models.Enums;

public class PaymentMethodTests
{
    [Theory]
    [InlineData(PaymentMethod.Cash, "1", 1)]
    [InlineData(PaymentMethod.Card, "2", 2)]
    [InlineData(PaymentMethod.Voucher, "3", 3)]
    [InlineData(PaymentMethod.Check, "4", 4)]
    [InlineData(PaymentMethod.Credit, "5", 5)]
    [InlineData(PaymentMethod.BankTransfer, "6", 6)]
    [InlineData(PaymentMethod.MobilePayment, "7", 7)]
    public void PaymentMethod_ShouldHaveCorrectXmlEnumAttributeAndValue(PaymentMethod method, string expectedXmlValue, int expectedIntValue)
    {
        // Arrange
        var memberInfo = typeof(PaymentMethod).GetMember(method.ToString())[0];
        var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .Cast<XmlEnumAttribute>()
            .FirstOrDefault();

        // Assert
        xmlEnumAttribute.Should().NotBeNull();
        xmlEnumAttribute!.Name.Should().Be(expectedXmlValue);
        ((int)method).Should().Be(expectedIntValue);
    }

    [Theory]
    [InlineData(PaymentMethod.Cash, "Gotówka")]
    [InlineData(PaymentMethod.Card, "Karta płatnicza")]
    [InlineData(PaymentMethod.Voucher, "Bon")]
    [InlineData(PaymentMethod.Check, "Czek")]
    [InlineData(PaymentMethod.Credit, "Kredyt")]
    [InlineData(PaymentMethod.BankTransfer, "Przelew bankowy")]
    [InlineData(PaymentMethod.MobilePayment, "Płatność mobilna")]
    public void PaymentMethod_ShouldHaveCorrectDescriptionAttribute(PaymentMethod method, string expectedDescription)
    {
        // Arrange
        var memberInfo = typeof(PaymentMethod).GetMember(method.ToString())[0];
        var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .FirstOrDefault();

        // Assert
        descriptionAttribute.Should().NotBeNull();
        descriptionAttribute!.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void PaymentMethod_ShouldHaveSevenValues()
    {
        // Assert
        Enum.GetValues<PaymentMethod>().Should().HaveCount(7);
    }

    [Fact]
    public void PaymentMethod_ValuesStartFromOne()
    {
        // Assert
        ((int)PaymentMethod.Cash).Should().Be(1);
    }

    [Fact]
    public void PaymentMethod_ValuesAreConsecutive()
    {
        // Arrange
        var values = Enum.GetValues<PaymentMethod>().Cast<int>().OrderBy(x => x).ToList();

        // Assert
        for (int i = 0; i < values.Count - 1; i++)
        {
            (values[i + 1] - values[i]).Should().Be(1, $"Values at index {i} and {i + 1} should be consecutive");
        }
    }
}
