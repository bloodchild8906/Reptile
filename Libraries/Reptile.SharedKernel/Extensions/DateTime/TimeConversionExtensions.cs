namespace Reptile.SharedKernel.Extensions.DateTime;

public static class TimeConversionExtensions
{
    private static readonly System.DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static System.DateTime FromUnixTimeToDateTimeUtc(this long secondsSinceEpoch) => Epoch.AddSeconds(secondsSinceEpoch);

	public static long DateTimeUtcToUnixTime(this System.DateTime dateTime) => (long)(dateTime - Epoch).TotalSeconds;
}