using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Seed.Seeds
{
    [Profile("local")]
    public class SeedWithLocalProfile : ISeedDocument
    {
        public string FriendlyName => "Local Seed Test";
        public string Database => "cakeddbseedtest";
        public string Collection => "MyCollection";
        public string PartitionKey => "/mypartitionKey";

        public object Document()
        {
            return new
            {
                id = "3",
                mypartitionKey = "3",
                firstname = "Local",
                surname = "Profile"
            };
        }
    }
}