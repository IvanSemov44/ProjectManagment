using Contracts.Tokens;
using Contracts.Users;
using System.Net.Http.Json;

namespace ProjectManagement.Tests.Integration.Common
{
    public static class AuthenticationHelper
    {
        private static TokenResponse _pmToken = null!;
        private static TokenResponse _adminToken = null!;

        public static async Task<TokenResponse> RegisterUserAndAuthenticateAsPmAsync(HttpClient httpClient)
        {
            if (_pmToken is not null)
                return _pmToken;

            var registerUser = new RegisterUserRequest(
                 Email: "testuser@code-maze.com",
                 UserName: "testUser",
                 FirstName: "Test",
                 LastName: "Test",
                 Password: "Test@1234",
                 PhoneNumber: null,
                 Roles: ["ProjectManager"]);

            await httpClient.PostAsJsonAsync("api/v1/auth/register", registerUser);

            var request = new LoginUserRequest(
               UserNameOrEmail: registerUser.Email,
               Password: registerUser.Password);

            var response = await httpClient.PostAsJsonAsync("api/v1/auth/login", request);

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
            
            _pmToken = token!;
            
            return _pmToken;
        }

        public static async Task<TokenResponse> RegisterUserAndAuthenticateAsAdminAsync(HttpClient httpClient)
        {
            if (_pmToken is not null)
                return _pmToken;

            var registerUser = new RegisterUserRequest(
                 Email: "testuser@code-maze.com",
                 UserName: "testUser",
                 FirstName: "Test",
                 LastName: "Test",
                 Password: "Test@1234",
                 PhoneNumber: null,
                 Roles: ["Administrator"]);

            await httpClient.PostAsJsonAsync("api/v1/auth/register", registerUser);

            var request = new LoginUserRequest(
               UserNameOrEmail: registerUser.Email,
               Password: registerUser.Password);

            var response = await httpClient.PostAsJsonAsync("api/v1/auth/login", request);

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

            _adminToken = token!;

            return _adminToken;
        }
    }
}