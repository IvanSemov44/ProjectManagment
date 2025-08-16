using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ProjectManagement.Tests.Integration
{
    public class ProjectManagementApplicationfactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private const string Database = "master";
        private const string Username = "sa";
        private const string Password = "yourStrong(!)Password";
        private const ushort MsSqlPort = 1433;

        private readonly IContainer _mssqlContainer = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPortBinding(MsSqlPort, true)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("SQLCMDUUSER", Username)
            .WithEnvironment("SQLCMDPASSWORD", Password)
            .WithEnvironment("MSSQL_SA_PASSWORD", Password)
            .WithWaitStrategy(
                Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlPort))
            .Build();

        public async Task InitializeAsync()
        {
            await _mssqlContainer.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _mssqlContainer.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var host = _mssqlContainer.Hostname;
            var port = _mssqlContainer.GetMappedPublicPort(MsSqlPort);

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    $"Server={host},{port};Database={Database};User Id={Username};Password={Password};TrustServerCertificate=True")
                );

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    appContext.Database.Migrate();
                }
                catch
                {
                    throw;
                }
            });
        }
    }
}
