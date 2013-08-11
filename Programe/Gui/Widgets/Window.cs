using System;
using SFML.Window;
using Texter;

namespace Programe.Gui.Widgets
{
    class Window : Widget, IContainer
    {
        private Container children;
        private bool dragging;
        private int dragOffset;

        public string Caption;
        public Func<bool> Closing = null;
        public event Action Closed;

        public Window(int x, int y, int w, int h, string caption)
        {
            Left = x;
            Top = y;
            Width = (uint)w;
            Height = (uint)h;

            children = new Container(Width - 2, Height - 2);
            Caption = caption;
        }

        public override void Draw(TextRenderer renderer)
        {
            var col = Character.Create(0, 16, 10);

            TexterBox.Double.Draw(renderer, 0, 0, Width, Height, col);
            renderer.DrawString(2, 0, "[\xFE]", col);

            if (!string.IsNullOrEmpty(Caption))
                renderer.DrawString(6, 0, string.Format(" {0} ", Caption), Character.Create(0, 10, 16));

            children.Draw(renderer.Region(1, 1, Width - 2, Height - 2));
        }

        public override void KeyPressed(Keyboard.Key key, string text)
        {
            children.KeyPressed(key, text);
        }

        public override void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (button == Mouse.Button.Left)
            {
                if (y == 0)
                {
                    if (x == 3)
                    {
                        var close = true;
                        if (Closing != null)
                            close = Closing();

                        if (close)
                        {
                            Visible = false;

                            if (Closed != null)
                                Closed();
                        }
                    }
                    else
                    {
                        dragging = pressed;
                        dragOffset = x;
                    }
                }

                if (dragging && !pressed)
                    dragging = false;
            }

            children.MousePressed(x - 1, y - 1, button, pressed);
        }

        public override void MouseMoved(int x, int y)
        {
            if (dragging)
            {
                Left += x - dragOffset;
                Top += y;

                Left = Clamp(Left, 0, (int)Parent.SurfaceWidth - 8);
                Top = Clamp(Top, 0, (int)Parent.SurfaceHeight - 1);
            }

            children.MouseMoved(x - 1, y - 1);
        }

        private static int Clamp(int value, int min, int max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        #region IContainer Methods
        public uint SurfaceWidth { get { return children.SurfaceWidth; } }
        public uint SurfaceHeight { get { return children.SurfaceHeight; } }

        public void Add(Widget widget)
        {
            children.Add(widget);
            widget.Parent = this;
        }

        public void Remove(Widget widget)
        {
            children.Remove(widget);
        }

        public void Focus(Widget widget)
        {
            children.Focus(widget);
        }

        public void BringToFront(Widget widget)
        {
            children.BringToFront(widget);
        }

        public void RemoveFocus()
        {
            children.RemoveFocus();
        }
        #endregion
    }
}
