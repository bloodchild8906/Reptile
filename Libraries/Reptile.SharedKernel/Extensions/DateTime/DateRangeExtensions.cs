namespace Reptile.SharedKernel.Extensions.DateTime;

public static class DateRangeExtensions
{
	public static bool IsBetween(this System.DateTime input, System.DateTime start, System.DateTime end) => input.IsBetween(start, end, true);

	public static bool IsBetween(this System.DateTime input, System.DateTime start, System.DateTime end,
		bool includeBoundaries) => includeBoundaries ? input >= start && input <= end : input > start && input < end;
}