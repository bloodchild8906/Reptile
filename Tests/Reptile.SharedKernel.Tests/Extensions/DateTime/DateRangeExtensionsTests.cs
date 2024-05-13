using FluentAssertions;
using Reptile.SharedKernel.Extensions.DateTime;

namespace Reptile.SharedKernel.Tests.Extensions.DateTime;

[TestFixture]
public class DateRangeExtensionsTests
{
    [TestCase("2023-05-10", "2023-05-01", "2023-05-15", true,
        Description = "Check if a date is correctly identified as within the range including boundaries.")]
    [Category("DateRangeChecks")]
    public void IsBetween_WithBoundaryInclusion_VerifyBehavior(System.DateTime input, System.DateTime start,
        System.DateTime end,
        bool expected)
    {
        var result = input.IsBetween(start, end, true);

        result.Should().Be(expected);
    }

    [TestCase("2023-05-10", "2023-05-01", "2023-05-15", true,
        Description = "Check if a date within the range is identified correctly excluding boundaries.")]
    [TestCase("2023-05-01", "2023-05-01", "2023-05-15", false,
        Description = "Ensure start boundary is excluded in the check.")]
    [TestCase("2023-05-15", "2023-05-01", "2023-05-15", false,
        Description = "Ensure end boundary is excluded in the check.")]
    [Category("DateRangeChecks")]
    public void IsBetween_WithoutBoundaryInclusion_VerifyBehavior(System.DateTime input, System.DateTime start,
        System.DateTime end,
        bool expected)
    {
        var result = input.IsBetween(start, end, false);

        result.Should().Be(expected);
    }
}