namespace Reptile.DataDive.Services.Events;

public class ProgressEventArgs : EventArgs
{
    private readonly object _processed;
    private readonly double _calculatePercentage;
    private readonly int _scrapedPages;
    private readonly int _totalUrls;
    public int DocumentsProcessed { get; }
    public int TotalDocuments { get; }
    public string Message { get; set; }
    public double ProgressPercentage { get; set; }

    public ProgressEventArgs(int processed, int total, string message)
    {
        DocumentsProcessed = processed;
        TotalDocuments = total;
        Message = message;
    }

    public ProgressEventArgs(string processed, double calculatePercentage)
    {
        
    }

    public ProgressEventArgs(string message, double calculatePercentage, int scrapedPages, int totalUrls)
    {
        Message = message;
        ProgressPercentage = calculatePercentage;
        DocumentsProcessed = scrapedPages;
        TotalDocuments = totalUrls;
    }
}