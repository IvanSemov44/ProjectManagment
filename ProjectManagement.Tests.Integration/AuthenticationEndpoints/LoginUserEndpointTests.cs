using Contracts.Tokens;
using Contracts.Users;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ProjectManagement.Tests.Integration.AuthenticationEndpoints
{
    public class LoginUserEndpointTests(ProjectManagementApplicationfactory applicationfactory)
        : IClassFixture<ProjectManagementApplicationfactory>
    {

        private readonly HttpClient _httpClient = applicationfactory.CreateClient();

        [Fact]
        public async Task GivenValidRequest_WhenLoginUserIsInvoked_ThenOkIsReturned()
        {
            // Arrage
            var registerUser = new RegisterUserRequest(
                 Email: "testuser@code-maze.com",
                 UserName: "testUser",
                 FirstName: "Test",
                 LastName: "Test",
                 Password: "Test@1234",
                 PhoneNumber: null,
                 Roles: ["ProjectManager"]);
            await _httpClient.PostAsJsonAsync("api/v1/auth/register", registerUser);

            var request = new LoginUserRequest(
                UserNameOrEmail: registerUser.Email,
                Password: registerUser.Password);

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/v1/auth/login", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

            token.ShouldNotBeNull();
        }

        [Fact]
        public async Task GivenInvalidRequest_WhenRegisterUserIsInvoked_ThenUnAuthorizedIsReturned()
        {
            // Arrange
            var request = new LoginUserRequest(
                UserNameOrEmail: "pm@code-maze.com",
                Password: "123456789");

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/v1/auth/login", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GivenUnprocessableRequest_WhenRegisterUserIsInvoked_ThenUnprecessableEntityIsReturned()
        {
            // Arrange
            var request = new LoginUserRequest(
                UserNameOrEmail: "",
                Password: "12345678");

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/v1/auth/login", request);

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        }
    }
}
