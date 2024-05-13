using FluentAssertions;
using Reptile.DataDive.Decoders;

// Assuming this is the correct namespace

namespace Reptile.DataDive.Tests.Decoders;

[TestFixture]
public class HtmlTextNodeTests
{
    [Test]
    [Category("Text Handling")]
    [Description("Ensure HtmlTextNode handles HTML entities correctly")]
    public void HtmlTextNode_HandlesHtmlEntities_Correctly()
    {
        // Arrange
        var encodedText = "Hello &lt;World&gt;";
        var expectedDecodedText = "Hello <World>";

        var htmlTextNode = new HtmlTextNode(encodedText);

        // Act
        var actualDecodedText = htmlTextNode.Text;

        // Assert
        actualDecodedText.Should().Be(expectedDecodedText, 
            "because HtmlTextNode should correctly handle HTML entities");
    }

    [Test]
    [Category("Text Serialization")]
    [Description("Ensure HtmlTextNode serializes text correctly in HTML context")]
    public void HtmlTextNode_SerializesText_Correctly()
    {
        // Arrange
        var rawText = "Hello <World>";
        var expectedSerializedText = "Hello &lt;World&gt;";
    
        var htmlTextNode = new HtmlTextNode(rawText);
    
        // Act
        var actualSerializedText = htmlTextNode.EncodedInnerHtml;

        // Assert
        actualSerializedText.Should().Be(expectedSerializedText,
            "because HtmlTextNode should correctly serialize special HTML characters");
    }
}