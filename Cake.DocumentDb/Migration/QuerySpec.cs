using System.Reflection;
using Microsoft.Azure.Documents;

namespace Cake.DocumentDb.Migration
{
    public class QuerySpec
    {
        public SqlQuerySpec SqlQuerySpec { get; }

        public QuerySpec(string queryText, object parameters = null)
        {
            if (parameters == null)
            {
                SqlQuerySpec = new SqlQuerySpec(queryText);
            }
            else
            {
                var parameterCollection = new SqlParameterCollection();
                var properties = parameters.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    var parameter = new SqlParameter($"@{property.Name}", property.GetValue(parameters));
                    parameterCollection.Add(parameter);
                }
                SqlQuerySpec = new SqlQuerySpec(queryText, parameterCollection);
            }
        }
    }
}