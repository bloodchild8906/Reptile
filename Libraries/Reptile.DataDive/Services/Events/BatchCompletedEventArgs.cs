namespace Reptile.DataDive.Services.Events;

public class BatchCompletedEventArgs : EventArgs
{
    public int DocumentsSaved { get; }
    public string Message { get; }

    public BatchCompletedEventArgs(int documentsSaved, string message)
    {
        DocumentsSaved = documentsSaved;
        Message = message;
    }
}