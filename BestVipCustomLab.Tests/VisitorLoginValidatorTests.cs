using BestVipCustomLab.Application;

namespace BestVipCustomLab.Tests;

public sealed class VisitorLoginValidatorTests
{
    [Fact]
    public void Should_fail_when_email_is_invalid()
    {
        var validator = new VisitorLoginRequestValidator();
        var request = new VisitorLoginRequest
        {
            Email = "email-invalido",
            Password = "qualquer"
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(VisitorLoginRequest.Email));
    }

    [Fact]
    public void Should_fail_when_password_is_missing()
    {
        var validator = new VisitorLoginRequestValidator();
        var request = new VisitorLoginRequest
        {
            Email = "usuario@bestvip.com",
            Password = string.Empty
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(VisitorLoginRequest.Password));
    }
}
