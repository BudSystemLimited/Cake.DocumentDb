#r .\tools\Dapper\lib\net45\Dapper.dll
#r .\Cake.DocumentDb\bin\Debug\Cake.DocumentDb.dll
#r .\tools\Newtonsoft.Json\lib\net45\Newtonsoft.Json.dll
#r .\tools\Microsoft.Azure.DocumentDB\lib\net45\Microsoft.Azure.Documents.Client.dll



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
        var migrations = GetFiles("./Cake.DocumentDb.IntegrationTests.Deletions/bin/Debug/Cake.DocumentDb.IntegrationTests.Deletions.dll");

        foreach(var migration in migrations) {
            Information("Running Document Migration: " + migration.FullPath);
            RunDocumentSeed(
                migration.FullPath,
				new DocumentDbMigrationSettings {
					Connection = new ConnectionSettings {
						Endpoint = "https://localhost:8081",
						Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
					},
					Profile = "local"
                });
        }
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
