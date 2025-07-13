using MediatR;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.Modules.Planning.Application.Common.Interfaces;
using MAPP.Modules.Planning.Domain.Entities;
using MAPP.Modules.Planning.Domain.ValueObjects;

namespace MAPP.Modules.Planning.Application.Plans.Commands.CreatePlan;

/// <summary>
/// Create plan command handler following CQRS pattern from Ardalis template
/// </summary>
public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, int>
{
    private readonly IPlanningDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreatePlanCommandHandler(
        IPlanningDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = new Plan(
            request.Title,
            request.Description,
            _currentUserService.UserId);

        plan.SetPriority(Priority.FromValue(request.Priority));

        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            plan.SetDates(request.StartDate, request.EndDate);
        }

        _context.Plans.Add(plan);

        await _context.SaveChangesAsync(cancellationToken);

        return plan.Id;
    }
}
