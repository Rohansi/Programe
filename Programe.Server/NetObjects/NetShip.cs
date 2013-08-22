using System;
using Lidgren.Network;
using Programe.Network;

namespace Programe.Server.NetObjects
{
    public class NetShip : NetObject
    {
        private Ship ship;

        public override NetObjectType Type
        {
            get { return NetObjectType.Ship; }
        }

        public NetShip() { }

        public NetShip(Ship ship)
        {
            this.ship = ship;
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write(ship.Body.Position.X * Constants.PixelsPerMeter);
            message.Write(ship.Body.Position.Y * Constants.PixelsPerMeter);

            const float maxRadians = (float)Math.PI * 2;
            var rotation = ship.Body.Rotation % maxRadians;
            if (rotation < 0)
                rotation += maxRadians;
            rotation /= maxRadians;

            message.Write((ushort)(rotation * ushort.MaxValue));
        }

        protected override void Read(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
