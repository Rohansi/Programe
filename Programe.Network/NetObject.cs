using System;
using Lidgren.Network;

namespace Programe.Network
{
    public enum NetObjectType : byte
    {
        Asteroid, Ship, Bullet
    }

    public abstract partial class NetObject
    {
        public abstract NetObjectType Type { get; }

        protected abstract void Write(NetOutgoingMessage message);
        protected abstract void Read(NetIncomingMessage message);
    }
}
