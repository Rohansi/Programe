using System;
using SFML.Window;
using Texter;

namespace Programe.Gui.Widgets
{
    class Button : Widget
    {
        private bool holding;

        public string Caption;
        public event Action Clicked;

        public Button(int x, int y, int w, string caption)
        {
            Left = x;
            Top = y;
            Width = (uint)w;
            Height = 2;

            Caption = caption;
        }

        public override void Draw(TextRenderer renderer)
        {
            if (!holding)
            {
                renderer.DrawRectangle(0, 0, Width - 1, 1, Character.Create(0, 0, 3));
                renderer.Set((int)Width - 1, 0, Character.Create(220, 1));
                renderer.DrawRectangle(1, 1, Width, 1, Character.Create(223, 1));
                renderer.DrawText(1, 0, Caption, Character.Create(0, 16, 3));
            }
            else
            {
                renderer.DrawRectangle(1, 0, Width, 1, Character.Create(0, 0, 3));
                renderer.DrawText(2, 0, Caption, Character.Create(0, 16, 3));
            }
        }

        public override void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (button == Mouse.Button.Left)
            {
                if (pressed)
                {
                    holding = true;
                }
                else
                {
                    if (x >= 0 && y >= 0 && x < Width && y < Height)
                    {
                        if (holding && Clicked != null)
                            Clicked();
                    }

                    holding = false;
                }
            }
        }
    }
}
