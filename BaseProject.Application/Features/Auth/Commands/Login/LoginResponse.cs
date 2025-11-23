namespace BaseProject.Application.Features.Auth.Commands.Login;

public record LoginResponse(
    string? Token,
    DateTime TokenExpire,
    string RefleshToken,
    DateTime RefleshTokenExpire);
