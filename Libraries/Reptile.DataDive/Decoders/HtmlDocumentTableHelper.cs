using CoreHelpers.WindowsAzure.Storage.Table;

namespace Reptile.DataDive.Decoders;

public static class HtmlDocumentTableHelper
{
    public static HtmlDocumentTable SetupConnection(this HtmlDocumentTable doc, StorageContext context)
    {
        doc.context = context;
        return doc;
    } 
    
    [MemberNotNull]
    public static async void Create(this HtmlDocumentTable doc) => await doc.context.EnableAutoCreateTable().MergeOrInsertAsync(doc);

    public static void CreateMapping(this HtmlDocumentTable doc) =>
        doc.context.AddEntityMapper(typeof(HtmlDocumentTable),
            new StorageEntityMapper()
            {
                TableName = $"{doc.ScrapedCollection}-{new Uri(doc.Document.Url).Host}",
                PartitionKeyFormat = "collectionName",
                RowKeyFormat = "collectionNam"
            });
}