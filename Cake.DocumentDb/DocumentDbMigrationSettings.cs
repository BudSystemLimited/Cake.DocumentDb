namespace Cake.DocumentDb
{
    public class DocumentDbMigrationSettings
    {
        public ConnectionSettings Connection { get; set; }
        public SqlDatabaseConnectionDetail[] SqlConnectionDetails { get; set; }
        public string Profile { get; set; }
    }
}