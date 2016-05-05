namespace FeMMORPG.Common
{
    public enum ErrorCodes
    {
        None = 0,
        GameVersionTooLow,
        MaxConnectionsExceeded,
        InvalidLogin,
        InvalidRegistration,
        UserAlreadyExists,
        UserAlreadyLoggedIn,
        UserIdleTimeout,
        NoGameServersAvailable,
    }
}
