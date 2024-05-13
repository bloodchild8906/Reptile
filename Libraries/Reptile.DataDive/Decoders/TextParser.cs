using System.Text;

namespace Reptile.DataDive.Decoders;

public class TextParser
{
    public const char NullChar = '\0';
    private int _internalIndex;

    public TextParser(string? text) => Reset(text);

    public string Text { get; private set; }

    public int Index
    {
        get => _internalIndex;
        set
        {
            _internalIndex = value;
            if (_internalIndex < 0)
                _internalIndex = 0;
            else if (_internalIndex > Text.Length)
                _internalIndex = Text.Length;
        }
    }

    public bool EndOfText => _internalIndex >= Text.Length;

    [MemberNotNull(nameof(Text))]
    public void Reset(string? text)
    {
        Text = text ?? string.Empty;
        _internalIndex = 0;
    }

	public char Peek() => _internalIndex < Text.Length ? Text[_internalIndex] : NullChar;

	public char Peek(int count)
    {
        var index = _internalIndex + count;
        return index >= 0 && index < Text.Length ? Text[index] : NullChar;
    }

    public char Get() => _internalIndex < Text.Length ? Text[_internalIndex++] : NullChar;

    public void Next()
    {
        if (_internalIndex < Text.Length)
            _internalIndex++;
    }

    public void SkipWhile(Func<char, bool> predicate)
    {
        while (_internalIndex < Text.Length && predicate(Text[_internalIndex]))
            _internalIndex++;
    }

    public void SkipWhiteSpace() => SkipWhile(char.IsWhiteSpace);

    public bool SkipTo(params char[] chars)
    {
        _internalIndex = Text.IndexOfAny(chars, _internalIndex);
        if (_internalIndex >= 0)
            return true;
        _internalIndex = Text.Length;
        return false;
    }

    public bool SkipTo(string s, bool includeToken = false)
    {
        _internalIndex = Text.IndexOf(s, _internalIndex, StringComparison.Ordinal);
        if (_internalIndex >= 0)
        {
            if (includeToken)
                _internalIndex += s.Length;
            return true;
        }

        _internalIndex = Text.Length;
        return false;
    }

    public bool SkipTo(string s, StringComparison comparison, bool includeToken = false)
    {
        _internalIndex = Text.IndexOf(s, _internalIndex, comparison);
        if (_internalIndex >= 0)
        {
            if (includeToken)
                _internalIndex += s.Length;
            return true;
        }

        _internalIndex = Text.Length;
        return false;
    }

    public string ParseCharacter() 
        => _internalIndex < Text.Length ? Text[_internalIndex++].ToString() : string.Empty;

    public string ParseWhile(Func<char, bool> predicate)
    {
        var start = _internalIndex;
        SkipWhile(predicate);
        return Extract(start, _internalIndex);
    }

    public string ParseQuotedText()
    {
        StringBuilder builder = new();
        var quote = Get();
        while (!EndOfText)
        {
            builder.Append(ParseTo(quote));
            Next();
            if (Peek() == quote)
            {
                builder.Append(quote);
                Next();
            }
            else
                break; 
        }

        return builder.ToString();
    }

    public string ParseTo(params char[] chars)
    {
        var start = _internalIndex;
        SkipTo(chars);
        return Extract(start, _internalIndex);
    }

    public string ParseTo(string s, StringComparison comparison, bool includeToken = false)
    {
        var start = _internalIndex;
        SkipTo(s, comparison, includeToken);
        return Extract(start, _internalIndex);
    }

    public bool MatchesCurrentPosition(string? s, StringComparison comparison) 
        => !string.IsNullOrEmpty(s) && string.Compare(Text, _internalIndex, s, 0, s.Length, comparison) == 0;

    public string Extract(int start, int end)
    {
        if (start < 0 || start > Text.Length)
            throw new ArgumentOutOfRangeException(nameof(start));
        if (end < start || end > Text.Length)
            throw new ArgumentOutOfRangeException(nameof(end));
        return Text[start..end];
    }
}