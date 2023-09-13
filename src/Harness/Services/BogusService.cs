using Bogus;
using Harness.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;

namespace Harness.Services;

public class BogusService: IBogusService
{
    private readonly ILogger<BogusService> _logger;
    private readonly HashSet<string> _emailSet = new HashSet<string>();

    public BogusService(ILogger<BogusService> logger)
    {
        _logger = logger;
    }
    private List<string> GenerateUniqueLoremWords(Faker faker, int wordCount)
    {
        var words = new HashSet<string>();

        while (words.Count < wordCount)
        {
            words.Add(faker.Lorem.Word());
        }

        return words.ToList();
    }

    public IEnumerable<Participant> GenerateFakeUsers(int count)
    {
        _logger.LogInformation("Generating {Count} fake users", count);
        
        var fakeUsers = new Faker<Participant>("en_GB")
            .CustomInstantiator(f =>
            {
                var id = f.Random.Guid().ToString();
                return new Participant
                {
                    Pk = "PARTICIPANT#" + id,
                    ParticipantId = id
                };
            })
            .RuleFor(p => p.Sk, f => "PARTICIPANT#")
            // Ensure Email uniqueness
            .RuleFor(p => p.Email, (f, u) =>
            {
                var email = f.Internet.Email();
                while (_emailSet.Contains(email))
                {
                    email = f.Internet.Email();
                }
                _emailSet.Add(email);
                return email;
            })
            .RuleFor(p => p.Firstname, f => f.Name.FirstName())
            .RuleFor(p => p.Lastname, f => f.Name.LastName())
            .RuleFor(p => p.ConsentRegistration, f => f.Random.Bool())
            .RuleFor(p => p.SelectedLocale, f => f.Random.ArrayElement(new[] { "en-GB", "cy-GB" }))
            .RuleFor(p => p.MobileNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.LandlineNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.Address, f => new ParticipantAddressModel
            {
                AddressLine1 = f.Address.StreetAddress(),
                AddressLine2 = f.Address.StreetAddress(),
                AddressLine3 = f.Address.StreetAddress(),
                AddressLine4 = f.Address.StreetAddress(),
                Town = f.Address.City(),
                Postcode = f.Address.ZipCode()
            })
            .RuleFor(p => p.DateOfBirth, f => f.Date.Past(80))
            .RuleFor(p => p.SexRegisteredAtBirth, f => f.Random.ArrayElement(new[] { "Male", "Female" }))
            .RuleFor(p => p.GenderIsSameAsSexRegisteredAtBirth, f => f.Random.Bool())
            .RuleFor(p => p.EthnicGroup, f => f.Random.ArrayElement(new[] { "White", "Black", "Asian", "Mixed", "Other" }))
            .RuleFor(p => p.EthnicBackground, f => f.Random.ArrayElement(new[] { "British", "African", "Chinese", "Indian", "Other" }))
            .RuleFor(p => p.Disability, f => f.Random.Bool())
            .RuleFor(p => p.DisabilityDescription, f => f.Random.Bool() ? f.Lorem.Sentence() : null)
            .RuleFor(p => p.HealthConditionInterests, f => GenerateUniqueLoremWords(f, f.Random.Int(1, 5)))
            .RuleFor(p => p.CreatedAtUtc, f => f.Date.Past(2))
            .RuleFor(p => p.UpdatedAtUtc, f => f.Random.Bool() ? (DateTime?)f.Date.Recent() : null)
            .GenerateLazy(count);
        
        return fakeUsers;
    }
}