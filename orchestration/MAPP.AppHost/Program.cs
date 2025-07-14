using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add Redis for caching
var redis = builder.AddRedis("redis")
    .WithDataVolume();

// Add PostgreSQL database - single shared database with schema separation
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var worldPlanningDb = postgres.AddDatabase("worldplanning");

// Add .NET API Services - all sharing the same database with schema separation
var planningApi = builder.AddProject<Projects.MAPP_Services_Planning>("planning-api")
    .WithReference(worldPlanningDb)
    .WaitFor(postgres);

var observationsApi = builder.AddProject<Projects.MAPP_Services_Observations>("observations-api")
    .WithReference(worldPlanningDb)
    .WithReference(redis)
    .WaitFor(postgres)
    .WaitFor(redis);

var userManagementApi = builder.AddProject<Projects.MAPP_Services_UserManagement>("usermanagement-api")
    .WithReference(worldPlanningDb)
    .WaitFor(postgres);

var reportsApi = builder.AddProject<Projects.MAPP_Services_Reports>("reports-api")
    .WithReference(worldPlanningDb)
    .WaitFor(postgres);

var app = builder.Build();

app.Run();
