using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Interfaces
{
    public interface ISqlDocumentMigration
    {
        ICakeLog Log { get; set; }
        SqlDatabaseConnectionDetail[] ConnectionDetails { get; set; }
        string Description { get; }
        string Database { get; }
        string Collection { get; }
        string PartitionKey { get; }
        SqlStatement[] SqlStatements { get; }
        void Transform(dynamic item);
        void ExecuteSql();
    }
}
