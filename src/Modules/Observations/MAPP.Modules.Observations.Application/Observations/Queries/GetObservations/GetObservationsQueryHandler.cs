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
        if (request.ChildId.HasValue)
        {
            query = query.Where(o => o.ChildId == request.ChildId.Value);
        }

        if (request.TeacherId.HasValue)
        {
            query = query.Where(o => o.TeacherId == request.TeacherId.Value);
        }

        if (request.DomainId.HasValue)
        {
            query = query.Where(o => o.DomainId == request.DomainId.Value);
        }

        if (request.AttributeId.HasValue)
        {
            query = query.Where(o => o.AttributeId == request.AttributeId.Value);
        }

        if (request.IsDraft.HasValue)
        {
            query = query.Where(o => o.IsDraft == request.IsDraft.Value);
        }

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            query = query.Where(o => o.ObservationText.Contains(request.SearchText) || 
                                   o.ChildName.Contains(request.SearchText) ||
                                   o.TeacherName.Contains(request.SearchText));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(o => o.ObservationDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(o => o.ObservationDate <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var observations = await query
            .OrderByDescending(o => o.ObservationDate)
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
