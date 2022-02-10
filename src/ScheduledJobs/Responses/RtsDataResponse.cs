using System;
using System.Collections.Generic;

namespace ScheduledJobs.Responses
{
    public class RtsDataResponse
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public Result Result { get; set; }
    }

    public class Result
    {
        public object RtsOids { get; set; }
        public List<RtsOrganisation> RtsOrganisations { get; set; }
        public List<RtsOrganisationSite> RtsOrganisationSites { get; set; }
        public object RtsOrganisationRoles { get; set; }
        public object RtsTermsets { get; set; }
        public long TotalRecords { get; set; }
        public long PageSize { get; set; }
        public long CurrentPageNumber { get; set; }
        public ResultResult ResultResult { get; set; }
    }

    public class ResultResult
    {
        public string Result { get; set; }
        public object Errors { get; set; }
        public object DetailedErrors { get; set; }
        public object Entity { get; set; }
    }

    public class RtsOrganisation
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public string ParentOrganisation { get; set; }
        public string Status { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string Postcode { get; set; }
        public string UkCountryIdentifier { get; set; }
        public string UkCountryName { get; set; }
    }

    public class RtsOrganisationSite
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public string ParentOrganisation { get; set; }
        public string Status { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string Postcode { get; set; }
        public string UkCountryIdentifier { get; set; }
        public string UkCountryName { get; set; }
    }
}