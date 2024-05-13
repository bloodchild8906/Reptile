using System.Collections;
using System.Diagnostics;

namespace Reptile.SharedKernel.Extensions.Collections;

public static class CollectionExtensions
{
    [DebuggerStepThrough]
    public static void Each<T>(this IEnumerable<T> source, Action<T> action)
    {
        var items = source as T[] ?? source.ToArray();
        foreach (var item in items)
            action(item);
    }

    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector,
        IComparer<TKey> comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(comparer);
        using var sourceIterator = source.GetEnumerator();
        if (!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence contains no elements");
        var max = sourceIterator.Current;
        var maxKey = selector(max);
        while (sourceIterator.MoveNext())
        {
            var candidate = sourceIterator.Current;
            var candidateProjected = selector(candidate);
            if (comparer.Compare(candidateProjected, maxKey) <= 0) continue;
            max = candidate;
            maxKey = candidateProjected;
        }

        return max;
    }

	[DebuggerStepThrough]
	public static IEnumerable<T> Map<T>(this IEnumerable<T> source, Func<T, T> action) => source.Select(action);

	[DebuggerStepThrough]
	public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> source) => source.GroupBy(x => x)
			.Where(g => g.Count() > 1)
			.Select(g => g.Key);

	[DebuggerStepThrough]
	public static bool In<T>(this T source, params T[] list) => null == source ? throw new ArgumentNullException(nameof(source)) : list.Contains(source);

	[DebuggerStepThrough]
    public static bool In<T>(this T source, IEnumerable<T> list)
    {
        if (null == source) throw new ArgumentNullException(nameof(source));
        return list.Contains(source);
    }

    [DebuggerStepThrough]
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
    {
        foreach (var cur in enumerable) collection.Add(cur);
    }

	public static T PickRandom<T>(this IEnumerable<T> source) => source.PickRandom(1).Single();

	public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count) => source.Shuffle().Take(count);

	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) => source.OrderBy(x => Guid.NewGuid());

	public static IEnumerable<IList<object?>> AsTableWith(this IEnumerable first, params IEnumerable[] lists)
    {
        var iterator1 = first.GetEnumerator();
        using var iterator2 = iterator1 as IDisposable;
        var enumerators = lists.Select(x => x.GetEnumerator()).ToList();
        {
            while (iterator1.MoveNext())
            {
                var result = new List<object?> { iterator1.Current };
                enumerators.Each(x =>
                {
                    x.MoveNext();
                    result.Add(x.Current);
                });
                yield return result;
            }
        }
    }

    public static string ToCsv<T>(this IEnumerable<T> items, char separator)
    {
        var output = "";
        var delimiter = separator;
        var properties = typeof(T).GetProperties()
            .Where(n =>
                n.PropertyType == typeof(string)
                || n.PropertyType == typeof(bool)
                || n.PropertyType == typeof(char)
                || n.PropertyType == typeof(byte)
                || n.PropertyType == typeof(decimal)
                || n.PropertyType == typeof(int)
                || n.PropertyType == typeof(System.DateTime)
                || n.PropertyType == typeof(System.DateTime?));
        using var sw = new StringWriter();
        var propertyInfos = properties.ToList();
        var header = propertyInfos
            .Select(n => n.Name)
            .Aggregate((a, b) => a + delimiter + b);
        sw.WriteLine(header);
        foreach (var item in items)
        {
            var row = propertyInfos
                .Select(n => n.GetValue(item, null))
                .Select(n => n?.ToString())
                .Aggregate((a, b) => a + delimiter + b);
            sw.WriteLine(row);
        }

        output = sw.ToString();
        return output;
    }
}