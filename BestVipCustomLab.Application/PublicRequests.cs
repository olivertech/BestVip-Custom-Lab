using BestVipCustomLab.Domain;

namespace BestVipCustomLab.Application;

public sealed class VisitorRegistrationRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string? AgeRange { get; set; }
    public string? Instagram { get; set; }
    public Guid? TrafficSourceId { get; set; }
    public bool AcceptPrivacyPolicy { get; set; }
    public bool AcceptMarketing { get; set; }
    public string ReturnUrl { get; set; } = "/Survey";
}

public sealed record RegistrationResultDto(Guid VisitorId, string RedirectUrl);

public sealed class SurveyAnswerInput
{
    public Guid QuestionId { get; set; }
    public string Value { get; set; } = string.Empty;
}

public sealed class SurveySubmissionRequest
{
    public Guid VisitorId { get; set; }
    public List<SurveyAnswerInput> Answers { get; set; } = [];
}

public sealed class VipInterestRequest
{
    public Guid CampaignId { get; set; }
    public Guid VisitorId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public sealed class ContactMessageRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public sealed class CampaignUpsertRequest
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string Subheadline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string HeroBannerLabel { get; set; } = string.Empty;
    public string AccentColor { get; set; } = "#c9a227";
    public string AccentSecondaryColor { get; set; } = "#8d6f12";
    public string SurfaceColor { get; set; } = "#121212";
    public string GradientStart { get; set; } = "#201507";
    public string GradientEnd { get; set; } = "#0d0d0d";
    public string TextColor { get; set; } = "#f4efe2";
    public string MutedTextColor { get; set; } = "#b8b0a2";
    public bool ShowPartialResults { get; set; }
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
    public bool IsActive { get; set; }
    public string PrimaryCta { get; set; } = "Quero participar";
    public string SecondaryCta { get; set; } = "Entrar na Lista VIP";
    public string StoryTitle { get; set; } = "Por que esta campanha existe";
    public string StoryBody { get; set; } = string.Empty;
}

public sealed class SurveyQuestionUpsertRequest
{
    public Guid? Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string HelpText { get; set; } = string.Empty;
    public SurveyQuestionType Type { get; set; } = SurveyQuestionType.SingleChoice;
    public bool IsRequired { get; set; } = true;
    public int DisplayOrder { get; set; }
    public string OptionsText { get; set; } = string.Empty;
}

public sealed class ProductUpsertRequest
{
    public Guid? Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PriceLabel { get; set; } = string.Empty;
    public string MockupUrl { get; set; } = string.Empty;
    public string CtaLabel { get; set; } = "Ver interesse";
    public int InterestScore { get; set; }
    public int DisplayOrder { get; set; }
}
