using Dte.Common.Lambda;
using Harness.Startup;

var builder = WebApplication.CreateBuilder(args);
var executionEnvironment = new LambdaExecutionEnvironment
{
    EnvironmentName = "Development",
    IsLambda = false,
    RunAsQueueListener = false
};

// Register lambda-related services
ScheduledJobs.DependencyRegistration.RegisterServices(builder.Services, executionEnvironment, builder.Configuration);
CognitoCustomMessageProcessor.DependencyRegistration.RegisterServices(builder.Services, executionEnvironment,
    builder.Configuration);

// Register additional development services
builder.Services.RegisterServices(builder.Environment, builder.Configuration);

var app = builder.Build();

// Apply Swagger configurations
app.ConfigureSwagger();

// Apply middleware components to the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();