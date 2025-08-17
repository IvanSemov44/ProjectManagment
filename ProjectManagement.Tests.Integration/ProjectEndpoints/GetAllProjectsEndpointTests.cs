using Domain.Models;
using ProjectManagement.Tests.Integration.Common;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProjectManagement.Tests.Integration.ProjectEndpoints
{
    public class GetAllProjectsEndpointTests(ProjectManagementApplicationfactory applicationfactory)
        :IClassFixture<ProjectManagementApplicationfactory>
    {
        private readonly HttpClient _httpClient=applicationfactory.CreateClient();

        [Fact]
        public async Task GivenUnauthenticatedUser_WhenGetAllProjectsIsInvoked_TheUnauthorizenIsReturn()
        {
            // Act
            var response = await _httpClient.GetAsync("/api/v1/projects?page=1&pageSize=1&properties=name");
        
            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GivenAuthenticatedUder_WhenGetAllProjectsIsInvoked_ThenOkIsReturned()
        {
            // Arrange
            var token = await AuthenticationHelper.RegisterUserAndAuthenticateAsPmAsync(_httpClient);
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/v1/projects?page=1&pageSize=1&properties=name");
            request.Headers.Authorization= new AuthenticationHeaderValue("Bearer",token.AccesssToken);

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var pagedList = await response.Content.ReadFromJsonAsync<PagedList<ShapedEntity>>();
            pagedList.ShouldNotBeNull();
            pagedList.Items.ShouldNotBeEmpty();
            pagedList.Items.Count.ShouldBe(1);
            pagedList.HasHextPage.ShouldBeTrue();
            pagedList.TotalPages.ShouldBe(2);
            pagedList.Items.First().Properties.ShouldNotBeEmpty();
            pagedList.Items.First().Properties.Count.ShouldBe(1);
        }
    }
}
