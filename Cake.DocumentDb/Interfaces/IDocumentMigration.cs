namespace Cake.DocumentDb.Interfaces
{
    public interface IDocumentMigration
    {
        string Description { get; }
        string Database { get; }
        string Collection { get; }
        string PartitionKey { get; }
        void Transform(dynamic item);
    }
}
