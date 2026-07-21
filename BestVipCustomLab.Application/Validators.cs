using FluentValidation;

namespace BestVipCustomLab.Application;

public sealed class VisitorRegistrationRequestValidator : AbstractValidator<VisitorRegistrationRequest>
{
    public VisitorRegistrationRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(160);
        RuleFor(x => x.WhatsApp).NotEmpty().MaximumLength(30);
        RuleFor(x => x.City).NotEmpty().MaximumLength(80);
        RuleFor(x => x.State).NotEmpty().MaximumLength(40);
        RuleFor(x => x.AcceptPrivacyPolicy).Equal(true);
        RuleFor(x => x).Must(x => x.BirthDate is not null || !string.IsNullOrWhiteSpace(x.AgeRange))
            .WithMessage("Informe data de nascimento ou faixa etaria.");
    }
}

public sealed class SurveySubmissionRequestValidator : AbstractValidator<SurveySubmissionRequest>
{
    public SurveySubmissionRequestValidator()
    {
        RuleFor(x => x.VisitorId).NotEmpty();
        RuleFor(x => x.Answers).NotEmpty();
        RuleForEach(x => x.Answers).ChildRules(answer =>
        {
            answer.RuleFor(x => x.QuestionId).NotEmpty();
            answer.RuleFor(x => x.Value).NotEmpty().MaximumLength(500);
        });
    }
}

public sealed class CampaignUpsertRequestValidator : AbstractValidator<CampaignUpsertRequest>
{
    public CampaignUpsertRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Headline).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Subheadline).NotEmpty().MaximumLength(260);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(800);
        RuleFor(x => x.HeroBannerLabel).NotEmpty().MaximumLength(120);
    }
}

public sealed class SurveyQuestionUpsertRequestValidator : AbstractValidator<SurveyQuestionUpsertRequest>
{
    public SurveyQuestionUpsertRequestValidator()
    {
        RuleFor(x => x.CampaignId).NotEmpty();
        RuleFor(x => x.Prompt).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

public sealed class ProductUpsertRequestValidator : AbstractValidator<ProductUpsertRequest>
{
    public ProductUpsertRequestValidator()
    {
        RuleFor(x => x.CampaignId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.PriceLabel).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(400);
    }
}
