using Contracts.Users;
using FluentValidation;

namespace ProjectManagement.Validators.Users
{
    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(request => request.UserNameOrEmail)
                .NotEmpty().WithMessage("Username or email is required");

            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(4).WithMessage("Password must be at least 8 characters long.");
        }
    }
}
