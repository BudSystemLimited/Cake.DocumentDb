using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Cake.Core.Diagnostics;
using Dapper;

namespace Cake.DocumentDb.Migration
{
    public abstract class SqlDocumentMigration : ISqlDocumentMigration
    {
        public ICakeLog Log { get; set; }
        public SqlDatabaseConnectionSettings[] ConnectionDetails { get; set; }
        protected IDictionary<string, IEnumerable<dynamic>> Data { get; set; }

        public abstract string Description { get; }
        public abstract string DatabaseName { get; }
        public abstract string CollectionName { get; }
        public abstract string PartitionKey { get; }
        public abstract SqlStatement[] SqlStatements { get; }
        public abstract void Transform(dynamic item);

        public void ExecuteSql()
        {
            foreach (var sqlStatement in SqlStatements)
            {
                Log.Write(Verbosity.Normal, LogLevel.Information, $"Executing Sql Using Source {sqlStatement.DataSource} and Statement {sqlStatement.Statement}");
                using (var conn = new SqlConnection(GetConnection(sqlStatement.DataSource)))
                {
                    conn.Open();
                    Data.Add(sqlStatement.DataSource, conn.Query<dynamic>(sqlStatement.Statement));
                }
            }
        }

        protected SqlDocumentMigration()
        {
            Data = new Dictionary<string, IEnumerable<dynamic>>();
        }

        private string GetConnection(string source)
        {
            var databaseConnectionDetail = ConnectionDetails.FirstOrDefault(cd => string.Equals(cd.DataSource, source, StringComparison.CurrentCultureIgnoreCase));

            return databaseConnectionDetail?.ConnectionString;
        }
    }
}
