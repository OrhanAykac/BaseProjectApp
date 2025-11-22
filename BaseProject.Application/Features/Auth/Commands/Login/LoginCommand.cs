using BaseProject.Application.Common.Abstract;
using BaseProject.Application.Data;
using BaseProject.Application.Utilities.Hashing;
using BaseProject.Application.Utilities.Results;

namespace BaseProject.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<IDataResult<LoginCommandResponse?>>;


internal sealed class LoginCommandHandler(IAppDbContext appDbContext, ITokenService tokenService)
    : IRequestHandler<LoginCommand, IDataResult<LoginCommandResponse?>>
{
    public async ValueTask<IDataResult<LoginCommandResponse?>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        HashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = await appDbContext.Users
              .Where(u => u.Email == request.Email)
              .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return Result.Fail<LoginCommandResponse>("Email or password is invalid.");

        bool verified = HashingHelper.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);


        if (verified)
        {
            string token = tokenService.GenerateJwtToken(user.UserId, user.Email);

            return Result.Success(new LoginCommandResponse(true, "Login successful.", token));
        }

        return Result.Fail<LoginCommandResponse>("Email or password is invalid.");

    }
}