using Contracts.Projects;
using FluentValidation;
using ProjectManagement.Validators.Subtasks;

namespace ProjectManagement.Validators.Projects
{
    public class BaseProjectRequestValidator<T>
        : AbstractValidator<T> where T : BaseProjectRequest
    {
        public BaseProjectRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(5, 30)
                .WithMessage("Project name must be between 5 and 30 characters long.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .Length(15, 120)
                .WithMessage("Project description must be between 15 and 120 characters long.");

            RuleFor(x => x.Subtasks)
                .ForEach(x => x.SetValidator(new CreateSubtaskRequestValidator()));
        }
    }
}
