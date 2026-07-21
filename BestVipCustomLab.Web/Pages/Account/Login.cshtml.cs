using System.Security.Claims;
using BestVipCustomLab.Application;
using BestVipCustomLab.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Account;

[AllowAnonymous]
public sealed class LoginModel(IVisitorService visitorService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; } = "/Survey";

    [BindProperty]
    public VisitorLoginRequest Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated == true &&
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var visitorId))
        {
            var currentLogin = await visitorService.GetVisitorLoginAsync(visitorId, ReturnUrl, HttpContext.RequestAborted);
            if (currentLogin is not null)
            {
                return LocalRedirect(currentLogin.RedirectUrl);
            }
        }

        Input.ReturnUrl = ReturnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.ReturnUrl = string.IsNullOrWhiteSpace(ReturnUrl) ? Input.ReturnUrl : ReturnUrl;

        try
        {
            var result = await visitorService.AuthenticateAsync(Input, HttpContext.RequestAborted);
            await SignInAsync(result);
            return LocalRedirect(result.RedirectUrl);
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }

    private async Task SignInAsync(VisitorLoginResultDto result)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, result.VisitorId.ToString()),
            new Claim(ClaimTypes.Email, result.Email),
            new Claim(ClaimTypes.Name, result.FullName)
        };

        var identity = new ClaimsIdentity(claims, AuthSchemes.UserScheme);
        await HttpContext.SignInAsync(AuthSchemes.UserScheme, new ClaimsPrincipal(identity));
    }
}
