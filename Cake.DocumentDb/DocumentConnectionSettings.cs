namespace Cake.DocumentDb
{
    public class DocumentConnectionSettings
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
