using System.Security.Claims;
using BestVipCustomLab.Application;
using BestVipCustomLab.Infrastructure;
using BestVipCustomLab.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, _, configuration) => configuration.AddBestVipSerilog());

builder.Services.Configure<AdminAuthOptions>(builder.Configuration.GetSection(AdminAuthOptions.SectionName));
builder.Services.AddScoped<CampaignContextAccessor>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin");
    options.Conventions.AllowAnonymousToPage("/Admin/Login");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

await app.Services.InitialiseDatabaseAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<CampaignContextMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/api/campaigns/active", async (ICampaignExperienceService service, CancellationToken cancellationToken) =>
{
    var campaign = await service.GetActiveCampaignAsync(cancellationToken);
    return campaign is null ? Results.NotFound() : Results.Ok(campaign);
});

app.MapPost("/api/visitors/register", async (
    VisitorRegistrationRequest request,
    IVisitorService service,
    CancellationToken cancellationToken) =>
{
    var result = await service.RegisterVisitorAsync(request, cancellationToken);
    return Results.Ok(result);
});

app.MapPost("/api/surveys/{campaignSlug}/responses", async (
    string campaignSlug,
    SurveySubmissionRequest request,
    ISurveyService service,
    CancellationToken cancellationToken) =>
{
    await service.SubmitSurveyAsync(campaignSlug, request, cancellationToken);
    return Results.Ok();
});

app.MapGet("/api/admin/dashboard/summary", async (
    HttpContext httpContext,
    IDashboardService service,
    CancellationToken cancellationToken) =>
{
    if (!httpContext.User.Identity?.IsAuthenticated ?? true)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(await service.GetSummaryAsync(cancellationToken));
});

app.MapGet("/api/admin/campaigns/{id:guid}/overview", async (
    Guid id,
    HttpContext httpContext,
    ICampaignExperienceService service,
    CancellationToken cancellationToken) =>
{
    if (!httpContext.User.Identity?.IsAuthenticated ?? true)
    {
        return Results.Unauthorized();
    }

    var result = await service.GetCampaignOverviewAsync(id, cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.Run();

public partial class Program;
