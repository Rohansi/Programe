using System;
using Lidgren.Network;

namespace Programe.Network
{
    public enum NetObjectType : byte
    {
        Asteroid, Ship
    }

    public abstract partial class NetObject
    {
        public abstract NetObjectType Type { get; }

        protected abstract void Write(NetOutgoingMessage message);
        protected abstract void Read(NetIncomingMessage message);
    }
}
