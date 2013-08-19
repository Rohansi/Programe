using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Programe.NetObjects;
using Programe.Network;
using Programe.Network.Packets;

namespace Programe
{
    public static class Client
    {
        private static NetClient client;

        public static string Server = "127.0.0.1";
        public static bool Connected { get; private set; }
        public static List<DrawableNetObject> Objects;
         
        public static void Start()
        {
            Packet.RegisterHandler(PacketId.Scene, HandleScene);

            NetObject.RegisterNetObject(typeof(NetShip));
            NetObject.RegisterNetObject(typeof(NetAsteroid));

            var config = new NetPeerConfiguration(Constants.ApplicationIdentifier);
            client = new NetClient(config);
            client.Start();

            Connect();
        }

        public static void Update()
        {
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
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

                        if (status == NetConnectionStatus.Connected)
                        {
                            Connected = true;
                            Interface.Connected();
                            Interface.AddStatusMessage(string.Format("Connected to server ({0})", Server));
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            Connected = false;
                            Interface.Disconnected();
                            Interface.AddStatusMessage("Disconnected");
                            Connect();
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        Packet.Handle(msg);
                        break;

                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }

                client.Recycle(msg);
            }
        }

        public static void Login(string username, string password)
        {
            // todo
        }

        public static void Register(string username, string password)
        {
            // todo
        }

        private static void Connect()
        {
            client.Connect(Server, Constants.Port);
        }

        private static void HandleScene(Packet packet)
        {
            var scene = (Scene)packet;
            Objects = scene.Objects.OfType<DrawableNetObject>().ToList();
        }
    }
}
