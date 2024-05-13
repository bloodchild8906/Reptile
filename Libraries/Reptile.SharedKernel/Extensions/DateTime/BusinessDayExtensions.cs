namespace Reptile.SharedKernel.Extensions.DateTime;

public static class BusinessDayExtensions
{
    public static System.DateTime AddBusinessDays(this System.DateTime current, int days)
    {
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);
        for (var i = 0; i < unsignedDays; i++)
            do
            {
                current = current.AddDays(sign);
            } while (current.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);

        return current;
    }

    public static double BusinessDaysBetween(this System.DateTime start, System.DateTime end)
    {
        var calcBusinessDays =
            1 + ((end - start).TotalDays * 5 - (start.DayOfWeek - end.DayOfWeek) * 2) / 7;

        if (end.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
        if (start.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

        return calcBusinessDays-1;
    }
}