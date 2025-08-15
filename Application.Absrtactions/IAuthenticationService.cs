using Contracts.Tokens;
using Contracts.Users;
using Microsoft.AspNetCore.Identity;

namespace Application.Absrtactions
{
    public interface IAuthenticationService
    {
        Task<TokenResponse> LoginUserAsync(LoginUserRequest request);
        Task<IdentityResult> RegisterUserAsync(RegisterUserRequest request);
        Task<TokenResponse> RefreshAccessTokenAsync(
            RefreshTokenRequest request,
            CancellationToken cancellationToken = default);
    }
}
