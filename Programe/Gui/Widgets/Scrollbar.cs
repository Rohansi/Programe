using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using Texter;

namespace Programe.Gui.Widgets
{
    class Scrollbar : Widget
    {
        private float internalValue;
        private bool dragging;

        public event Action Changed;
        public float Minimum, Maximum;
        public float Step = 1;

        public Scrollbar(int x, int y, uint height)
        {
            Left = x;
            Top = y;
            Height = height;
            Width = 1;
        }

        private float InternalValue
        {
            get { return internalValue; }
            set
            {
                internalValue = value < 0 ? 0 : (value > 1 ? 1 : value);
                if (Changed != null)
                    Changed();
            }
        }

        public float Value
        {
            get { return (InternalValue * (Maximum - Minimum)) + Minimum; }
            set { InternalValue = (value - Minimum) / (Maximum - Minimum); }
        }

        public override void Draw(TextRenderer renderer)
        {
            renderer.Set(0, 0, Character.Create(30, 16));
            for (var y = 1; y < Height - 1; y++)
            {
                renderer.Set(0, y, Character.Create(176, 16));
            }
            renderer.Set(0, (int)Height - 1, Character.Create(31, 16));

            var x = 1 + (InternalValue * (Height - 3));
            renderer.Set(0, (int)x, Character.Create(219, 16));
        }

        public override void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (button != Mouse.Button.Left)
                return;

            if (pressed && x == 0)
            {
                if (y == 0)
                {
                    Value -= Step;
                }
                else if (y == Height - 1)
                {
                    Value += Step;
                }
                else
                {
                    dragging = true;
                }
            }

            if (!pressed)
                dragging = false;
        }

        public override void MouseMoved(int x, int y)
        {
            if (!dragging)
                return;

            InternalValue = ((float)y) / (Height - 2);
        }
    }
}
