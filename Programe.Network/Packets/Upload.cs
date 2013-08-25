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

        public short[] Program;

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write((ushort)Program.Length);
            foreach (var s in Program)
            {
                message.Write(s);
            }
        }

        protected override void Read(NetIncomingMessage message)
        {
            var length = message.ReadUInt16();
            Program = new short[length];
            for (var i = 0; i < length; i++)
            {
                Program[i] = message.ReadInt16();
            }
        }
    }
}
