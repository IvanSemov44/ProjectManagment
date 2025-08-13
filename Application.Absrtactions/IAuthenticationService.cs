using Contracts.Users;
using Microsoft.AspNetCore.Identity;

namespace Application.Absrtactions
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserRequest request);
    }
}
