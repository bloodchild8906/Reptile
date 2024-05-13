using FluentAssertions;
using Reptile.SharedKernel.Extensions.Collections;

namespace Reptile.SharedKernel.Tests.Extensions.Collections;

[TestFixture]
public class SetExtensionsTests
{
    [Test(Description =
        "Add a range of non-empty strings to a set, ensuring duplicates and whitespaces are not added.")]
    [Category("SetOperations")]
    public void AddRange_AddsNonEmptyUniqueItems()
    {
        var set = new HashSet<string> { "existing" };
        var itemsToAdd = new List<string> { "new", "existing", " ", "" };
        set.AddRange(itemsToAdd);
        set.Should().BeEquivalentTo(new HashSet<string> { "existing", "new" });
    }
}