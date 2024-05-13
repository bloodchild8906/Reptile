using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Reptile.UI;
using Reptile.UI.Data;

// For reading JSON data over HTTP

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var s = 
    "DefaultEndpointsProtocol=https;" +
    "AccountName=reptileaitable;" +
    "AccountKey=8dzjk0mU4ofvw7ptU/59Pq9kr4/oHZmoEArkmW67nWnSTOBUgZG4qxlAgVY3xl79MH6knzdPFT2a+AStW6ii3g==;" +
    "BlobEndpoint=https://reptileaitable.blob.core.windows.net/;" +
    "QueueEndpoint=https://reptileaitable.queue.core.windows.net/;" +
    "TableEndpoint=https://reptileaitable.table.core.windows.net/;" +
    "FileEndpoint=https://reptileaitable.file.core.windows.net/;";

builder.Services.AddMasaBlazor();
// Fetch the navigation configuration via HTTP

var nav = new List<NavModel>()
{
    new NavModel(
        id: 1,
        title: "Tools",
        icon: "mdi-home",
        href: "app/Home",
        children:
        [
            new NavModel(2, title: "WebScraper", icon: "mdi-information", href: "app/WebScraper", children: null),
            new NavModel(3, title: "Search", icon: "mdi-email", href: "app/Search", children: null)
        ])
};


builder.Services.AddReptileComponents(nav, s);

await builder.Build().RunAsync();