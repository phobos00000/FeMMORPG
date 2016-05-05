using System;

namespace FeMMORPG.Data
{
    public class LoginToken
    {
        public Guid Id { get; set; }
        public DateTime LoginTime { get; set; }
        public string ServerIP { get; set; }

        public virtual User User { get; set; }
        public virtual Server Server { get; set; }
    }
}
