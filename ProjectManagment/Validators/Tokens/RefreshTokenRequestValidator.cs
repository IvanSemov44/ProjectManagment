using Contracts.Tokens;
using FluentValidation;

namespace ProjectManagement.Validators.Tokens
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("RefreshToken must not be empty");
        }
    }
}
