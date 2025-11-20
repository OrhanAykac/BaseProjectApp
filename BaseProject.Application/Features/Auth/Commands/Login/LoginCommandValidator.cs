using FluentValidation;

namespace BaseProject.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Length(5, 100)
            .EmailAddress();
        RuleFor(x => x.Password)
            .Length(8, 50)
            .NotEmpty();
    }
}
