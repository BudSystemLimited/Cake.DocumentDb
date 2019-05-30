namespace Cake.DocumentDb
{
    public class WriteSettings
    {
        public double ThroughputFactor { get; set; }
        public int MinTaskCount { get; set; }
        public int MaxTaskCount { get; set; }

        public static WriteSettings Default = new WriteSettings
        {
            ThroughputFactor = 0.005,
            MinTaskCount = 1,
            MaxTaskCount = 150
        };
    }
}