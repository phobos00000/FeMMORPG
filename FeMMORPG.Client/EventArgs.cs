using System;
using System.Collections.Generic;
using FeMMORPG.Common.Models;

namespace FeMMORPG.Client
{
    public delegate void CharacterEventHandler(object sender, CharacterEventArgs e);

    public class CharacterEventArgs : EventArgs
    {
        public List<Character> Characters { get; }

        public CharacterEventArgs(List<Character> characters)
        {
            Characters = characters;
        }
    }
}
