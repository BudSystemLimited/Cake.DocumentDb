namespace Cake.DocumentDb
{
    public class DocumentDbMigrationSettings
    {
        public ConnectionSettings Connection { get; set; }
        public SqlDatabaseConnectionSettings[] SqlConnections { get; set; }
        public StorageConnectionSettings StorageConnection { get; set; }
        public string Profile { get; set; }
    }
}