using System;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Programe.Network;

namespace Programe.Server.NetObjects
{
    public class NetAsteroid : NetObject
    {
        public override NetObjectType Type { get { return NetObjectType.Asteroid; } }
        public override bool IsStatic { get { return true; } }

        private Body body;
        private byte type;

        public NetAsteroid() { }

        public NetAsteroid(Body body, byte type)
        {
            this.body = body;
            this.type = type;
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write(body.Position.X * Constants.PixelsPerMeter);
            message.Write(body.Position.Y * Constants.PixelsPerMeter);
            message.Write(body.Rotation.ToNetworkRotation());
            message.Write(type);
        }

        protected override void Read(NetIncomingMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
