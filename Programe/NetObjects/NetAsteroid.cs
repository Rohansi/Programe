using System;
using Lidgren.Network;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;

namespace Programe.NetObjects
{
    public class NetAsteroid : DrawableNetObject
    {
        private float x;
        private float y;
        private float rotation;

        public override NetObjectType Type
        {
            get { return NetObjectType.Asteroid; }
        }

        protected override void Write(NetOutgoingMessage message)
        {
            throw new NotImplementedException();
        }

        protected override void Read(NetIncomingMessage message)
        {
            x = message.ReadFloat();
            y = message.ReadFloat();
            rotation = message.ReadUInt16().FromNetworkRotation() * (180f / (float)Math.PI);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            sprite.Position = new Vector2f(x, y);
            sprite.Rotation = rotation;
            target.Draw(sprite);
        }

        private static Sprite sprite;
        static NetAsteroid()
        {
            var texture = new Texture("Data/asteroid.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f((float)texture.Size.X / 2, (float)texture.Size.Y / 2);
        }
    }
}
