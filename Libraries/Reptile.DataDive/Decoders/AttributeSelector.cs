using System.Text.RegularExpressions;

namespace Reptile.DataDive.Decoders;

public class AttributeSelector
{
    private readonly RegexOptions _regexOptions;
    private readonly StringComparison _stringComparison;
    private AttributeSelectorMode _mode;

    public AttributeSelector(string name, string? value = null, bool ignoreCase = true)
    {
        Name = name;
        Value = value;
        _stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        _regexOptions = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        IsMatch = MatchComparer; 
    }

    public string Name { get; set; }
    public string? Value { get; set; }

    public AttributeSelectorMode Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            IsMatch = GetComparisonFunction(value);
        }
    }

    public Func<HtmlElementNode, bool> IsMatch { get; private set; }

    private static readonly char[] Separator = [' ', '\t', '\n'];

    private Func<HtmlElementNode, bool> GetComparisonFunction(AttributeSelectorMode mode) 
        => mode switch
        {
            AttributeSelectorMode.RegEx => RegExComparer,
            AttributeSelectorMode.Contains => ContainsComparer,
            AttributeSelectorMode.ExistsOnly => ExistsOnlyComparer,
            _ => MatchComparer
        };

    private bool MatchComparer(HtmlElementNode node) 
        => Value != null &&
           node.Attributes.TryGetValue(Name, out var attribute) &&
           attribute.Value != null &&
           string.Equals(attribute.Value, Value, _stringComparison);

    private bool RegExComparer(HtmlElementNode node) 
        => Value != null &&
           node.Attributes.TryGetValue(Name, out var attribute) &&
           attribute.Value != null &&
           Regex.IsMatch(attribute.Value, Value, _regexOptions);

    private bool ContainsComparer(HtmlElementNode node) 
        => Value != null &&
           node.Attributes.TryGetValue(Name, out var attribute) &&
           attribute.Value != null &&
           IsWordPresent(attribute.Value);

    private bool IsWordPresent(string text) =>
        new HashSet<string?>(text.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
            StringComparer.FromComparison(_stringComparison))
            .Contains(Value);

    private bool ExistsOnlyComparer(HtmlElementNode node) 
        => node.Attributes.Contains(Name);

    public override string ToString() => $"{Name}={Value ?? "(null)"}";
}
public enum SelectorAttributeMode
{
    Match,
    RegEx,
    Contains,
    ExistsOnly
}

public enum AttributeSelectorMode
{
    Match,
    RegEx,
    Contains,
    ExistsOnly
}