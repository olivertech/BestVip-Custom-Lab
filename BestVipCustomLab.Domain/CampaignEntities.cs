namespace BestVipCustomLab.Domain;

public sealed class Campaign
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string Subheadline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string HeroBannerLabel { get; set; } = string.Empty;
    public CampaignStatus Status { get; set; }
    public DateTimeOffset? StartsAtUtc { get; set; }
    public DateTimeOffset? EndsAtUtc { get; set; }
    public int DisplayOrder { get; set; }
    public bool ShowPartialResults { get; set; }
    public bool IsActive { get; set; }
    public CampaignTheme Theme { get; set; } = new();
    public List<CampaignAsset> Assets { get; set; } = [];
    public List<CampaignContent> Contents { get; set; } = [];
    public List<Product> Products { get; set; } = [];
    public List<SurveyQuestion> SurveyQuestions { get; set; } = [];
    public List<VipInterest> VipInterests { get; set; } = [];
}

public sealed class CampaignTheme
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public string AccentColor { get; set; } = "#c9a227";
    public string AccentSecondaryColor { get; set; } = "#8d6f12";
    public string SurfaceColor { get; set; } = "#121212";
    public string GradientStart { get; set; } = "#201507";
    public string GradientEnd { get; set; } = "#0d0d0d";
    public string TextColor { get; set; } = "#f4efe2";
    public string MutedTextColor { get; set; } = "#b8b0a2";
}

public sealed class CampaignAsset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public sealed class CampaignContent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public sealed class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public Guid? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PriceLabel { get; set; } = string.Empty;
    public string MockupUrl { get; set; } = string.Empty;
    public string CtaLabel { get; set; } = string.Empty;
    public int InterestScore { get; set; }
    public int DisplayOrder { get; set; }
}
