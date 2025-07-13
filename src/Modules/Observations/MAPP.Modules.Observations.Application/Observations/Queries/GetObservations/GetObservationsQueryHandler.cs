using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MAPP.Modules.Observations.Application.Common.Interfaces;

namespace MAPP.Modules.Observations.Application.Observations.Queries.GetObservations;

/// <summary>
/// Get observations query handler following CQRS pattern from Ardalis template
/// </summary>
public class GetObservationsQueryHandler : IRequestHandler<GetObservationsQuery, ObservationsVm>
{
    private readonly IObservationsDbContext _context;
    private readonly IMapper _mapper;

    public GetObservationsQueryHandler(IObservationsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ObservationsVm> Handle(GetObservationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Observations.AsQueryable();

        // Apply filters
        if (request.Status.HasValue)
        {
            query = query.Where(o => (int)o.Status == request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(o => o.Priority.Value == request.Priority.Value);
        }

        if (!string.IsNullOrEmpty(request.ObserverId))
        {
            query = query.Where(o => o.ObserverId == request.ObserverId);
        }

        if (!string.IsNullOrEmpty(request.Location))
        {
            query = query.Where(o => o.Location != null && o.Location.Contains(request.Location));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(o => o.ObservedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(o => o.ObservedAt <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var observations = await query
            .OrderByDescending(o => o.ObservedAt ?? o.Created)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ObservationBriefDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new ObservationsVm
        {
            Observations = observations,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
