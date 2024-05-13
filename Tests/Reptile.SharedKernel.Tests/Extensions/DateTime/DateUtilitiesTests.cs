using FluentAssertions;
using Reptile.SharedKernel.Extensions.DateTime;

namespace Reptile.SharedKernel.Tests.Extensions.DateTime;

[TestFixture]
public class DateUtilitiesTests
{
    [TestCase("2023-05-10", "2023-05-20", Description = "Min should return the earlier of two dates.")]
    [TestCase("2023-05-20", "2023-05-10",
        Description = "Min should still return the earlier date regardless of order.")]
    [Category("DateUtilities")]
    public void Min_ReturnsMinimumDate(System.DateTime t1, System.DateTime t2)
    {
        var result = DateUtilities.Min(t1, t2);
        var expected = t1 < t2 ? t1 : t2;

        result.Should().Be(expected);
    }

    [TestCase("2023-05-10", "2023-05-20", Description = "Max should return the later of two dates.")]
    [TestCase("2023-05-20", "2023-05-10", Description = "Max should still return the later date regardless of order.")]
    [Category("DateUtilities")]
    public void Max_ReturnsMaximumDate(System.DateTime t1, System.DateTime t2)
    {
        var result = DateUtilities.Max(t1, t2);
        var expected = t1 > t2 ? t1 : t2;

        result.Should().Be(expected);
    }

    [Test(Description = "Check if the age calculation is correct when the birthday has not occurred yet this year.")]
    [Category("DateUtilities")]
    public void CalculateAge_BeforeBirthday_ReturnsCorrectAge()
    {
        var dob = new System.DateTime(1990, 12, 15);
        var atDate = new System.DateTime(2023, 5, 1);

        var result = dob.CalculateAge(atDate);

        result.Should().Be(32);
    }

    [Test(Description = "Verify the EndOfDay method returns the very last second of the day.")]
    [Category("DateUtilities")]
    public void EndOfDay_ReturnsLastSecond()
    {
        var date = new System.DateTime(2023, 5, 1);
        var result = date.EndOfDay();

        result.Should().Be(new System.DateTime(2023, 5, 1, 23, 59, 59));
    }
}