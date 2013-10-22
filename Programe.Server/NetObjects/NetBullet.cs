using System;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Programe.Network;

namespace Programe.Server.NetObjects
{
    public class NetBullet : NetObject
    {
        public override NetObjectType Type { get { return NetObjectType.Bullet; } }
        public override bool IsStatic { get { return false; } }

        private Body body;

        public NetBullet() { }

        public NetBullet(Body body)
        {
            this.body = body;
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write((short)(body.Position.X * Constants.PixelsPerMeter));
            message.Write((short)(body.Position.Y * Constants.PixelsPerMeter));
            message.Write(body.Rotation.ToNetworkRotation());
        }

        protected override void Read(NetIncomingMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
