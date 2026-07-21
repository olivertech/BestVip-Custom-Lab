using BestVipCustomLab.Application;
using BestVipCustomLab.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BestVipCustomLab.Tests;

public sealed class CampaignExperienceServiceTests
{
    [Fact]
    public async Task Should_seed_geek_campaign_as_active()
    {
        await using var provider = await CreateProviderAsync();
        var service = provider.GetRequiredService<ICampaignExperienceService>();

        var activeCampaign = await service.GetActiveCampaignAsync();

        Assert.NotNull(activeCampaign);
        Assert.Equal("Geek & Anime", activeCampaign!.Name);
        Assert.Equal("geek-anime", activeCampaign.Slug);
        Assert.NotEmpty(activeCampaign.Products);
        Assert.NotEmpty(activeCampaign.Questions);
    }

    [Fact]
    public async Task Should_switch_active_campaign()
    {
        await using var provider = await CreateProviderAsync();
        var service = provider.GetRequiredService<ICampaignExperienceService>();
        var campaigns = await service.GetCampaignsAsync();
        var fathersDay = campaigns.Single(x => x.Slug == "dia-dos-pais");

        await service.SetActiveCampaignAsync(fathersDay.Id);
        var activeCampaign = await service.GetActiveCampaignAsync();

        Assert.NotNull(activeCampaign);
        Assert.Equal("dia-dos-pais", activeCampaign!.Slug);
    }

    private static async Task<ServiceProvider> CreateProviderAsync()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DatabaseName"] = $"test-db-{Guid.NewGuid():N}"
            })
            .Build();

        services.AddInfrastructure(configuration);
        var provider = services.BuildServiceProvider();
        await provider.InitialiseDatabaseAsync();
        return provider;
    }
}
