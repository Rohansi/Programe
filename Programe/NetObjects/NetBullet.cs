using System;
using Lidgren.Network;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;

namespace Programe.NetObjects
{
    public class NetBullet : DrawableNetObject
    {
        public override NetObjectType Type { get { return NetObjectType.Bullet; } }
        public override bool IsStatic { get { return false; } }

        private float x;
        private float y;

        protected override void Write(NetOutgoingMessage message)
        {
            throw new NotSupportedException();
        }

        protected override void Read(NetIncomingMessage message)
        {
            x = message.ReadInt16();
            y = message.ReadInt16();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            shape.Position = new Vector2f(x, y);
            target.Draw(shape);
        }

        private static CircleShape shape;
        static NetBullet()
        {
            shape = new CircleShape(3, 6);
            shape.Origin = new Vector2f(3, 3);
            shape.FillColor = Color.White;
        }
    }
}
