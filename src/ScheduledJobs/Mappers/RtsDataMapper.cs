using System;
using System.Collections.Generic;
using ScheduledJobs.Models;
using ScheduledJobs.Responses;

namespace ScheduledJobs.Mappers
{
    public static class RtsDataMapper
    {
        public static RtsData MapTo(RtsOrganisationSite source)
        {
            return new RtsData
            {
                Pk = $"IDENTIFIER#{source.Identifier}",
                Sk = "RTSSITE#",
                Name = source.Name,
                NameSearch = source.Name.ToLower(),
                Identifier = source.Identifier,
                IdentifierSearch = source.Identifier.ToLower(),
                Type = source.Type,
                ParentOrganisation = source.ParentOrganisation,
                Status = source.Status,
                EffectiveStartDate = source.EffectiveStartDate,
                EffectiveEndDate = source.EffectiveEndDate,
                CreatedDate = source.CreatedDate,
                ModifiedDate = source.ModifiedDate,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                AddressLine4 = source.AddressLine4,
                AddressLine5 = source.AddressLine5,
                Postcode = source.Postcode,
                PostcodeSearch = source.Postcode.ToLower().Replace(" ", ""),
                UKCountryIdentifier = source.UkCountryIdentifier,
                UKCountryName = source.UkCountryName,
                CreatedAtUtc = DateTime.Now
            };
        }

        public static RtsData MapTo(RtsOrganisation source)
        {
            return new RtsData
            {
                Pk = $"IDENTIFIER#{source.Identifier}",
                Sk = "RTSORG#",
                Name = source.Name,
                NameSearch = source.Name.ToLower(),
                Identifier = source.Identifier,
                IdentifierSearch = source.Identifier.ToLower(),
                Type = source.Type,
                ParentOrganisation = source.ParentOrganisation,
                Status = source.Status,
                EffectiveStartDate = source.EffectiveStartDate,
                EffectiveEndDate = source.EffectiveEndDate,
                CreatedDate = source.CreatedDate,
                ModifiedDate = source.ModifiedDate,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                AddressLine4 = source.AddressLine4,
                AddressLine5 = source.AddressLine5,
                Postcode = source.Postcode,
                PostcodeSearch = source.Postcode.ToLower().Replace(" ", ""),
                UKCountryIdentifier = source.UkCountryIdentifier,
                UKCountryName = source.UkCountryName,
                CreatedAtUtc = DateTime.Now
            };
        }
        
        public static IDictionary<string, string> GetMappedTypeDictionary()
        {
            return new Dictionary<string, string>
            {
                {"CAREHOMESITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Care Home"},
                {"CT@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Care Trust"},
                {"CTSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Care Trust Site"},
                {"CCGSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Clinical Commissioning Group Site"},
                {"DISPENSARYSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Dispensary Site"},
                {"EDEST01@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Educational Establishment"},
                {"GPSURGERYSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "GP Branch Surgery Site"},
                {"GPPRACTICE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "GP Practice"},
                {"HOMECAREAGENCY@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Home Care Agency"},
                {"HOSPICE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Hospice"},
                {"ISHPSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "ISHP Site"},
                {"LAUTHSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Local Authority Site"},
                {"NHSDENTAL@2.16.840.1.113883.2.1.3.8.5.11.1.106", "NHS Dental Practice"},
                {"TRUST@2.16.840.1.113883.2.1.3.8.5.11.1.106", "NHS Trust"},
                {"TRUSTSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "NHS Trust Site"},
                {"NIHSCTSite@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Northern Ireland Health & Social Care Trust Site"},
                {"NRSRY_SCHL@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Nursery School"},
                {"OPTICALSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Optical Site"},
                {"PATHLABSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Path Lab Site"},
                {"PRISON@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Prison"},
                {"SRHBSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Scottish Regional Health Board Site"},
                {"SARC@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Sexual Assault Referral Centre"},
                {"UNIVERSITY@2.16.840.1.113883.2.1.3.8.5.11.1.106", "University"},
                {"WLHBSITE@2.16.840.1.113883.2.1.3.8.5.11.1.106", "Welsh Local Health Board Site"}
            };
        }
    }
}