using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Seed.Seeds
{
    [Profile("random")]
    public class SeedWithRandomProfileWillNotCreate : ISeedDocument
    {
        public string FriendlyName => "Random Seed Test";
        public string Database => "cakeddbseedtest";
        public string Collection => "MyCollection";
        public string PartitionKey => "/mypartitionKey";

        public object Document()
        {
            return new
            {
                id = "4",
                mypartitionKey = "4",
                firstname = "Random",
                surname = "Profile"
            };
        }
    }
}