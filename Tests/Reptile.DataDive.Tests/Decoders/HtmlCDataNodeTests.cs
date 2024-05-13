using FluentAssertions;
using Reptile.DataDive.Decoders;

namespace Reptile.DataDive.Tests.Decoders;

[TestFixture]
public class HtmlCDataNodeTests
{
    [Test]
    [Category("CDATA Handling")]
    [Description("Ensure HtmlCDataNode preserves content within CDATA sections")]
    public void HtmlCDataNode_PreservesContent_Correctly()
    {
        // Arrange
        var content = "Some <code>code</code> & data;";
        var node = new HtmlCDataNode("<![CDATA[", "]]>", content);

        // Act & Assert
        node.Text.Should().BeEmpty(); // CDATA text should not be output as readable text
        node.InnerHtml.Should().Be(content);
        node.OuterHtml.Should().Be($"<![CDATA[{content}]]>");
    }
}