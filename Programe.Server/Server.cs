using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Programe.Network;

namespace Programe.Server
{
    class Server
    {
        private static NetServer server;

        public static void Start()
        {
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
                        break;

                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }

                server.Recycle(msg);
            }
        }

        public static NetOutgoingMessage CreateMessage()
        {
            return server.CreateMessage();
        }

        public static void Broadcast(NetOutgoingMessage message, NetDeliveryMethod method, int sequenceChannel)
        {
            server.SendToAll(message, null, method, sequenceChannel);
        }
    }
}
