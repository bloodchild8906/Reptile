using FluentAssertions;
using Reptile.DataDive.Decoders;

namespace Reptile.DataDive.Tests.Decoders;

[TestFixture]
public class HtmlDocumentPerformanceTests
{
    private HtmlParser _parser;

    [SetUp]
    public void Setup()
    {
        _parser = new HtmlParser();
    }

    [Test]
    [Category("Performance")]
    [Description("Measure parsing time for a large HTML document")]
    public void ParseLargeHtml_MeasurePerformance()
    {
        // Arrange
        var largeHtml = new string('<', 10000) + "div>" + new string('a', 100000) + "</div>" + new string('>', 10000);

        // Act
        var watch = System.Diagnostics.Stopwatch.StartNew();
        _parser.Parse(largeHtml);
        watch.Stop();

        // Assert
        watch.ElapsedMilliseconds.Should().BeLessThan(1000, "Parsing should be performant even for large documents");
    }
}