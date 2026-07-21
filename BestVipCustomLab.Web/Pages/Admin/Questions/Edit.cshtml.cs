using BestVipCustomLab.Application;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Admin.Questions;

public sealed class EditModel(ISurveyService surveyService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid CampaignId { get; set; }

    [BindProperty]
    public SurveyQuestionUpsertRequest Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Input.CampaignId = CampaignId;

        if (Id is null)
        {
            return;
        }

        var question = await surveyService.GetQuestionForEditAsync(Id.Value, HttpContext.RequestAborted);
        if (question is not null)
        {
            Input = question;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await surveyService.UpsertQuestionAsync(Input, HttpContext.RequestAborted);
            return RedirectToPage("/Admin/Questions/Index", new { campaignId = Input.CampaignId });
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }
}
