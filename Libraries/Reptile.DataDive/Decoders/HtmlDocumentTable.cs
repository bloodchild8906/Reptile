
using Azure;
using Azure.Data.Tables;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Reptile.DataDive.Decoders;

public class HtmlDocumentTable : ITableEntity
{
	private string PartitionKey { get; set; }

	public string RowKey { get; set; }
	internal StorageContext context { get; set; }
    public CustomHtmlDocument Document { get; set; }
    public string ScrapedCollection { get; set; }
	string ITableEntity.PartitionKey { get; set; }
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }

	public HtmlDocumentTable(string partitionKey, string rowKey, CustomHtmlDocument document, string scrapedCollection)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
        Document = document;
        ScrapedCollection = scrapedCollection;
    }
    
    public HtmlDocumentTable(string partitionKey, string rowKey, CustomHtmlDocument document, string scrapedCollection, StorageContext context)
    {
        _=new HtmlDocumentTable( partitionKey,  rowKey,  document,  scrapedCollection);
        this.context = context;
    }

	public static HtmlDocumentTable CreateInstanceBinder(string partitionKey, string rowKey, CustomHtmlDocument document, string scrapedCollection, StorageContext context) => new HtmlDocumentTable(partitionKey, rowKey, document, scrapedCollection, context);

	public HtmlDocumentTable()
    {
    }
}