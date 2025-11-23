using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkConnect.Infrastructure.Data;

namespace WorkConnect.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o DbContext real (PostgreSQL)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<WorkConnectContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona DbContext InMemory para testes
                services.AddDbContext<WorkConnectContext>(options =>
                {
                    options.UseInMemoryDatabase("WorkConnectTestDb");
                });

                // Garante que o banco em memória está criado
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<WorkConnectContext>();
                db.Database.EnsureCreated();
            });
        }
    }
}
