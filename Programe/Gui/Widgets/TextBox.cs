using System;
using SFML.Window;
using Texter;

namespace Programe.Gui.Widgets
{
    class TextBox : Widget
    {
        private string value;
        private int selectedIndex;
        private int view;
        private int frames;
        private bool caretVisible;

        public event Action Changed;

        public TextBox(int x, int y, uint w)
        {
            Left = x;
            Top = y;
            Width = w;
            Height = 1;

            Value = "";
        }

        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                if (Changed != null)
                    Changed();
            }
        }

        public int SelectedIndex
        {
            get { return Math.Max(Math.Min(selectedIndex, Value.Length), 0); }
            set { selectedIndex = value; }
        }

        public override void Draw(TextRenderer renderer)
        {
            if (SelectedIndex > view + Width - 1)
                view = SelectedIndex - (int)Width + 1;
            else if (SelectedIndex <= view)
                view = SelectedIndex - (int)Width - 1;
            if (view < 0)
                view = 0;
            if (view > Value.Length)
                view = Value.Length - 1;
            if (Value.Length < Width)
                view = 0;

            renderer.Clear(Character.Create(0, 16, 2));
            renderer.DrawString(0, 0, Value.Substring(view), Character.Create(0));

            frames++;
            if (frames >= 30)
            {
                caretVisible = !caretVisible;
                frames = 0;
            }

            if (Focussed && caretVisible)
                renderer.Set(SelectedIndex - view, 0, Character.Create(0, 0, 16));
        }

        public override void KeyPressed(Keyboard.Key key, string text)
        {
            caretVisible = true;

            if (text == null)
            {
                switch (key)
                {
                    case Keyboard.Key.Left:
                        SelectedIndex -= 1;
                        break;
                    case Keyboard.Key.Right:
                        SelectedIndex += 1;
                        break;
                    case Keyboard.Key.Delete:
                        if (Value.Length > 0 && SelectedIndex < Value.Length)
                            Value = Value.Remove(SelectedIndex, 1);
                        break;
                }
                return;
            }

            if (text == "\b")
            {
                if (Value.Length == 0 || SelectedIndex - 1 < 0)
                    return;

                Value = Value.Remove(SelectedIndex - 1, 1);
                selectedIndex -= 1;
            }
            else
            {
                var c = (int)text[0];
                if (c < 32 || c > 126)
                    return;

                Value = Value.Insert(SelectedIndex, text);
                SelectedIndex += text.Length;
            }

            if (Changed != null)
                Changed();
        }

        public override void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (button != Mouse.Button.Left || !pressed)
                return;

            SelectedIndex = view + x;
        }
    }
}
