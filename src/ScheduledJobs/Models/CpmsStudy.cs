using Amazon.DynamoDBv2.DataModel;
using CsvHelper.Configuration;

namespace ScheduledJobs.Models
{
    public class CpmsStudy
    {
        [DynamoDBHashKey("PK")] public string Pk { get; set; } 
        [DynamoDBRangeKey("SK")] public string Sk { get; set; }
	
        [DynamoDBProperty] public string Region { get; set; }
        [DynamoDBProperty] public string Country { get; set; }
        [DynamoDBProperty] public string ItemType { get; set; }
    }
    
    public sealed class CpmsStudyMap : ClassMap<CpmsStudy>
    {
        public CpmsStudyMap()
        {
            Map(m => m.Region).Name("Region");
            Map(m => m.Country).Name("Country");
            Map(m => m.ItemType).Name("Item Type");
        }
    }
}