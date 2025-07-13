using FluentValidation;

namespace MAPP.Modules.Planning.Application.Plans.Commands.CreatePlan;

/// <summary>
/// Create plan command validator following Ardalis patterns
/// </summary>
public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
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

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("Start date must be before or equal to end date")
            .WithName("DateRange");
    }
}
