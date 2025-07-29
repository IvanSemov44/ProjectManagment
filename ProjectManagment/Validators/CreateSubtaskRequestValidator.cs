using Contracts.Subtasks;
using FluentValidation;

namespace ProjectManagement.Validators
{
    public class CreateSubtaskRequestValidator : AbstractValidator<CreateSubtaskRequest>
    {
        public CreateSubtaskRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(5, 30)
                .WithMessage("Subtask title must be between 5 and 30 characters long.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .Length(15, 120)
                .WithMessage("Subtask description must by between 15 and 120 characters long.");
        }
    }
}
