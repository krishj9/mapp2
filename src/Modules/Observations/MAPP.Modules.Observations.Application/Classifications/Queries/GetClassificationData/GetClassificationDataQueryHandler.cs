using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Application.Classifications.Services;

namespace MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;

/// <summary>
/// Handler for getting complete classification data with Redis caching
/// </summary>
public class GetClassificationDataQueryHandler : IRequestHandler<GetClassificationDataQuery, ClassificationDataVm>
{
    private readonly IObservationsDbContext _context;
    private readonly IClassificationCacheService _cacheService;
    private readonly ILogger<GetClassificationDataQueryHandler> _logger;

    public GetClassificationDataQueryHandler(
        IObservationsDbContext context,
        IClassificationCacheService cacheService,
        ILogger<GetClassificationDataQueryHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ClassificationDataVm> Handle(GetClassificationDataQuery request, CancellationToken cancellationToken)
    {
        // Try to get from cache first
        var cachedResult = await _cacheService.GetCachedClassificationDataAsync(request.IncludeInactive, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("Returning cached classification data, domains count: {DomainsCount}", cachedResult.Domains.Count);
            return cachedResult;
        }

        _logger.LogDebug("Cache miss, querying database for classification data");

        // Query database if not in cache
        var domainsQuery = _context.ObservationDomains
            .Include(d => d.Attributes.Where(a => request.IncludeInactive || a.IsActive))
                .ThenInclude(a => a.ProgressionPoints.Where(p => request.IncludeInactive || p.IsActive))
            .Where(d => request.IncludeInactive || d.IsActive)
            .OrderBy(d => d.SortOrder);

        var domains = await domainsQuery.ToListAsync(cancellationToken);

        var result = new ClassificationDataVm
        {
            Domains = domains.Select(d => new DomainDto
            {
                Id = d.Id,
                Name = d.Name,
                CategoryName = d.CategoryName,
                CategoryTitle = d.CategoryTitle,
                SortOrder = d.SortOrder,
                Attributes = d.Attributes
                    .OrderBy(a => a.SortOrder)
                    .Select(a => new AttributeDto
                    {
                        Id = a.Id,
                        Number = a.Number,
                        Name = a.Name,
                        CategoryInformation = a.CategoryInformation,
                        SortOrder = a.SortOrder,
                        ProgressionPoints = a.ProgressionPoints
                            .OrderBy(p => p.SortOrder)
                            .Select(p => new ProgressionPointDto
                            {
                                ProgressionId = p.Id,
                                Points = p.Points,
                                Title = p.Title,
                                Description = p.Description,
                                Order = p.Order,
                                CategoryInformation = p.CategoryInformation,
                                SortOrder = p.SortOrder
                            }).ToList()
                    }).ToList()
            }).ToList()
        };

        // Cache the result for future requests
        await _cacheService.SetCachedClassificationDataAsync(result, request.IncludeInactive, cancellationToken);

        _logger.LogInformation("Classification data loaded from database and cached, domains count: {DomainsCount}", result.Domains.Count);

        return result;
    }
}
