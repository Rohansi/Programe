using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using Texter;

namespace Programe.Gui.Widgets
{
    public class ListBoxItem
    {
        public string Text;
        public object Tag;

        public ListBoxItem(string text, object tag = null)
        {
            Text = text;
            Tag = tag;
        }
    }

    public class ListBox : Widget
    {
        public List<ListBoxItem> Items { get; private set; }
        public int Selected;
        public bool SelectEnabled;
        public event Action Changed;

        private Scrollbar scrollbar;

        public ListBox(int x, int y, uint w, uint h)
        {
            Left = x;
            Top = y;
            Width = w;
            Height = h;

            Items = new List<ListBoxItem>();

            Selected = -1;
            SelectEnabled = false;

            scrollbar = new Scrollbar((int)Width - 1, 1, Height - 2);
            scrollbar.Minimum = 0;
        }

        public override void Draw(TextRenderer renderer)
        {
            TexterBox.Single.Draw(renderer, 0, 0, Width, Height, Character.Create(0, 16, 10));

            scrollbar.Maximum = Math.Max(Items.Count - (Height - 2), 0);
            var startIndex = (int)scrollbar.Value;

            for (var i = 0; i < Height - 2; i++)
            {
                var index = startIndex + i;
                if (index >= Items.Count)
                    break;

                var reg = renderer.Region(1, i + 1, Width - 2, 1);
                Character col = (SelectEnabled && Selected == index) ? Character.Create(0, 10, 16) : Character.Create(0, 16, 10);
                reg.Clear(col);
                reg.DrawString(0, 0, Items[index].Text, Character.Create(0));
            }

            scrollbar.Draw(renderer.Region(scrollbar.Left, scrollbar.Top, scrollbar.Width, scrollbar.Height));
        }

        public override void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (SelectEnabled && pressed && (x > 0 && y > 0 && x < Width - 1 && y < Height - 1))
            {
                scrollbar.Maximum = Math.Max(Items.Count - (Height - 2), 0);
                var index = (int)scrollbar.Value + (y - 1);

                if (index < Items.Count)
                {
                    Selected = index;

                    if (Changed != null)
                        Changed();
                }
            }

            scrollbar.MousePressed(x - scrollbar.Left, y - scrollbar.Top, button, pressed);
        }

        public override void MouseMoved(int x, int y)
        {
            scrollbar.MouseMoved(x - scrollbar.Left, y - scrollbar.Top);
        }
    }
}
