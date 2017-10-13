﻿namespace FeMMORPG.Data
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
