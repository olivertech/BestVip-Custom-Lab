using System.Security.Claims;
using BestVipCustomLab.Application;
using BestVipCustomLab.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Admin;

[AllowAnonymous]
public sealed class LoginModel(IAdminAuthService adminAuthService) : PageModel
{
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var result = await adminAuthService.AuthenticateAsync(new AdminLoginRequest
            {
                Email = Email,
                Password = Password
            }, HttpContext.RequestAborted);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, result.AdminUserId.ToString()),
                new Claim(ClaimTypes.Name, result.Name),
                new Claim(ClaimTypes.Email, result.Email)
            };

            var identity = new ClaimsIdentity(claims, AuthSchemes.AdminScheme);
            await HttpContext.SignInAsync(AuthSchemes.AdminScheme, new ClaimsPrincipal(identity));
            return RedirectToPage("/Admin/Index");
        }
        catch (ValidationException exception)
        {
            ErrorMessage = exception.Message;
            return Page();
        }
    }
}
