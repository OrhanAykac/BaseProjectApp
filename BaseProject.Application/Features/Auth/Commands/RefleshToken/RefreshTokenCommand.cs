namespace BaseProject.Application.Features.Auth.Commands.RefleshToken;

public record RefreshTokenCommand(string RefreshToken)
    : IRequest<IDataResult<RefreshTokenResponse?>>;
