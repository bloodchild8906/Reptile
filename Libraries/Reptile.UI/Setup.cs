using CoreHelpers.WindowsAzure.Storage.Table;
using Microsoft.Extensions.DependencyInjection;
using Reptile.DataDive;
using Reptile.UI.Data;

// Using Newtonsoft.Json for deserialization

// Required for accessing FileSystem

namespace Reptile.UI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReptileComponents(this IServiceCollection services, List<NavModel>? navModel, string connecton)
    {
        var _context = new StorageContext(connecton);
        services.AddSingleton(_context);
        // This registers HttpClient to be used with IHttpClientFactory by the dependent services
        services.AddScoped<HttpClient>();

        // Attempt to load navigation data
        services.AddNav(navModel);

        services.AddScoped<CookieStorage>();
        services.AddScoped<GlobalConfig>();

        services.RegisterDataServices(_context);

        services.AddMasaBlazor(builder =>
        {
            builder.ConfigureTheme(theme =>
            {
                theme.Themes.Light.Primary = "#4318FF";
                theme.Themes.Light.Accent = "#4318FF";
            });
        });

        return services;
    }
    public static IServiceCollection AddNav(this IServiceCollection services, List<NavModel> navList)
    {
        services.AddSingleton(navList);
        services.AddScoped<NavHelper>();

        return services;
    }

    public static IServiceCollection AddNav(this IServiceCollection services, string navSettingsFile)
    {
        var navList = JsonSerializer.Deserialize<List<NavModel>>(File.ReadAllText(navSettingsFile));

        if (navList is null) throw new Exception("Please configure the navigation first!");

        services.AddNav(navList);

        return services;
    }



}