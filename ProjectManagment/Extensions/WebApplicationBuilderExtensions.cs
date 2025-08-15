using Serilog;
using Asp.Versioning;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using System.Text;
using System.Threading.RateLimiting;

using Domain;
using Infrastructure;
using Application.Options;
using Contracts.Projects;
using ProjectManagement.Policies;
using ProjectManagement.Swagger;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.Xml;

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

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Name = "JWT Authentication",
                        Description = "Enter your JWT token here",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id=JwtBearerDefaults.AuthenticationScheme,
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            []
                        }
                    });
            });

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
                options.Password.RequiredLength = 4;
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

        public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddOptions<JwtOptions>()
                .BindConfiguration(nameof(JwtOptions))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            using var serviceProvide = builder.Services.BuildServiceProvider();
            var jwtOptions = serviceProvide.GetRequiredService<IOptions<JwtOptions>>().Value;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
                    };
                });

            return builder;
        }

        public static WebApplicationBuilder ConfigureAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("RequireAdministrator", policy =>
                {
                    policy.RequireRole("Administrator");
                });

            return builder;
        }
    }
}
