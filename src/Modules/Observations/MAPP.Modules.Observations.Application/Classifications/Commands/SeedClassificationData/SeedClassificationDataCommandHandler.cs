using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Application.Classifications.Commands.SeedClassificationData;

/// <summary>
/// Handler for seeding classification data from JSON file
/// </summary>
public class SeedClassificationDataCommandHandler : IRequestHandler<SeedClassificationDataCommand, bool>
{
    private readonly IObservationsDbContext _context;
    private readonly ILogger<SeedClassificationDataCommandHandler> _logger;

    public SeedClassificationDataCommandHandler(
        IObservationsDbContext context,
        ILogger<SeedClassificationDataCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(SeedClassificationDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!File.Exists(request.JsonFilePath))
            {
                _logger.LogError("JSON file not found at path: {FilePath}", request.JsonFilePath);
                return false;
            }

            // Check if data already exists
            var existingDomainsCount = await _context.ObservationDomains.CountAsync(cancellationToken);
            if (existingDomainsCount > 0 && !request.OverwriteExisting)
            {
                _logger.LogInformation("Classification data already exists. Use OverwriteExisting=true to replace.");
                return true;
            }

            // Read and parse JSON
            var jsonContent = await File.ReadAllTextAsync(request.JsonFilePath, cancellationToken);
            var jsonData = JsonSerializer.Deserialize<JsonClassificationData>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (jsonData?.Domains == null)
            {
                _logger.LogError("Invalid JSON structure - domains not found");
                return false;
            }

            // Clear existing data if overwriting
            if (request.OverwriteExisting)
            {
                _context.ProgressionPoints.RemoveRange(_context.ProgressionPoints);
                _context.ObservationAttributes.RemoveRange(_context.ObservationAttributes);
                _context.ObservationDomains.RemoveRange(_context.ObservationDomains);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Seed domains, attributes, and progression points
            await SeedData(jsonData, cancellationToken);

            _logger.LogInformation("Successfully seeded {DomainCount} domains with classification data", 
                jsonData.Domains.Count);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding classification data from {FilePath}", request.JsonFilePath);
            return false;
        }
    }

    private async Task SeedData(JsonClassificationData jsonData, CancellationToken cancellationToken)
    {
        foreach (var domainData in jsonData.Domains)
        {
            var domain = ObservationDomain.Create(
                domainData.Id,
                domainData.Name,
                domainData.CategoryName,
                domainData.CategoryTitle,
                domainData.SortOrder);

            _context.ObservationDomains.Add(domain);

            foreach (var attributeData in domainData.Attributes)
            {
                var attribute = ObservationAttribute.Create(
                    attributeData.Id,
                    attributeData.Number,
                    attributeData.Name,
                    attributeData.SortOrder,
                    domainData.Id,
                    attributeData.CategoryInformation);

                _context.ObservationAttributes.Add(attribute);

                foreach (var progressionData in attributeData.ProgressionPoints)
                {
                    var progressionPoint = ProgressionPoint.Create(
                        progressionData.ProgressionId,
                        progressionData.Points,
                        progressionData.Title,
                        progressionData.Description,
                        progressionData.SortOrder,
                        attributeData.Id,
                        progressionData.Order,
                        progressionData.CategoryInformation);

                    _context.ProgressionPoints.Add(progressionPoint);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

// JSON deserialization models
public class JsonClassificationData
{
    public List<JsonDomain> Domains { get; set; } = new();
}

public class JsonDomain
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryTitle { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<JsonAttribute> Attributes { get; set; } = new();
}

public class JsonAttribute
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CategoryInformation { get; set; }
    public int SortOrder { get; set; }
    public List<JsonProgressionPoint> ProgressionPoints { get; set; } = new();
}

public class JsonProgressionPoint
{
    public int ProgressionId { get; set; }
    public int Points { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Order { get; set; }
    public string? CategoryInformation { get; set; }
    public int SortOrder { get; set; }
}
