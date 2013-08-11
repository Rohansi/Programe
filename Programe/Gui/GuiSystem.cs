using System;
using SFML.Graphics;
using SFML.Window;

namespace Programe.Gui
{
    class GuiSystem : Container
    {
        private RenderWindow window;

        public GuiSystem(RenderWindow window, uint w, uint h)
            : base(w, h)
        {
            this.window = window;

            window.TextEntered += (sender, args) => KeyPressed(Keyboard.Key.Unknown, args.Unicode);
            window.KeyPressed += (sender, args) => KeyPressed(args.Code, null);
            window.MouseButtonPressed += (sender, args) => WindowMousePressed(args.X, args.Y, args.Button, true);
            window.MouseButtonReleased += (sender, args) => WindowMousePressed(args.X, args.Y, args.Button, false);
            window.MouseMoved += WindowMouseMoved;
        }

        private void WindowMousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (pressed)
                RemoveFocus();

            var pos = window.MapPixelToCoords(new Vector2i(x, y));
            MousePressed((int)pos.X / 8, (int)pos.Y / 12, button, pressed);
        }

        private void WindowMouseMoved(object sender, MouseMoveEventArgs args)
        {
            var pos = window.MapPixelToCoords(new Vector2i(args.X, args.Y));
            MouseMoved((int)pos.X / 8, (int)pos.Y / 12);
        }
    }
}
