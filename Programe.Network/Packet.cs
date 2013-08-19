using System;
using Lidgren.Network;

namespace Programe.Network
{
    public enum PacketId : byte
    {
        Scene
    }

    public abstract partial class Packet
    {
        public abstract PacketId Id { get; }

        protected abstract void Write(NetOutgoingMessage message);
        protected abstract void Read(NetIncomingMessage message);
    }
}
