using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.Endpoints;
using ProjectManagement.Endpoints.Extensions;

namespace ProjectManagement.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication AddVersionedEndpoints(this WebApplication app)
        {
            var versionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .HasApiVersion(new ApiVersion(2))
                .HasDeprecatedApiVersion(new ApiVersion(3))
                .ReportApiVersions()
                .Build();

            var routerBuilder = app.MapGroup("/api/v{version:apiVersion}")
                .WithApiVersionSet(versionSet);

            routerBuilder.RegisterProjectEndpoints();
            app.RegisterMinimalEndpoints(routerBuilder);

            return app;
        }

        public static WebApplication AddSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var versions = app.DescribeApiVersions();

                foreach (var version in versions)
                {
                    var url = $"/swagger/{version.GroupName}/swagger.json";
                    var name = version.GroupName.ToLowerInvariant();

                    options.SwaggerEndpoint(url, name);
                }
            });

            return app;
        }

        public static async Task AddRoles(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Administrator", "ProjectManager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
