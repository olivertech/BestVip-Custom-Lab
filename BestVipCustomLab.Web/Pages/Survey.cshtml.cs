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
    [TempData]
    public string? SuccessMessage { get; set; }

    [BindProperty]
    public SurveySubmissionRequest Input { get; set; } = new();

    public ActiveCampaignDto? Campaign { get; private set; }
    public bool HasSubmittedCurrentSurvey { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Campaign = campaignContextAccessor.ActiveCampaign;
        var visitorId = TryGetVisitorId();
        if (visitorId is null)
        {
            return RedirectToPage("/Account/Login", new { ReturnUrl = "/Survey" });
        }

        Input.VisitorId = visitorId.Value;
        await LoadParticipationAsync(visitorId.Value);
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
        await LoadParticipationAsync(visitorId.Value);

        if (HasSubmittedCurrentSurvey)
        {
            ModelState.AddModelError(string.Empty, "Você já respondeu esta pesquisa e não pode enviá-la novamente.");
            return Page();
        }

        try
        {
            await surveyService.SubmitSurveyAsync(Campaign.Slug, Input, HttpContext.RequestAborted);
            SuccessMessage = "Pesquisa enviada com sucesso. Suas respostas foram confirmadas e não poderão mais ser alteradas.";
            return RedirectToPage("/Survey");
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

    private async Task LoadParticipationAsync(Guid visitorId)
    {
        if (Campaign is null)
        {
            HasSubmittedCurrentSurvey = false;
            SubmittedAtUtc = null;
            return;
        }

        var participation = await surveyService.GetParticipationStatusAsync(Campaign.Id, visitorId, HttpContext.RequestAborted);
        HasSubmittedCurrentSurvey = participation.HasSubmitted;
        SubmittedAtUtc = participation.SubmittedAtUtc;
    }
}
