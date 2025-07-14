using MediatR;

namespace MAPP.Modules.Observations.Application.Classifications.Commands.SeedClassificationData;

/// <summary>
/// Command to seed classification data from JSON file
/// </summary>
public record SeedClassificationDataCommand : IRequest<bool>
{
    public string JsonFilePath { get; init; } = string.Empty;
    public bool OverwriteExisting { get; init; } = false;
}
