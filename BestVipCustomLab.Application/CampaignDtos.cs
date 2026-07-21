using BestVipCustomLab.Domain;

namespace BestVipCustomLab.Application;

public sealed record CampaignThemeDto(
    string AccentColor,
    string AccentSecondaryColor,
    string SurfaceColor,
    string GradientStart,
    string GradientEnd,
    string TextColor,
    string MutedTextColor);

public sealed record CampaignAssetDto(Guid Id, string AssetType, string Label, string Url);

public sealed record CampaignContentDto(string Key, string Value);

public sealed record ProductCardDto(
    Guid Id,
    string Name,
    string Description,
    string PriceLabel,
    string MockupUrl,
    string CtaLabel,
    int InterestScore);

public sealed record SurveyOptionDto(Guid Id, string Label, string Value);

public sealed record SurveyQuestionDto(
    Guid Id,
    string Prompt,
    string HelpText,
    SurveyQuestionType Type,
    bool IsRequired,
    IReadOnlyList<SurveyOptionDto> Options);

public sealed record PartialResultItemDto(string Question, string TopAnswer, int Votes);

public sealed record ActiveCampaignDto(
    Guid Id,
    string Name,
    string Slug,
    string Headline,
    string Subheadline,
    string Description,
    string HeroBannerLabel,
    bool ShowPartialResults,
    CampaignThemeDto Theme,
    IReadOnlyList<CampaignAssetDto> Assets,
    IReadOnlyList<CampaignContentDto> Contents,
    IReadOnlyList<ProductCardDto> Products,
    IReadOnlyList<SurveyQuestionDto> Questions,
    IReadOnlyList<PartialResultItemDto> PartialResults);

public sealed record CampaignListItemDto(Guid Id, string Name, string Slug, CampaignStatus Status, bool IsActive);

public sealed record DashboardSummaryDto(
    int TotalVisitors,
    int NewVisitorsLast7Days,
    int SurveyResponses,
    int VipSubscriptions,
    decimal MarketingOptInRate,
    string ActiveCampaignName);

public sealed record GeographicStatDto(string State, int Count);

public sealed record TrafficSourceStatDto(string Source, int Count);

public sealed record CampaignOverviewDto(
    Guid Id,
    string Name,
    int VisitorCount,
    int SurveyResponseCount,
    int ProductCount,
    int QuestionCount,
    int VipCount,
    bool IsActive);
