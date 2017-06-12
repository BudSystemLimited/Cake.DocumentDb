﻿using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.DocumentDb.Operations;
using LogLevel = Cake.Core.Diagnostics.LogLevel;
using Verbosity = Cake.Core.Diagnostics.Verbosity;

namespace Cake.DocumentDb
{
    [CakeAliasCategory("DocumentDatabase")]
    public static class DocumentDatabaseAlias
    {
        [CakeMethodAlias]
        public static void RunDocumentSeed(this ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Using Profile: " + settings.Profile);

            DatabaseCreations.Run(context, assembly, settings);
            CollectionCreations.Run(context, assembly, settings);
            Seeds.Run(context, assembly, settings);
            Migrations.Run(context, assembly, settings);
        }
    }
}
