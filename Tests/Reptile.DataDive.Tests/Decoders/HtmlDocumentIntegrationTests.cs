using FluentAssertions;
using Reptile.DataDive.Decoders;

namespace Reptile.DataDive.Tests.Decoders;

[TestFixture]
public class HtmlDocumentIntegrationTests
{
    private HtmlParser _parser;

    [SetUp]
    public void Setup()
    {
        _parser = new HtmlParser(); // Assuming default constructor is available
    }

    [Test]
    [Category("Integration")]
    [Description("Parse and manipulate an HTML document and verify output")]
    public void ParseAndManipulateHtml_VerifyOutput()
    {
        // Arrange
        var html = "<html><head><title>Test</title></head><body><p>Hello World</p></body></html>";
        var expectedHtml = "<html><head><title>Modified</title></head><body><p>Hello Universe</p><div>New Element</div></body></html>";

        // Act
        var document = _parser.Parse(html);
        document.Find("title").Single().InnerHtml = "Modified";
        document.Find("p").Single().InnerHtml = "Hello Universe";
        document.RootNodes.Find("body").Single().Children.Add(new HtmlElementNode("div") { InnerHtml = "New Element" });

        var outputHtml = document.ToHtml();

        // Assert
        outputHtml.Should().Be(expectedHtml);
    }
}