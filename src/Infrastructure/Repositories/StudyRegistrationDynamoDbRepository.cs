using System.Threading.Tasks;
using Adapter.Contracts;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Common.Settings;
using Domain.Persistence.Models;

namespace Infrastructure.Repositories
{
    public class StudyRegistrationDynamoDbRepository : IStudyRegistrationRepository
    {
        private const string HashKeyPrefix = "STUDY#";

        private readonly IDynamoDBContext _context;
        private readonly DynamoDBOperationConfig _config;

        public StudyRegistrationDynamoDbRepository(IAmazonDynamoDB client, AwsSettings awsSettings)
        {
            _context = new DynamoDBContext(client);
            _config = new DynamoDBOperationConfig { OverrideTableName = awsSettings.StudyRegistrationDynamoDbTableName };
        }
        
        public async Task SaveStudyRegistration(StudyRegistration studyRegistration)
        {
            studyRegistration.Pk = $"{HashKeyPrefix}{studyRegistration.StudyId}";

            await _context.SaveAsync(studyRegistration, _config);
        }
    }
}