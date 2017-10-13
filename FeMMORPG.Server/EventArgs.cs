using System;
using System.Collections.Generic;
using FeMMORPG.Common;
using FeMMORPG.Data;

namespace FeMMORPG.Server
{
    public delegate void PacketEventHandler(object sender, PacketEventArgs e);
    public delegate void LoginEventHandler(object sender, LoginEventArgs e);
    public delegate void LogoutEventHandler(object sender, LogoutEventArgs e);

    public class PacketEventArgs : EventArgs
    {
        public Commands Command { get; }
        public List<object> Parameters { get; }

        public PacketEventArgs(Commands command, List<object> parameters)
        {
            Command = command;
            Parameters = parameters;
        }
    }

    public class LoginEventArgs : EventArgs
    {
        public User User { get; }
        public DateTime LoginTime { get; }

        public LoginEventArgs(User user, DateTime loginTime)
        {
            User = user;
            LoginTime = loginTime;
        }
    }

    public class LogoutEventArgs : EventArgs
    {
        public User User { get; }
        public DateTime LogoutTime { get; }

        public LogoutEventArgs(User user, DateTime logoutTime)
        {
            User = user;
            LogoutTime = logoutTime;
        }
    }
}
