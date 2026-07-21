using BestVipCustomLab.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestVipCustomLab.Web.Pages.Admin.Products;

public sealed class IndexModel(IProductService productService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid CampaignId { get; set; }

    public IReadOnlyList<ProductAdminDto> Products { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Products = await productService.GetAdminProductsAsync(CampaignId, HttpContext.RequestAborted);
    }
}
