using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;

namespace _01_PrimoEsempio.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Rimuovi tutte le registrazioni relative a DbContext
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(CinemaDbContext) ||
                    d.ServiceType == typeof(DbContextOptions<CinemaDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType.Name.Contains("DbContext") ||
                    (d.ImplementationType != null && d.ImplementationType.Name.Contains("MySql")))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Registra InMemory database
            services.AddDbContext<CinemaDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });

        return base.CreateHost(builder);
    }
}
