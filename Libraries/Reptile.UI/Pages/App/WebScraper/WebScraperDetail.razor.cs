using Masa.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using Reptile.UI.Data;

namespace Reptile.UI.Pages.App.WebScraper;

public partial class WebScraperDetail
{
    private MForm? _mForm;
    private bool _isEdit;
    private WebscraperDto _selectData = new();

    private string CompletedColor
    {
        get
        {
            return _selectData.IsCompleted
                ? "text-capitalize neutral-lighten-5 neutral-lighten-2--text"
                : "theme--dark primary";
        }
    }

    private string CompletedText
    {
        get { return _selectData.IsCompleted ? "Completed" : "Mark Complete"; }
    }

    [CascadingParameter] public WebScraper WebScraper { get; set; } = default!;

    [Parameter] public bool? Value { get; set; }

    [Parameter] public EventCallback<bool?> ValueChanged { get; set; }

    [Parameter] public WebscraperDto? SelectItem { get; set; }

    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    private async Task HideNavigationDrawer()
    {
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(false);
        }
    }

    private void Complete()
    {
        _selectData.IsCompleted = !_selectData.IsCompleted;
        _selectData.IsChecked = _selectData.IsCompleted;
    }


    protected override void OnParametersSet()
    {
        if (SelectItem == null)
        {
            SelectItem = new WebscraperDto();
            _isEdit = false;
        }
        else
        {
            _isEdit = true;
        }

        _selectData = new WebscraperDto
        {
            Id = SelectItem.Id,
            IsChecked = SelectItem.IsChecked,
            Url = SelectItem.Url,
            IsCompleted = SelectItem.IsCompleted,
            IsDeleted = SelectItem.IsDeleted,
            Title = SelectItem.Title
        };

        if (ValueChanged.HasDelegate && !(Value is true) && _mForm != null)
        {
            _mForm.ResetValidation();
        }
    }

    private async Task AddAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            WebScraper.AddData(_selectData);
            await HideNavigationDrawer();

            NavigationManager.NavigateTo("app/WebScraper");
        }
    }

    private async Task UpdateAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            var data = (WebscraperDto)context.Model;
            WebScraper.UpdateData(data);
            await HideNavigationDrawer();
        }
    }

    private void Reset()
    {
        if (_mForm != null)
        {
            _mForm.ResetValidation();
        }

        if (SelectItem != null)
        {
            _selectData = new WebscraperDto
            {
                Id = SelectItem.Id,
                IsChecked = SelectItem.IsChecked,
                Url = SelectItem.Url,
                IsCompleted = SelectItem.IsCompleted,
                IsDeleted = SelectItem.IsDeleted,
                Title = SelectItem.Title
            };
        }
    }

    private async Task DeleteAsync()
    {
        _selectData.IsDeleted = true;
        WebScraper.UpdateData(_selectData);
        await HideNavigationDrawer();
    }
}