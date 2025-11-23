namespace BaseProject.Application.Features.Auth.Commands.RefleshToken;

public class RefleshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefleshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}