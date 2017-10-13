using System;

namespace FeMMORPG.Common.Models
{
    [Serializable]
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
