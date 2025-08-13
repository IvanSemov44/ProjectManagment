using Contracts.Users;
using FluentValidation;

namespace ProjectManagement.Validators.Users
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserValidator()
        {
            RuleFor(request => request.Roles)
                .NotEmpty().WithMessage("Roles are required.")
                .Must(roles => roles.All(role => role is "Administrator" || role is "ProjectManager"))
                .WithMessage("Roles must only contain 'Administrator' or 'ProjectManager'.");
        }
    }
}
