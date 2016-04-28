using System;
using System.Collections.Generic;

namespace FeMMORPG.Common
{
    [Serializable]
    public class Packet
    {
        public Commands Command { get; set; }
        public List<object> Parameters { get; set; } = new List<object>();
    }
}
