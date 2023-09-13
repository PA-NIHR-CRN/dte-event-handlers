using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Harness.Contracts;
using Harness.Repositories;
using Harness.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Harness.Startup;

public static class DependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IWebHostEnvironment env,
        IConfiguration configuration)
    {
        // Add services to the container.
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // check if in development
        if (env.IsDevelopment())
        {
            services.RemoveAll<IAmazonS3>();
            
            var awsOptions = configuration.GetAWSOptions<AmazonS3Config>("AwsSettings");
            var s3Options = awsOptions.DefaultClientConfig as AmazonS3Config;
            // s3Options.ForcePathStyle = true; uncomment if working with localstack
            services.AddAWSService<IAmazonS3>(awsOptions);
            var amazonDynamoDbConfig = new AmazonDynamoDBConfig();
            services.AddScoped<IDynamoDBContext>(_ => new DynamoDBContext(new AmazonDynamoDBClient(amazonDynamoDbConfig)));
            
            services.AddScoped<IBogusService, BogusService>();
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
            services.AddScoped<IParticipantService, ParticipantService>();
        }

        return services;
    }
}