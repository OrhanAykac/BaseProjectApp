namespace BaseProject.Application.Features.Auth.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
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
