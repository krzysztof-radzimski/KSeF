using System.ComponentModel;
using System.Xml.Serialization;
using FluentAssertions;
using KSeF.Invoice.Models.Enums;
using Xunit;

namespace KSeF.Invoice.Tests.Models.Enums;

public class SubjectRoleTests
{
    [Theory]
    [InlineData(SubjectRole.Factor, "1", 1)]
    [InlineData(SubjectRole.Recipient, "2", 2)]
    [InlineData(SubjectRole.OriginalEntity, "3", 3)]
    [InlineData(SubjectRole.AdditionalBuyer, "4", 4)]
    [InlineData(SubjectRole.InvoiceIssuer, "5", 5)]
    [InlineData(SubjectRole.Payer, "6", 6)]
    [InlineData(SubjectRole.LocalGovernmentIssuer, "7", 7)]
    [InlineData(SubjectRole.LocalGovernmentRecipient, "8", 8)]
    [InlineData(SubjectRole.VatGroupMemberIssuer, "9", 9)]
    [InlineData(SubjectRole.VatGroupMemberRecipient, "10", 10)]
    [InlineData(SubjectRole.Employee, "11", 11)]
    public void SubjectRole_ShouldHaveCorrectXmlEnumAttributeAndValue(SubjectRole role, string expectedXmlValue, int expectedIntValue)
    {
        // Arrange
        var memberInfo = typeof(SubjectRole).GetMember(role.ToString())[0];
        var xmlEnumAttribute = memberInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false)
            .Cast<XmlEnumAttribute>()
            .FirstOrDefault();

        // Assert
        xmlEnumAttribute.Should().NotBeNull();
        xmlEnumAttribute!.Name.Should().Be(expectedXmlValue);
        ((int)role).Should().Be(expectedIntValue);
    }

    [Fact]
    public void SubjectRole_ShouldHaveElevenValues()
    {
        // Assert
        Enum.GetValues<SubjectRole>().Should().HaveCount(11);
    }

    [Fact]
    public void SubjectRole_AllValuesShouldHaveDescriptionAttribute()
    {
        // Arrange
        var allValues = Enum.GetValues<SubjectRole>();

        // Assert
        foreach (var value in allValues)
        {
            var memberInfo = typeof(SubjectRole).GetMember(value.ToString())[0];
            var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            descriptionAttribute.Should().NotBeNull($"Value {value} should have DescriptionAttribute");
        }
    }

    [Theory]
    [InlineData(SubjectRole.LocalGovernmentIssuer)]
    [InlineData(SubjectRole.LocalGovernmentRecipient)]
    public void SubjectRole_LocalGovernmentRoles_ShouldHaveCorrectPrefix(SubjectRole role)
    {
        // Assert
        role.ToString().Should().StartWith("LocalGovernment");
    }

    [Theory]
    [InlineData(SubjectRole.VatGroupMemberIssuer)]
    [InlineData(SubjectRole.VatGroupMemberRecipient)]
    public void SubjectRole_VatGroupRoles_ShouldHaveCorrectPrefix(SubjectRole role)
    {
        // Assert
        role.ToString().Should().StartWith("VatGroupMember");
    }
}
