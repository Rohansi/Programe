using System;
using Texter;

namespace Programe.Gui.Widgets
{
    public class Label : Widget
    {
        public string Caption;

        public Label(int x, int y, uint w, uint h, string caption)
        {
            Left = x;
            Top = y;
            Width = w;
            Height = h;

            Caption = caption;
        }

        public override void Draw(TextRenderer renderer)
        {
            renderer.DrawText(0, 0, Caption, Character.Create(foreground: 16));
        }
    }
}
