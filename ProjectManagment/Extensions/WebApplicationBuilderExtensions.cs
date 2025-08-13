using Serilog;
using Asp.Versioning;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

using Infrastructure;
using Contracts.Projects;
using ProjectManagement.Policies;
using ProjectManagement.Swagger;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

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

        public static WebApplicationBuilder ConfigureRateLimiting(this WebApplicationBuilder builder)
        {
            builder.Services.AddRateLimiter(limiterOptions =>
            {
                limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                limiterOptions.AddFixedWindowLimiter("fixed-window", options =>
                {
                    options.Window = TimeSpan.FromSeconds(10);
                    options.PermitLimit = 10;
                    options.QueueLimit = 1;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                limiterOptions.AddSlidingWindowLimiter("sliding-window", options =>
                {
                    options.Window = TimeSpan.FromSeconds(10);
                    options.SegmentsPerWindow = 2;
                    options.PermitLimit = 10;
                    options.QueueLimit = 1;
                });

                limiterOptions.AddConcurrencyLimiter("concurrency", options =>
                {
                    options.PermitLimit = 10;
                    options.QueueLimit = 1;
                });

                limiterOptions.AddTokenBucketLimiter("token-bucket", options =>
                {
                    options.TokenLimit = 2;
                    options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                    options.TokensPerPeriod = 1;
                    options.QueueLimit = 1;
                });
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureMicrosoftIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return builder;
        }
    }
}
