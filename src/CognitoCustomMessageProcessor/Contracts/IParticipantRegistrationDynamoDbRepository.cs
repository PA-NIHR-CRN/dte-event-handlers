using System.Threading.Tasks;

namespace CognitoCustomMessageProcessor.Contracts;

public interface IParticipantRegistrationDynamoDbRepository
{
    Task<string> GetParticipantLocaleAsync(string participantId);
}