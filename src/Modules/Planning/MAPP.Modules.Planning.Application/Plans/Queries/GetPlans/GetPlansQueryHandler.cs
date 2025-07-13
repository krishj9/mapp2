using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MAPP.Modules.Planning.Application.Common.Interfaces;
using MAPP.Modules.Planning.Domain.Enums;

namespace MAPP.Modules.Planning.Application.Plans.Queries.GetPlans;

/// <summary>
/// Get plans query handler following CQRS pattern from Ardalis template
/// </summary>
public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, PlansVm>
{
    private readonly IPlanningDbContext _context;
    private readonly IMapper _mapper;

    public GetPlansQueryHandler(IPlanningDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PlansVm> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Plans.AsQueryable();

        // Apply filters
        if (request.Status.HasValue)
        {
            query = query.Where(p => (int)p.Status == request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(p => p.Priority.Value == request.Priority.Value);
        }

        if (!string.IsNullOrEmpty(request.OwnerId))
        {
            query = query.Where(p => p.OwnerId == request.OwnerId);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var plans = await query
            .OrderByDescending(p => p.Created)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<PlanBriefDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PlansVm
        {
            Plans = plans,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
