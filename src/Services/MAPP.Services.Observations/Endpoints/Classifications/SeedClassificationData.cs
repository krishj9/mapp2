using FastEndpoints;
using MediatR;
using MAPP.Modules.Observations.Application.Classifications.Commands.SeedClassificationData;

namespace MAPP.Services.Observations.Endpoints.Classifications;

/// <summary>
/// Request model for seeding classification data
/// </summary>
public class SeedClassificationDataRequest
{
    public string JsonFilePath { get; set; } = string.Empty;
    public bool OverwriteExisting { get; set; } = false;
}

/// <summary>
/// Response model for seeding classification data
/// </summary>
public class SeedClassificationDataResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Fast Endpoint for seeding classification data from JSON file
/// </summary>
public class SeedClassificationDataEndpoint : Endpoint<SeedClassificationDataRequest, SeedClassificationDataResponse>
{
    private readonly IMediator _mediator;

    public SeedClassificationDataEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/observations/classifications/seed");
        AllowAnonymous(); // TODO: Add proper authorization - this should be admin only
        Summary(s =>
        {
            s.Summary = "Seed classification data from JSON file";
            s.Description = "Seeds domains, attributes, and progression points from a JSON file";
            s.Response<SeedClassificationDataResponse>(200, "Seeding completed");
        });
    }

    public override async Task HandleAsync(SeedClassificationDataRequest req, CancellationToken ct)
    {
        var command = new SeedClassificationDataCommand
        {
            JsonFilePath = req.JsonFilePath,
            OverwriteExisting = req.OverwriteExisting
        };

        var success = await _mediator.Send(command, ct);

        var response = new SeedClassificationDataResponse
        {
            Success = success,
            Message = success 
                ? "Classification data seeded successfully" 
                : "Failed to seed classification data. Check logs for details."
        };

        await SendOkAsync(response, ct);
    }
}
