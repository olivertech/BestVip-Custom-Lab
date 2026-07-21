namespace BestVipCustomLab.Domain;

public sealed class Visitor
{
    public Guid Id { get; set; } = Guid.NewGuid();
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
    public bool AllowMarketing { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<ConsentRecord> ConsentRecords { get; set; } = [];
}

public sealed class TrafficSource
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public sealed class ConsentRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VisitorId { get; set; }
    public string PolicyVersion { get; set; } = string.Empty;
    public bool AcceptedPrivacy { get; set; }
    public bool AcceptedMarketing { get; set; }
    public DateTimeOffset AcceptedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string Source { get; set; } = string.Empty;
}

public sealed class VipInterest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public Guid VisitorId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class Interaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? CampaignId { get; set; }
    public Guid? VisitorId { get; set; }
    public InteractionType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
