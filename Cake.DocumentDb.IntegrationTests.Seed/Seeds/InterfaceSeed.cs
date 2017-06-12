using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Seed.Seeds
{
    public class InterfaceSeed : ISeedDocument
    {
        public string FriendlyName => "Interface Seed Test";
        public string Database => "cakeddbseedtest";
        public string Collection => "MyCollection";
        public string PartitionKey => "/mypartitionKey";

        public object Document()
        {
            return new
            {
                id = "2",
                mypartitionKey = "2",
                firstname = "Interface",
                surname = "Seed"
            };
        }
    }
}
