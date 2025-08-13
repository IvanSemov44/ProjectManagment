using Application.Absrtactions;
using AutoMapper;
using Contracts.Users;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Application
{
    public class AuthenticationService(UserManager<User> userManager, IMapper mapper)
    : IAuthenticationService
    {
        public async Task<IdentityResult> RegisterUserAsync(RegisterUserRequest request)
        {
            var user = mapper.Map<User>(request);

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(user, request.Roles);
            }

            return result;
        }
    }
}
