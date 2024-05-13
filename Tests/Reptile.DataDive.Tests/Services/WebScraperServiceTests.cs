using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using Reptile.DataDive.Services;
using Reptile.DataDive.Services.Contracts;

namespace Reptile.DataDive.Tests.Services;

[TestFixture(Category = "Web Scraper Service Tests", TestOf = typeof(WebScraperServiceTests),
    Description = "Test Suite for the Web Scraper Service", TestName = "WebScraperServiceTestSuite")]
public class WebScraperServiceTests
{
    private Mock<HttpMessageHandler> _handlerMock;
    private IWebScraperService _scraperService;
    private Mock<IMemoryCache?> _cacheMock;
    private MemoryCacheEntryOptions _cacheEntryOptions;
    private const string BaseUrl = "http://example.com";

    [SetUp]
    public void Setup()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_handlerMock.Object) { BaseAddress = new Uri(BaseUrl) };
        _cacheMock = new Mock<IMemoryCache?>();
        _cacheEntryOptions = new MemoryCacheEntryOptions();

        // Set up the cache
        var cache = new Dictionary<object, object?>();
        _cacheMock.Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns((object key) =>
            {
                var entry = new Mock<ICacheEntry>();
                entry.SetupProperty(e => e.Value); // Makes Value settable
                entry.Setup(e => e.Dispose()).Callback(() => cache[key] = entry.Object.Value);
                return entry.Object;
            });

        _cacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
            .Returns((object key, out object value) =>
            {
                value = (cache.GetValueOrDefault(key)) ?? new object();
                return value != null;
            });

        _scraperService =new WebScraperService();
        _scraperService.SetHttpClient(httpClient);
        _scraperService.SetMemoryCache(_cacheMock.Object);
    }

    [Test]
    [Category("Content Fetching")]
    [Description("Ensure the service correctly fetches page content using HttpClient")]
    public async Task FetchPageContent_ShouldRetrieveFromNetworkAndCache()
    {
        string? url = "http://example.com/test";
        string contentFromNetwork = "<html>Content from Network</html>";
        object returnCacheObject;

        // Setup the _handlerMock to return specific content when the HttpClient makes a request to a specific URL
        _handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == url),
            ItExpr.IsAny<System.Threading.CancellationToken>()
        ).ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(contentFromNetwork)
        });

        // Call the service function
        string? content = await _scraperService.GetPageContent(url, true);

        // Assert the content matches the expected network content
        content.Should().Be(contentFromNetwork);

        // Verify that the HttpClient was called once to fetch the content
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<System.Threading.CancellationToken>());

        // Verify the content was cached properly - we check if the cache method was called once for the URL
        _cacheMock.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [Test]
    [Category("Cache Handling")]
    [Description("Verify that the service uses cached content if available")]
    public async Task FetchPageContent_ShouldUseCachedContentIfAvailable()
    {
        var url = "http://example.com/test";
        var cachedContent = "<html>Cached</html>";
        object expectedContent = cachedContent; // Box to allow reference comparison in mock setup

        _cacheMock.Setup(x => x.TryGetValue(url, out expectedContent)).Returns(true);

        var content = await _scraperService.GetPageContent(url, true);
        content.Should().Be(cachedContent);

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<System.Threading.CancellationToken>());
    }

    [Test]
    [Category("API Handling")]
    [Description("Ensure the service correctly processes JSON data from an API")]
    public async Task ScrapeAPI_ShouldProcessJsonDataCorrectly()
    {
        var apiUrl = "http://example.com/api/data";
        var jsonData = "{\"key\":\"value\"}";

        _handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == apiUrl),
            ItExpr.IsAny<System.Threading.CancellationToken>()
        ).ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonData)
        });

        JObject processedData = null;
        await _scraperService.ScrapeAPI(apiUrl, (data) =>
        {
            processedData = (JObject)data;
            return Task.CompletedTask;
        });

        (processedData?["key"] ?? new JObject()).Value<string>().Should().Be("value");
    }
    
    [Test]
    [Category("Cache Management")]
    [Description("Ensure GetPageContent creates an entry in the cache for newly fetched contents")]
    public async Task FetchPageContent_InsertNewContentIntoCache()
    {
        var url = "http://example.com/newcontent";
        var fetchedContent = "<html>New Content</html>";
    
        // If the content is not found in cache, create a new entry
        _cacheMock.Setup(x => x.TryGetValue(url, out It.Ref<object>.IsAny!)).Returns(false);
    
        // Add setup for new fetched content from page
        _handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<System.Threading.CancellationToken>()
        ).ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(fetchedContent)
        });
    
        // Call the service function
        var content = await _scraperService.GetPageContent(url, true);

        // Verify the content is indeed fetched from the page
        content.Should().Be(fetchedContent);

        // Verify an entry has been made in the cache
        _cacheMock.Verify(x => x.CreateEntry(url), Times.Once);
    }
    
    [Test]
    [Category("Cache Efficiency")]
    [Description("Verify that HttpClient request is skipped when content is already available in cache")]
    public async Task FetchPageContent_ShouldNotCallHttpClientForCachedContents()
    {
        var url = "http://example.com/cachecontent";
        var cachedContent = "<html>Cached Content</html>";
        object? cacheReturnValue = cachedContent;

        // If the content is found in cache, return the cached content
        _cacheMock.Setup(x => x.TryGetValue(url, out cacheReturnValue)).Returns(true);

        // Call the service function
        var content = await _scraperService.GetPageContent(url, true);

        // Verify the content is indeed fetched from the cache
        content.Should().Be(cachedContent);

        // Verify that HttpClient request was never called
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<System.Threading.CancellationToken>());
    }
    
    
    
}