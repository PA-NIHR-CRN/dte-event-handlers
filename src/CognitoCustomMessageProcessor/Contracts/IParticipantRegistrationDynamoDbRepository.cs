using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CognitoCustomMessageProcessor.Contracts;

public interface IParticipantRegistrationDynamoDbRepository
{
    Task<string> GetParticipantLocaleAsync(string participantId, [EnumeratorCancellation] CancellationToken cancellationToken = default);
}