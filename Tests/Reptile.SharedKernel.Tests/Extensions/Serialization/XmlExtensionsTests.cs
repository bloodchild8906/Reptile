using FluentAssertions;
using Reptile.SharedKernel.Extensions.Serialization;

namespace Reptile.SharedKernel.Tests.Extensions.Serialization;

[TestFixture]
public class XmlExtensionsTests
{
    [TestCase(true, 1, Description = "Convert `true` to its XML boolean representation (1).")]
    [TestCase(false, 0, Description = "Convert `false` to its XML boolean representation (0).")]
    [Category("XmlSerialization")]
    public void ToXmlBoolean_ConvertsCorrectly(bool input, int expected)
    {
        var result = input.ToXmlBoolean();

        result.Should().Be(expected);
    }
}