using ProjectManagement.Tests.Integration.Common;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;

namespace ProjectManagement.Tests.Integration.ProjectEndpoints
{
    public class DeleteProjectEndpointTests(ProjectManagementApplicationfactory applicationfactory)
        : IClassFixture<ProjectManagementApplicationfactory>
    {
        private const string ProjectId = "c9d4c053-49b6-410c-bc78-2d54a9991870";
        private readonly HttpClient _httpClient = applicationfactory.CreateClient();

        [Fact]
        public async Task GivenUnauthenticatedUser_WhenDeleteProjectIsInvoked_ThenUnauthorizedIsReturned()
        {
            // Act
            var response = await _httpClient.DeleteAsync($"/api/v1/projects/{ProjectId}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GivenUnauthorizedUser_WhenDeleteProjectIsInvoked_ThenForbiddenIsReturned()
        {
            // Arrange
            var token = await AuthenticationHelper.RegisterUserAndAuthenticateAsPmAsync(_httpClient);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/projects/{ProjectId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccesssToken);

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GivenAuthorizedUser_WhenDeleteProjectIsInvoked_ThenNoContentIsReturned()
        {
            // Arrange
            var token = await AuthenticationHelper.RegisterUserAndAuthenticateAsAdminAsync(_httpClient);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/projects/{ProjectId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccesssToken);

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert 
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var deletedProjectRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{ProjectId}");
            deletedProjectRequest.Headers.Authorization = (new AuthenticationHeaderValue("Bearer", token.AccesssToken));

            var deletedProjectResponse = await _httpClient.SendAsync(deletedProjectRequest);
            deletedProjectResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenNonExistingProject_WhenDeleteProjectIsInvoked_ThenNotFoundIsReturned()
        {
            // Arrange
            var token = await AuthenticationHelper.RegisterUserAndAuthenticateAsAdminAsync(_httpClient);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/projects/{Guid.NewGuid()}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccesssToken);

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert 
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
