#r .\tools\Dapper\lib\net45\Dapper.dll
#r .\Cake.DocumentDb\bin\Debug\Cake.DocumentDb.dll
#r .\tools\Newtonsoft.Json\lib\net45\Newtonsoft.Json.dll
#r .\tools\Microsoft.Azure.DocumentDB\lib\net45\Microsoft.Azure.Documents.Client.dll
#r .\tools\EnterpriseLibrary.TransientFaultHandling\lib\portable-net45+win+wp8\Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.dll
#r .\tools\Microsoft.Azure.DocumentDB.TransientFaultHandling\lib\net45\Microsoft.Azure.Documents.Client.TransientFaultHandling.dll

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .Does(() =>
    {
        var hydrations = GetFiles("./Cake.DocumentDb.IntegrationTests.Hydration/bin/Debug/Cake.DocumentDb.IntegrationTests.Hydration.dll");

        foreach(var hydration in hydrations) {
            Information("Running Document Hydration: " + hydration.FullPath);
            RunDocumentSeed(
                hydration.FullPath,
				new DocumentDbMigrationSettings {
					Connection = new ConnectionSettings {
						Endpoint = "https://localhost:8081",
						Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
					},
					SqlConnections = new []
					{
						new SqlDatabaseConnectionSettings { DataSource = "MyDataSourceOne", ConnectionString = "Server=.;Database=cake-documentdb;User Id=sa;Password=ChorusAlan;" },
						new SqlDatabaseConnectionSettings { DataSource = "MyDataSourceTwo", ConnectionString = "Server=.;Database=cake-documentdb-two;User Id=sa;Password=ChorusAlan;" }
					},
					Profile = "local"
                });
        }
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);