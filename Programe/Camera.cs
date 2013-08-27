using System;
using SFML.Graphics;
using SFML.Window;

namespace Programe
{
    public class Camera
    {
        /// <summary>
        /// Center point of the camera
        /// </summary>
        public Vector2f Position;

        /// <summary>
        /// Gets or sets the current zoom level of the camera
        /// </summary>
        public float Zoom
        {
            get { return view.Size.X / originalSize.X; }
            set { view.Size = originalSize; view.Zoom(value); }
        }

        /// <summary>
        /// Calculates the area the camera should display
        /// </summary>
        public FloatRect Bounds
        {
            get { return new FloatRect(view.Center.X - (view.Size.X / 2), view.Center.Y - (view.Size.Y / 2), view.Size.X, view.Size.Y); }
        }

        private View view;
        private Vector2f originalSize;

        public Camera(FloatRect rect) : this(new View(rect)) { }

        public Camera(View view)
        {
            this.view = new View(view);
            Position = view.Size / 2;
            originalSize = view.Size;
        }

        public void Apply(RenderTarget rt)
        {
            var center = Position;
            var offset = 0.25f * Zoom;
            center.X += offset;
            center.Y += offset;

            view.Center = center;
            rt.SetView(view);
        }
    }
}
