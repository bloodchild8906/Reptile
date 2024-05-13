using System.Collections.Specialized;
using FluentAssertions;
using Reptile.SharedKernel.Extensions.Common;

namespace Reptile.SharedKernel.Tests.Extensions.Common;

[TestFixture]
public class HttpExtensionsTests
{
    [Test(Description = "Format a NameValueCollection into a URL-encoded query string without a leading '?'.")]
    [Category("HttpUtilities")]
    public void ToQueryString_FormatsNameValueCollectionIntoQueryString()
    {
        var coll = new NameValueCollection { { "key", "value" }, { "anotherKey", "anotherValue" } };
        var result = coll.ToQueryString();

        result.Should().Be("key=value&anotherKey=anotherValue");
    }

    [Test(Description =
        "Format a NameValueCollection into a query string and prepend a '?' to make it suitable for URL concatenation.")]
    [Category("HttpUtilities")]
    public void ToQueryString_UsesQuestionMarkIfSpecified()
    {
        var coll = new NameValueCollection { { "key", "value" } };
        var result = coll.ToQueryString(true);

        result.Should().Be("?key=value");
    }
}