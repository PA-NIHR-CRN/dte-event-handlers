using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using FluentAssertions;
using NUnit.Framework;
using ScheduledJobs.Services;

namespace ScheduledJobsTests.Services
{
    [TestFixture]
    public class CsvFileReaderTests
    {
        private CsvFileReader _csvFileReader;
        
        [SetUp]
        public void SetUp()
        {
            _csvFileReader = new CsvFileReader();
        }
        
        [Test]
        public async Task ParseStringCsvContent_Parses_Correctly()
        {
            var stringContent = await File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}/Resources/CpmsImport/export1.csv");
            
            var result = _csvFileReader.ParseStringCsvContent<TestModelMap, TestModel>(stringContent).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(205);
            result.ElementAt(0).Region.Should().Be("Australia and Oceania");
            result.ElementAt(0).Country.Should().Be("Palau");
            result.ElementAt(0).ItemType.Should().Be("Office Supplies");
        }
        
        public class TestModel
        {
            public string Region { get; set; }
            public string Country { get; set; }
            public string ItemType { get; set; }
        }
    
        public sealed class TestModelMap : ClassMap<TestModel>
        {
            public TestModelMap()
            {
                Map(m => m.Region).Name("Region");
                Map(m => m.Country).Name("Country");
                Map(m => m.ItemType).Name("Item Type");
            }
        }
    }
}