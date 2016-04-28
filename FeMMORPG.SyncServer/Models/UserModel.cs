namespace FeMMORPG.SyncServer.Models
{
    using System;

    /// <summary>
    /// User viewmodel
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// The user's unique identifier, i.e., their username
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The user's encrypted password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The last time the user logged into the service
        /// </summary>
        public DateTime LastLogin { get; set; }
    }
}