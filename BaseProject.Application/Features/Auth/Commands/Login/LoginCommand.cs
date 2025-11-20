using BaseProject.Application.Common.Abstract;
using BaseProject.Application.Data;
using BaseProject.Application.Utilities.Hashing;

namespace BaseProject.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginCommandResponse?>;


internal sealed class LoginCommandHandler(IAppDbContext appDbContext, ITokenService tokenService) : IRequestHandler<LoginCommand, LoginCommandResponse?>
{
    public async ValueTask<LoginCommandResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        HashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = await appDbContext.Users
              .Where(u => u.Email == request.Email)
              .FirstOrDefaultAsync(cancellationToken);


        //Results.Fail<LoginCommandResponse>("Invalid email or password.");
        if (user is null)
            return null;

        bool verified = HashingHelper.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);


        if (verified)
        {
            string token = tokenService.GenerateJwtToken(user.UserId, user.Email);

            return new LoginCommandResponse(true, "Login successful.", token);
        }

        return null;

    }
}