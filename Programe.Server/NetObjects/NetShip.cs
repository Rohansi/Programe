using System;
using Lidgren.Network;
using Programe.Network;

namespace Programe.Server.NetObjects
{
    public class NetShip : NetObject
    {
        public override NetObjectType Type { get { return NetObjectType.Ship; } }
        public override bool IsStatic { get { return false; } }

        private Ship ship;

        public NetShip() { }

        public NetShip(Ship ship)
        {
            this.ship = ship;
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write(ship.Name);
            message.Write(ship.Body.Position.X * Constants.PixelsPerMeter);
            message.Write(ship.Body.Position.Y * Constants.PixelsPerMeter);
            message.Write(ship.Body.Rotation.ToNetworkRotation());
            message.Write((byte)(ship.Health * byte.MaxValue));
        }

        protected override void Read(NetIncomingMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
