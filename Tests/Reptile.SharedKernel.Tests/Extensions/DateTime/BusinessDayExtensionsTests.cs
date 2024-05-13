using FluentAssertions;
using Reptile.SharedKernel.Extensions.DateTime;

namespace Reptile.SharedKernel.Tests.Extensions.DateTime;

[TestFixture]
public class BusinessDayExtensionsTests
{
    [Test(Description = "Test that business days are correctly added when the number is positive.")]
    [Category("BusinessDayCalculations")]
    public void AddBusinessDays_PositiveDays_AddsOnlyBusinessDays()
    {
        var start = new System.DateTime(2023, 5, 5); // Friday
        var result = start.AddBusinessDays(3); // Should skip Saturday and Sunday

        result.Should().Be(new System.DateTime(2023, 5, 10)); // Next Wednesday
    }

    [Test(Description = "Test that business days are correctly subtracted when the number is negative.")]
    [Category("BusinessDayCalculations")]
    public void AddBusinessDays_NegativeDays_SubtractsOnlyBusinessDays()
    {
        var start = new System.DateTime(2023, 5, 5); // Friday
        var result = start.AddBusinessDays(-3); // Should skip Weekend going backwards

        result.Should().Be(new System.DateTime(2023, 5, 2)); // Previous Tuesday
    }

    [Test(Description = "Verify that the method calculates the correct number of business days within a week.")]
    [Category("BusinessDayCalculations")]
    public void BusinessDaysBetween_WeekSpan_CalculatesOnlyBusinessDays()
    {
        var start = new System.DateTime(2023, 5, 1); // Monday
        var end = new System.DateTime(2023, 5, 8); // Next Monday

        var result = start.BusinessDaysBetween(end);

        result.Should().Be(5); // 5 business days in one week
    }

    [Test(Description = "Check calculation adjustment when the start day is a weekend.")]
    [Category("BusinessDayCalculations")]
    public void BusinessDaysBetween_StartOnWeekend_AdjustsStartDate()
    {
        var start = new System.DateTime(2023, 4, 30); // Sunday
        var end = new System.DateTime(2023, 5, 8); // Next Monday

        var result = start.BusinessDaysBetween(end);

        result.Should().BeGreaterOrEqualTo(5);
    }
}