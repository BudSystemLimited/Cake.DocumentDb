using System;
using System.Linq;

namespace Cake.DocumentDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProfileAttribute : Attribute
    {
        public string[] Profiles { get; set; }

        public ProfileAttribute(string profiles)
        {
            Profiles = profiles.Replace(" ", string.Empty).Split(',');
        }
    }
}
