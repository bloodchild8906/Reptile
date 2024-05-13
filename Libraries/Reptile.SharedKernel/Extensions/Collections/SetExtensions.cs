namespace Reptile.SharedKernel.Extensions.Collections;

public static class SetExtensions
{
    public static void AddRange(this ISet<string> set, IEnumerable<string> other)
    {
        foreach (var element in other)
            if (!string.IsNullOrWhiteSpace(element))
                set.Add(element);
    }
}