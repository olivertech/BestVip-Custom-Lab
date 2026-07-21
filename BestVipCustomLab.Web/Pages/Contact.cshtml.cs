using BestVipCustomLab.Application;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages;

public sealed class ContactModel(IInteractionService interactionService) : PageModel
{
    [BindProperty]
    public ContactMessageRequest Input { get; set; } = new();

    public string? SuccessMessage { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await interactionService.SubmitContactMessageAsync(Input, HttpContext.RequestAborted);
            SuccessMessage = "Mensagem enviada. A equipe pode usar esse contato para futuras validacoes.";
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }

        return Page();
    }
}
