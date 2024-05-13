namespace Reptile.DataDive.Decoders;

public class CustomHtmlDocument
{
    public string Url { get; set; }
    public HtmlNodeCollection RootNodes { get; }

    public CustomHtmlDocument()
    {
        RootNodes = new HtmlNodeCollection();
    }

	// Method to set nodes directly for this document
	public void SetNodes(IEnumerable<HtmlNode> nodes) => RootNodes.SetNodes(nodes);

	// Parse HTML content and set the root nodes
	public static CustomHtmlDocument Parse(string htmlContent)
    {
        var parser = new HtmlParser();
        var nodes = parser.ParseChildren(htmlContent);
        var document = new CustomHtmlDocument();
        document.SetNodes(nodes);
        return document;
    }

	// Utility to get HTML content of the document
	public string ToHtml() => RootNodes.Count > 0 ? string.Concat(RootNodes.Select(n => n.OuterHtml)) : string.Empty;

	// Find elements by tag name
	public IEnumerable<HtmlElementNode> Find(string tagName) => RootNodes.FindOfType<HtmlElementNode>(node => string.Equals(node.TagName, tagName, StringComparison.OrdinalIgnoreCase));

	// Find elements by a complex selector, using the Selector class
	public IEnumerable<HtmlElementNode> Find(Selector selector) => selector.Find(RootNodes);

	public static async ValueTask<CustomHtmlDocument> FromHtmlAsync(string pageContent) => CustomHtmlDocument.Parse(pageContent);
}