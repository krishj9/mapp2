using FluentValidation;

namespace MAPP.Modules.Observations.Application.Observations.Commands.CreateObservation;

/// <summary>
/// Create observation command validator following clean architecture patterns
/// </summary>
public class CreateObservationCommandValidator : AbstractValidator<CreateObservationCommand>
{
    public CreateObservationCommandValidator()
    {
        RuleFor(x => x.ChildId)
            .GreaterThan(0)
            .WithMessage("Child ID is required and must be greater than 0");

        RuleFor(x => x.ChildName)
            .NotEmpty()
            .WithMessage("Child name is required")
            .MaximumLength(200)
            .WithMessage("Child name must not exceed 200 characters");

        RuleFor(x => x.TeacherId)
            .GreaterThan(0)
            .WithMessage("Teacher ID is required and must be greater than 0");

        RuleFor(x => x.TeacherName)
            .NotEmpty()
            .WithMessage("Teacher name is required")
            .MaximumLength(200)
            .WithMessage("Teacher name must not exceed 200 characters");

        RuleFor(x => x.DomainId)
            .GreaterThan(0)
            .WithMessage("Domain ID is required and must be greater than 0");

        RuleFor(x => x.DomainName)
            .NotEmpty()
            .WithMessage("Domain name is required")
            .MaximumLength(200)
            .WithMessage("Domain name must not exceed 200 characters");

        RuleFor(x => x.AttributeId)
            .GreaterThan(0)
            .WithMessage("Attribute ID is required and must be greater than 0");

        RuleFor(x => x.AttributeName)
            .NotEmpty()
            .WithMessage("Attribute name is required")
            .MaximumLength(200)
            .WithMessage("Attribute name must not exceed 200 characters");

        RuleFor(x => x.ObservationText)
            .NotEmpty()
            .WithMessage("Observation text is required")
            .MaximumLength(5000)
            .WithMessage("Observation text must not exceed 5000 characters");

        RuleFor(x => x.ObservationDate)
            .NotEmpty()
            .WithMessage("Observation date is required")
            .LessThanOrEqualTo(DateTime.Now.AddDays(1))
            .WithMessage("Observation date cannot be more than 1 day in the future");

        RuleFor(x => x.LearningContext)
            .MaximumLength(1000)
            .WithMessage("Learning context must not exceed 1000 characters");

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .WithMessage("Tags cannot be empty")
            .MaximumLength(100)
            .WithMessage("Each tag must not exceed 100 characters");
    }
}
