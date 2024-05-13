using System.Diagnostics.CodeAnalysis;

namespace Reptile.SharedKernel.Extensions.Common;

public static class Extensions
{
	public static T As<T>(this object value) => (T)value;

	public static Guid ToNewGuidIfEmpty(this Guid guid) => guid.Equals(Guid.Empty) ? Guid.NewGuid() : guid;

	[MemberNotNull]
	public static T? To<T>(this object value) => value.ToString().To<T>();

	public static T? To<T>(this string? value)
    {
        if (!typeof(T).IsEnum)
            return string.IsNullOrEmpty(value) ? default : (T)Convert.ChangeType(value, typeof(T));
        if (string.IsNullOrEmpty(value)) return default;
        return (T)Enum.Parse(typeof(T), value);
    }
}