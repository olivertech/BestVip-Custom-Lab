using System.Security.Claims;
using BestVipCustomLab.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace BestVipCustomLab.Web.Pages.Admin;

[AllowAnonymous]
public sealed class LoginModel(IOptions<AdminAuthOptions> options) : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.Equals(Username, options.Value.Username, StringComparison.Ordinal) ||
            !string.Equals(Password, options.Value.Password, StringComparison.Ordinal))
        {
            ErrorMessage = "Credenciais invalidas.";
            return Page();
        }

        var claims = new[] { new Claim(ClaimTypes.Name, Username) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return RedirectToPage("/Admin/Index");
    }
}
