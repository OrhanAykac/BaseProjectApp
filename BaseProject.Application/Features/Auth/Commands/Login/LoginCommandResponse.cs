namespace BaseProject.Application.Features.Auth.Commands.Login;

public record LoginCommandResponse(bool Success, string Message, string? Token);
