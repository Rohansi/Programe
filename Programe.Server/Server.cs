using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Programe.Network;
using Programe.Server.NetObjects;

namespace Programe.Server
{
    public class Server
    {
        private static NetServer server;

        public static void Start()
        {
            NetObject.RegisterNetObject(typeof(NetShip));
            NetObject.RegisterNetObject(typeof(NetAsteroid));

            var config = new NetPeerConfiguration(Constants.ApplicationIdentifier);
            config.Port = Constants.Port;

            server = new NetServer(config);
            server.Start();
        }

        public static void Update()
        {
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        Console.WriteLine(msg.ReadString());
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        var status = (NetConnectionStatus)msg.ReadByte();

                        string reason = msg.ReadString();
                        Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                        break;

                    case NetIncomingMessageType.Data:
                        Packet.Handle(msg);
                        break;

                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }

                server.Recycle(msg);
            }
        }

        public static void Broadcast(Packet packet, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
        {
            var message = server.CreateMessage();
            Packet.WriteToMessage(packet, message);
            server.SendToAll(message, null, method, sequenceChannel);
        }
    }
}
