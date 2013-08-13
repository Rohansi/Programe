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

                        string reason = msg.ReadString();
                        Interface.AddStatusMessage(status + ": " + reason);

                        if (status == NetConnectionStatus.Connected)
                        {
                            Connected = true;
                            Interface.Connected();
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            Connected = false;
                            Interface.Disconnected();
                            Connect();
                        }
                        break;
                    case NetIncomingMessageType.Data:
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
    }
}
