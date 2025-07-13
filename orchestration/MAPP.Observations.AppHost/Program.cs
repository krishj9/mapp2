using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database - shared worldplanning database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var worldPlanningDb = postgres.AddDatabase("worldplanning");

// Add Observations API Service
var observationsApi = builder.AddProject<Projects.MAPP_Services_Observations>("observations-api")
    .WithReference(worldPlanningDb)
    .WaitFor(postgres);

var app = builder.Build();

app.Run();
