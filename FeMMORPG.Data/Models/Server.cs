using System.Collections.Generic;

namespace FeMMORPG.Data
{
    public class Server
    {
        public string IP { get; set; }
        public int CurrentUsers { get; set; }
        public int MaxUsers { get; set; }
        public bool Enabled { get; set; }

        public virtual ICollection<LoginToken> LoginTokens { get; set; }
    }
}
