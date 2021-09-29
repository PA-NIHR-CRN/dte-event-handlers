using System.Threading.Tasks;
using Adapter.Contracts;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Domain.Persistence.Models;

namespace Infrastructure.Repositories
{
    public class StudyDynamoDbRepository : IStudyRepository
    {
        private const string HashKeyPrefix = "STUDY#";

        private readonly IDynamoDBContext _context;

        public StudyDynamoDbRepository(IAmazonDynamoDB client)
        {
            _context = new DynamoDBContext(client);
        }
        
        public async Task SaveStudyRegistration(StudyRegistration studyRegistration)
        {
            studyRegistration.Pk = $"{HashKeyPrefix}{studyRegistration.StudyId}";

            await _context.SaveAsync(studyRegistration);
        }
    }
}