#addin "nuget:?package=Cake.SqlServer&version=1.9.0"
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

Task("Execute-Sql")
    .IsDependentOn("Create-Database")
    .Does(() =>
    {
        using (var connection = OpenSqlConnection("Server=.;User Id=sa;Password=ChorusAlan;Initial Catalog=cake-documentdb"))
        {
            ExecuteSqlFile(connection, "./cake-documentdb.sql");
        }    

        using (var connection = OpenSqlConnection("Server=.;User Id=sa;Password=ChorusAlan;Initial Catalog=cake-documentdb-two"))
        {
            ExecuteSqlFile(connection, "./cake-documentdb-two.sql");
        }            
    });

Task("Create-Database")
    .Does(() =>
    {
        Information("Create cake-documentdb");
        CreateDatabaseIfNotExists("Server=.;User Id=sa;Password=ChorusAlan", "cake-documentdb");

        Information("Create cake-documentdb-two");
        CreateDatabaseIfNotExists("Server=.;User Id=sa;Password=ChorusAlan", "cake-documentdb-two");
    });

Task("Copy-Dependencies")
    .Does(() =>
    {
        var path = ".\\tools\\Microsoft.Azure.DocumentDB\\runtimes\\win7-x64\\native";
        if (DirectoryExists(path))
        {
            Information("Copying dependencies");
            CopyFiles(path + "\\*.dll", ".\\");
        }
        else
        {
            Error("Could not find dependencies to copy");
        }
    });

Task("Default")
    .IsDependentOn("Execute-Sql")
    .IsDependentOn("Copy-Dependencies")
    .Does(() =>
    {
        var migrations = GetFiles("./Cake.DocumentDb.IntegrationTests.Migration/bin/Debug/Cake.DocumentDb.IntegrationTests.Migration.dll");

        foreach(var migration in migrations) {
            Information("Running Document Migration: " + migration.FullPath);
            RunDocumentSeed(
                migration.FullPath,
				new DocumentDbMigrationSettings {
					Connection = new ConnectionSettings {
						Endpoint = "https://localhost:8081",
						Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
					},
					SqlConnections = new []
					{
						new SqlDatabaseConnectionSettings { DataSource = "MyDataSourceOne", ConnectionString = "Server=.;Database=cake-documentdb;User Id=sa;Password=ChorusAlan;" }
					},
					Profile = "local"
                });
        }
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
