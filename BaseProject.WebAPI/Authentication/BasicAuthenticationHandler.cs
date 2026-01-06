using BaseProject.Application.Constants;
using BaseProject.Application.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace BaseProject.WebAPI.Authentication;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IAppDbContextRO appDbContextRO) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var value))
            return AuthenticateResult.NoResult();

        string authHeader = value.ToString();

        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        try
        {
            string encodedCredentials = authHeader["Basic ".Length..].Trim();
            string decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));

            string[] credentials = decodedCredentials.Split(':', 2);

            if (credentials.Length != 2)
                return AuthenticateResult.Fail(Messages.Auth.InvalidBasicAuthenticationFormat);

            string userIdStr = credentials[0];
            string apiSecret = credentials[1];

            if (!Guid.TryParse(userIdStr, out Guid userId))
                return AuthenticateResult.Fail(Messages.Auth.InvalidApiKey);

            var userData = await appDbContextRO.Users
                .Where(b => b.UserId == userId)
                .Select(b => new { b.UserId, b.Email, b.IsActive, b.ApiSecret, b.TenantId })
                .FirstOrDefaultAsync();

            if (userData is null)
                return AuthenticateResult.Fail(Messages.Auth.InvalidCredentials);

            if (!userData.IsActive)
                return AuthenticateResult.Fail(Messages.User.UserInactive);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userData.UserId.ToString()),
                new Claim(ClaimTypes.Email, userData.Email),
                new Claim("TenantId", userData.TenantId.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, Messages.Log.ErrorDuringBasicAuthentication);
            return AuthenticateResult.Fail(Messages.Auth.AuthenticationError);
        }
    }
}
