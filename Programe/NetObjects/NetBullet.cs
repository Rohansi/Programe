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
        private float rotation;

        protected override void Write(NetOutgoingMessage message)
        {
            throw new NotSupportedException();
        }

        protected override void Read(NetIncomingMessage message)
        {
            x = message.ReadInt16();
            y = message.ReadInt16();
            rotation = message.ReadUInt16().FromNetworkRotation() * (180f / (float)Math.PI);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            sprite.Position = new Vector2f(x, y);
            sprite.Rotation = rotation;
            target.Draw(sprite);
        }

        private static Sprite sprite;
        static NetBullet()
        {
            var texture = new Texture("Data/bullet.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f((float)texture.Size.X / 2, 0);
        }
    }
}
