using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace Programe.Network
{
    public abstract partial class NetObject
    {
        private static Dictionary<NetObjectType, Type> netObjectTypes;

        static NetObject()
        {
            netObjectTypes = new Dictionary<NetObjectType, Type>();
        }

        public static void RegisterNetObject(Type type)
        {
            if (!type.IsSubclassOf(typeof(NetObject)))
                throw new Exception("Type is not a NetObject");

            var instance = (NetObject)Activator.CreateInstance(type);
            if (netObjectTypes.ContainsKey(instance.Type))
                throw new Exception("Duplicate NetObject type");

            netObjectTypes[instance.Type] = type;
        }

        public static void WriteToMessage(NetObject obj, NetOutgoingMessage message)
        {
            message.Write((byte)obj.Type);
            obj.Write(message);
        }

        public static NetObject ReadFromMessage(NetIncomingMessage message)
        {
            var type = (NetObjectType)message.ReadByte();
            var instance = (NetObject)Activator.CreateInstance(netObjectTypes[type]);
            instance.Read(message);
            return instance;
        }
    }
}
