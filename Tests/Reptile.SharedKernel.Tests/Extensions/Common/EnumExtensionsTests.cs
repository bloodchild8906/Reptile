using FluentAssertions;
using Reptile.SharedKernel.Extensions.Common;

namespace Reptile.SharedKernel.Tests.Extensions.Common;

[TestFixture]
public class EnumExtensionsTests
{
    [Flags]
    public enum SampleEnum
    {
        [System.ComponentModel.Description("Value One")]
        Value1,

        [System.ComponentModel.Description("Value Two")]
        Value2,
        Value3
    }

    [TestCase("Value One", SampleEnum.Value1,
        Description = "Parse enums by their description attribute where the description matches the enum value.")]
    [TestCase("Value Two", SampleEnum.Value2,
        Description = "Ensure descriptions are case-insensitive and correctly parsed.")]
    [Category("EnumParsing")]
    public void ParseAsEnumByDescriptionAttribute_ReturnsCorrectEnum(string description, SampleEnum expected)
    {
        var result = description.ParseAsEnumByDescriptionAttribute<SampleEnum>();
        result.Should().Be(expected);
    }

    [Test(Description = "Retrieve the XmlEnumAttribute of an enum value, or default to the enum's name if not set.")]
    [Category("EnumAttributes")]
    public void GetXmlEnumAttribute_ReturnsCorrectAttribute()
    {
        var result = SampleEnum.Value1.GetXmlEnumAttribute();
        result.Should().Be("Value1");
    }

    [Test(Description =
        "Enumerate all values of an enum type to verify the GetAllItems method includes all defined values.")]
    [Category("EnumOperations")]
    public void GetAllItems_ReturnsAllItems()
    {
        var items = SampleEnum.Value1.GetAllItems<SampleEnum>();
        items.Should().BeEquivalentTo(new[] { SampleEnum.Value1, SampleEnum.Value2, SampleEnum.Value3 });
    }

    [Test(Description = "Test if a composite enum value contains a specific flag using bitwise operations.")]
    [Category("EnumOperations")]
    public void Contains_ChecksIfContainsValue_ReturnsTrue()
    {
        var compositeEnumValue = SampleEnum.Value1 | SampleEnum.Value2; // Here I'm assuming SampleEnum has Flags attribute 
        var specificFlag = SampleEnum.Value1;

        var result = compositeEnumValue.Contains(specificFlag);

        result.Should().BeTrue();
    }
}