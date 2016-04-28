using FeMMORPG.Common;

namespace FeMMORPG.LoginServer.Models
{
    /// <summary>
    /// Registration response model
    /// </summary>
    public class RegisterResultModel
    {
        /// <summary>
        /// True if registration was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error if registration failed
        /// </summary>
        public ErrorCodes ErrorType { get; set; }
    }
}