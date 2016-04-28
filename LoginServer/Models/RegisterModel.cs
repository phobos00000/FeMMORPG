﻿namespace FeMMORPG.LoginServer.Models
{
    /// <summary>
    /// Registration request model
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}