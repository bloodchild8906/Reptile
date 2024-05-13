using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static System.Text.RegularExpressions.Regex;

namespace Reptile.SharedKernel.Extensions.String;

public static partial class StringExtension
{
	public static string SurroundWithDoubleQuotes(this string text) => SurroundWith(text, "\"");

	public static string SurroundWith(this string text, string ends) => ends + text + ends;

	public static IEnumerable<string> SplitSentenceIntoWords(this string sentence)
    {
        var punctuation = sentence.Where(char.IsPunctuation).Distinct().ToArray();
        var words = sentence.Split().Select(x => x.Trim(punctuation));
        return words;
    }

	public static string Escape(this string s) => HttpUtility.HtmlEncode(s);

	public static string Unescape(this string s) => HttpUtility.HtmlDecode(s);

	public static bool In(this string value, params string[] stringValues) => stringValues.Any(otherValue => string.CompareOrdinal(value, otherValue) == 0);

	public static string Right(this string? value, int length) => value is not null && value.Length > length ? value[^length..] : value ?? string.Empty;

	public static string Left(this string? value, int length) => value is not null && value.Length > length ? value[..length] : value ?? string.Empty;

	public static string Format(this string value, object arg) => string.Format(value, arg);

	public static string Format(this string value, params object[] args) => string.Format(value, args);


	public static string Base64Encode(this string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText)) return string.Empty;
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(this string base64EncodedData)
    {
        if (string.IsNullOrWhiteSpace(base64EncodedData)) return string.Empty;
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static bool IsValidRegex(this string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return false;
        try
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Match(string.Empty, pattern);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }


	public static string ToCamelCase(this string str) => string.IsNullOrEmpty(str) switch
	{
		false when str.Length < 2 => str.ToLowerInvariant(),
		false => string.Concat(str[..1].ToLowerInvariant(), str.AsSpan(1)),
		_ => str
	};

	public static bool EndsWithWhitespace(string q) => EndsWithWhitespaceRegex().IsMatch(q);

	public static string ToTitleCase(this string str)
    {
        var cultureInfo = Thread.CurrentThread.CurrentCulture;
        return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
    }

    public static string ToTitleCase(this string str, string cultureInfoName)
    {
        var cultureInfo = new CultureInfo(cultureInfoName);
        return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
    }

	public static string ToTitleCase(this string str, CultureInfo cultureInfo) => cultureInfo.TextInfo.ToTitleCase(str.ToLower());

	public static TSelf TrimAllStrings<TSelf>(this TSelf obj)
    {
        if (obj is null)
            return default;

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.FlattenHierarchy;

        foreach (var p in obj.GetType().GetProperties(flags))
        {
            var currentNodeType = p.PropertyType;
            if (currentNodeType == typeof(string))
            {
                var currentValue = (string)p.GetValue(obj, null);
                if (currentValue != null) p.SetValue(obj, currentValue.Trim(), null);
            }
            // see http://stackoverflow.com/questions/4444908/detecting-native-objects-with-reflection
            else if (currentNodeType != typeof(object) && Type.GetTypeCode(currentNodeType) == TypeCode.Object)
            {
                if (p.GetIndexParameters().Length == 0)
                    p.GetValue(obj, null).TrimAllStrings();
                else
                    p.GetValue(obj, [0]).TrimAllStrings();
            }
        }

        return obj;
    }

    public static string Clean(this string input, string regex = "[^a-z^0-9^ ^-^_]", string replaceWith = "")
    {
        if (string.IsNullOrEmpty(input)) return input;
        var text = Replace(input, regex, replaceWith, RegexOptions.IgnoreCase).Trim();
        text = CleanRegex().Replace(text, " ");
        return text;
    }

    public static string ToAlphaNumericOnly(this string input, string replaceWith = "")
    {
        var rgx = AlphaNumericRegex();
        return rgx.Replace(input, replaceWith).Trim();
    }

    public static string ToAlphaOnly(this string input, string replaceWith = "")
    {
        var rgx = AlphaRegex();
        return rgx.Replace(input, replaceWith).Trim();
    }

    public static string ToNumericOnly(this string input, string replaceWith = "")
    {
        var rgx = Numeric();
        return rgx.Replace(input, replaceWith).Trim();
    }

	public static string ToLowerInvariantWithOutSpaces(this string s) => s.ToLowerInvariant().Replace(" ", string.Empty).Trim();

	public static string TrimToLength(this string s, int length) => string.IsNullOrWhiteSpace(s) ? s : s.Length > length ? s[..length] : s;

	public static string MakeValidXml(this string s) => XMLRegex().Replace(s, "-");

	public static string MakeValidUrl(this string s)
    {
        s = Replace(s, @"[^a-z0-9\s-]",
            "");
        return s;
    }

	public static string ToDefaultString(this string s, string defaultText) => string.IsNullOrWhiteSpace(s) ? defaultText.Trim() : s.Trim();

	public static string RemoveJunkWordsFromNumber(this string s)
    {
        s = s.Replace("years", string.Empty);
        s = s.Replace("year", string.Empty);
        s = s.Replace("%", string.Empty);
        s = s.Replace("$", string.Empty);
        s = s.Replace("-", string.Empty);
        return s;
    }

	public static string MakeValidFileName(this string s) => Path.GetInvalidFileNameChars().Aggregate(s, (current, c) => current.Replace(c, '_'));

	public static string ToSentenceCase(this string str)
    {
        if (str.Length == 0) return str;

        var lowerCase = str.ToLower();
        var r = SentenceCaseRegex();
        var result = r.Replace(lowerCase, s => s.Value.ToUpper());
        return result;
    }

    public static string ToPascalCase(this string str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str), "Null text cannot be converted!");

        if (str.Length == 0) return str;
        var words = str.Split(' ');
        for (var i = 0; i < words.Length; i++)
            if (words[i].Length > 0)
            {
                var word = words[i];
                var firstLetter = char.ToUpper(word[0]);
                words[i] = firstLetter + word[1..];
            }

        return string.Join(string.Empty, words);
    }

	public static string AddSpacesToSentence(this string text) => AddSpacesRegex().Replace(text, "$1 $2");

	public static bool IsGibberish(this string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return true;

        try
        {
            var chars = word.ToCharArray();
            if (!chars.Any()) return true;

            var numbers = chars.Count(char.IsDigit);
            var letters = chars.Count(char.IsLetter);

            if (letters == 0) return true;

            return numbers > letters;
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
            return true;
        }
    }

    [GeneratedRegex("^.*\\s$")]
    private static partial Regex EndsWithWhitespaceRegex();

    [GeneratedRegex(@"\s+", RegexOptions.Multiline)]
    private static partial Regex CleanRegex();

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex AlphaNumericRegex();

    [GeneratedRegex("[^a-zA-Z]")]
    private static partial Regex AlphaRegex();

    [GeneratedRegex("[^0-9]")]
    private static partial Regex Numeric();

    [GeneratedRegex(@"(?<=\<\w+)[#\{\}\(\)\&](?=\>)|(?<=\</\w+)[#\{\}\(\)\&](?=\>)")]
    private static partial Regex XMLRegex();

    [GeneratedRegex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture)]
    private static partial Regex SentenceCaseRegex();

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex AddSpacesRegex();
}