using BestVipCustomLab.Application;

namespace BestVipCustomLab.Tests;

public sealed class SurveySubmissionValidatorTests
{
    [Fact]
    public void Should_fail_when_answers_are_missing()
    {
        var validator = new SurveySubmissionRequestValidator();
        var request = new SurveySubmissionRequest
        {
            VisitorId = Guid.NewGuid()
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SurveySubmissionRequest.Answers));
    }

    [Fact]
    public void Should_pass_with_valid_answers()
    {
        var validator = new SurveySubmissionRequestValidator();
        var request = new SurveySubmissionRequest
        {
            VisitorId = Guid.NewGuid(),
            Answers =
            [
                new SurveyAnswerInput
                {
                    QuestionId = Guid.NewGuid(),
                    Value = "Naruto"
                }
            ]
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
