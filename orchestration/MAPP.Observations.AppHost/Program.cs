using MAPP.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

// Add Observations API Service (using existing database connection)
var observationsApi = builder.AddProject<Projects.MAPP_Services_Observations>("observations-api")
    .WithHttpEndpoint(port: 5000, name: "http")
    .WithHttpsEndpoint(port: 5001, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var app = builder.Build();

app.Run();
