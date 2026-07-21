using BestVipCustomLab.Application;
using BestVipCustomLab.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages;

public sealed class AboutModel(CampaignContextAccessor campaignContextAccessor) : PageModel
{
    public ActiveCampaignDto? Campaign { get; private set; }

    public void OnGet()
    {
        Campaign = campaignContextAccessor.ActiveCampaign;
    }
}
