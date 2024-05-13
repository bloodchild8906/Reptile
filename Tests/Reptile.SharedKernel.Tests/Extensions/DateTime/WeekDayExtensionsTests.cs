using FluentAssertions;
using Reptile.SharedKernel.Extensions.DateTime;

namespace Reptile.SharedKernel.Tests.Extensions.DateTime;

[TestFixture]
public class WeekDayExtensionsTests
{
    [Test(Description = "Count weekdays in a typical month.")]
    [Category("WeekDayCalculations")]
    public void WeekDaysInMonthCount_RegularMonth_CorrectCount()
    {
        var dt = new System.DateTime(2023, 5, 1);
        var result = dt.WeekDaysInMonthCount();

        result.Should().Be(23);
    }

    [Test(Description = "List weekdays in a month.")]
    [Category("WeekDayCalculations")]
    public void WeekDaysInMonthList_RegularMonth_CorrectList()
    {
        var dt = new System.DateTime(2023, 5, 1);
        var weekdays = dt.WeekDaysInMonthList();

        var dateTimes = weekdays.ToList();
        dateTimes.Should().NotBeEmpty();
        dateTimes.All(day =>
        {
            if (day.DayOfWeek != DayOfWeek.Saturday) return true;
            return day.DayOfWeek != DayOfWeek.Sunday;
        }).Should().BeTrue();
    }
}