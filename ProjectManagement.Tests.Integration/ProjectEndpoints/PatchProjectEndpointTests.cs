using Contracts.Projects;
using Contracts.Tokens;
using Microsoft.AspNetCore.JsonPatch;
using ProjectManagement.Tests.Integration.Common;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ProjectManagement.Tests.Integration.ProjectEndpoints
{
    public class PatchProjectEndpointTests(ProjectManagementApplicationfactory applicationfactory)
        : IClassFixture<ProjectManagementApplicationfactory>
    {
        private const string NewName = "New Project Name";
        private const string ProjectId = "c9d4c053-49b6-410c-bc78-2d54a9991870";
        private readonly HttpClient _httpClient = applicationfactory.CreateClient();

        private static HttpRequestMessage CreatePatchProjectName(
            TokenResponse? token, string projectId, string name)
        {
            var patchDocument = new JsonPatchDocument<UpdateProjectRequest>();
            patchDocument.Replace(p => p.Name, name);
            var json = JsonSerializer.Serialize(patchDocument.Operations);

            var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/projects/{projectId}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccesssToken);

            return request;
        }

        [Fact]
        public async Task GivenValidPatchRequest_WhenPatchProjectIsInvoked_ThenNoContentIsReturned()
        {
            // Arrange
            var token = await AuthenticationHelper.RegisterUserAndAuthenticateAsPmAsync(_httpClient);
            var request = CreatePatchProjectName(token, ProjectId, NewName);

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

            var updatedProjectRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{ProjectId}");
            updatedProjectRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccesssToken);
            var updatedProjectResponse = await _httpClient.SendAsync(updatedProjectRequest);
            var updatedProject = await updatedProjectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

            updatedProjectResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            updatedProject.ShouldNotBeNull();
            updatedProject!.Name.ShouldBe(NewName);
        }
    }
}
