namespace BestVipCustomLab.Application;

public interface ICampaignExperienceService
{
    Task<ActiveCampaignDto?> GetActiveCampaignAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CampaignListItemDto>> GetCampaignsAsync(CancellationToken cancellationToken = default);
    Task<CampaignAdminEditDto?> GetCampaignForEditAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CampaignOverviewDto?> GetCampaignOverviewAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> UpsertCampaignAsync(CampaignUpsertRequest request, CancellationToken cancellationToken = default);
    Task SetActiveCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default);
}

public interface IVisitorService
{
    Task<RegistrationResultDto> RegisterVisitorAsync(VisitorRegistrationRequest request, CancellationToken cancellationToken = default);
    Task JoinVipListAsync(VipInterestRequest request, CancellationToken cancellationToken = default);
}

public interface ISurveyService
{
    Task<IReadOnlyList<SurveyQuestionAdminDto>> GetAdminQuestionsAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<SurveyQuestionUpsertRequest?> GetQuestionForEditAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<Guid> UpsertQuestionAsync(SurveyQuestionUpsertRequest request, CancellationToken cancellationToken = default);
    Task SubmitSurveyAsync(string campaignSlug, SurveySubmissionRequest request, CancellationToken cancellationToken = default);
}

public interface IProductService
{
    Task<IReadOnlyList<ProductAdminDto>> GetAdminProductsAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<ProductUpsertRequest?> GetProductForEditAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Guid> UpsertProductAsync(ProductUpsertRequest request, CancellationToken cancellationToken = default);
}

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeographicStatDto>> GetGeographicBreakdownAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrafficSourceStatDto>> GetTrafficSourceBreakdownAsync(CancellationToken cancellationToken = default);
}

public interface IInteractionService
{
    Task SubmitContactMessageAsync(ContactMessageRequest request, CancellationToken cancellationToken = default);
}
