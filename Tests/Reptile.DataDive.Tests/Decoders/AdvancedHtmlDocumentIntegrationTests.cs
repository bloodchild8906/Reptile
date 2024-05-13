using FluentAssertions;
using Reptile.DataDive.Decoders;
using Reptile.DataDive.Services;

namespace Reptile.DataDive.Tests.Decoders;

[TestFixture( Category = "Integration Tests", TestOf = typeof(WebScraperService), Description = "", TestName = "AdvancedHtmlDocumentIntegrationTests")]
public class AdvancedHtmlDocumentIntegrationTests
{
    private HtmlParser _parser;

    [SetUp]
    public void Setup() => _parser = new HtmlParser();

    [Test]
    [Category("Advanced Integration")]
    [Description("Perform complex manipulations on parsed HTML document and verify final output")]
    public void ComplexManipulationAndOutputVerification()
    {
        // Arrange
        const string html = "<html><body><div id=\"container\"><p>Original Text</p></div></body></html>";
        const string expectedHtml = "<html><body><div id=\"container\"><p>Updated Text</p><div>New Content</div></div></body></html>";

        // Act
        var document = _parser.Parse(html);
        var pTag = document.Find("p").Single();
        pTag.InnerHtml = "Updated Text";
        var container = document.Find("#container").Single();
        container.Children.Add(new HtmlElementNode("div") { InnerHtml = "New Content" });

        var outputHtml = document.ToHtml();

        // Assert
        outputHtml.Should().Be(expectedHtml);
    }
}