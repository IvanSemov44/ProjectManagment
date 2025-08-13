using Application.Absrtactions;
using AutoMapper;
using Contracts.Users;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Application
{
    public class AuthenticationService(
        UserManager<User> userManager,
        IMapper mapper,
        IJwtService jwtService)
    : IAuthenticationService
    {
        public async Task<string> LoginUserAsync(LoginUserRequest request)
        {
            var user = (await userManager.FindByNameAsync(request.UserNameOrEmail)
                ?? await userManager.FindByEmailAsync(request.UserNameOrEmail))
                ?? throw new UnauthorizedAccessException("Ivalid username or email.");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid password");

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtService.GenerateToken(user, roles);

            return token;
        }

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
