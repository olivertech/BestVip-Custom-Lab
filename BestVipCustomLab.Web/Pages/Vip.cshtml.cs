using BestVipCustomLab.Application;
using BestVipCustomLab.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages;

public sealed class VipModel(
    CampaignContextAccessor campaignContextAccessor,
    IVisitorService visitorService) : PageModel
{
    [BindProperty]
    public VipInterestRequest Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public Guid VisitorId { get; set; }

    public ActiveCampaignDto? Campaign { get; private set; }
    public string? SuccessMessage { get; private set; }

    public void OnGet()
    {
        Campaign = campaignContextAccessor.ActiveCampaign;
        if (Campaign is not null)
        {
            Input.CampaignId = Campaign.Id;
            Input.VisitorId = VisitorId;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Campaign = campaignContextAccessor.ActiveCampaign;
        if (Campaign is null)
        {
            return Page();
        }

        try
        {
            await visitorService.JoinVipListAsync(Input, HttpContext.RequestAborted);
            SuccessMessage = "Seu interesse foi registrado para esta campanha.";
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }

        return Page();
    }
}
