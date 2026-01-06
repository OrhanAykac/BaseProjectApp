namespace BaseProject.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<DataResult<LoginResponse?>>;
