using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;

namespace Programe.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("Programe");
            config.Port = 17394;

            var server = new NetServer(config);
            server.Start();

            while (true)
            {
                NetIncomingMessage im;
                while ((im = server.ReadMessage()) != null)
                {
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            Console.WriteLine(im.ReadString());
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            var status = (NetConnectionStatus)im.ReadByte();

                            string reason = im.ReadString();
                            Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            break;

                        case NetIncomingMessageType.Data:
                            break;

                        default:
                            Console.WriteLine("Unhandled type: " + im.MessageType);
                            break;
                    }

                    server.Recycle(im);
                }

                Thread.Sleep(1);
            }
        }
    }
}
