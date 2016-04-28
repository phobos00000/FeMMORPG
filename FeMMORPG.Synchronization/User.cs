namespace FeMMORPG.Synchronization
{
    using System;

    public class User : IEntity
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public DateTime LastLogin { get; set; }
    }
}