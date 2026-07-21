using BestVipCustomLab.Application;

namespace BestVipCustomLab.Tests;

public sealed class VisitorRegistrationValidatorTests
{
    [Fact]
    public void Should_fail_when_privacy_policy_is_not_accepted()
    {
        var validator = new VisitorRegistrationRequestValidator();
        var request = CreateValidRequest();
        request.AcceptPrivacyPolicy = false;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(VisitorRegistrationRequest.AcceptPrivacyPolicy));
    }

    [Fact]
    public void Should_fail_when_age_and_birthdate_are_missing()
    {
        var validator = new VisitorRegistrationRequestValidator();
        var request = CreateValidRequest();
        request.BirthDate = null;
        request.AgeRange = null;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("faixa etaria", StringComparison.OrdinalIgnoreCase));
    }

    private static VisitorRegistrationRequest CreateValidRequest() => new()
    {
        FirstName = "Marcelo",
        LastName = "Oliveira",
        Email = "marcelo@example.com",
        WhatsApp = "5511999999999",
        City = "Sao Paulo",
        State = "SP",
        BirthDate = new DateOnly(1990, 5, 10),
        AcceptPrivacyPolicy = true,
        AcceptMarketing = true
    };
}
