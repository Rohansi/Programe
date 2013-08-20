using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lidgren.Network;

namespace Programe.Network
{
    public abstract partial class Packet
    {
        public delegate void PacketHandler(NetConnection connection, Packet packet);

        private static Dictionary<PacketId, Type> packetTypes;
        private static Dictionary<PacketId, PacketHandler> packetHandlers;

        static Packet()
        {
            packetTypes = new Dictionary<PacketId, Type>();
            packetHandlers = new Dictionary<PacketId, PacketHandler>();

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(i => i.IsSubclassOf(typeof(Packet)));
            foreach (var type in types)
            {
                var instance = (Packet)Activator.CreateInstance(type);
                packetTypes[instance.Id] = type;
            }
        }

        public static void WriteToMessage(Packet packet, NetOutgoingMessage message)
        {
            message.Write((byte)packet.Id);
            packet.Write(message);
        }

        public static void RegisterHandler(PacketId packetId, PacketHandler handler)
        {
            if (packetHandlers.ContainsKey(packetId))
                throw new Exception("Duplicate packet handler");

            packetHandlers[packetId] = handler;
        }

        public static void Handle(NetIncomingMessage message)
        {
            var packet = ReadFromMessage(message);
            packetHandlers[packet.Id](message.SenderConnection, packet);
        }

        private static Packet ReadFromMessage(NetIncomingMessage message)
        {
            var id = (PacketId)message.ReadByte();
            var instance = (Packet)Activator.CreateInstance(packetTypes[id]);
            instance.Read(message);
            return instance;
        }
    }
}
