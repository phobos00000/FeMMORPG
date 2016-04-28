using FeMMORPG.Common;

namespace FeMMORPG.LoginServer.Models
{
    /// <summary>
    /// Login Response model
    /// </summary>
    public class LoginResultModel
    {
        /// <summary>
        /// True if login was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error if login failed
        /// </summary>
        public ErrorCodes ErrorType { get; set; }

        /// <summary>
        /// Connection info for the game server
        /// </summary>
        public string ServerUrl { get; set; }
    }
}