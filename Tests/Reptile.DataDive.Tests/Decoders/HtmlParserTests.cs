using FluentAssertions;
using Moq;
using Reptile.DataDive.Decoders;

namespace Reptile.DataDive.Tests.Decoders
{
    [TestFixture]
    public class HtmlParserTests
    {
        private HtmlParser _parser;
        private Mock<TextParser> _mockTextParser;

        [SetUp]
        public void Setup()
        {
            _mockTextParser = new Mock<TextParser>(null); // Assuming constructor takes a nullable string
            _parser = new HtmlParser();
        }

        [Test]
        [Category("Parsing")]
        [Description("Ensure Parse method creates an HtmlDocument with correct structure from HTML string")]
        public void Parse_GivenHtmlString_ReturnsCorrectDocumentStructure()
        {
            // Arrange
            var html = "<html><head><title>Test</title></head><body>Hello World</body></html>";

            // Act
            var document = HtmlDocument.FromHtml(html);  // Using the static method from HtmlDocument

            // Assert
            document.Should().NotBeNull("because the parser should successfully create a document");
            document.RootNodes.Should().ContainSingle("because the document should have one root element");

            // Checking the structure of the <html> node
            var rootNode = (HtmlElementNode)document.RootNodes.First();
            rootNode.TagName.Should().Be("html", "because the root element should be <html>");
            rootNode.Children.Count.Should().Be(2, "because <html> should contain <head> and <body>");

            // Checking the contents of the <head> node
            var headNode = (HtmlElementNode)rootNode.Children.First(n => ((HtmlElementNode)n).TagName == "head");
            headNode.Should().NotBeNull("because <html> should contain a <head> element");
            headNode.Children.Count.Should().Be(1, "because <head> should contain a <title>");
            headNode.Children.First().InnerHtml.Should().Be("Test", "because the <title> should contain 'Test'");

            // Checking the contents of the <body> node
            var bodyNode = rootNode.Children.First(n => ((HtmlElementNode)n).TagName == "body");
            bodyNode.Should().NotBeNull("because <html> should contain a <body> element");
            bodyNode.InnerHtml.Should().Be("Hello World", "because the <body> should contain 'Hello World'");

            // Verify if document to HTML conversion is correct
            var outputHtml = document.ToHtml();
            outputHtml.Should().Be(html, "because converting the document back to HTML should give the original HTML");
        }



        // Additional tests for parsing attributes, children, text nodes, etc.
    }
}