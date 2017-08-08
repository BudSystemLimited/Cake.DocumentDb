using System;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration.Loqacious;

namespace Cake.DocumentDb.Hydration
{
    public class DocumentHydration
    {
        internal MigrationAttribute Attribute { get; set; }

        internal DocumentHydrationTask Task { get; private set; }

        protected void Migrate(Action<DocumentHydrationConfigurationCreator> action)
        {
            var creator = new DocumentHydrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}