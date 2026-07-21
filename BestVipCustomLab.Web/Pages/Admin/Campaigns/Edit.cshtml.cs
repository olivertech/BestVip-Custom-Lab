using BestVipCustomLab.Application;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Admin.Campaigns;

public sealed class EditModel(ICampaignExperienceService campaignService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? Id { get; set; }

    [BindProperty]
    public CampaignUpsertRequest Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (Id is null)
        {
            return;
        }

        var campaign = await campaignService.GetCampaignForEditAsync(Id.Value, HttpContext.RequestAborted);
        if (campaign is not null)
        {
            Input = campaign.Campaign;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await campaignService.UpsertCampaignAsync(Input, HttpContext.RequestAborted);
            return RedirectToPage("/Admin/Campaigns/Index");
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }
}
