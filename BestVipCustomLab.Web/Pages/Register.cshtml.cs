using System.Security.Claims;
using BestVipCustomLab.Application;
using BestVipCustomLab.Infrastructure.Persistence;
using BestVipCustomLab.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(string.IsNullOrWhiteSpace(ReturnUrl) ? "/Survey" : ReturnUrl);
        }

        Input.ReturnUrl = ReturnUrl;
        await LoadSourcesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadSourcesAsync();
        Input.ReturnUrl = string.IsNullOrWhiteSpace(ReturnUrl) ? Input.ReturnUrl : ReturnUrl;

        try
        {
            var result = await visitorService.RegisterVisitorAsync(Input, HttpContext.RequestAborted);
            await SignInAsync(result);
            return LocalRedirect(result.RedirectUrl);
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

    private async Task SignInAsync(RegistrationResultDto result)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, result.VisitorId.ToString()),
            new Claim(ClaimTypes.Email, result.Email),
            new Claim(ClaimTypes.Name, $"{Input.FirstName} {Input.LastName}".Trim())
        };

        var identity = new ClaimsIdentity(claims, AuthSchemes.UserScheme);
        await HttpContext.SignInAsync(AuthSchemes.UserScheme, new ClaimsPrincipal(identity));
    }

    public sealed record SelectOption(Guid Id, string Label);
}
