using System.Threading.Tasks;
using CognitoCustomMessageProcessor.Domain;

namespace CognitoCustomMessageProcessor.Contracts;

public interface IParticipantRegistrationDynamoDbRepository
{
    Task<Participant> GetParticipantAsync(string userId);
}