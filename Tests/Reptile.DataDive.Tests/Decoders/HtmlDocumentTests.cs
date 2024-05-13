using FluentAssertions;
using Moq;
using Reptile.DataDive.Decoders;

namespace Reptile.DataDive.Tests.Decoders
{
    [TestFixture]
    public class HtmlDocumentTests
    {
        private HtmlDocument _document;
        private Mock<HtmlNodeCollection> _mockRootNodes;

        [SetUp]
        public void Setup()
        {
            _mockRootNodes = new Mock<HtmlNodeCollection>();
            _document = new HtmlDocument(rootNodes: _mockRootNodes.Object);
        }

        [Test]
        [Category("Find Operations")]
        [Description("Ensure Find returns correct elements based on string selector")]
        public void Find_WithStringSelector_ReturnsCorrectElements()
        {
            // Arrange
            var rootNodes = new HtmlNodeCollection(null); // Assuming null is acceptable for the parent node
            rootNodes.Add(new HtmlElementNode("div"));
            rootNodes.Add(new HtmlElementNode("span"));
            rootNodes.Add(new HtmlElementNode("div")); // Add another 'div' to test multiple returns

            // HtmlDocument setup if necessary
            var document = new HtmlDocument(rootNodes: rootNodes);

            // Act
            var results = document.RootNodes.Find("div"); // Directly call the real Find method

            // Assert
            results.Should().HaveCount(2); // Assuming there are 2 divs added to the rootNodes
            results.All(node => node.TagName == "div").Should().BeTrue(); // Further assert that all found nodes are 'div's
        }
    }
}