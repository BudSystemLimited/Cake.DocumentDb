using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cake.DocumentDb.Attributes;

namespace Cake.DocumentDb.Providers
{
    public class InstanceProvider
    {
        public static IList<TEntity> GetInstances<TEntity>(string assembly, string profile)
            where TEntity : class 
        {
            return (from t in Assembly.LoadFile(assembly).GetTypes()
                where 
                    (t.GetInterfaces().Contains(typeof(TEntity)) ||
                    t.IsSubclassOf(typeof(TEntity))) && 
                    t.GetConstructor(Type.EmptyTypes) != null && 
                    (t.CustomAttributes.All(a => 
                        a.AttributeType != typeof(ProfileAttribute)) || 
                        t.GetCustomAttribute<ProfileAttribute>().Profiles.Contains(profile))
                select Activator.CreateInstance(t) as TEntity)
                .ToList();
        }
    }
}
