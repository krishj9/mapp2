using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database - shared worldplanning database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var worldPlanningDb = postgres.AddDatabase("worldplanning");

// Add Planning API Service
var planningApi = builder.AddProject<Projects.MAPP_Services_Planning>("planning-api")
    .WithReference(worldPlanningDb)
    .WaitFor(postgres);

var app = builder.Build();

app.Run();
