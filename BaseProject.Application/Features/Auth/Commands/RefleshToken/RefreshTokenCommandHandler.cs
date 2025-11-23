namespace BaseProject.Application.Features.Auth.Commands.RefleshToken;

internal sealed class RefreshTokenCommandHandler(
    ITokenService tokenService,
    IConfiguration configuration,
    IAppDbContext appDbContext)
    : IRequestHandler<RefreshTokenCommand, IDataResult<RefreshTokenResponse?>>
{
    public async ValueTask<IDataResult<RefreshTokenResponse?>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await appDbContext.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user?.RefreshTokenExpire is null || user.RefreshTokenExpire < DateTime.UtcNow)
            return Result.Fail<RefreshTokenResponse?>("Invalid token or refresh token expired.");

        if (!user.IsActive)
            return Result.Fail<RefreshTokenResponse?>("User is not active");

        return await GenerateRefreshTokenResponse(user, cancellationToken);
    }

    private async Task<IDataResult<RefreshTokenResponse?>> GenerateRefreshTokenResponse(
        Domain.Concrete.User user, CancellationToken c)
    {
        var token = tokenService.GenerateJwtToken(user.UserId, user.Email, user.TenantId, []);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        int refleshTokenExpirationDays = int.Parse(configuration["JwtSettings:RefreshTokenExpireDays"]!);
        int tokenExpirationMinutes = int.Parse(configuration["JwtSettings:ExpireMinutes"]!);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpire = DateTime.UtcNow.AddDays(refleshTokenExpirationDays);

        var response = new RefreshTokenResponse(
            Token: token,
            TokenExpiration: DateTime.UtcNow.AddMinutes(tokenExpirationMinutes),
            RefleshToken: user.RefreshToken,
            RefleshTokenExpiration: user.RefreshTokenExpire);

        await appDbContext.SaveChangesAsync(c);
        return Result.Success(response);
    }
}