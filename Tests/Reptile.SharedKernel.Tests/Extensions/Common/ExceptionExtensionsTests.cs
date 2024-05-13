using FluentAssertions;
using Reptile.SharedKernel.Extensions.Common;

namespace Reptile.SharedKernel.Tests.Extensions.Common;

[TestFixture]
public class ExceptionExtensionsTests
{
    [Test(Description =
        "Unwrap an exception recursively to obtain the innermost exception, useful in dynamic invocation scenarios.")]
    [Category("ExceptionHandling")]
    public void UnrollDynamicallyInvokedException_ReturnsInnerMostException()
    {
        var innerException = new Exception("Inner");
        var exception = new Exception("Outer", innerException);

        var result = exception.UnrollDynamicallyInvokedException();

        result.SourceException.Should().Be(innerException);
    }

    [Test(Description = "Retrieve the deepest exception in a nested exception hierarchy.")]
    [Category("ExceptionHandling")]
    public void GetInnerMostException_ReturnsInnerMostException()
    {
        var innerException = new Exception("Inner");
        var exception = new Exception("Outer", innerException);

        var result = exception.GetInnerMostException();

        result.Should().Be(innerException);
    }
}