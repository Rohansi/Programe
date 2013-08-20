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
            var x = 0;
            var y = 0;

            foreach (var c in Caption)
            {
                renderer.Set(x, y, Character.Create(c, 16));

                x++;
                if (x >= Width)
                {
                    x = 0;
                    y++;
                }

                if (y >= Height)
                    break;
            }
        }
    }
}
