using BestVipCustomLab.Application;
using BestVipCustomLab.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages;

public sealed class SurveyModel(
    CampaignContextAccessor campaignContextAccessor,
    ISurveyService surveyService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid VisitorId { get; set; }

    [BindProperty]
    public SurveySubmissionRequest Input { get; set; } = new();

    public ActiveCampaignDto? Campaign { get; private set; }
    public string? SuccessMessage { get; private set; }

    public void OnGet()
    {
        Campaign = campaignContextAccessor.ActiveCampaign;
        Input.VisitorId = VisitorId;
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
            await surveyService.SubmitSurveyAsync(Campaign.Slug, Input, HttpContext.RequestAborted);
            SuccessMessage = "Pesquisa enviada com sucesso. Obrigado por ajudar a decidir os proximos produtos.";
            ModelState.Clear();
            Input = new SurveySubmissionRequest { VisitorId = Input.VisitorId };
            return Page();
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }
}
