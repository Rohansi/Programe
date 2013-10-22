using System;
using Lidgren.Network;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;

namespace Programe.NetObjects
{
    public class NetShip : DrawableNetObject
    {
        public override NetObjectType Type { get { return NetObjectType.Ship; } }
        public override bool IsStatic { get { return false; } }

        public string Name;
        public float X;
        public float Y;
        public float Health;
        private float rotation;

        protected override void Write(NetOutgoingMessage message)
        {
            throw new NotSupportedException();
        }

        protected override void Read(NetIncomingMessage message)
        {
            Name = message.ReadString();
            X = message.ReadFloat();
            Y = message.ReadFloat();
            rotation = message.ReadUInt16().FromNetworkRotation() * (180f / (float)Math.PI);
            Health = message.ReadByte() / (float)byte.MaxValue;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            sprite.Position = new Vector2f(X, Y);
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
