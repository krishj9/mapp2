using MediatR;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Domain.Entities;
using MAPP.Modules.Observations.Domain.ValueObjects;

namespace MAPP.Modules.Observations.Application.Observations.Commands.CreateObservation;

/// <summary>
/// Create observation command handler following CQRS pattern from Ardalis template
/// </summary>
public class CreateObservationCommandHandler : IRequestHandler<CreateObservationCommand, int>
{
    private readonly IObservationsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateObservationCommandHandler(
        IObservationsDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateObservationCommand request, CancellationToken cancellationToken)
    {
        var observation = new Observation(
            request.Title,
            request.Description,
            _currentUserService.UserId);

        observation.SetPriority(Priority.FromValue(request.Priority));

        if (request.ObservedAt.HasValue)
        {
            observation.SetObservedAt(request.ObservedAt.Value);
        }

        if (!string.IsNullOrEmpty(request.Location))
        {
            observation.SetLocation(request.Location);
        }

        _context.Observations.Add(observation);

        await _context.SaveChangesAsync(cancellationToken);

        return observation.Id;
    }
}
