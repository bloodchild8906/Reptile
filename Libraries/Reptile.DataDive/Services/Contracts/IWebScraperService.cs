using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Reptile.DataDive.Decoders;

namespace Reptile.DataDive.Services.Contracts;

public interface IWebScraperService
{
    Task<List<CustomHtmlDocument?>> StartScraping(IEnumerable<string?>? specificUrls = null, bool useCache = false);
    Task<List<CustomHtmlDocument?>> StartScraping(string specificUrls = null, bool useCache = false);
    Task<CustomHtmlDocument?> ScrapePage(string? url);

    Task ScrapeApi(string? apiUrl, Func<JToken, Task> processData);

    // public bool IsApi { get; set; }
    void SetMemoryCache(IMemoryCache? cache);
    void SetHttpClient(HttpClient httpClient);
}