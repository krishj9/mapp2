using FastEndpoints;
using FastEndpoints.Swagger;
using MAPP.BuildingBlocks.Web;
using MAPP.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.UseHttpsRedirection();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Endpoints.ShortNames = true;
});

app.MapDefaultEndpoints();

app.Run();
