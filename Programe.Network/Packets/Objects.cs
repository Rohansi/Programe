using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace Programe.Network.Packets
{
    public class Objects : Packet
    {
        public override PacketId Id
        {
            get { return PacketId.Objects; }
        }

        public List<NetObject> Items;

        public Objects()
        {
            Items = new List<NetObject>();
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write((ushort)Items.Count);
            foreach (var obj in Items)
            {
                NetObject.WriteToMessage(obj, message);
            }
        }

        protected override void Read(NetIncomingMessage message)
        {
            Items.Clear();

            var count = message.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                Items.Add(NetObject.ReadFromMessage(message));
            }
        }
    }
}
