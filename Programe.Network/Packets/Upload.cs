using System;
using Lidgren.Network;

namespace Programe.Network.Packets
{
    public class Upload : Packet
    {
        public override PacketId Id
        {
            get { return PacketId.Upload; }
        }

        protected override void Write(NetOutgoingMessage message)
        {
            throw new NotImplementedException();
        }

        protected override void Read(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
