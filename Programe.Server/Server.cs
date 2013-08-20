using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Programe.Network;
using Programe.Network.Packets;
using Programe.Server.NetObjects;

namespace Programe.Server
{
    public class Server
    {
        private static NetServer server;

        public static SessionManager SessionManager;

        public static void Start()
        {
            Packet.RegisterHandler(PacketId.Auth, HandleAuth);

            NetObject.RegisterNetObject(typeof(NetShip));
            NetObject.RegisterNetObject(typeof(NetAsteroid));

            SessionManager = new SessionManager();

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

                        if (status == NetConnectionStatus.Connected)
                        {
                            SessionManager.Create(msg.SenderConnection);
                        }

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
                SessionManager.Clean();
            }
        }

        public static void Send(Packet packet, Session session, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
        {
            var message = server.CreateMessage();
            Packet.WriteToMessage(packet, message);
            session.Connection.SendMessage(message, method, sequenceChannel);
        }

        public static void Broadcast(Packet packet, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered, int sequenceChannel = 0)
        {
            var message = server.CreateMessage();
            Packet.WriteToMessage(packet, message);
            server.SendToAll(message, null, method, sequenceChannel);
        }

        public static void HandleAuth(NetConnection connection, Packet packet)
        {
            var auth = (Auth)packet;
            var session = SessionManager.Get(connection.RemoteUniqueIdentifier);

            if (session.Account != null)
                return;

            Account account;
            var error = "";
            switch (auth.Type)
            {
                case AuthType.Login:
                    account = Account.Login(auth.Username, auth.Password, out error);
                    break;
                case AuthType.Register:
                    account = Account.Register(auth.Username, auth.Password, out error);
                    break;
                default:
                    return;
            }

            if (account != null)
                SessionManager.Kick(account.Username);

            session.Account = account;

            var response = new AuthResponse
            {
                Type = auth.Type,
                Success = account != null,
                Message = error
            };

            Send(response, session);
        }
    }
}
