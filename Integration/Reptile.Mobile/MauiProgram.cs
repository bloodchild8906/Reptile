using System.Reflection;
using Newtonsoft.Json;
using Reptile.UI;
using Reptile.UI.Data;

namespace Reptile.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        var s = 
            "DefaultEndpointsProtocol=https;" +
            "AccountName=reptileaitable;" +
            "AccountKey=8dzjk0mU4ofvw7ptU/59Pq9kr4/oHZmoEArkmW67nWnSTOBUgZG4qxlAgVY3xl79MH6knzdPFT2a+AStW6ii3g==;" +
            "BlobEndpoint=https://reptileaitable.blob.core.windows.net/;" +
            "QueueEndpoint=https://reptileaitable.queue.core.windows.net/;" +
            "TableEndpoint=https://reptileaitable.table.core.windows.net/;" +
            "FileEndpoint=https://reptileaitable.file.core.windows.net/;";
        
        // Load navigation data from embedded resources and add required services
        var Navigation = LoadNavigationDataFromManifest();
        builder.Services.AddReptileComponents(Navigation, s);
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMasaBlazor();

        // Debug tools are only added in debug configuration
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }

    private static List<NavModel>? LoadNavigationDataFromManifest()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith("nav.json"));

        if (resourceName == null)
        {
            throw new InvalidOperationException("nav.json is not found in the manifest resources.");
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException("Cannot open nav.json from the manifest resources.");
        }

        using var reader = new StreamReader(stream);
        string jsonContent = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<List<NavModel>>(jsonContent);
    }
}