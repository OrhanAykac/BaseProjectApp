namespace BaseProject.Application.Common.Abstract;

public interface ITokenService
{
    string GenerateJwtToken(Guid userId, string email, Guid tenantId, string[] roles);
    string GenerateRefreshToken();

    string GenerateSecretKey(int length = 32);
}