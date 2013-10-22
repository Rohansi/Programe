using System;
using System.Collections.Generic;
using Lidgren.Network;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;

namespace Programe.NetObjects
{
    public class NetAsteroid : DrawableNetObject
    {
        public override NetObjectType Type { get { return NetObjectType.Asteroid; } }
        public override bool IsStatic { get { return true; } }

        private float x;
        private float y;
        private float rotation;
        private byte type;

        protected override void Write(NetOutgoingMessage message)
        {
            throw new NotSupportedException();
        }

        protected override void Read(NetIncomingMessage message)
        {
            x = message.ReadFloat();
            y = message.ReadFloat();
            rotation = message.ReadUInt16().FromNetworkRotation() * (180f / (float)Math.PI);
            type = message.ReadByte();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            var sprite = sprites[type];
            sprite.Position = new Vector2f(x, y);
            sprite.Rotation = rotation;
            target.Draw(sprite);
        }

        private static List<Sprite> sprites;
        static NetAsteroid()
        {
            sprites = new List<Sprite>();

            for (var i = 0; i < Constants.AsteroidRadiuses.Count; i++)
            {
                var texture = new Texture(string.Format("Data/asteroid{0}.png", i));
                var sprite = new Sprite(texture);
                sprite.Origin = new Vector2f((float)texture.Size.X / 2, (float)texture.Size.Y / 2);
                sprites.Add(sprite);
            }
        }
    }
}
