using System.Diagnostics;

namespace Reptile.DataDive.Decoders;

public class HtmlParser
{
    private readonly TextParser _parser = new(null);


    public CustomHtmlDocument? Parse(string? html)
    {
        CustomHtmlDocument? document = new();
        document.RootNodes.SetNodes(ParseChildren(html));
        return document;
    }

    public IEnumerable<HtmlNode> ParseChildren(string? html, bool ignoreHtmlRules = false)
    {
        HtmlElementNode rootNode = new("[Temp]");
        var parentNode = rootNode;
        _parser.Reset(html);

        while (!_parser.EndOfText)
        {
            if (_parser.Peek() == HtmlRules.TagStart)
            {
                var definition = HtmlRules.CDataDefinitions.FirstOrDefault(dd =>
                    _parser.MatchesCurrentPosition(dd.StartText, dd.StartComparison));
                if (definition != null)
                {
                    parentNode.Children.Add(ParseCDataNode(definition));
                    continue;
                }

                string? tag;
                if (_parser.Peek(1) == HtmlRules.ForwardSlash)
                {
                    _parser.Index += 2;
                    tag = _parser.ParseWhile(HtmlRules.IsTagCharacter);
                    if (tag.Length > 0)
                    {
                        if (parentNode.TagName.Equals(tag, HtmlRules.TagStringComparison))
                        {
                            if (!parentNode.IsTopLevelNode)
                                parentNode = parentNode.ParentNode;
                        }
                        else
                        {
                            var tagPriority = HtmlRules.GetTagNestLevel(tag);

                            while (!parentNode.IsTopLevelNode &&
                                   tagPriority > HtmlRules.GetTagNestLevel(parentNode.TagName))
                                parentNode = parentNode.ParentNode;

                            if (parentNode.TagName.Equals(tag, HtmlRules.TagStringComparison))
                                if (!parentNode.IsTopLevelNode)
                                    parentNode = parentNode.ParentNode;
                        }
                    }

                    _parser.SkipTo(HtmlRules.TagEnd);
                    _parser.Next();
                    continue;
                }

                if (ParseTag(out tag))
                {
                    var tagFlags = ignoreHtmlRules ? HtmlTagFlag.None : HtmlRules.GetTagFlags(tag);
                    if (tagFlags.HasFlag(HtmlTagFlag.HtmlHeader))
                    {
                        parentNode.Children.Add(ParseHtmlHeader());
                    }
                    else if (tagFlags.HasFlag(HtmlTagFlag.XmlHeader))
                    {
                        parentNode.Children.Add(ParseXmlHeader());
                    }
                    else
                    {
                        var attributes = ParseAttributes();
                        bool selfClosing;
                        if (_parser.Peek() == HtmlRules.ForwardSlash)
                        {
                            _parser.Next();
                            _parser.SkipWhiteSpace();
                            selfClosing = true;
                        }
                        else
                        {
                            selfClosing = false;
                        }

                        _parser.SkipTo(HtmlRules.TagEnd);
                        _parser.Next();

                        HtmlElementNode node = new(tag, attributes);
                        while (!HtmlRules.TagMayContain(parentNode.TagName, tag) && !parentNode.IsTopLevelNode)
                            parentNode = parentNode.ParentNode;
                        parentNode.Children.Add(node);

                        if (tagFlags.HasFlag(HtmlTagFlag.CData))
                        {
                            if (!selfClosing)
                                if (ParseToClosingTag(tag, out var content) && content?.Length > 0)
                                    node.Children.Add(new HtmlCDataNode(string.Empty, string.Empty, content));
                        }
                        else
                        {
                            if (selfClosing && tagFlags.HasFlag(HtmlTagFlag.NoSelfClosing))
                                selfClosing = false;
                            if (!selfClosing && !tagFlags.HasFlag(HtmlTagFlag.NoChildren))
                                parentNode = node; 
                        }
                    }

                    continue;
                }
            }
            var text = _parser.ParseCharacter();
            text += _parser.ParseTo(HtmlRules.TagStart);
            parentNode.Children.Add(new HtmlTextNode(text));
        }
        parentNode.Children.ForEach(n => n.ParentNode = null);
        return parentNode.Children;
    }

    private bool ParseTag([NotNullWhen(true)] out string? tag)
    {
        tag = null;
        var position = 0;

        Debug.Assert(_parser.Peek() == HtmlRules.TagStart);
        var c = _parser.Peek(++position);
        if (c is '!' or '?')
            c = _parser.Peek(++position);

        if (!HtmlRules.IsTagCharacter(c)) return false;
        while (HtmlRules.IsTagCharacter(_parser.Peek(++position)))
        {
        }

        _parser.Next();
        var length = position - 1;
        tag = _parser.Text.Substring(_parser.Index, length);
        _parser.Index += length;
        return true;
    }


    private HtmlAttributeCollection ParseAttributes()
    {
        HtmlAttributeCollection attributes = [];

        _parser.SkipWhiteSpace();
        var ch = _parser.Peek();
        while (HtmlRules.IsAttributeNameCharacter(ch) || HtmlRules.IsQuoteChar(ch))
        {
            HtmlAttribute attribute = new()
            {
                Name = HtmlRules.IsQuoteChar(ch)
                    ? $"\"{_parser.ParseQuotedText()}\""
                    : _parser.ParseWhile(HtmlRules.IsAttributeNameCharacter)
            };
            Debug.Assert(attribute.Name.Length > 0);
            _parser.SkipWhiteSpace();
            if (_parser.Peek() == '=')
            {
                _parser.Next();
                _parser.SkipWhiteSpace();
                if (HtmlRules.IsQuoteChar(_parser.Peek()))
                {
                    attribute.Value = _parser.ParseQuotedText();
                }
                else
                {
                    attribute.Value = _parser.ParseWhile(HtmlRules.IsAttributeValueCharacter);
                    Debug.Assert(attribute.Value.Length > 0);
                }
            }
            else
            {
                attribute.Value = null;
            }

            attributes.Add(attribute);
            _parser.SkipWhiteSpace();
            ch = _parser.Peek();
        }

        return attributes;
    }

    private HtmlHeaderNode ParseHtmlHeader()
    {
        HtmlHeaderNode node = new(ParseAttributes());
        const string tagEnd = ">";
        _parser.SkipTo(tagEnd);
        _parser.Index += tagEnd.Length;
        return node;
    }

    private XmlHeaderNode ParseXmlHeader()
    {
        var node = new XmlHeaderNode(ParseAttributes().ToString());
        const string tagEnd = "?>";
        _parser.SkipTo(tagEnd);
        _parser.Index += tagEnd.Length;
        return node;
    }

    private bool ParseToClosingTag(string tag, out string? content)
    {
        var endTag = $"</{tag}";
        var start = _parser.Index;
        Debug.Assert(_parser.Index > 0 && _parser.Peek(-1) == HtmlRules.TagEnd);
        while (!_parser.EndOfText)
        {
            _parser.SkipTo(endTag, StringComparison.OrdinalIgnoreCase);
            if (!HtmlRules.IsTagCharacter(_parser.Peek(endTag.Length)))
            {
                content = _parser.Extract(start, _parser.Index);
                _parser.Index += endTag.Length;
                _parser.SkipTo(HtmlRules.TagEnd);
                _parser.Next();
                return true;
            }

            _parser.Next();
        }

        content = null;
        return false;
    }

    private HtmlCDataNode ParseCDataNode(CDataDefinition definition)
    {
        Debug.Assert(_parser.MatchesCurrentPosition(definition.StartText, definition.StartComparison));
        _parser.Index += definition.StartText.Length;
        var content = _parser.ParseTo(definition.EndText, definition.EndComparison);
        _parser.Index += definition.EndText.Length;
        return new HtmlCDataNode(definition.StartText, definition.EndText, content);
    }
}