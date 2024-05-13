using Microsoft.AspNetCore.Components.Routing;
using Reptile.UI.Data;

namespace Reptile.UI.Shared;

public partial class Breadcrumb
{
    [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] NavHelper NavHelper { get; set; }
    [Inject] GlobalConfig GlobalConfig { get; set; }
    
    private List<BreadcrumbItem> _items = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        GetBreadcrumbItems();

        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private RenderFragment DividerContent => (builder) =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "page-mode--breadcrumb__divider");
        builder.CloseElement();
    };

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        GetBreadcrumbItems();
        InvokeAsync(StateHasChanged);
    }

    private void GetBreadcrumbItems()
    {
        var items = new List<BreadcrumbItem>();

        var currentNav =
            NavHelper.SameLevelNavs.FirstOrDefault(n => n.Href is not null && NavigationManager.Uri.Contains(n.Href));

        if (currentNav is not null)
        {
            if (currentNav.ParentId != 0)
            {
                var parentNav = NavHelper.SameLevelNavs.First(n => n.Id == currentNav.ParentId);
                items.Add(new BreadcrumbItem
                {
                    Text = parentNav.Title,
                    Href = "/" + (parentNav.Href ?? parentNav.Children?.FirstOrDefault()?.Href ?? ""), Exact = true
                });
            }

            items.Add(new BreadcrumbItem { Text = currentNav.Title, Href = "/" + currentNav.Href, Exact = true });

            items.Last().Href = "/" + currentNav.Href;
            items.Last().Disabled = true;
        }

        _items = items;
    }

	public void Dispose() => NavigationManager.LocationChanged -= OnLocationChanged;
}
