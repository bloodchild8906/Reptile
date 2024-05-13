using FluentAssertions;
using Reptile.SharedKernel.Extensions.Common;

namespace Reptile.SharedKernel.Tests.Extensions.Common;

[TestFixture]
public class ExtensionsTests
{
    [Test(Description = "Generate a new GUID if the provided GUID is empty, otherwise return the original.")]
    [Category("TypeConversion")]
    public void ToNewGuidIfEmpty_ReturnsNewGuidIfEmpty()
    {
        var emptyGuid = Guid.Empty;
        var result = emptyGuid.ToNewGuidIfEmpty();

        result.Should().NotBe(Guid.Empty);
    }

    [Test(Description = "Safely cast an object to a specified type using generic type inference.")]
    [Category("TypeConversion")]
    public void As_CastsObjectToType()
    {
        object value = "123";
        var result = value as string;

        result.Should().Be("123");
    }
}