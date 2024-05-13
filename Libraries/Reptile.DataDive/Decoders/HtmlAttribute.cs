namespace Reptile.DataDive.Decoders;

public class HtmlAttribute(string name = "", string? value = null)
{
    public HtmlAttribute() : this(string.Empty)
    {
    }

    public HtmlAttribute(HtmlAttribute attribute) : this(attribute.Name, attribute.Value)
    {
    }

    public string Name { get; set; } = name == "" ? "null" : name;
    public string? Value { get; set; } = value;

    public override string ToString() 
        =>
        Value != null ? $"{Name}=\"{Value}\"" : name;
}

public class HtmlAttributeCollection : IEnumerable<HtmlAttribute>
{
    private readonly List<HtmlAttribute> _attributes;
    private readonly Dictionary<string, int> _indexLookup;

    public HtmlAttributeCollection()
    {
        _attributes = new List<HtmlAttribute>();
        _indexLookup = new Dictionary<string, int>(HtmlRules.TagStringComparer);
    }

    public HtmlAttributeCollection(HtmlAttributeCollection attributes)
    {
        _attributes = new List<HtmlAttribute>(attributes);
        _indexLookup = new Dictionary<string, int>(attributes._indexLookup, HtmlRules.TagStringComparer);
    }

    public void Add(string name, string? value) 
        => Add(new HtmlAttribute(name, value));

    public void Add(HtmlAttribute attribute)
    {
        ArgumentNullException.ThrowIfNull(attribute);
        if (string.IsNullOrEmpty(attribute.Name))
            throw new ArgumentException("An attribute name is required.");

        if (_indexLookup.TryGetValue(attribute.Name, out var existingIndex))
        {
            _attributes[existingIndex] = attribute;
        }
        else
        {
            var index = _attributes.Count;
            _attributes.Add(attribute);
            _indexLookup.Add(attribute.Name, index);
        }
    }

    public void AddRange(IEnumerable<HtmlAttribute> attributes)
    {
        foreach (var attribute in attributes)
            Add(attribute);
    }

    public bool Remove(string name)
    {
        if (!_indexLookup.TryGetValue(name, out var index)) return false;
        RemoveIndexLookup(index);
        _attributes.RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _attributes.Count) return;
        RemoveIndexLookup(index);
        _attributes.RemoveAt(index);
    }

    private void RemoveIndexLookup(int index)
    {
        foreach (var pair in _indexLookup)
            if (pair.Value > index)
                _indexLookup[pair.Key]--;
            else if (pair.Value == index)
                _indexLookup.Remove(pair.Key);
    }

    public HtmlAttribute? this[string? name] => name != null && _indexLookup.TryGetValue(name, out var index)
        ? _attributes[index]
        : default;
    public HtmlAttribute this[int index] => _attributes[index];


    public bool TryGetValue(string name, [MaybeNullWhen(false)] out HtmlAttribute value)
    {
        if (_indexLookup.TryGetValue(name, out var index))
        {
            value = _attributes[index];
            return true;
        }

        value = default;
        return false;
    }

    public override string ToString() => _attributes.Count > 0 ? $" {string.Join(" ", this)}" : string.Empty;

    public int Count => _attributes.Count;

    public bool Contains(string name) => _indexLookup.ContainsKey(name);

    public IEnumerable<string> Names => _attributes.Select(a => a.Name);
    public IEnumerable<string?> Values => _attributes.Select(a => a.Value);


    public IEnumerator<HtmlAttribute> GetEnumerator() => _attributes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _attributes.GetEnumerator();
}