using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectManagement.Swagger
{
    public class SwaggerGetOptionsConfiguration(IApiVersionDescriptionProvider provider)
        : IConfigureNamedOptions<SwaggerGenOptions>
    {
        public void Configure(string? name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                var version = description.ApiVersion.ToString();

                options.SwaggerDoc($"v{version}", new OpenApiInfo
                {
                    Title = $"Project Management API v{version}",
                    Version = $"v{version}",
                    Description = "API for managing projects and related subtasks",
                    Contact = new OpenApiContact
                    {
                        Name = "Code Maze",
                        Email = "info@code-maze.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });
            }
        }
    }
}
