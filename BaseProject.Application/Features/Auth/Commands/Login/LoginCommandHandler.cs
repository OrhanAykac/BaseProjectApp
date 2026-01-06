using BaseProject.Application.Common.Hashing;
using BaseProject.Application.Constants;

namespace BaseProject.Application.Features.Auth.Commands.Login;

internal sealed class LoginCommandHandler(
    IAppDbContext appDbContext,
    IConfiguration configuration,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, DataResult<LoginResponse?>>
{
    public async ValueTask<DataResult<LoginResponse?>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        HashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = await appDbContext.Users
              .Where(u => u.Email == request.Email)
              .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return Result.Fail<LoginResponse>(Messages.Auth.InvalidEmailOrPassword);

        bool verified = HashingHelper.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);


        if (verified)
        {
            await appDbContext.SaveChangesAsync(cancellationToken);

            return GenerateLoginResponse(user);
        }

        return Result.Fail<LoginResponse>(Messages.Auth.InvalidEmailOrPassword);
    }

    private DataResult<LoginResponse?> GenerateLoginResponse(Domain.Concrete.User user)
    {
        string token = tokenService.GenerateJwtToken(user.UserId, user.Email, user.TenantId, []);

        int refleshTokenExpirationDays = int.Parse(configuration["JwtSettings:RefreshTokenExpireDays"]!);
        int tokenExpirationMinutes = int.Parse(configuration["JwtSettings:ExpireMinutes"]!);

        DateTime tokenExpire = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes);

        user.RefreshTokenExpire = DateTime.UtcNow.AddDays(refleshTokenExpirationDays);
        user.RefreshToken = tokenService.GenerateRefreshToken();

        return Result.Success(new LoginResponse(
            token, tokenExpire, user.RefreshToken, user.RefreshTokenExpire.Value));
    }
}