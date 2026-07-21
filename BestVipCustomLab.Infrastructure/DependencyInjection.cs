using BestVipCustomLab.Application;
using BestVipCustomLab.Infrastructure.Persistence;
using BestVipCustomLab.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Data;
using System.Reflection;

namespace BestVipCustomLab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? configuration.GetConnectionString("DefaultConnection");
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
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<IVisitorService, VisitorService>();
        services.AddScoped<ISurveyService, SurveyService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IInteractionService, InteractionService>();

        services.AddScoped<IValidator<VisitorRegistrationRequest>, VisitorRegistrationRequestValidator>();
        services.AddScoped<IValidator<VisitorLoginRequest>, VisitorLoginRequestValidator>();
        services.AddScoped<IValidator<AdminLoginRequest>, AdminLoginRequestValidator>();
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

        if (dbContext.Database.IsRelational())
        {
            await BaselineExistingSchemaAsync(dbContext, cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        await DatabaseSeeder.SeedAsync(dbContext, cancellationToken);
    }

    public static LoggerConfiguration AddBestVipSerilog(this LoggerConfiguration configuration)
    {
        return configuration
            .Enrich.FromLogContext()
            .WriteTo.Console();
    }

    private static async Task BaselineExistingSchemaAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var migrationsAssembly = dbContext.GetService<IMigrationsAssembly>();
        var allMigrations = migrationsAssembly.Migrations.Keys.OrderBy(x => x).ToList();
        if (allMigrations.Count == 0)
        {
            return;
        }

        var connection = dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;
        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var userTablesCommand = connection.CreateCommand();
            userTablesCommand.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name <> '__EFMigrationsHistory'
                );
                """;

            var hasUserTables = Convert.ToBoolean(await userTablesCommand.ExecuteScalarAsync(cancellationToken));
            if (!hasUserTables)
            {
                return;
            }

            var appliedMigrations = new List<string>();

            await using var historyTableCommand = connection.CreateCommand();
            historyTableCommand.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = '__EFMigrationsHistory'
                );
                """;

            var hasHistoryTable = Convert.ToBoolean(await historyTableCommand.ExecuteScalarAsync(cancellationToken));
            if (hasHistoryTable)
            {
                await using var appliedMigrationsCommand = connection.CreateCommand();
                appliedMigrationsCommand.CommandText = """
                    SELECT "MigrationId"
                    FROM "__EFMigrationsHistory"
                    ORDER BY "MigrationId";
                    """;

                await using var reader = await appliedMigrationsCommand.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    appliedMigrations.Add(reader.GetString(0));
                }
            }

            if (appliedMigrations.Count > 0)
            {
                return;
            }

            var historyRepository = dbContext.GetService<IHistoryRepository>();
            var productVersion = typeof(DbContext).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                ?.Split('+')[0] ?? "10.0.10";

            await dbContext.Database.ExecuteSqlRawAsync(historyRepository.GetCreateIfNotExistsScript(), cancellationToken);

            foreach (var migrationId in allMigrations)
            {
                var insertScript = historyRepository.GetInsertScript(new HistoryRow(migrationId, productVersion));
                await dbContext.Database.ExecuteSqlRawAsync(insertScript, cancellationToken);
            }
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }
}
