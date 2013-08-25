using System;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Programe.Network;

namespace Programe.Server.NetObjects
{
    public class NetAsteroid : NetObject
    {
        private Body body;

        public override NetObjectType Type
        {
            get { return NetObjectType.Asteroid; }
        }

        public NetAsteroid() { }

        public NetAsteroid(Body body)
        {
            this.body = body;
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write(body.Position.X * Constants.PixelsPerMeter);
            message.Write(body.Position.Y * Constants.PixelsPerMeter);
            message.Write(body.Rotation.ToNetworkRotation());
        }

        protected override void Read(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
