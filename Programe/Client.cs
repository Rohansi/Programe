using System;
using System.Collections.Generic;
using Lidgren.Network;
using Programe.Network;

namespace Programe
{
    public static class Client
    {
        private static NetClient client;

        public static bool Connected { get; private set; }
        public static List<NetworkObject> Objects;
         
        public static void Start()
        {
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
                            Interface.AddStatusMessage("Connected to server");
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            Connected = false;
                            Interface.Disconnected();
                            Interface.AddStatusMessage("Disconnected from server");
                            Connect();
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        HandleMessage(msg);
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

        private static void Connect()
        {
            client.Connect(Constants.Server, Constants.Port);
        }

        private static void HandleMessage(NetIncomingMessage message)
        {
            var id = message.ReadByte();
            switch (id)
            {
                case 10:
                {
                    var newObjects = new List<NetworkObject>();
                    while (message.Position < message.LengthBits)
                    {
                        newObjects.Add(new NetworkObject(message));
                    }
                    Console.WriteLine("DATA! " + DateTime.Now.Millisecond + " " + newObjects.Count);
                    Objects = newObjects;
                    break;
                }
            }
        }
    }
}
