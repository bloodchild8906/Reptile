using static Reptile.DataDive.Decoders.HtmlTagFlag;

namespace Reptile.DataDive.Decoders;

[Flags]
internal enum HtmlTagFlag
{
    None = 0x0000, HtmlHeader = 0x0001, XmlHeader = 0x0002, NoChildren = 0x0004, NoNested = 0x0008, NoSelfClosing = 0x0010, CData = 0x0020
}

internal class HtmlRules
{
    public const string HtmlHeaderTag = "!doctype";
    public const string XmlHeaderTag = "?xml";

    public const char TagStart = '<';
    public const char TagEnd = '>';
    public const char ForwardSlash = '/';

    public const char DoubleQuote = '"';
    public const char SingleQuote = '\'';

    public static readonly List<CDataDefinition> CDataDefinitions =
    [
        new CDataDefinition("<!--", "-->", StringComparison.Ordinal,StringComparison.Ordinal),
        new CDataDefinition("<![CDATA[", "]]>", StringComparison.Ordinal, StringComparison.Ordinal)
    ];

    public const StringComparison TagStringComparison = StringComparison.CurrentCultureIgnoreCase;
    public static readonly StringComparer TagStringComparer = StringComparer.CurrentCultureIgnoreCase;

    private static readonly HashSet<char> InvalidChars;


    public static bool IgnoreHtmlRules = false;

    
    private static readonly Dictionary<string, HtmlTagFlag> TagRules = new(StringComparer.CurrentCultureIgnoreCase)
    {
        ["!doctype"] = HtmlHeader, ["?xml"] = XmlHeader, ["a"] = NoNested, ["area"] = NoChildren,
        ["base"] = NoChildren, ["basefont"] = NoChildren, ["bgsound"] = NoChildren, ["br"] = NoChildren,
        ["col"] = NoChildren, ["dd"] = NoNested, ["dt"] = NoNested, ["embed"] = NoChildren, ["frame"] = NoChildren,
        ["hr"] = NoChildren, ["img"] = NoChildren, ["input"] = NoChildren, ["isindex"] = NoChildren, ["keygen"] = NoChildren,
        ["li"] = NoNested, ["link"] = NoChildren, ["menuitem"] = NoChildren, ["meta"] = NoChildren, ["noxhtml"] = CData,
        ["p"] = NoNested, ["param"] = NoChildren, ["script"] = CData, ["select"] = NoSelfClosing, ["source"] = NoChildren,
        ["spacer"] = NoChildren, ["style"] = CData, ["table"] = NoNested, ["td"] = NoNested, ["th"] = NoNested,
        ["textarea"] = NoSelfClosing, ["track"] = NoChildren, ["wbr"] = NoChildren
    };

    private static readonly Dictionary<string, int> NestLevelLookup = new(StringComparer.CurrentCultureIgnoreCase)
    {
        ["div"] = 150, ["td"] = 160, ["th"] = 160, ["tr"] = 170, ["thead"] = 180, ["tbody"] = 180, ["tfoot"] = 180,
        ["table"] = 190, ["head"] = 200, ["body"] = 200, ["html"] = 220
    };

    static HtmlRules()
    {
        InvalidChars = new HashSet<char> { '!', '?', '<', '"', '\'', '>', '/', '=' };
        for (var i = 0xfdd0; i <= 0xfdef; i++)
            InvalidChars.Add((char)i);
        InvalidChars.Add('\ufffe');
        InvalidChars.Add('\uffff');
    }


    public static bool IsQuoteChar(char c) => c is DoubleQuote or SingleQuote;

    public static bool IsTagCharacter(char c) => !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);

    public static bool IsAttributeNameCharacter(char c) => !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);

    public static bool IsAttributeValueCharacter(char c) => !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);

    public static HtmlTagFlag GetTagFlags(string tag) => IgnoreHtmlRules == false && TagRules.TryGetValue(tag, out var flags) ? flags : None;

    [Obsolete(
        "This method is deprecated and will be removed in a future version. Please use GetTagNestLevel() instead.")]
    public static int GetTagPriority(string tag) => GetTagNestLevel(tag);

    public static int GetTagNestLevel(string tag) => NestLevelLookup.GetValueOrDefault(tag, 100);


    public static bool TagMayContain(string parentTag, string childTag) => TagMayContain(parentTag, childTag, GetTagFlags(parentTag));

    public static bool TagMayContain(string parentTag, string childTag, HtmlTagFlag parentFlags)
    {
        if (parentFlags.HasFlag(NoChildren))
            return false;
        if (parentFlags.HasFlag(NoNested) && parentTag.Equals(childTag, TagStringComparison))
            return false;
        return GetTagNestLevel(childTag) <= GetTagNestLevel(parentTag);
    }
}
internal record CDataDefinition(string StartText, string EndText, StringComparison StartComparison, StringComparison EndComparison);
