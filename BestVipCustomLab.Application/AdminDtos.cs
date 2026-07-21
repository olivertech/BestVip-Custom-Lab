using BestVipCustomLab.Domain;

namespace BestVipCustomLab.Application;

public sealed class CampaignAdminEditDto
{
    public Guid Id { get; init; }
    public CampaignUpsertRequest Campaign { get; init; } = new();
}

public sealed record SurveyQuestionAdminDto(
    Guid Id,
    Guid CampaignId,
    string Prompt,
    SurveyQuestionType Type,
    bool IsRequired,
    int DisplayOrder,
    string OptionsText);

public sealed record ProductAdminDto(
    Guid Id,
    Guid CampaignId,
    string Name,
    string PriceLabel,
    int InterestScore,
    int DisplayOrder);
