using Reptile.SharedKernel.Extensions.Collections;

namespace Reptile.SharedKernel.Extensions.String;

public static class SpellingExtensions
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

	public static IEnumerable<string> SpellingVariants(this string word, SpellingVariants spellingVariants) => spellingVariants! switch
	{
		String.SpellingVariants.None => new[] { word },
		String.SpellingVariants.One => Edits(word),
		String.SpellingVariants.Two => Edits(word).SelectMany(Edits).ToArray(),
		_ => null
	} ?? [];

	private static IEnumerable<string> Edits(string word)
    {
        var set = new HashSet<string>();
        var splits = Splits(set, word);
        var arr = splits as Tuple<string, string>[] ?? splits.ToArray();
        set.AddRange(Deletes(arr));
        set.AddRange(Transposes(arr));
        set.AddRange(Replaces(arr));
        set.AddRange(Inserts(arr));
        return set;
    }

	private static IEnumerable<Tuple<string, string>> Splits(HashSet<string> set, string word) => word.Select((_, i) => new Tuple<string, string>(word[..i], word.Substring(i, word.Length - i))).ToList();

	private static IEnumerable<string> Deletes(IEnumerable<Tuple<string, string>> splits) => splits
			.Where(x => x.Item2.Length > 0)
			.Select(x => string.Concat(x.Item1, x.Item2.AsSpan(1)));

	private static IEnumerable<string> Transposes(IEnumerable<Tuple<string, string>> splits) => splits
			.Where(x => x.Item2.Length > 1)
			.Select(x => x.Item1 + x.Item2[1] + x.Item2[0] + x.Item2[2..]);

	private static IEnumerable<string> Replaces(IEnumerable<Tuple<string, string>> splits) => Alphabet.SelectMany(x => Replaces(splits, x));

	private static IEnumerable<string> Replaces(IEnumerable<Tuple<string, string>> splits, char letter) => splits
			.Where(x => x.Item2.Length > 0)
			.Select(x => x.Item1 + letter + x.Item2[1..]);

	private static IEnumerable<string> Inserts(IEnumerable<Tuple<string, string>> splits) => Alphabet.SelectMany(x => Inserts(splits, x));

	private static IEnumerable<string> Inserts(IEnumerable<Tuple<string, string>> splits, char letter) => splits
			.Select(x => x.Item1 + letter + x.Item2);
}

public enum SpellingVariants
{
    None = 0,
    One = 1,
    Two = 2
}