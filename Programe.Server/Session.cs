using System;
using Lidgren.Network;

namespace Programe.Server
{
    public class Session
    {
        public readonly long Id;
        public readonly NetConnection Connection;
        public Account Account;

        public Session(NetConnection connection)
        {
            Id = connection.RemoteUniqueIdentifier;
            Connection = connection;
        }
    }
}
