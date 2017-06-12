namespace Cake.DocumentDb.Collection
{
    public interface ICreateDocumentDatabaseCollection
    {
        string DatabaseName { get; }
        string CollectionName { get; }
        string PartitionKey { get; }
        int? Throughput { get; }
    }
}
