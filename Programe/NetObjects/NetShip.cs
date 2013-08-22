using System;
using Lidgren.Network;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;

namespace Programe.NetObjects
{
    public class NetShip : DrawableNetObject
    {
        private float x;
        private float y;
        private float rotation;

        public override NetObjectType Type
        {
            get { return NetObjectType.Ship; }
        }

        protected override void Write(NetOutgoingMessage message)
        {
            throw new NotImplementedException();
        }

        protected override void Read(NetIncomingMessage message)
        {
            x = message.ReadFloat();
            y = message.ReadFloat();
            rotation = ((float)message.ReadUInt16()) / ushort.MaxValue;
            rotation *= 360f;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            sprite.Position = new Vector2f(x, y);
            sprite.Rotation = rotation;
            target.Draw(sprite);
        }

        private static Sprite sprite;
        static NetShip()
        {
            var texture = new Texture("Data/ship.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f((float)texture.Size.X / 2, (float)texture.Size.Y / 2);
        }
    }
}
