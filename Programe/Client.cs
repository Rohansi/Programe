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
        public static bool LoggedIn { get; private set; }

        public static List<DrawableNetObject> Objects;
         
        public static void Start()
        {
            Packet.RegisterHandler(PacketId.Scene, HandleScene);
            Packet.RegisterHandler(PacketId.AuthResponse, HandleAuthResponse);
            Packet.RegisterHandler(PacketId.Message, HandleMessage);

            NetObject.RegisterNetObject(typeof(NetShip));
            NetObject.RegisterNetObject(typeof(NetAsteroid));
            NetObject.RegisterNetObject(typeof(NetBullet));

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
                            LoggedIn = false;
                            Interface.Connected();
                            Console.WriteLine("Connected to server ({0})", Server);
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            Connected = false;
                            LoggedIn = false;
                            Interface.Disconnected();
                            Console.WriteLine("Disconnected");
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

        private static void Connect()
        {
            client.Connect(Server, Constants.Port);
        }

        public static void Send(Packet packet, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
        {
            var message = client.CreateMessage();
            Packet.WriteToMessage(packet, message);
            client.SendMessage(message, method, sequenceChannel);
        }

        public static void Login(string username, string password)
        {
            var login = new Auth
            {
                Type = AuthType.Login,
                Username = username,
                Password = password
            };

            Send(login);
        }

        public static void Register(string username, string password)
        {
            var register = new Auth
            {
                Type = AuthType.Register,
                Username = username,
                Password = password
            };

            Send(register);
        }

        private static void HandleScene(NetConnection connection, Packet packet)
        {
            var scene = (Scene)packet;
            Objects = scene.Objects.OfType<DrawableNetObject>().OrderByDescending(o => o.Type).ToList();
            Interface.MainForm.UpdateShips();
        }

        private static void HandleAuthResponse(NetConnection connection, Packet packet)
        {
            var resp = (AuthResponse)packet;

            if (resp.Success)
                LoggedIn = true;

            Interface.ShowMessage(resp.Type.ToString(), resp.Message);
        }

        private static void HandleMessage(NetConnection connection, Packet packet)
        {
            var message = (Message)packet;
            Interface.ShowMessage(message.Title, message.Content);
        }
    }
}
