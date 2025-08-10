using Asp.Versioning;
using Contracts.Projects;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Policies;
using ProjectManagement.Swagger;
using Serilog;

namespace ProjectManagement.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnectionString");


            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    connectionString,
                    x => x.MigrationsAssembly("ProjectManagement")));

            return builder;
        }

        public static WebApplicationBuilder ConfigureApiVersioning(this WebApplicationBuilder builder)
        {
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureSwagger(this WebApplicationBuilder builder)
        {

            builder.Services.ConfigureOptions<SwaggerGetOptionsConfiguration>();

            builder.Services.AddSwaggerGen();

            return builder;
        }
        public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((hostContext, configuration) =>
            {
                configuration.ReadFrom.Configuration(hostContext.Configuration);
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
        {

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureOutputCaching(this WebApplicationBuilder builder)
        {
            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(policy =>
                {
                    policy.Expire(TimeSpan.FromMinutes(3));
                });

                options.AddPolicy("FiveMinutes", policy =>
                {
                    policy.Tag(ProjectConstants.GetAllProjects);
                    policy.Expire(TimeSpan.FromMinutes(5));
                });

                options.AddPolicy("Id", policy =>
                {
                    policy.AddPolicy<VaryByIndentifierCachePolicy>();
                    policy.Expire(TimeSpan.FromMinutes(5));
                });
            });

            return builder;
        }
    }
}
