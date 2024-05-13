using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Reptile.DataDive.Decoders;
using Reptile.DataDive.Services.Contracts;
using Reptile.DataDive.Services.Events;

namespace Reptile.DataDive.Services;

public class AzureTableStorageService : IAzureTableStorageService
{
    public readonly TableClient TableClient; // Consider making this private or protected
    private readonly string _partitionKey;
    private readonly ILogger? _logger;
    private const int MaxBatchSize = 100;

    public event EventHandler<ProgressEventArgs>? ProgressChanged;
    public event EventHandler<BatchCompletedEventArgs>? BatchCompleted;

    public AzureTableStorageService(string connectionString, string tableName, string partitionKey = "HtmlDocuments", ILogger? logger = null)
    {
        _partitionKey = partitionKey;
        TableClient = new TableClient(connectionString, tableName);
        _logger = logger;
        EnsureTableExistsAsync().GetAwaiter().GetResult();
    }

    public async Task EnsureTableExistsAsync()
    {
        try
        {
            await TableClient.CreateIfNotExistsAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Failed to create table: {ex.Message}");
            throw;
        }
    }

    public async Task SaveDocumentsAsync(Task<List<CustomHtmlDocument?>> documentsTask)
    {
        var documents = await documentsTask;
        var batch = new List<TableTransactionAction>();
        int processedDocuments = 0;

        foreach (var entity in from document in documents.OfType<CustomHtmlDocument>()
                 where ValidateDocument(document)
                 select new TableEntity
                 {
                     ["PartitionKey"] = _partitionKey,
                     ["RowKey"] = Guid.NewGuid().ToString(),
                     ["Url"] = document.Url,
                     ["Content"] = document.ToHtml()
                 })
        {
            batch.Add(new TableTransactionAction(TableTransactionActionType.Add, entity));
            processedDocuments++;

            if (batch.Count < MaxBatchSize) continue;
            await SubmitBatchAsync(batch, processedDocuments);
            batch.Clear();
        }

        if (batch.Count > 0)
        {
            await SubmitBatchAsync(batch, processedDocuments);
        }
    }

    private async Task SubmitBatchAsync(IReadOnlyCollection<TableTransactionAction> batch, int processedDocuments)
    {
        try
        {
            await TableClient.SubmitTransactionAsync(batch);
            ProgressChanged?.Invoke(this, new ProgressEventArgs(processedDocuments, batch.Count, "Batch submitted successfully"));
            BatchCompleted?.Invoke(this, new BatchCompletedEventArgs(batch.Count, "Batch completed"));
        }
        catch (RequestFailedException ex)
        {
            _logger?.LogError($"Error during batch submission: {ex.Message}");
            ProgressChanged?.Invoke(this, new ProgressEventArgs(processedDocuments, batch.Count, "Failed to submit batch"));
        }
    }

	internal static bool ValidateDocument(CustomHtmlDocument document) => !string.IsNullOrWhiteSpace(document.Url) && !string.IsNullOrWhiteSpace(document.ToHtml());
}