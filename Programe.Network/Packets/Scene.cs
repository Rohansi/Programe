using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace Programe.Network.Packets
{
    public class Scene : Packet
    {
        public override PacketId Id
        {
            get { return PacketId.Scene; }
        }

        public List<NetObject> Objects;

        public Scene()
        {
            Objects = new List<NetObject>();
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write((ushort)Objects.Count);
            foreach (var obj in Objects)
            {
                NetObject.WriteToMessage(obj, message);
            }
        }

        protected override void Read(NetIncomingMessage message)
        {
            Objects.Clear();

            var count = message.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                Objects.Add(NetObject.ReadFromMessage(message));
            }
        }
    }
}
