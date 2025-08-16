using Contracts.Users;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ProjectManagement.Tests.Integration.AuthenticationEndpoints
{
    public class RegisterUserEndpointTests(ProjectManagementApplicationfactory applicationfactory)
        : IClassFixture<ProjectManagementApplicationfactory>
    {
        private readonly HttpClient _httpClient=applicationfactory.CreateClient();

        [Fact]
        public async Task GivenValidRequest_WhenRegisterUserIsInvoked_ThenCreateIsReturned()
        {
            // Arrange
            var request = new RegisterUserRequest(
                Email: "testuser@code-maze.com",
                UserName: "testUser",
                FirstName: "Test",
                LastName: "Test",
                Password: "Test@1234",
                PhoneNumber: null,
                Roles: ["ProjectManager"]);

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/v1/auth/register", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task GivenInvalidRequest_WhenRegisterUserIsInvoked_ThenUnprocessableEnityIsReturned()
        {
            // Arrange
            var request = new RegisterUserRequest(
                 Email: "testuser@code-maze.com",
                 UserName: "testUser",
                 FirstName: "Test",
                 LastName: "Test",
                 Password: "Test@1234",
                 PhoneNumber: null,
                 Roles: []);

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/v1/auth/register", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task GivenExistingUser_WhenRegisterUserIsInvoked_ThenBadRequestIsReturned()
        {
            // Arrange
            var request = new RegisterUserRequest(
                 Email: "testuser@code-maze.com",
                 UserName: "testUser",
                 FirstName: "Test",
                 LastName: "Test",
                 Password: "Test@1234",
                 PhoneNumber: null,
                 Roles: ["ProjectManager"]);
            await _httpClient.PostAsJsonAsync("api/v1/auth/register", request);

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/v1/auth/register", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
