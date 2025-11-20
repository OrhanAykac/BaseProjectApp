namespace BaseProject.Application.Common.Abstract;

public interface ITokenService
{
    string GenerateJwtToken(Guid userId, string email);
    string GenerateRefreshToken();

    string GenerateSecretKey(int length = 32);
}