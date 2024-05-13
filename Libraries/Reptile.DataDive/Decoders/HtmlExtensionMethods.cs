

namespace Reptile.DataDive.Decoders;

public static class HtmlExtensionMethods
{
    public static IEnumerable<HtmlNode> Find(this IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate)
    {
        var stack = new Stack<IEnumerator<HtmlNode>>();
        var enumerator = nodes.GetEnumerator();

        try
        {
            while (true)
                if (enumerator.MoveNext())
                {
                    var node = enumerator.Current;
                    if (predicate(node))
                        yield return node;

                    if (node is not HtmlElementNode elementNode) continue;
                    stack.Push(enumerator);
                    enumerator = elementNode.Children.GetEnumerator();
                }
                else if (stack.Count > 0)
                {
                    enumerator.Dispose();
                    enumerator = stack.Pop();
                }
                else
                {
                    yield break;
                }
        }
        finally
        {
            enumerator.Dispose();
            while (stack.Count > 0)
            {
                enumerator = stack.Pop();
                enumerator.Dispose();
            }
        }
    }

    public static IEnumerable<HtmlElementNode> Find(this IEnumerable<HtmlNode> nodes, string? selector) 
        => Selector.ParseSelector(selector).Find(nodes);

    public static IEnumerable<HtmlElementNode> Find(this IEnumerable<HtmlNode> nodes, SelectorCollection selectors) 
        => selectors.Find(nodes);

    public static IEnumerable<T> FindOfType<T>(this IEnumerable<HtmlNode> nodes) where T : HtmlNode 
        => Find(nodes, n => n is T).Cast<T>();

    public static IEnumerable<T> FindOfType<T>(this IEnumerable<HtmlNode> nodes, Func<T, bool> predicate)
        where T : HtmlNode 
        => Find(nodes, n => n is T node && predicate(node)).Cast<T>();

    public static string ToHtml(this IEnumerable<HtmlNode> nodes) 
        => string.Concat(nodes.Select(n => n.OuterHtml));
}