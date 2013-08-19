using System;
using Programe.Network;
using SFML.Graphics;

namespace Programe
{
    public abstract class DrawableNetObject : NetObject, Drawable
    {
        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}
