using BestVipCustomLab.Application;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Admin.Products;

public sealed class EditModel(IProductService productService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid CampaignId { get; set; }

    [BindProperty]
    public ProductUpsertRequest Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Input.CampaignId = CampaignId;
        if (Id is null)
        {
            return;
        }

        var product = await productService.GetProductForEditAsync(Id.Value, HttpContext.RequestAborted);
        if (product is not null)
        {
            Input = product;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await productService.UpsertProductAsync(Input, HttpContext.RequestAborted);
            return RedirectToPage("/Admin/Products/Index", new { campaignId = Input.CampaignId });
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }
}
