using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Migration
{
    public interface ISqlDocumentMigration
    {
        ICakeLog Log { get; set; }
        SqlDatabaseConnectionSettings[] ConnectionDetails { get; set; }
        string Description { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
        string PartitionKey { get; }
        SqlStatement[] SqlStatements { get; }
        void Transform(dynamic item);
        void ExecuteSql();
    }
}
