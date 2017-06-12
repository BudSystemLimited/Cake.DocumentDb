namespace Cake.DocumentDb.Seed
{
    public interface ISeedDocument
    {
        string FriendlyName { get; }
        string Database { get; }
        string Collection { get; }
        string PartitionKey { get; }
        object Document();
    }
}
