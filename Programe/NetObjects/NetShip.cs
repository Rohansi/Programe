using System;
using Lidgren.Network;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;

namespace Programe.NetObjects
{
    public class NetShip : DrawableNetObject
    {
        public string Name;
        public float X;
        public float Y;
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
            Name = message.ReadString();
            X = message.ReadFloat();
            Y = message.ReadFloat();
            rotation = message.ReadUInt16().FromNetworkRotation() * (180f / (float)Math.PI);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            sprite.Position = new Vector2f(X, Y);
            sprite.Rotation = rotation;
            target.Draw(sprite);

            text.DisplayedString = Name;
            var bounds = text.GetLocalBounds();
            text.Origin = new Vector2f(bounds.Width / 2, (bounds.Height / 2) + bounds.Top);
            text.Rotation = rotation - 90;
            text.Position = new Vector2f(X, Y);
            target.Draw(text);
        }

        private static Text text;
        private static Sprite sprite;
        static NetShip()
        {
            var font = new Font("Data/SourceSansPro-Regular.otf");
            text = new Text("", font, 14);
            text.Color = Color.Black;

            var texture = new Texture("Data/ship.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f((float)texture.Size.X / 2, (float)texture.Size.Y / 2);
        }
    }
}
