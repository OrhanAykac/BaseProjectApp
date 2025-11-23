namespace BaseProject.Application.Features.Auth.Commands.RefleshToken;

public record RefreshTokenResponse(
    string Token,
    DateTime TokenExpiration,
    string RefleshToken,
    DateTime? RefleshTokenExpiration);