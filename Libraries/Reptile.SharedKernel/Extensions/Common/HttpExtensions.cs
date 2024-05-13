using System.Collections.Specialized;
using System.Web;

namespace Reptile.SharedKernel.Extensions.Common;

public static class HttpExtensions
{
    public static string ToQueryString(this NameValueCollection coll, bool useQuestionMarkToStart = false)
    {
        var qs = string.Join("&",
            coll.Cast<string>().Select(a => $"{HttpUtility.UrlEncode(a)}={HttpUtility.UrlEncode(coll[a])}"));

        if (useQuestionMarkToStart) return "?" + qs;

        return qs;
    }
}