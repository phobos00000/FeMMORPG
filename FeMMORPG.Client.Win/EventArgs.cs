using System;
using System.Collections.Generic;
using FeMMORPG.Common;
using FeMMORPG.Common.Models;

namespace FeMMORPG.Client.Win
{
    public delegate void ConnectEventHandler(object sender, ConnectEventArgs e);

    public class ConnectEventArgs : EventArgs
    {
        public bool Success { get; }
        public ErrorCodes Error { get; }
        public List<Character> Characters { get; }

        public ConnectEventArgs(bool success, ErrorCodes error, List<Character> characters = null)
        {
            Success = success;
            Error = error;
            Characters = characters;
        }
    }
}
