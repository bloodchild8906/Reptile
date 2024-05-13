namespace Reptile.UI.Data;

public class WebscraperDto
{
    public int Id { get; set; }

    public bool IsChecked { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsDeleted { get; set; }

    [Required] public string Title { get; set; } = "";

    public string?  Url { get; set; }

    public WebscraperDto()
    {
    }

    public WebscraperDto(int id, bool isChecked, bool isImportant, bool isCompleted, bool isDeleted, string title,
        string assignee, int avatar, DateOnly dueDate, List<string> tags, string description)
    {
        Id = id;
        IsChecked = isChecked;
        IsCompleted = isCompleted;
        Title = title;
        Url = description;
    }
}