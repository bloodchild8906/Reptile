using Reptile.DataDive.Decoders;

namespace Reptile.DataDive.Services.Events;

public class ScraperCompleteTask:EventArgs
{
    public ScraperCompleteTask(string? url, CustomHtmlDocument? doc)
    {
        Url = url;
        Document = doc;
    }
    public string? Url { get; private set; }
    public CustomHtmlDocument? Document { get; private set; }

}