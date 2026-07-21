using BestVipCustomLab.Application;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Admin;

public sealed class IndexModel(
    IDashboardService dashboardService,
    ICampaignExperienceService campaignService) : PageModel
{
    public DashboardSummaryDto Summary { get; private set; } = new(0, 0, 0, 0, 0, "Nenhuma");
    public IReadOnlyList<GeographicStatDto> Geography { get; private set; } = [];
    public IReadOnlyList<TrafficSourceStatDto> Sources { get; private set; } = [];
    public IReadOnlyList<CampaignListItemDto> Campaigns { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Summary = await dashboardService.GetSummaryAsync(HttpContext.RequestAborted);
        Geography = await dashboardService.GetGeographicBreakdownAsync(HttpContext.RequestAborted);
        Sources = await dashboardService.GetTrafficSourceBreakdownAsync(HttpContext.RequestAborted);
        Campaigns = await campaignService.GetCampaignsAsync(HttpContext.RequestAborted);
    }
}
