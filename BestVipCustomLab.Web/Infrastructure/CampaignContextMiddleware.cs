using BestVipCustomLab.Application;

namespace BestVipCustomLab.Web.Infrastructure;

public sealed class CampaignContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, CampaignContextAccessor accessor, ICampaignExperienceService service)
    {
        if (!context.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase) &&
            !context.Request.Path.StartsWithSegments("/api/admin", StringComparison.OrdinalIgnoreCase))
        {
            accessor.ActiveCampaign = await service.GetActiveCampaignAsync(context.RequestAborted);
        }

        await next(context);
    }
}
