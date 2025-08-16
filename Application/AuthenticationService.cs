using Application.Absrtactions;
using AutoMapper;
using Contracts.Tokens;
using Contracts.Users;
using Domain.Expetions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public class AuthenticationService(
        UserManager<User> userManager,
        IMapper mapper,
        IJwtService jwtService)
    : IAuthenticationService
    {
        public async Task<TokenResponse> LoginUserAsync(LoginUserRequest request)
        {
            var user = (await userManager.FindByNameAsync(request.UserNameOrEmail)
                ?? await userManager.FindByEmailAsync(request.UserNameOrEmail))
                ?? throw new UnauthorizedAccessException("Ivalid username or email.");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid password");

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtService.GenerateToken(user, roles);
            user.RefreshToken = jwtService.GenerateRefreshToken();
            user.RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(7);

            await userManager.UpdateAsync(user);

            return new TokenResponse(token, user.RefreshToken!);
        }

        public async Task<TokenResponse> RefreshAccessTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var user = await userManager.Users
                .Where(x => x.RefreshToken == request.RefreshToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || user.RefreshTokenExpiresOn < DateTime.UtcNow)
            {
                throw new RefreshTokenBadRequestException();
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtService.GenerateToken(user, roles);
            user.RefreshToken = jwtService.GenerateRefreshToken();
            user.RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(7);

            await userManager.UpdateAsync(user);

            return new TokenResponse(
                token,
                user.RefreshToken!);
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
