using MAPP.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database for Planning domain only
var postgres = builder.AddPostgreSQL("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var planningDb = postgres.AddDatabase("planningdb");

// Add Planning API Service
var planningApi = builder.AddProject<Projects.MAPP_Services_Planning>("planning-api")
    .WithReference(planningDb)
    .WaitFor(postgres);

// Add Planning GenAI Service
var planningAi = builder.AddPythonApp("planning-ai", "../src/Services/genai/planning-ai")
    .WithHttpEndpoint(port: 8001, targetPort: 8001)
    .WithEnvironment("DATABASE_URL", planningDb.ConnectionStringExpression)
    .WithEnvironment("PLANNING_API_URL", planningApi.GetEndpoint("https"));

// Add Planning MFE (when implemented)
// var planningMfe = builder.AddNpmApp("planning-mfe", "../src/Web/planning-mfe")
//     .WithHttpEndpoint(port: 4201, targetPort: 4200)
//     .WithEnvironment("PLANNING_API_URL", planningApi.GetEndpoint("https"));

var app = builder.Build();

app.Run();
