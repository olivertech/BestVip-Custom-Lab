namespace BestVipCustomLab.Domain;

public sealed class SurveyQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string HelpText { get; set; } = string.Empty;
    public SurveyQuestionType Type { get; set; }
    public bool IsRequired { get; set; } = true;
    public int DisplayOrder { get; set; }
    public List<SurveyOption> Options { get; set; } = [];
}

public sealed class SurveyOption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SurveyQuestionId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public sealed class SurveyResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public Guid VisitorId { get; set; }
    public DateTimeOffset SubmittedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<SurveyAnswer> Answers { get; set; } = [];
}

public sealed class SurveyAnswer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SurveyResponseId { get; set; }
    public Guid SurveyQuestionId { get; set; }
    public string Value { get; set; } = string.Empty;
}
