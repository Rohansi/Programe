using System;
using System.Collections.Generic;
using Lidgren.Network;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;

namespace Programe
{
    public class NetworkObject : Drawable
    {
        public readonly int Id;

        private byte type;
        private float x;
        private float y;
        private float rotation;

        public NetworkObject(NetIncomingMessage message)
        {
            type = message.ReadByte();

            switch (type)
            {
                case 0:
                case 1:
                {
                    Id = message.ReadInt32();
                    x = message.ReadFloat();
                    y = message.ReadFloat();
                    rotation = message.ReadFloat();
                    break;
                }
            }
        }

        public void Update(NetIncomingMessage message)
        {
            
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            switch (type)
            {
                case 0:
                {
                    Ship.Position = new Vector2f(x * Constants.PixelsPerMeter, y * Constants.PixelsPerMeter);
                    Ship.Rotation = rotation * (180 / (float)Math.PI);
                    target.Draw(Ship);
                    break;
                }

                case 1:
                {
                    Asteroid.Position = new Vector2f(x * Constants.PixelsPerMeter, y * Constants.PixelsPerMeter);
                    Asteroid.Rotation = rotation * (180 / (float)Math.PI);
                    target.Draw(Asteroid);
                    break;
                }
            }
        }

        private static readonly Sprite Ship;
        private static readonly Sprite Asteroid;

        static NetworkObject()
        {
            var shipTex = new Texture("Data/ship.png");
            Ship = new Sprite(shipTex);
            Ship.Origin = new Vector2f(shipTex.Size.X / 2f, shipTex.Size.Y / 2f);

            var asteroidTex = new Texture("Data/asteroid.png");
            Asteroid = new Sprite(asteroidTex);
            Asteroid.Origin = new Vector2f(asteroidTex.Size.X / 2f, asteroidTex.Size.Y / 2f);
        }
    }
}
