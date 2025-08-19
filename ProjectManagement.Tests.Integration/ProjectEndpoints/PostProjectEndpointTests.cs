using Contracts.Projects;
using Contracts.Tokens;
using ProjectManagement.Tests.Integration.Common;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ProjectManagement.Tests.Integration.ProjectEndpoints
{
    public class PostProjectEndpointTests(ProjectManagementApplicationfactory applicationfactory)
        : IClassFixture<ProjectManagementApplicationfactory>
    {
        private readonly HttpClient _httpClient = applicationfactory.CreateClient();

        [Fact]
        public async Task GivenAuthenticatedUser_WhenCreateProjectIsInvoked_ThenCreatedIsReturned()
        {
            // Arrange
            var token = await AuthenticationHelper.RegisterUserAndAuthenticateAsPmAsync(_httpClient);
            var request = CreateProjectRequest(token, "Develop a new Api", "Develop an API thet deals with software project management.");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task GivenUnauthenticateUser_WhenCreateProjectIsInvoked_ThenUnauthorizedIsReturn()
        {
            // Arrange
            var request = CreateProjectRequest(null, "Develop a new Api", "Develop an API thet deals with software project management.");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        private static HttpRequestMessage CreateProjectRequest(TokenResponse? token, string name, string description)
        {
            var project = new CreateProjectRequest
            {
                Name = name,
                Description = description
            };

            var json = JsonSerializer.Serialize(project);

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/projects")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccesssToken);

            return request;
        }
    }
}
