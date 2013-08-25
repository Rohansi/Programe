using System;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Programe.Network;

namespace Programe.Server.NetObjects
{
    public class NetBullet : NetObject
    {
        private Body body;

        public override NetObjectType Type
        {
            get { return NetObjectType.Bullet; }
        }

        public NetBullet() { }

        public NetBullet(Body body)
        {
            this.body = body;
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write((short)(body.Position.X * Constants.PixelsPerMeter));
            message.Write((short)(body.Position.Y * Constants.PixelsPerMeter));
        }

        protected override void Read(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
