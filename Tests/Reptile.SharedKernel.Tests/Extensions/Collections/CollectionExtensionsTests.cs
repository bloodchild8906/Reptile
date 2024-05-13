using FluentAssertions;
using Reptile.SharedKernel.Extensions.Collections;

namespace Reptile.SharedKernel.Tests.Extensions.Collections;

[TestFixture]
public class CollectionExtensionsTests
{
    [Test(Description = "Apply an action to each item in the collection.")]
    [Category("CollectionManipulation")]
    public void Each_AppliesActionToEachItem()
    {
        var data = new List<int> { 1, 2, 3 };
        var result = new List<int>();

        data.Each(Action);

        result.Should().BeEquivalentTo(new List<int> { 2, 4, 6 });
        return;

        void Action(int item)
        {
            result.Add(item * 2);
        }
    }

    [Test(Description = "Find the maximum item by a given key selector.")]
    [Category("CollectionManipulation")]
    public void MaxBy_ReturnsMaxItem()
    {
        var data = new List<(string name, int age)> { ("Alice", 30), ("Bob", 25), ("Charlie", 35) };
        var max = data.MaxBy(x => x.age);
        max.Should().Be(("Charlie", 35));
    }

    [Test(Description = "Transform each item in the collection.")]
    [Category("CollectionManipulation")]
    public void Map_TransformsItems()
    {
        // Data for the test
        var data = new List<int> { 1, 2, 3 };

        // Action to multiply items by 2
        Func<int, int> action = item => item * 2;

        // Apply the transformation
        var result = data.Map(action).ToList();

        // Assert
        result.Should().Equal(new List<int> { 2, 4, 6 });
    }

    [Test(Description = "Retrieve duplicate items from the collection.")]
    [Category("CollectionAnalysis")]
    public void GetDuplicates_IdentifiesDuplicates()
    {
        var data = new List<int> { 1, 2, 2, 3, 4, 4, 4 };
        data=data.GetDuplicates().ToList();
        data.Should().BeEquivalentTo(new List<int> { 2, 4 });
    }

    [Test(Description = "Check if an item is in the collection.")]
    [Category("CollectionAnalysis")]
    public void In_ChecksItemInCollection()
    {
        const int item = 3;
        var data = new List<int> { 1, 2, 3, 4 };
        var isIn = item.In(data);
        isIn.Should().BeTrue();
    }

    [Test(Description = "Add a range of items to a collection.")]
    [Category("CollectionManipulation")]
    public void AddRange_AddsItemsToCollection()
    {
        var data = new List<int> { 1, 2, 3 };
        var additionalItems = new List<int> { 4, 5 };
        data.AddRange(additionalItems);
        data.Should().Equal(new List<int> { 1, 2, 3, 4, 5 });
    }
}