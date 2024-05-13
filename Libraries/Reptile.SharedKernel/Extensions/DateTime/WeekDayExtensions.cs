namespace Reptile.SharedKernel.Extensions.DateTime;

public static class WeekDayExtensions
{
    public static int WeekDaysInMonthCount(this System.DateTime dt)
    {
        var year = dt.Year;
        var month = dt.Month;
        var days = System.DateTime.DaysInMonth(year, month);
        var dates = new List<System.DateTime>();

        for (var i = 1; i <= days; i++)
            dates.Add(new System.DateTime(year, month, i));

        return dates.Count(d => d.DayOfWeek is > DayOfWeek.Sunday and < DayOfWeek.Saturday);
    }

    public static IEnumerable<System.DateTime> WeekDaysInMonthList(this System.DateTime dt)
    {
        var year = dt.Year;
        var month = dt.Month;
        var days = System.DateTime.DaysInMonth(year, month);

        for (var i = 1; i <= days; i++)
        {
            var dt2 = new System.DateTime(year, month, i);
            if (dt2.DayOfWeek != DayOfWeek.Saturday && dt2.DayOfWeek != DayOfWeek.Sunday)
                yield return dt;
        }
    }
}