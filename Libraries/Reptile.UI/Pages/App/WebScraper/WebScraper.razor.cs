using Reptile.DataDive.Services;
using CoreHelpers.WindowsAzure.Storage.Table;
using Reptile.DataDive.Decoders;
using Reptile.DataDive.Services.Events;
using Reptile.UI.Data;

namespace Reptile.UI.Pages.App.WebScraper;

public partial class WebScraper
{
    [Inject] private DataDiveService DataDiveService { get; set; }

    [Inject] private StorageContext Context { get; set; }

    private WebscraperDto _selectedItem = new();
    private List<WebscraperDto> _displayList = new();
    private readonly List<WebscraperDto> _allItems = WebScraperService.GetList();
    private int _scrapingProgress;
    private string _filterText;
    private string _inputText;
    private bool _isVisible;
    private string upload;

    [Parameter]
    public string FilterText
    {
        get => _filterText;
        set
        {
            if (_filterText == value) return;
            _filterText = value;
            UpdateDisplayList();
        }
    }

    public string? InputText
    {
        get => _inputText;
        set
        {
            if (_inputText == value) return;
            _inputText = value;
            FilterByTitle(_inputText);
        }
    }

    private bool UpdateDisplayList()
    {
        _displayList = _filterText switch
        {
            "completed" => _allItems.Where(item => item is { IsCompleted: true, IsDeleted: false }).ToList(),
            "deleted" => _allItems.Where(item => item.IsDeleted).ToList(),
            _ => _allItems.Where(item => !item.IsDeleted).ToList()
        };
        return true;
    }

    private void FilterByTitle(string? title)
    {
        _displayList = !string.IsNullOrWhiteSpace(title)
            ? _allItems.Where(item => item.Title.Contains(title)).ToList()
            : _allItems;
        StateHasChanged();
    }

    private string content;

    public void AddData(WebscraperDto data)
    {
        _allItems.Insert(0, data);
        UpdateDisplayList();
        StateHasChanged(); // Ensure the component knows to re-render.
    }

    public bool UpdateData(WebscraperDto data)
    {
        var index = _allItems.FindIndex(d => d.Id == data.Id);
        if (index == -1) return false;
        _allItems[index] = data;
        UpdateDisplayList();
        StateHasChanged();
        return true; // Ensure the component knows to re-render.
    }

    private async Task StartScraping()
    {
        content = "Scraping in progress...";
        _scrapingProgress = 0;
        DataDiveService.ReportProgress += HandleScrapingProgress;
        var urls = _allItems.Select(x => x.Url).Where(x => x != null);
        var startScraping = await DataDiveService.StartScraping(_allItems.Select(x => x.Url).Where(x => x != null));
        startScraping.ForEach(table =>
        {
            try
            {
				string coll = _allItems.FirstOrDefault(x => x is not null && x.Url == table.Url).Title;
                var tbl = new HtmlDocumentTable(partitionKey: coll, rowKey: coll, document: table,
                    scrapedCollection: coll, Context);
                tbl.CreateMapping();
                tbl.Create();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        });
        DataDiveService.ReportProgress -= HandleScrapingProgress;
    }


    private void HandleScrapingProgress(object? sender, ProgressEventArgs e)
    {
        _scrapingProgress = (int)e.ProgressPercentage;
        content = $"{e.Message} {e.ProgressPercentage}%";
        InvokeAsync(StateHasChanged);
    }


    private void ShowDetail(WebscraperDto item)
    {
        _isVisible = true;
        _selectedItem = item;
    }

    private Task<bool> DeleteItem(WebscraperDto item)
    {
        _allItems.Remove(item);
        return Task.FromResult<bool>(UpdateDisplayList());
    }

	private void Toggle(bool obj) => _isVisible = obj;
}