using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;

namespace Reptile.DataDive.Decoders;

public class HtmlNodeCollection : List<HtmlNode>
{
    private HtmlElementNode _parentNode;
    public HtmlNodeCollection() : this(null)
    {
    }
    public HtmlNodeCollection(HtmlElementNode? parentNode = null) : base()
    {
        _parentNode = parentNode;
    }

	public List<HtmlNode> GetChildreNodes() => this.ToList();

	public new void Add(HtmlNode node) => AddNode(node);

	public T Add<T>(T node) where T : HtmlNode => (T)AddNode(node);

	private HtmlNode AddNode(HtmlNode node)
    {
        // Add node to the end of list
        base.Add(node);

        // if the list has more than one node
        if (Count > 1)
        {
            var prevNode = this[Count - 2];
            prevNode.NextNode = node;  // Set NextNode of previous node
            node.PrevNode = prevNode;  // Set PrevNode of current node
        }

        // Set ParentNode of current node
        node.ParentNode = _parentNode;

        return node;
    } 

    public new void AddRange(IEnumerable<HtmlNode> nodes)
    {
        foreach (var node in nodes)
            AddNode(node);
    }

    internal void SetNodes(IEnumerable<HtmlNode> nodes)
    {
        Clear();
        AddRange(nodes);
    }

    public new void Remove(HtmlNode node)
    {
        if (Contains(node)) 
            throw new ArgumentException("Node not found in list");

        var index = IndexOf(node);
        if (index > 0)
            this[index - 1].NextNode = node.NextNode;
        
        if (index < Count - 1)
            this[index + 1].PrevNode = node.PrevNode;
            
        base.Remove(node);
    }

    public new void RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException();

        var node = this[index];
        if (index > 0)
            this[index - 1].NextNode = node.NextNode;
        if (index < Count - 1)
            this[index + 1].PrevNode = node.PrevNode;
        base.RemoveAt(index);
    }
}
public abstract class HtmlNode
{
    
    public HtmlElementNode? ParentNode { get; internal set; }

    public HtmlNode? NextNode { get; internal set; }

    public HtmlNode? PrevNode { get; internal set; }


    [MemberNotNullWhen(false, nameof(ParentNode))]
    public bool IsTopLevelNode => ParentNode == null;

    public virtual string InnerHtml
    {
        get => string.Empty;
        set { }
    }

    public virtual string OuterHtml => string.Empty;

    public virtual string Text
    {
        get => string.Empty;
        set { }
    }

    public HtmlNode? NavigateNextNode()
    {
        var node = this;

        if (node is HtmlElementNode elementNode && elementNode.Children.Any())
            return elementNode.Children[0];

        while (node != null)
        {
            if (node.NextNode != null)
                return node.NextNode;
            node = node.ParentNode;
        }

        return null;
    }

    public HtmlNode? NavigatePrevNode()
    {
        var node = this;

        if (node.PrevNode == null) return node.ParentNode;
        node = node.PrevNode;
        while (node is HtmlElementNode elementNode && elementNode.Children.Any())
            node = elementNode.Children[^1];
        return node;

    }
}

public class HtmlHeaderNode(HtmlAttributeCollection attributes) : HtmlNode
{
    public HtmlHeaderNode() : this([])
    {
    }

    public HtmlAttributeCollection Attributes { get; } = attributes;

    public override string OuterHtml => string.Concat(HtmlRules.TagStart,
        HtmlRules.HtmlHeaderTag,
        Attributes.ToString(),
        HtmlRules.TagEnd);

    public override string ToString() => $"<{HtmlRules.HtmlHeaderTag} />";
}

public class XmlHeaderNode(HtmlAttributeCollection attributes, string innerHtml) : HtmlNode
{
    public XmlHeaderNode(string innerHtml) : this([], innerHtml)
    {
    }

    public HtmlAttributeCollection Attributes { get; } = attributes;

    public sealed override string InnerHtml { get; set; } = innerHtml;

    public override string OuterHtml => string.Concat(HtmlRules.TagStart,
        HtmlRules.XmlHeaderTag,
        Attributes.ToString(),
        "?",
        HtmlRules.TagEnd);

    public override string ToString() => $"<{HtmlRules.XmlHeaderTag} />";
}

public class HtmlElementNode : HtmlNode
{
    public HtmlElementNode(string? tagName, HtmlAttributeCollection? attributes = null)
    {
        TagName = tagName ?? string.Empty;
        Attributes = attributes ?? [];
        Children = new HtmlNodeCollection(this);
    }

    public HtmlElementNode(string? tagName, HtmlAttributeCollection? attributes, HtmlNodeCollection children)
    {
        TagName = tagName ?? string.Empty;
        Attributes = attributes ?? [];
        Children = children ?? throw new ArgumentNullException(nameof(children));
    }

    public string TagName { get; set; }
    public HtmlAttributeCollection Attributes { get; }
    public HtmlNodeCollection Children { get; }
    public bool IsSelfClosing => !Children.Any() && !HtmlRules.GetTagFlags(TagName).HasFlag(HtmlTagFlag.NoSelfClosing);

    public override string InnerHtml
    {
        get
        {
            if (!Children.Any())
                return string.Empty;
            StringBuilder builder = new();
            foreach (var node in Children)
                builder.Append(node.OuterHtml);
            return builder.ToString();
        }
        set
        {
            Children.Clear();
            if (string.IsNullOrEmpty(value)) return;
            var parser = new HtmlParser();
            Children.SetNodes(parser.ParseChildren(value));
        }
    }

    public override string OuterHtml
    {
        get
        {
            StringBuilder builder = new();

            builder.Append(HtmlRules.TagStart);
            builder.Append(TagName);
            builder.Append(Attributes);
            if (IsSelfClosing)
            {
                Debug.Assert(!Children.Any());
                builder.Append(' ');
                builder.Append(HtmlRules.ForwardSlash);
                builder.Append(HtmlRules.TagEnd);
            }
            else
            {
                builder.Append(HtmlRules.TagEnd);
                builder.Append(InnerHtml);
                builder.Append(HtmlRules.TagStart);
                builder.Append(HtmlRules.ForwardSlash);
                builder.Append(TagName);
                builder.Append(HtmlRules.TagEnd);
            }

            return builder.ToString();
        }
    }

    public override string Text
    {
        get
        {
            if (!Children.Any())
                return string.Empty;
            StringBuilder builder = new();
            foreach (var node in Children)
                builder.Append(node.Text);
            return builder.ToString();
        }
        set
        {
            Children.Clear();
            if (!string.IsNullOrEmpty(value))
                Children.Add(new HtmlTextNode { Text = value });
        }
    }

    public override string ToString() => $"<{TagName} />";
}

public class HtmlTextNode(string? html = null) : HtmlNode
{
    protected string Content = html ?? string.Empty;

    public override string InnerHtml
    {
        get => Content;
        set => Content = value;
    }
    public string EncodedInnerHtml
    {
        get => HttpUtility.HtmlEncode(Content);
        set => Content = value;
    }


    public override string OuterHtml => InnerHtml;

    public override string Text
    {
        get => WebUtility.HtmlDecode(Content);
        set => Content = WebUtility.HtmlEncode(value);
    }

    public override string ToString() => Text;
}

public class HtmlCDataNode(string prefix, string suffix, string html) : HtmlTextNode(html)
{
    public string Prefix { get; set; } = prefix;
    public string Suffix { get; set; } = suffix;

    public override string OuterHtml => $"{Prefix}{InnerHtml}{Suffix}";

    public override string Text
    {
        get => string.Empty;
        set { }
    }

    public override string ToString() => $"{Prefix}...{Suffix}";
}