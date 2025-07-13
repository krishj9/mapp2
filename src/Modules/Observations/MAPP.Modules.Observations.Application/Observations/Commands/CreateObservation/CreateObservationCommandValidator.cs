using FluentValidation;

namespace MAPP.Modules.Observations.Application.Observations.Commands.CreateObservation;

/// <summary>
/// Create observation command validator following Ardalis patterns
/// </summary>
public class CreateObservationCommandValidator : AbstractValidator<CreateObservationCommand>
{
    public CreateObservationCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 4)
            .WithMessage("Priority must be between 1 (Low) and 4 (Critical)");

        RuleFor(x => x.Location)
            .MaximumLength(500)
            .WithMessage("Location must not exceed 500 characters");
    }
}
