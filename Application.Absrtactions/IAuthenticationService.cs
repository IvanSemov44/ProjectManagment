using Contracts.Users;
using Microsoft.AspNetCore.Identity;

namespace Application.Absrtactions
{
    public interface IAuthenticationService
    {
        Task<string> LoginUserAsync(LoginUserRequest request);
        Task<IdentityResult> RegisterUserAsync(RegisterUserRequest request);
    }
}
