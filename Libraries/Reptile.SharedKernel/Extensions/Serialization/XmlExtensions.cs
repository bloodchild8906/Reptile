namespace Reptile.SharedKernel.Extensions.Serialization;

public static class XmlExtensions
{
	public static int ToXmlBoolean(this bool value) => value ? 1 : 0;
}