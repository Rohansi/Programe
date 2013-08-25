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

        public string Title;
        public string Content;

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write(Title);
            message.Write(Content);
        }

        protected override void Read(NetIncomingMessage message)
        {
            Title = message.ReadString();
            Content = message.ReadString();
        }
    }
}
