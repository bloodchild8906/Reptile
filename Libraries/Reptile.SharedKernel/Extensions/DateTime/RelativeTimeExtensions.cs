using System.Globalization;

namespace Reptile.SharedKernel.Extensions.DateTime;

public static class RelativeTimeExtensions
{
    public static string RelativeFormat(this System.DateTime source, string defaultFormat)
    {
        var ts = new TimeSpan(System.DateTime.UtcNow.Ticks - source.Ticks);
        var delta = ts.TotalSeconds;
        var result = delta > 0 ? delta switch
            {
                < 60 => ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago",
                < 120 => "a minute ago",
                < 2700 => ts.Minutes + " minutes ago",
                < 5400 => "an hour ago",
                < 86400 => ts.Hours + " hours ago",
                < 172800 => "yesterday",
                < 2592000 => ts.Days + " days ago",
                < 31104000 => Convert.ToInt32(Math.Floor((double)ts.Days / 30)) + " months ago",
                _ => Convert.ToInt32(Math.Floor((double)ts.Days / 365)) + " years ago"
            } :
            !string.IsNullOrEmpty(defaultFormat) ? source.ToString(defaultFormat) :
            source.ToString(CultureInfo.InvariantCulture);

        return result;
    }
}