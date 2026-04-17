/*=====================================================================================

    KSeF.Invoice.Tests
    Testy jednostkowe dla RegistryDataBuilder. Weryfikują poprawność
    budowania danych rejestrowych (RegistryData) z numerami KRS, REGON,
    BDO oraz pełną nazwą podmiotu. Testy obejmują Fluent API.

	Autor: (C)2009-2026 ITORG Krzysztof Radzimski
    Licencja MIT
	http://itorg.pl

  ===================================================================================*/

using FluentAssertions;
using KSeF.Invoice.Models.Attachments;
using KSeF.Invoice.Services.Builders;
using Xunit;

namespace KSeF.Invoice.Tests.Builders;

public class RegistryDataBuilderTests
{
    [Fact]
    public void WithFullName_ShouldSetFullName()
    {
        // Arrange
        var builder = new RegistryDataBuilder();
        var fullName = "Test Company Limited";

        // Act
        var result = builder.WithFullName(fullName).Build();

        // Assert
        result.FullName.Should().Be(fullName);
        result.KRS.Should().BeNull();
        result.REGON.Should().BeNull();
        result.BDO.Should().BeNull();
    }

    [Fact]
    public void WithKRS_ShouldSetKRS()
    {
        // Arrange
        var builder = new RegistryDataBuilder();
        var krs = "0000123456";

        // Act
        var result = builder.WithKRS(krs).Build();

        // Assert
        result.KRS.Should().Be(krs);
        result.HasKRS.Should().BeTrue();
        result.FullName.Should().BeNull();
        result.REGON.Should().BeNull();
        result.BDO.Should().BeNull();
    }

    [Fact]
    public void WithREGON_ShouldSetREGON()
    {
        // Arrange
        var builder = new RegistryDataBuilder();
        var regon = "123456789";

        // Act
        var result = builder.WithREGON(regon).Build();

        // Assert
        result.REGON.Should().Be(regon);
        result.HasREGON.Should().BeTrue();
        result.FullName.Should().BeNull();
        result.KRS.Should().BeNull();
        result.BDO.Should().BeNull();
    }

    [Fact]
    public void WithBDO_ShouldSetBDO()
    {
        // Arrange
        var builder = new RegistryDataBuilder();
        var bdo = "BDO123456";

        // Act
        var result = builder.WithBDO(bdo).Build();

        // Assert
        result.BDO.Should().Be(bdo);
        result.HasBDO.Should().BeTrue();
        result.FullName.Should().BeNull();
        result.KRS.Should().BeNull();
        result.REGON.Should().BeNull();
    }

    [Fact]
    public void ChainedMethods_ShouldSetAllProperties()
    {
        // Arrange
        var builder = new RegistryDataBuilder();
        var fullName = "Complete Company Inc.";
        var krs = "9999888777";
        var regon = "987654321";
        var bdo = "XYZ789012";

        // Act
        var result = builder
            .WithFullName(fullName)
            .WithKRS(krs)
            .WithREGON(regon)
            .WithBDO(bdo)
            .Build();

        // Assert
        result.FullName.Should().Be(fullName);
        result.KRS.Should().Be(krs);
        result.REGON.Should().Be(regon);
        result.BDO.Should().Be(bdo);
        result.HasKRS.Should().BeTrue();
        result.HasREGON.Should().BeTrue();
        result.HasBDO.Should().BeTrue();
    }

    [Fact]
    public void Build_ShouldReturnRegistryDataInstance()
    {
        // Arrange
        var builder = new RegistryDataBuilder();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RegistryData>();
    }

    [Fact]
    public void FluentAPI_ShouldReturnBuilderInstance()
    {
        // Arrange
        var builder = new RegistryDataBuilder();

        // Act & Assert
        builder.WithFullName("Test").Should().BeSameAs(builder);
        builder.WithKRS("1234567890").Should().BeSameAs(builder);
        builder.WithREGON("123456789").Should().BeSameAs(builder);
        builder.WithBDO("BDO123").Should().BeSameAs(builder);
    }
}
