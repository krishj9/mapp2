using MediatR;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Domain.Entities;

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
            request.ChildId,
            request.ChildName,
            request.TeacherId,
            request.TeacherName,
            request.DomainId,
            request.DomainName,
            request.AttributeId,
            request.AttributeName,
            request.ObservationText,
            request.ObservationDate,
            request.LearningContext,
            request.IsDraft);

        // Set progression points
        observation.SetProgressionPoints(request.ProgressionPointIds);

        // Set tags
        observation.SetTags(request.Tags);

        _context.Observations.Add(observation);

        await _context.SaveChangesAsync(cancellationToken);

        return observation.Id;
    }
}
