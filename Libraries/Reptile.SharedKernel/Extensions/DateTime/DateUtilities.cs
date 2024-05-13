namespace Reptile.SharedKernel.Extensions.DateTime;

public static class DateUtilities
{
	public static System.DateTime Min(System.DateTime t1, System.DateTime t2) => System.DateTime.Compare(t1, t2) > 0 ? t2 : t1;

	public static System.DateTime Max(System.DateTime t1, System.DateTime t2) => System.DateTime.Compare(t1, t2) < 0 ? t2 : t1;

	public static int CalculateAge(this System.DateTime dob, System.DateTime atDate)
    {
        var years = atDate.Year - dob.Year;
        if (atDate.Month < dob.Month || (atDate.Month == dob.Month && atDate.Day < dob.Day))
            --years;

        return years;
    }

	public static System.DateTime EndOfDay(this System.DateTime date) => date.Date.AddDays(1).AddSeconds(-1);
}