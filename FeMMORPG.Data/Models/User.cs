using System;
using System.Collections.Generic;

namespace FeMMORPG.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastLogout { get; set; }
        public int SecondsPlayed { get; set; }
        public bool Enabled { get; set; }

        public virtual LoginToken LoginToken { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
    }
}
