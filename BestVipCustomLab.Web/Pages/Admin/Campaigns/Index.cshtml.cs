using BestVipCustomLab.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Admin.Campaigns;

public sealed class IndexModel(ICampaignExperienceService campaignService) : PageModel
{
    public IReadOnlyList<CampaignListItemDto> Campaigns { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Campaigns = await campaignService.GetCampaignsAsync(HttpContext.RequestAborted);
    }

    public async Task<IActionResult> OnPostActivateAsync(Guid id)
    {
        await campaignService.SetActiveCampaignAsync(id, HttpContext.RequestAborted);
        return RedirectToPage();
    }
}
