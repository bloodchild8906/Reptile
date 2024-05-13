namespace Reptile.DataDive.Services.Events;

public class Progress : EventArgs
{
    public Progress(string? url, double percentage)
    {
        Url = url;
        Percentage = Percentage;
    }
    public string? Url { get; private set; }
    public double Percentage { get; private set; }
}