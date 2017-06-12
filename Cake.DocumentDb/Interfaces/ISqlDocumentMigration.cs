using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Interfaces
{
    public interface ISqlDocumentMigration
    {
        ICakeLog Log { get; set; }
        SqlDatabaseConnectionDetail[] ConnectionDetails { get; set; }
        string Description { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
        string PartitionKey { get; }
        SqlStatement[] SqlStatements { get; }
        void Transform(dynamic item);
        void ExecuteSql();
    }
}
