using BestVipCustomLab.Application;
using BestVipCustomLab.Domain;
using BestVipCustomLab.Infrastructure.Persistence;
using Dapper;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BestVipCustomLab.Infrastructure.Services;

internal sealed class CampaignExperienceService(
    AppDbContext dbContext,
    IValidator<CampaignUpsertRequest> campaignValidator) : ICampaignExperienceService
{
    public async Task<ActiveCampaignDto?> GetActiveCampaignAsync(CancellationToken cancellationToken = default)
    {
        var campaign = await dbContext.Campaigns
            .Include(x => x.Theme)
            .Include(x => x.Assets)
            .Include(x => x.Contents)
            .Include(x => x.Products)
            .Include(x => x.SurveyQuestions)
                .ThenInclude(x => x.Options)
            .FirstOrDefaultAsync(x => x.IsActive, cancellationToken);

        return campaign is null ? null : await ToActiveCampaignDtoAsync(campaign, cancellationToken);
    }

    public async Task<IReadOnlyList<CampaignListItemDto>> GetCampaignsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Campaigns
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new CampaignListItemDto(x.Id, x.Name, x.Slug, x.Status, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<CampaignAdminEditDto?> GetCampaignForEditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await dbContext.Campaigns
            .Include(x => x.Theme)
            .Include(x => x.Contents)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (campaign is null)
        {
            return null;
        }

        string GetContent(string key) => campaign.Contents.FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;

        return new CampaignAdminEditDto
        {
            Id = campaign.Id,
            Campaign = new CampaignUpsertRequest
            {
                Id = campaign.Id,
                Name = campaign.Name,
                Slug = campaign.Slug,
                Headline = campaign.Headline,
                Subheadline = campaign.Subheadline,
                Description = campaign.Description,
                HeroBannerLabel = campaign.HeroBannerLabel,
                AccentColor = campaign.Theme.AccentColor,
                AccentSecondaryColor = campaign.Theme.AccentSecondaryColor,
                SurfaceColor = campaign.Theme.SurfaceColor,
                GradientStart = campaign.Theme.GradientStart,
                GradientEnd = campaign.Theme.GradientEnd,
                TextColor = campaign.Theme.TextColor,
                MutedTextColor = campaign.Theme.MutedTextColor,
                ShowPartialResults = campaign.ShowPartialResults,
                Status = campaign.Status,
                IsActive = campaign.IsActive,
                PrimaryCta = GetContent("primary-cta"),
                SecondaryCta = GetContent("secondary-cta"),
                StoryTitle = GetContent("story-title"),
                StoryBody = GetContent("story-body")
            }
        };
    }

    public async Task<CampaignOverviewDto?> GetCampaignOverviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await dbContext.Campaigns
            .Include(x => x.Products)
            .Include(x => x.SurveyQuestions)
            .Include(x => x.VipInterests)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (campaign is null)
        {
            return null;
        }

        var responseCount = await dbContext.SurveyResponses.CountAsync(x => x.CampaignId == id, cancellationToken);

        return new CampaignOverviewDto(
            campaign.Id,
            campaign.Name,
            await dbContext.Visitors.CountAsync(cancellationToken),
            responseCount,
            campaign.Products.Count,
            campaign.SurveyQuestions.Count,
            campaign.VipInterests.Count,
            campaign.IsActive);
    }

    public async Task<Guid> UpsertCampaignAsync(CampaignUpsertRequest request, CancellationToken cancellationToken = default)
    {
        await campaignValidator.ValidateAndThrowAsync(request, cancellationToken);

        Campaign campaign;
        if (request.Id is { } campaignId)
        {
            campaign = await dbContext.Campaigns
                .Include(x => x.Theme)
                .Include(x => x.Contents)
                .FirstAsync(x => x.Id == campaignId, cancellationToken);
        }
        else
        {
            campaign = new Campaign();
            dbContext.Campaigns.Add(campaign);
        }

        campaign.Name = request.Name;
        campaign.Slug = request.Slug.Trim().ToLowerInvariant();
        campaign.Headline = request.Headline;
        campaign.Subheadline = request.Subheadline;
        campaign.Description = request.Description;
        campaign.HeroBannerLabel = request.HeroBannerLabel;
        campaign.ShowPartialResults = request.ShowPartialResults;
        campaign.Status = request.Status;
        campaign.IsActive = request.IsActive;
        campaign.Theme ??= new CampaignTheme();
        campaign.Theme.AccentColor = request.AccentColor;
        campaign.Theme.AccentSecondaryColor = request.AccentSecondaryColor;
        campaign.Theme.SurfaceColor = request.SurfaceColor;
        campaign.Theme.GradientStart = request.GradientStart;
        campaign.Theme.GradientEnd = request.GradientEnd;
        campaign.Theme.TextColor = request.TextColor;
        campaign.Theme.MutedTextColor = request.MutedTextColor;
        campaign.Theme.CampaignId = campaign.Id;

        UpsertContent(campaign, "primary-cta", request.PrimaryCta);
        UpsertContent(campaign, "secondary-cta", request.SecondaryCta);
        UpsertContent(campaign, "story-title", request.StoryTitle);
        UpsertContent(campaign, "story-body", request.StoryBody);

        await dbContext.SaveChangesAsync(cancellationToken);

        if (request.IsActive)
        {
            await SetActiveCampaignAsync(campaign.Id, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return campaign.Id;
    }

    public async Task SetActiveCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var campaigns = await dbContext.Campaigns.ToListAsync(cancellationToken);
        foreach (var campaign in campaigns)
        {
            campaign.IsActive = campaign.Id == campaignId;
            campaign.Status = campaign.IsActive ? CampaignStatus.Active : campaign.Status == CampaignStatus.Active ? CampaignStatus.Scheduled : campaign.Status;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void UpsertContent(Campaign campaign, string key, string value)
    {
        var content = campaign.Contents.FirstOrDefault(x => x.Key == key);
        if (content is null)
        {
            campaign.Contents.Add(new CampaignContent
            {
                CampaignId = campaign.Id,
                Key = key,
                Value = value
            });
            return;
        }

        content.Value = value;
    }

    private async Task<ActiveCampaignDto> ToActiveCampaignDtoAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        List<PartialResultItemDto> partialResults = [];

        if (campaign.ShowPartialResults)
        {
            var responses = await dbContext.SurveyResponses
                .Include(x => x.Answers)
                .Where(x => x.CampaignId == campaign.Id)
                .ToListAsync(cancellationToken);

            partialResults = campaign.SurveyQuestions
                .Select(question =>
                {
                    var topAnswer = responses
                        .SelectMany(x => x.Answers)
                        .Where(x => x.SurveyQuestionId == question.Id)
                        .GroupBy(x => x.Value)
                        .OrderByDescending(x => x.Count())
                        .Select(x => new PartialResultItemDto(question.Prompt, x.Key, x.Count()))
                        .FirstOrDefault();

                    return topAnswer;
                })
                .Where(x => x is not null)
                .Cast<PartialResultItemDto>()
                .ToList();
        }

        return new ActiveCampaignDto(
            campaign.Id,
            campaign.Name,
            campaign.Slug,
            campaign.Headline,
            campaign.Subheadline,
            campaign.Description,
            campaign.HeroBannerLabel,
            campaign.ShowPartialResults,
            new CampaignThemeDto(
                campaign.Theme.AccentColor,
                campaign.Theme.AccentSecondaryColor,
                campaign.Theme.SurfaceColor,
                campaign.Theme.GradientStart,
                campaign.Theme.GradientEnd,
                campaign.Theme.TextColor,
                campaign.Theme.MutedTextColor),
            campaign.Assets.OrderBy(x => x.AssetType).Select(x => new CampaignAssetDto(x.Id, x.AssetType, x.Label, x.Url)).ToList(),
            campaign.Contents.Select(x => new CampaignContentDto(x.Key, x.Value)).ToList(),
            campaign.Products.OrderBy(x => x.DisplayOrder)
                .Select(x => new ProductCardDto(x.Id, x.Name, x.Description, x.PriceLabel, x.MockupUrl, x.CtaLabel, x.InterestScore))
                .ToList(),
            campaign.SurveyQuestions.OrderBy(x => x.DisplayOrder)
                .Select(x => new SurveyQuestionDto(
                    x.Id,
                    x.Prompt,
                    x.HelpText,
                    x.Type,
                    x.IsRequired,
                    x.Options.OrderBy(o => o.DisplayOrder).Select(o => new SurveyOptionDto(o.Id, o.Label, o.Value)).ToList()))
                .ToList(),
            partialResults);
    }
}

internal sealed class VisitorService(
    AppDbContext dbContext,
    IValidator<VisitorRegistrationRequest> validator) : IVisitorService
{
    public async Task<RegistrationResultDto> RegisterVisitorAsync(VisitorRegistrationRequest request, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var email = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Visitors.AnyAsync(x => x.Email == email, cancellationToken))
        {
            throw new ValidationException("Ja existe um cadastro com este email.");
        }

        var visitor = request.Adapt<Visitor>();
        visitor.Email = email;
        visitor.AllowMarketing = request.AcceptMarketing;

        visitor.ConsentRecords.Add(new ConsentRecord
        {
            PolicyVersion = "2026-07-20",
            AcceptedPrivacy = request.AcceptPrivacyPolicy,
            AcceptedMarketing = request.AcceptMarketing,
            Source = "landing-register"
        });

        dbContext.Visitors.Add(visitor);
        await dbContext.SaveChangesAsync(cancellationToken);

        var redirectUrl = $"{request.ReturnUrl}?visitorId={visitor.Id}";
        return new RegistrationResultDto(visitor.Id, redirectUrl);
    }

    public async Task JoinVipListAsync(VipInterestRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.VipInterests.AnyAsync(
            x => x.CampaignId == request.CampaignId && x.VisitorId == request.VisitorId,
            cancellationToken);

        if (exists)
        {
            return;
        }

        dbContext.VipInterests.Add(new VipInterest
        {
            CampaignId = request.CampaignId,
            VisitorId = request.VisitorId,
            Notes = request.Notes
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

internal sealed class SurveyService(
    AppDbContext dbContext,
    IValidator<SurveySubmissionRequest> submissionValidator,
    IValidator<SurveyQuestionUpsertRequest> questionValidator) : ISurveyService
{
    public async Task<IReadOnlyList<SurveyQuestionAdminDto>> GetAdminQuestionsAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return await dbContext.SurveyQuestions
            .Include(x => x.Options)
            .Where(x => x.CampaignId == campaignId)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new SurveyQuestionAdminDto(
                x.Id,
                x.CampaignId,
                x.Prompt,
                x.Type,
                x.IsRequired,
                x.DisplayOrder,
                string.Join(Environment.NewLine, x.Options.OrderBy(o => o.DisplayOrder).Select(o => o.Label))))
            .ToListAsync(cancellationToken);
    }

    public async Task<SurveyQuestionUpsertRequest?> GetQuestionForEditAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        var question = await dbContext.SurveyQuestions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);

        return question is null
            ? null
            : new SurveyQuestionUpsertRequest
            {
                Id = question.Id,
                CampaignId = question.CampaignId,
                Prompt = question.Prompt,
                HelpText = question.HelpText,
                Type = question.Type,
                IsRequired = question.IsRequired,
                DisplayOrder = question.DisplayOrder,
                OptionsText = string.Join(Environment.NewLine, question.Options.OrderBy(x => x.DisplayOrder).Select(x => x.Label))
            };
    }

    public async Task<Guid> UpsertQuestionAsync(SurveyQuestionUpsertRequest request, CancellationToken cancellationToken = default)
    {
        await questionValidator.ValidateAndThrowAsync(request, cancellationToken);

        SurveyQuestion question;
        if (request.Id is { } id)
        {
            question = await dbContext.SurveyQuestions.Include(x => x.Options).FirstAsync(x => x.Id == id, cancellationToken);
            dbContext.SurveyOptions.RemoveRange(question.Options);
            question.Options.Clear();
        }
        else
        {
            question = new SurveyQuestion();
            dbContext.SurveyQuestions.Add(question);
        }

        question.CampaignId = request.CampaignId;
        question.Prompt = request.Prompt;
        question.HelpText = request.HelpText;
        question.Type = request.Type;
        question.IsRequired = request.IsRequired;
        question.DisplayOrder = request.DisplayOrder;

        var options = request.OptionsText.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (var index = 0; index < options.Length; index++)
        {
            question.Options.Add(new SurveyOption
            {
                Label = options[index],
                Value = options[index].ToLowerInvariant().Replace(' ', '-'),
                DisplayOrder = index + 1
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return question.Id;
    }

    public async Task SubmitSurveyAsync(string campaignSlug, SurveySubmissionRequest request, CancellationToken cancellationToken = default)
    {
        await submissionValidator.ValidateAndThrowAsync(request, cancellationToken);

        var campaign = await dbContext.Campaigns
            .Include(x => x.SurveyQuestions)
            .ThenInclude(x => x.Options)
            .FirstAsync(x => x.Slug == campaignSlug, cancellationToken);

        var visitorExists = await dbContext.Visitors.AnyAsync(x => x.Id == request.VisitorId, cancellationToken);
        if (!visitorExists)
        {
            throw new ValidationException("Visitante nao encontrado.");
        }

        var response = new SurveyResponse
        {
            CampaignId = campaign.Id,
            VisitorId = request.VisitorId
        };

        foreach (var answer in request.Answers)
        {
            response.Answers.Add(new SurveyAnswer
            {
                SurveyQuestionId = answer.QuestionId,
                Value = answer.Value.Trim()
            });
        }

        dbContext.SurveyResponses.Add(response);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

internal sealed class ProductService(
    AppDbContext dbContext,
    IValidator<ProductUpsertRequest> validator) : IProductService
{
    public async Task<IReadOnlyList<ProductAdminDto>> GetAdminProductsAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .Where(x => x.CampaignId == campaignId)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new ProductAdminDto(x.Id, x.CampaignId, x.Name, x.PriceLabel, x.InterestScore, x.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductUpsertRequest?> GetProductForEditAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);
        return product?.Adapt<ProductUpsertRequest>();
    }

    public async Task<Guid> UpsertProductAsync(ProductUpsertRequest request, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        Product product;
        if (request.Id is { } id)
        {
            product = await dbContext.Products.FirstAsync(x => x.Id == id, cancellationToken);
        }
        else
        {
            product = new Product();
            dbContext.Products.Add(product);
        }

        product.CampaignId = request.CampaignId;
        product.Name = request.Name;
        product.Description = request.Description;
        product.PriceLabel = request.PriceLabel;
        product.MockupUrl = request.MockupUrl;
        product.CtaLabel = request.CtaLabel;
        product.InterestScore = request.InterestScore;
        product.DisplayOrder = request.DisplayOrder;

        await dbContext.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}

internal sealed class DashboardService(AppDbContext dbContext) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var activeCampaignName = await dbContext.Campaigns.Where(x => x.IsActive).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken) ?? "Nenhuma";

        if (dbContext.Database.IsRelational())
        {
            await using var connection = dbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            const string sql = """
                select
                    (select count(*) from "Visitors") as TotalVisitors,
                    (select count(*) from "Visitors" where "CreatedAtUtc" >= @Cutoff) as NewVisitorsLast7Days,
                    (select count(*) from "SurveyResponses") as SurveyResponses,
                    (select count(*) from "VipInterests") as VipSubscriptions,
                    (select coalesce(avg(case when "AllowMarketing" then 1.0 else 0.0 end), 0) from "Visitors") as MarketingOptInRate
                """;

            var result = await connection.QuerySingleAsync(sql, new { Cutoff = DateTimeOffset.UtcNow.AddDays(-7) });
            return new DashboardSummaryDto(
                (int)result.totalvisitors,
                (int)result.newvisitorslast7days,
                (int)result.surveyresponses,
                (int)result.vipsubscriptions,
                Math.Round((decimal)result.marketingoptinrate * 100, 2),
                activeCampaignName);
        }

        var totalVisitors = await dbContext.Visitors.CountAsync(cancellationToken);
        var newVisitors = await dbContext.Visitors.CountAsync(x => x.CreatedAtUtc >= DateTimeOffset.UtcNow.AddDays(-7), cancellationToken);
        var surveyResponses = await dbContext.SurveyResponses.CountAsync(cancellationToken);
        var vipSubscriptions = await dbContext.VipInterests.CountAsync(cancellationToken);
        var marketingRate = totalVisitors == 0
            ? 0
            : Math.Round((decimal)await dbContext.Visitors.CountAsync(x => x.AllowMarketing, cancellationToken) / totalVisitors * 100, 2);

        return new DashboardSummaryDto(totalVisitors, newVisitors, surveyResponses, vipSubscriptions, marketingRate, activeCampaignName);
    }

    public async Task<IReadOnlyList<GeographicStatDto>> GetGeographicBreakdownAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Visitors
            .GroupBy(x => x.State)
            .Select(group => new GeographicStatDto(group.Key, group.Count()))
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrafficSourceStatDto>> GetTrafficSourceBreakdownAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Visitors
            .GroupJoin(dbContext.TrafficSources, visitor => visitor.TrafficSourceId, source => source.Id, (visitor, sources) => new { visitor, source = sources.FirstOrDefault() })
            .GroupBy(x => x.source != null ? x.source.Name : "Nao informado")
            .Select(group => new TrafficSourceStatDto(group.Key, group.Count()))
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);
    }
}

internal sealed class InteractionService(AppDbContext dbContext) : IInteractionService
{
    public async Task SubmitContactMessageAsync(ContactMessageRequest request, CancellationToken cancellationToken = default)
    {
        dbContext.Interactions.Add(new Interaction
        {
            Type = InteractionType.Contact,
            Name = request.Name,
            Email = request.Email,
            Message = request.Message
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
