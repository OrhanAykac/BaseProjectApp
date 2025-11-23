namespace BaseProject.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<IDataResult<LoginResponse?>>;
