using BestVipCustomLab.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Admin.Questions;

public sealed class IndexModel(ISurveyService surveyService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid CampaignId { get; set; }

    public IReadOnlyList<SurveyQuestionAdminDto> Questions { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Questions = await surveyService.GetAdminQuestionsAsync(CampaignId, HttpContext.RequestAborted);
    }
}
