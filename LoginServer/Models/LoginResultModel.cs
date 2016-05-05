using System;
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
        /// Login token for game server
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Connection info for the game server
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// Time login was completed
        /// </summary>
        public DateTime LoginTime { get; set; }
    }
}