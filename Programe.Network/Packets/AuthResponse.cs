using System;
using Lidgren.Network;

namespace Programe.Network.Packets
{
    public class AuthResponse : Packet
    {
        public override PacketId Id
        {
            get { return PacketId.AuthResponse; }
        }

        public AuthType Type;
        public bool Success;
        public string Message;

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write((byte)Type);
            message.Write(Success);
            message.Write(Message);
        }

        protected override void Read(NetIncomingMessage message)
        {
            Type = (AuthType)message.ReadByte();
            Success = message.ReadBoolean();
            Message = message.ReadString();
        }
    }
}
