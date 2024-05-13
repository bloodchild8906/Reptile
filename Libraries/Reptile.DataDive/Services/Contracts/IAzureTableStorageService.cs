using Reptile.DataDive.Decoders;
using Reptile.DataDive.Services.Events;

namespace Reptile.DataDive.Services.Contracts;

public interface IAzureTableStorageService
{
    // Events
    event EventHandler<ProgressEventArgs>? ProgressChanged;
    event EventHandler<BatchCompletedEventArgs>? BatchCompleted;

    // Methods
    Task EnsureTableExistsAsync();
    Task SaveDocumentsAsync(Task<List<CustomHtmlDocument?>> documentsTask);
}