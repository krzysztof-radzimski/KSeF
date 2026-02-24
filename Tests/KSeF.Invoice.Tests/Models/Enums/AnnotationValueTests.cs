using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Summary;
using Xunit;

namespace KSeF.Invoice.Tests.Models.Enums;

public class AnnotationValueTests
{
    [Theory]
    [InlineData(AnnotationValue.Yes, "1", 1)]
    [InlineData(AnnotationValue.No, "2", 2)]
    public void AnnotationValue_ShouldHaveCorrectXmlEnumAttributeAndValue(AnnotationValue value, string expectedXmlValue, int expectedIntValue)
    {
        // Arrange
        var memberInfo = typeof(AnnotationValue).GetMember(value.ToString())[0];
        var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .Cast<XmlEnumAttribute>()
            .FirstOrDefault();

        // Assert
        xmlEnumAttribute.Should().NotBeNull();
        xmlEnumAttribute!.Name.Should().Be(expectedXmlValue);
        ((int)value).Should().Be(expectedIntValue);
    }

    [Fact]
    public void AnnotationValue_ShouldHaveTwoValues()
    {
        // Assert
        Enum.GetValues<AnnotationValue>().Should().HaveCount(2);
    }

    [Fact]
    public void AnnotationValue_Yes_ShouldBeOne()
    {
        // Assert
        ((int)AnnotationValue.Yes).Should().Be(1);
    }

    [Fact]
    public void AnnotationValue_No_ShouldBeTwo()
    {
        // Assert
        ((int)AnnotationValue.No).Should().Be(2);
    }
}
