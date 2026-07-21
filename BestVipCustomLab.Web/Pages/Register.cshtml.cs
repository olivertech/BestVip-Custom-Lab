using BestVipCustomLab.Application;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BestVipCustomLab.Infrastructure.Persistence;

namespace BestVipCustomLab.Web.Pages;

public sealed class RegisterModel(
    IVisitorService visitorService,
    AppDbContext dbContext) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; } = "/Survey";

    [BindProperty]
    public VisitorRegistrationRequest Input { get; set; } = new();

    public IReadOnlyList<SelectOption> TrafficSources { get; private set; } = [];
    public string? SuccessMessage { get; private set; }

    public async Task OnGetAsync()
    {
        Input.ReturnUrl = ReturnUrl;
        await LoadSourcesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadSourcesAsync();
        Input.ReturnUrl = string.IsNullOrWhiteSpace(ReturnUrl) ? Input.ReturnUrl : ReturnUrl;

        try
        {
            var result = await visitorService.RegisterVisitorAsync(Input, HttpContext.RequestAborted);
            return Redirect(result.RedirectUrl);
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }

    private async Task LoadSourcesAsync()
    {
        TrafficSources = await dbContext.TrafficSources
            .OrderBy(x => x.Name)
            .Select(x => new SelectOption(x.Id, x.Name))
            .ToListAsync(HttpContext.RequestAborted);
    }

    public sealed record SelectOption(Guid Id, string Label);
}
