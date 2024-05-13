using Microsoft.Extensions.DependencyInjection;

namespace Reptile.AI.Services;

public static class ComponentServices
{
	public static IServiceCollection AddAiServices(this IServiceCollection source, string openAiKey, string? org = null) => source;
}