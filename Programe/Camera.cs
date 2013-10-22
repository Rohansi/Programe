﻿using System;
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
            get { return View.Size.X / originalSize.X; }
            set { View.Size = originalSize; View.Zoom(value); }
        }

        /// <summary>
        /// Calculates the area the camera should display
        /// </summary>
        public FloatRect Bounds
        {
            get { return new FloatRect(View.Center.X - (View.Size.X / 2), View.Center.Y - (View.Size.Y / 2), View.Size.X, View.Size.Y); }
        }

        public View View { get; private set; }

        private Vector2f originalSize;

        public Camera(FloatRect rect) : this(new View(rect)) { }

        public Camera(View view)
        {
            this.View = new View(view);
            Position = view.Size / 2;
            originalSize = view.Size;
        }

        public void Apply(RenderTarget rt)
        {
            var center = Position;
            var offset = 0.25f * Zoom;
            center.X += offset;
            center.Y += offset;

            View.Center = center;
            rt.SetView(View);
        }
    }
}
