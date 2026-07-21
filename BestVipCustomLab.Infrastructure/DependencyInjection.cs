using BestVipCustomLab.Application;
using BestVipCustomLab.Infrastructure.Persistence;
using BestVipCustomLab.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BestVipCustomLab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        var inMemoryDatabaseName = configuration["DatabaseName"] ?? "bestvip-custom-lab";

        services.AddDbContext<AppDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase(inMemoryDatabaseName);
            }
        });

        services.AddScoped<ICampaignExperienceService, CampaignExperienceService>();
        services.AddScoped<IVisitorService, VisitorService>();
        services.AddScoped<ISurveyService, SurveyService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IInteractionService, InteractionService>();

        services.AddScoped<IValidator<VisitorRegistrationRequest>, VisitorRegistrationRequestValidator>();
        services.AddScoped<IValidator<SurveySubmissionRequest>, SurveySubmissionRequestValidator>();
        services.AddScoped<IValidator<CampaignUpsertRequest>, CampaignUpsertRequestValidator>();
        services.AddScoped<IValidator<SurveyQuestionUpsertRequest>, SurveyQuestionUpsertRequestValidator>();
        services.AddScoped<IValidator<ProductUpsertRequest>, ProductUpsertRequestValidator>();

        return services;
    }

    public static async Task InitialiseDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DatabaseSeeder.SeedAsync(dbContext, cancellationToken);
    }

    public static LoggerConfiguration AddBestVipSerilog(this LoggerConfiguration configuration)
    {
        return configuration
            .Enrich.FromLogContext()
            .WriteTo.Console();
    }
}
