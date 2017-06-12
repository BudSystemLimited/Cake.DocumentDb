namespace Cake.DocumentDb
{
    public class DocumentDbMigrationSettings
    {
        public ConnectionSettings Connection { get; set; }
        public string Profile { get; set; }
    }

    public class ConnectionSettings
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public SqlDatabaseConnectionDetail[] SqlConnectionDetails { get; set; }
    }

    public class SqlDatabaseConnectionDetail
    {
        public string DataSource { get; set; }
        public string ConnectionString { get; set; }
    }
}
