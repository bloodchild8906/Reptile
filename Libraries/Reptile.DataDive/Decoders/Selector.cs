using System.Diagnostics;

namespace Reptile.DataDive.Decoders;

public class SelectorCollection : List<Selector>
{
    public IEnumerable<HtmlElementNode> Find(HtmlNode rootNode) => Find(new[] { rootNode });

    public IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes) =>
        this.SelectMany(s => s.Find(nodes))
            .Distinct();


    internal Selector AddChildSelector()
    {
        var selector = GetLastSelector();
        Debug.Assert(selector != null);
        Debug.Assert(selector.ChildSelector == null);
        selector.ChildSelector = new Selector();
        return selector.ChildSelector;
    }

    internal Selector GetLastSelector()
    {
        Selector selector;

        if (Count > 0)
        {
            // Get last selector
            selector = this[Count - 1];
            // Get last child selector (return selector if no children)
            while (selector.ChildSelector != null)
                selector = selector.ChildSelector;
            return selector;
        }

        selector = new Selector();
        Add(selector);
        return selector;
    }

    internal void RemoveEmptySelectors()
    {
        for (var i = Count - 1; i >= 0; i--)
        {
            var selector = RemoveEmptyChildSelectors(this[i]);
            if (selector == null)
                RemoveAt(i);
            else if (!Equals(this[i], selector))
                this[i] = selector;
        }
    }

    private static Selector? RemoveEmptyChildSelectors(Selector? selector)
    {
        var parent = selector;
        Selector? prev = null;

        while (selector != null)
        {
            if (selector.IsEmpty)
            {
                if (prev == null)
                    parent = selector.ChildSelector;
                else
                    prev.ChildSelector = selector.ChildSelector;
            }
            else
            {
                prev = selector;
            }

            selector = selector.ChildSelector;
        }

        return parent;
    }
}
public class Selector
{
    private static readonly Dictionary<char, string> SpecialCharacters = new()
    {
        { '#', "id" },
        { '.', "class" },
        { ':', "type" }
    };

    public string? Tag { get; set; }
    public List<AttributeSelector> Attributes { get; } = [];
    public Selector? ChildSelector { get; set; }

    public bool ImmediateChildOnly { get; set; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Tag) && Attributes.Count == 0;

    public bool IsMatch(HtmlElementNode node) =>
        (string.IsNullOrWhiteSpace(Tag) || string.Equals(Tag, node.TagName, HtmlRules.TagStringComparison)) &&
        Attributes.All(selector => selector.IsMatch(node));

    public IEnumerable<HtmlElementNode> Find(HtmlNode rootNode) => Find(new[] { rootNode });

    public IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes)
    {
        List<HtmlElementNode>? results = null;
        var matchTopLevelNodes = true;

        for (var selector = this; selector != null; selector = selector.ChildSelector)
        {
            results = new List<HtmlElementNode>();
            FindRecursive(nodes, selector, matchTopLevelNodes, true, results);
            nodes = results;
            matchTopLevelNodes = false;
        }

        return results?.Distinct() ?? Enumerable.Empty<HtmlElementNode>();
    }

    private void FindRecursive(IEnumerable<HtmlNode> nodes, Selector selector,
        bool matchTopLevelNodes, bool recurse, ICollection<HtmlElementNode> results)
    {
        Debug.Assert(matchTopLevelNodes || recurse);

        foreach (var node in nodes)
            if (node is HtmlElementNode elementNode)
            {
                if (matchTopLevelNodes && selector.IsMatch(elementNode))
                    results.Add(elementNode);

                if (recurse)
                    FindRecursive(elementNode.Children, selector, true, !selector.ImmediateChildOnly, results);
            }
    }

    public override string ToString() => Tag ?? "(null)";

    public static SelectorCollection ParseSelector(string? selectorText)
    {
        SelectorCollection selectors = [];

        if (!string.IsNullOrWhiteSpace(selectorText))
        {
            TextParser parser = new(selectorText);
            parser.SkipWhiteSpace();

            while (!parser.EndOfText)
            {
                var ch = parser.Peek();
                if (IsNameCharacter(ch) || ch == '*')
                {
                    var selector = selectors.GetLastSelector();
                    selector.Tag = ch == '*' ? null : 
                        parser.ParseWhile(IsNameCharacter);
                }
                else if (SpecialCharacters.TryGetValue(ch, out var name))
                {
                    parser.Next();
                    var value = parser.ParseWhile(IsValueCharacter);
                    if (value.Length <= 0) continue;
                    AttributeSelector attribute = new(name, value)
                    {
                        Mode = AttributeSelectorMode.Contains
                    };

                    var selector = selectors.GetLastSelector();
                    selector.Attributes.Add(attribute);
                }
                else switch (ch)
                {
                    case '[':
                    {
                        parser.Next();
                        parser.SkipWhiteSpace();
                        name = parser.ParseWhile(IsNameCharacter);
                        if (name.Length > 0)
                        {
                            AttributeSelector attribute = new(name);

                            parser.SkipWhiteSpace();
                            if (parser.Peek() == '=')
                            {
                                attribute.Mode = AttributeSelectorMode.Match;
                                parser.Next();
                            }
                            else if (parser.Peek() == ':' && parser.Peek(1) == '=')
                            {
                                attribute.Mode = AttributeSelectorMode.RegEx;
                                parser.Index += 2;
                            }
                            else
                            {
                                attribute.Mode = AttributeSelectorMode.ExistsOnly;
                            }

                            if (attribute.Mode != AttributeSelectorMode.ExistsOnly)
                            {
                                parser.SkipWhiteSpace();
                                attribute.Value = HtmlRules.IsQuoteChar(parser.Peek()) ? parser.ParseQuotedText() : parser.ParseWhile(IsValueCharacter);
                            }

                            var selector = selectors.GetLastSelector();
                            selector.Attributes.Add(attribute);
                        }

                        parser.SkipWhiteSpace();
                        if (parser.Peek() == ']')
                            parser.Next();
                        break;
                    }
                    case ',':
                        parser.Next();
                        parser.SkipWhiteSpace();
                        selectors.Add(new Selector());
                        break;
                    case '>':
                    {
                        parser.Next();
                        parser.SkipWhiteSpace();
                        var selector = selectors.AddChildSelector();
                        selector.ImmediateChildOnly = true;
                        break;
                    }
                    default:
                    {
                        if (char.IsWhiteSpace(ch))
                        {
                            parser.SkipWhiteSpace();
                            if (parser.Peek() != ',' && parser.Peek() != '>')
                                selectors.AddChildSelector();
                        }
                        else
                        {
                            parser.Next();
                        }

                        break;
                    }
                }
            }
        }

        selectors.RemoveEmptySelectors();
        return selectors;
    }

    private static bool IsNameCharacter(char c) => char.IsLetterOrDigit(c) || c == '-';

    private static bool IsValueCharacter(char c) => IsNameCharacter(c);
}