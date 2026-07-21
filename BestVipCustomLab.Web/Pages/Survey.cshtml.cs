using System.Security.Claims;
using BestVipCustomLab.Application;
using BestVipCustomLab.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages;

[Authorize(Policy = AuthSchemes.UserPolicy)]
public sealed class SurveyModel(
    CampaignContextAccessor campaignContextAccessor,
    ISurveyService surveyService) : PageModel
{
    [BindProperty]
    public SurveySubmissionRequest Input { get; set; } = new();

    public ActiveCampaignDto? Campaign { get; private set; }
    public string? SuccessMessage { get; private set; }

    public IActionResult OnGet()
    {
        Campaign = campaignContextAccessor.ActiveCampaign;
        var visitorId = TryGetVisitorId();
        if (visitorId is null)
        {
            return RedirectToPage("/Account/Login", new { ReturnUrl = "/Survey" });
        }

        Input.VisitorId = visitorId.Value;
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
            return RedirectToPage("/Account/Login", new { ReturnUrl = "/Survey" });
        }

        Input.VisitorId = visitorId.Value;

        try
        {
            await surveyService.SubmitSurveyAsync(Campaign.Slug, Input, HttpContext.RequestAborted);
            SuccessMessage = "Pesquisa enviada com sucesso. Obrigado por ajudar a decidir os próximos produtos.";
            ModelState.Clear();
            Input = new SurveySubmissionRequest { VisitorId = visitorId.Value };
            return Page();
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }

    private Guid? TryGetVisitorId()
    {
        var rawValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(rawValue, out var visitorId) ? visitorId : null;
    }
}
