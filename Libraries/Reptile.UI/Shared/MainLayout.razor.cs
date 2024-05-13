using Reptile.UI.Data;

namespace Reptile.UI.Shared;

public partial class MainLayout
{
    private bool? _showSetting;
    private string? _pageTab;
    private string PageModeClass => _pageTab == PageModes.PageTab ? "page-mode--tab" : "page-mode--breadcrumb";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await GlobalConfig.InitFromStorage();
        }
    }
}