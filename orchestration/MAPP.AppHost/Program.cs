using MAPP.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgreSQL("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var planningDb = postgres.AddDatabase("planningdb");
var observationsDb = postgres.AddDatabase("observationsdb");
var userManagementDb = postgres.AddDatabase("usermanagementdb");
var reportsDb = postgres.AddDatabase("reportsdb");

// Add .NET API Services
var planningApi = builder.AddProject<Projects.MAPP_Services_Planning>("planning-api")
    .WithReference(planningDb)
    .WaitFor(postgres);

var observationsApi = builder.AddProject<Projects.MAPP_Services_Observations>("observations-api")
    .WithReference(observationsDb)
    .WaitFor(postgres);

var userManagementApi = builder.AddProject<Projects.MAPP_Services_UserManagement>("usermanagement-api")
    .WithReference(userManagementDb)
    .WaitFor(postgres);

var reportsApi = builder.AddProject<Projects.MAPP_Services_Reports>("reports-api")
    .WithReference(reportsDb)
    .WaitFor(postgres);

// Add Python GenAI Services
var planningAi = builder.AddPythonApp("planning-ai", "../src/Services/genai/planning-ai")
    .WithHttpEndpoint(port: 8001, targetPort: 8001)
    .WithEnvironment("DATABASE_URL", planningDb.ConnectionStringExpression)
    .WithEnvironment("PLANNING_API_URL", planningApi.GetEndpoint("https"));

var observationsAi = builder.AddPythonApp("observations-ai", "../src/Services/genai/observations-ai")
    .WithHttpEndpoint(port: 8002, targetPort: 8002)
    .WithEnvironment("DATABASE_URL", observationsDb.ConnectionStringExpression)
    .WithEnvironment("OBSERVATIONS_API_URL", observationsApi.GetEndpoint("https"));

var reportsAi = builder.AddPythonApp("reports-ai", "../src/Services/genai/reports-ai")
    .WithHttpEndpoint(port: 8003, targetPort: 8003)
    .WithEnvironment("DATABASE_URL", reportsDb.ConnectionStringExpression)
    .WithEnvironment("REPORTS_API_URL", reportsApi.GetEndpoint("https"));

// Add Angular Shell and MFEs
var shell = builder.AddNpmApp("shell", "../src/Web/shell")
    .WithHttpEndpoint(port: 4200, targetPort: 4200)
    .WithEnvironment("PLANNING_API_URL", planningApi.GetEndpoint("https"))
    .WithEnvironment("OBSERVATIONS_API_URL", observationsApi.GetEndpoint("https"))
    .WithEnvironment("USERMANAGEMENT_API_URL", userManagementApi.GetEndpoint("https"))
    .WithEnvironment("REPORTS_API_URL", reportsApi.GetEndpoint("https"));

var planningMfe = builder.AddNpmApp("planning-mfe", "../src/Web/planning-mfe")
    .WithHttpEndpoint(port: 4201, targetPort: 4201)
    .WithEnvironment("NODE_ENV", "development");

var observationsMfe = builder.AddNpmApp("observations-mfe", "../src/Web/observations-mfe")
    .WithHttpEndpoint(port: 4202, targetPort: 4202)
    .WithEnvironment("NODE_ENV", "development");

var userManagementMfe = builder.AddNpmApp("usermanagement-mfe", "../src/Web/usermanagement-mfe")
    .WithHttpEndpoint(port: 4203, targetPort: 4203)
    .WithEnvironment("NODE_ENV", "development");

var reportsMfe = builder.AddNpmApp("reports-mfe", "../src/Web/reports-mfe")
    .WithHttpEndpoint(port: 4204, targetPort: 4204)
    .WithEnvironment("NODE_ENV", "development");

var app = builder.Build();

app.Run();
