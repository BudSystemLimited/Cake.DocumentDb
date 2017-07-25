namespace Cake.DocumentDb.Migration
{
    public class SqlStatement
    {
        public string DataSource { get; set; }
        public string Statement { get; set; }
        public string StatementLookupKey { get; set; }
    }
}