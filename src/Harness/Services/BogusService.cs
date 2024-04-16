using System.Reflection;
using Bogus;
using Harness.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;

namespace Harness.Services;

public class BogusService : IBogusService
{
    private readonly ILogger<BogusService> _logger;
    private readonly HashSet<string> _emailSet = new HashSet<string>();
    private const string HealthConditionResourceSuffix = "Services.HealthConditions.txt";

    private static readonly Dictionary<string, List<string>> EthnicityBackgrounds = new Dictionary<string, List<string>>
    {
        { "asian", new List<string> { "Bangladeshi", "Chinese", "Indian", "Pakistani" } },
        { "black", new List<string> { "African", "Black British", "Caribbean" } },
        { "mixed", new List<string> { "Asian and White", "Black African and White", "Black Caribbean and White" } },
        {
            "white",
            new List<string>
                { "British, English, Northern Irish, Scottish, or Welsh", "Irish", "Irish Traveller", "Roma" }
        },
        { "other", new List<string> { "Arab" } }
    };

    public BogusService(ILogger<BogusService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<Participant> GenerateFakeUsers(int participantRecords, int deletedRecords)
    {
        return GenerateUsers(participantRecords, false)
            .Concat(GenerateUsers(deletedRecords, true));
    }

    private IEnumerable<Participant> GenerateUsers(int count, bool isDeleted)
    {
        return new Faker<Participant>("en_GB")
            .CustomInstantiator(f => CreateParticipant(f, isDeleted))
            .RuleFor(p => p.Firstname, (f, p) => isDeleted ? null : f.Name.FirstName())
            .RuleFor(p => p.Lastname, (f, p) => isDeleted ? null : f.Name.LastName())
            .RuleFor(p => p.Email, (f, p) => isDeleted ? null : GenerateUniqueEmail(f))
            .RuleFor(p => p.SelectedLocale,
                (f, p) => isDeleted ? null : f.Random.ArrayElement(new[] { "en-GB", "cy-GB" }))
            .RuleFor(p => p.DateOfBirth, (f, p) => f.Date.Between(DateTime.Now.AddYears(-90), DateTime.Now.AddYears(-18)))
            .RuleFor(p => p.CreatedAtUtc, (f, p) => f.Date.Past(2))
            .RuleFor(p => p.UpdatedAtUtc, (f, p) => f.Random.Bool() ? (DateTime?)f.Date.Recent() : null)
            .RuleFor(p => p.ConsentRegistration, !isDeleted)
            .RuleFor(p => p.ConsentRegistrationAtUtc, f => isDeleted ? null : f.Date.Recent())
            .RuleFor(p => p.RemovalOfConsentRegistrationAtUtc, f => isDeleted ? f.Date.Recent() : null)
            .Rules((f, p) => ApplyStageTwoRules(p, f, isDeleted))
            .GenerateLazy(count);
    }

    private void ApplyStageTwoRules(Participant p, Faker f, bool isDeleted)
    {
        if (f.Random.Bool())
        {
            var hasDisability = f.Random.Bool();
            // Apply common fields for stage two
            p.Address = GenerateAddress(f, isDeleted);
            p.MobileNumber = isDeleted ? null : f.Phone.PhoneNumber();
            p.LandlineNumber = isDeleted ? null : f.Phone.PhoneNumber();
            p.SexRegisteredAtBirth = f.Random.ArrayElement(new[] { "Male", "Female", "Prefer Not to Say", });
            p.GenderIsSameAsSexRegisteredAtBirth = f.Random.Bool();
            p.EthnicGroup = f.Random.ArrayElement(EthnicityBackgrounds.Keys.ToArray());
            p.EthnicBackground = f.Random.ArrayElement(EthnicityBackgrounds[p.EthnicGroup].ToArray());
            p.Disability = hasDisability;
            p.DisabilityDescription = hasDisability
                ? f.Random.ArrayElement(new[] { "Yes, a lot", "Yes, a little", "Not at all", "Prefer not to say" })
                : null;
            p.HealthConditionInterests = f.Random.ListItems(LoadHealthConditionArrayFromResource(), f.Random.Int(0, 5));
            p.Stage2CompleteUtc = f.Date.Recent();
        }
    }

    private Participant CreateParticipant(Faker f, bool isDeleted)
    {
        var participant = new Participant();
        var id = f.Random.Guid().ToString();

        // Common fields for all participants
        participant.Pk = isDeleted ? "DELETED#" + id : "PARTICIPANT#" + id;
        participant.Sk = isDeleted ? "DELETED#" : "PARTICIPANT#";

        // Deciding which IDs to assign
        var option = f.Random.Int(1, 3);
        switch (option)
        {
            case 1: // Only ParticipantId
                participant.ParticipantId = id;
                break;
            case 2: // NhsId and NhsNumber
                participant.NhsId = id;
                participant.NhsNumber = isDeleted ? null : f.Random.Replace("{##########}");
                break;
            case 3: // NhsId, NhsNumber and ParticipantId
                participant.NhsId = id;
                participant.NhsNumber = isDeleted ? null : f.Random.Replace("{##########}");
                participant.ParticipantId = f.Random.Guid().ToString();
                break;
        }

        return participant;
    }

    private string GenerateUniqueEmail(Faker f)
    {
        var email = f.Internet.Email();
        while (_emailSet.Contains(email))
        {
            email = f.Internet.Email();
        }

        _emailSet.Add(email);
        return email;
    }

    private ParticipantAddressModel GenerateAddress(Faker f, bool isDeleted = false)
    {
        return new ParticipantAddressModel
        {
            AddressLine1 = isDeleted ? null : f.Address.StreetAddress(),
            AddressLine2 = isDeleted ? null : f.Address.SecondaryAddress(),
            AddressLine3 = isDeleted ? null : f.Address.County(),
            AddressLine4 = isDeleted ? null : f.Address.Country(),
            Town = isDeleted ? null : f.Address.City(),
            Postcode = isDeleted ? GetOutcodeFromPostcode(f.Address.ZipCode()) : f.Address.ZipCode(),
        };
    }

    private static string[] LoadHealthConditionArrayFromResource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly
            .GetManifestResourceNames()
            .SingleOrDefault(str =>
                str.EndsWith(HealthConditionResourceSuffix, StringComparison.InvariantCultureIgnoreCase));

        if (string.IsNullOrWhiteSpace(resourceName))
        {
            throw new FileNotFoundException($"Email template resource '{HealthConditionResourceSuffix}' not found.");
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string? GetOutcodeFromPostcode(string? postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return null;
        }

        var postcodeWithoutSpace = postcode.Replace(" ", "");
        return postcodeWithoutSpace[..^3];
    }
}
