using System;
using Amazon.DynamoDBv2.DataModel;
using Dte.Common.Converters;

namespace ScheduledJobs.Domain
{
    public class RtsData
    {
        [DynamoDBHashKey("PK")] public string Pk { get; set; }
        [DynamoDBRangeKey("SK"), DynamoDBGlobalSecondaryIndexHashKey] public string Sk { get; set; }

        [DynamoDBProperty] public string Name { get; set; }
        [DynamoDBGlobalSecondaryIndexRangeKey] public string NameSearch { get; set; }

        [DynamoDBProperty] public string Identifier { get; set; }
        [DynamoDBGlobalSecondaryIndexRangeKey] public string IdentifierSearch { get; set; }

        [DynamoDBProperty] public string Type { get; set; }
        [DynamoDBProperty] public string ParentOrganisation { get; set; }
        [DynamoDBProperty] public string Status { get; set; }
        [DynamoDBProperty] public DateTime? EffectiveStartDate { get; set; }
        [DynamoDBProperty] public DateTime? EffectiveEndDate { get; set; }
        [DynamoDBProperty] public DateTime? CreatedDate { get; set; }
        [DynamoDBProperty] public DateTime? ModifiedDate { get; set; }
        [DynamoDBProperty] public string AddressLine1 { get; set; }
        [DynamoDBProperty] public string AddressLine2 { get; set; }
        [DynamoDBProperty] public string AddressLine3 { get; set; }
        [DynamoDBProperty] public string AddressLine4 { get; set; }
        [DynamoDBProperty] public string AddressLine5 { get; set; }

        [DynamoDBProperty] public string Postcode { get; set; }
        [DynamoDBGlobalSecondaryIndexRangeKey] public string PostcodeSearch { get; set; }
	
        [DynamoDBProperty] public string UKCountryIdentifier { get; set; }
        [DynamoDBProperty] public string UKCountryName { get; set; }
        
        [DynamoDBProperty(typeof(DateTimeUtcConverter))] public DateTime CreatedAtUtc { get; set; }
    }
}