using System;
using Lidgren.Network;

namespace Programe.Network.Packets
{
    public class Message : Packet
    {
        public override PacketId Id
        {
            get { return PacketId.Message; }
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
