using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Migration
{
    public interface IDocumentMigration
    {
        ICakeLog Log { get; set; }
        string Description { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
        string PartitionKey { get; }
        void Transform(dynamic item);
    }
}