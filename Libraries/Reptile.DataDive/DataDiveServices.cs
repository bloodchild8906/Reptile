
using CoreHelpers.WindowsAzure.Storage.Table;
using Microsoft.Extensions.DependencyInjection;
using Reptile.DataDive.Services;

namespace Reptile.DataDive;

public static class DataDiveServices
{
    public static IServiceCollection RegisterDataServices(this IServiceCollection services, StorageContext context)
    {
        // Configure the Memory Cache
        services.AddMemoryCache();

        // Register DataDiveService with HttpClient dependency
        services.AddHttpClient<DataDiveService>((serviceProvider, httpClient) =>
        {
             
        });
        services.RegisterDataServices(context);
       
        
         return services;
    }


    
}