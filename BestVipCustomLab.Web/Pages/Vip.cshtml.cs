using System.Security.Claims;
using BestVipCustomLab.Application;
using BestVipCustomLab.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages;

[Authorize(Policy = AuthSchemes.UserPolicy)]
public sealed class VipModel(
    CampaignContextAccessor campaignContextAccessor,
    IVisitorService visitorService) : PageModel
{
    [BindProperty]
    public VipInterestRequest Input { get; set; } = new();

    public ActiveCampaignDto? Campaign { get; private set; }
    public string? SuccessMessage { get; private set; }

    public IActionResult OnGet()
    {
        Campaign = campaignContextAccessor.ActiveCampaign;
        var visitorId = TryGetVisitorId();
        if (visitorId is null)
        {
            return RedirectToPage("/Account/Login", new { ReturnUrl = "/Vip" });
        }

        if (Campaign is not null)
        {
            Input.CampaignId = Campaign.Id;
            Input.VisitorId = visitorId.Value;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Campaign = campaignContextAccessor.ActiveCampaign;
        if (Campaign is null)
        {
            return Page();
        }

        var visitorId = TryGetVisitorId();
        if (visitorId is null)
        {
            return RedirectToPage("/Account/Login", new { ReturnUrl = "/Vip" });
        }

        Input.VisitorId = visitorId.Value;
        Input.CampaignId = Campaign.Id;

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

    private Guid? TryGetVisitorId()
    {
        var rawValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(rawValue, out var visitorId) ? visitorId : null;
    }
}
