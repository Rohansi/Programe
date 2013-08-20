using System;
using Lidgren.Network;

namespace Programe.Network.Packets
{
    public enum AuthType : byte
    {
        Login, Register
    }

    public class Auth : Packet
    {
        public override PacketId Id
        {
            get { return PacketId.Auth; }
        }

        public AuthType Type;
        public string Username;
        public string Password;

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write((byte)Type);
            message.Write(Username);
            message.Write(Password);
        }

        protected override void Read(NetIncomingMessage message)
        {
            Type = (AuthType)message.ReadByte();
            Username = message.ReadString();
            Password = message.ReadString();
        }
    }
}
