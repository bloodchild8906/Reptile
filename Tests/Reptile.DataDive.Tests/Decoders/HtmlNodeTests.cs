using FluentAssertions;
using Reptile.DataDive.Decoders;

namespace Reptile.DataDive.Tests.Decoders
{
    [TestFixture]
    public class HtmlNodeTests
    {
        private HtmlElementNode _elementNode;

        [SetUp]
        public void Setup()
        {
            _elementNode = new HtmlElementNode("div");
        }

        [Test]
        [Category("Node Properties")]
        [Description("Ensure HtmlElementNode initializes with correct tag name and empty children")]
        public void HtmlElementNode_Initialization_CorrectSetup()
        {
            // Assert
            _elementNode.TagName.Should().Be("div");
            _elementNode.Children.Should().BeEmpty();
        }

        [Test]
        [Category("Node Navigation")]
        [Description("Ensure that navigating to next and previous nodes correctly follows node links")]
        public void NavigateNextPrevNode_CorrectTraversal()
        {
            // Arrange
            var childNode1 = new HtmlElementNode("p");
            var childNode2 = new HtmlElementNode("span");
            _elementNode.Children.Add(childNode1);
            _elementNode.Children.Add(childNode2);

            // Act & Assert
            childNode1.NavigateNextNode().Should().BeSameAs(childNode2);
            childNode2.NavigatePrevNode().Should().BeSameAs(childNode1);
        }
    }
}