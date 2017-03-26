# Cake.DocumentDb

A Cake Addin for [Document Db](https://azure.microsoft.com/en-gb/services/documentdb/).

[![cakebuild.net](https://img.shields.io/badge/WWW-cakebuild.net-blue.svg)](http://cakebuild.net/)

[![Join the chat at https://gitter.im/cake-build/cake](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cake-build/cake?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Functionality

Supports creating databases, collections and seeding documents when given an assembly which has types implementing ICreateDocumentDatabase, ICreateDocumentDatabaseCollection and ISeedDocument.

## Usage

To use the addin just add it to Cake call the aliases and configure any settings you want.

```csharp
#addin "Cake.DocumentDb"
...

// Running Seeds
Task("Document-Seed")
    .Does(() =>
    {
        RunDocumentSeed(
            @"C:\myfilepath\Cake.Example.Seeds.dll",
            new DocumentConnectionSettings {
                Endpoint = "https://localhost:8081",
                Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
            });
    });