namespace BaseProject.Application.Constants;

/// <summary>
/// Uygulama genelinde kullanılan sabit mesajları içerir.
/// </summary>
public static class Messages
{
    /// <summary>
    /// Genel hata mesajları
    /// </summary>
    public static class Error
    {
        public const string UnexpectedError = "İşlenemeyen bir hata oluştu.";
        public const string AnErrorOccurred = "An error occurred";
    }

    /// <summary>
    /// Kimlik doğrulama ile ilgili mesajlar
    /// </summary>
    public static class Auth
    {
        public const string InvalidEmailOrPassword = "Email or password is invalid.";
        public const string InvalidTokenOrRefreshTokenExpired = "Invalid token or refresh token expired.";
        public const string InvalidBasicAuthenticationFormat = "Invalid Basic authentication format";
        public const string InvalidApiKey = "Invalid ApiKey";
        public const string InvalidCredentials = "Invalid credentials";
        public const string AuthenticationError = "Authentication error";
    }

    /// <summary>
    /// Kullanıcı ile ilgili mesajlar
    /// </summary>
    public static class User
    {
        public const string UserNotActive = "User is not active";
        public const string UserInactive = "User is inactive.";
    }

    /// <summary>
    /// Log mesajları (Serilog structured logging için)
    /// </summary>
    public static class Log
    {
        public const string ErrorHandlingMessage = "Error handling {MessageType}";
        public const string UnhandledExceptionOccurred = "Unhandled exception occurred";
        public const string ErrorDuringBasicAuthentication = "Error during basic authentication";
    }
}

