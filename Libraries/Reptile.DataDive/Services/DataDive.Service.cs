using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Caching.Memory;
using Reptile.DataDive.Decoders;
using Reptile.DataDive.Services.Events;

namespace Reptile.DataDive.Services;

public class DataDiveService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> _visitedUrls = new HashSet<string>();
    private int _scrapedPages = 0;
    private int _totalUrls = 0;

    public event EventHandler<ProgressEventArgs> ReportProgress;

    public DataDiveService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<List<CustomHtmlDocument>> StartScraping(IEnumerable<string?> urls, bool useCache = false)
    {
        _totalUrls = urls.Count(); // Initialize total URLs count
        var docs = new List<CustomHtmlDocument?>();

        foreach (var url in urls)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                continue;

            var doc = await ScrapePage(url, useCache);
            if (doc != null)
            {
                doc.Url = url;
                docs.Add(doc);
            }
        }

        return docs.Where(d => d != null).Cast<CustomHtmlDocument>().ToList();
    }

    private async Task<CustomHtmlDocument?> ScrapePage(string url, bool useCache)
    {
        if (!_visitedUrls.Add(url))
            return null;

        var pageContent = await GetPageContent(url, useCache);
        if (string.IsNullOrEmpty(pageContent))
            return null;

        var document = await CustomHtmlDocument.FromHtmlAsync(pageContent);
        _scrapedPages++;
        ReportProgress?.Invoke(this,
            new ProgressEventArgs(url, CalculatePercentage(_scrapedPages, _totalUrls), _scrapedPages, _totalUrls));

        await ProcessLinks(document, useCache);
        return document;
    }

    [EnableCors("OpenCorsPolicy")]
    private async Task<string?> GetPageContent(string url, bool useCache)
    {
        if (useCache && _cache.TryGetValue(url, out string cachedContent))
            return cachedContent;

        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (useCache)
                _cache.Set(url, content, TimeSpan.FromMinutes(30));
            return content;
        }

        return null;
    }

    private async Task ProcessLinks(CustomHtmlDocument document, bool useCache)
    {
        foreach (var link in ExtractLinks(document))
        {
            if (_visitedUrls.Add(link)) // Check if the link has not been visited
            {
                await ScrapePage(link, useCache); // Recursively scrape the new link
            }
        }
    }

	private IEnumerable<string> ExtractLinks(CustomHtmlDocument document) => document.Find("a")
			.SelectMany(link => link.Attributes)
			.Where(attr => attr.Name == "href" && !string.IsNullOrEmpty(attr.Value))
			.Select(attr => attr.Value);

	private static double CalculatePercentage(int scrapedPages, int totalUrls) => totalUrls == 0 ? 0.0 : (double)scrapedPages / totalUrls * 100;

}