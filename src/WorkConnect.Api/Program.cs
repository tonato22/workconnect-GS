using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkConnect.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// DbContext + PostgreSQL
builder.Services.AddDbContext<WorkConnectContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly("WorkConnect.Infrastructure");
        });
});

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HealthChecks (usa o DbContext)
builder.Services.AddHealthChecks()
    .AddDbContextCheck<WorkConnectContext>("Database");

// HTTP Logging
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestPath
                           | HttpLoggingFields.RequestMethod
                           | HttpLoggingFields.ResponseStatusCode;
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

var app = builder.Build();

// Aplica migrations automaticamente na inicialização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkConnectContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Necessário para WebApplicationFactory nos testes
public partial class Program { }
