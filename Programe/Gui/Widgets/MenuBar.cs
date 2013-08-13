using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Window;
using Texter;

namespace Programe.Gui.Widgets
{
    public class MenuBar : Widget
    {
        public List<MenuItem> Items { get; private set; }

        private MenuItem selected;
        private MenuDrop drop;

        public MenuBar()
        {
            Left = 0;
            Top = 0;
            Width = 500; // assuming 500 is impossibly wide
            Height = 1;

            Items = new List<MenuItem>();
            drop = new MenuDrop();
        }

        public override void Initialize(IContainer parent)
        {
            base.Initialize(parent);
            Parent.Add(drop);
        }

        public override void Draw(TextRenderer renderer)
        {
            Width = Parent.SurfaceWidth;

            if (!drop.Visible)
                selected = null;

            renderer.Clear(Character.Create(0, 1, 8));

            var x = 0;
            foreach (var i in Items)
            {
                var highlight = selected != null && selected == i;
                i.Draw(renderer.Region(x, 0, (uint)i.Caption.Length + 2, 1), highlight, false);
                x += i.Caption.Length + 2;
            }
        }

        public override void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (button != Mouse.Button.Left || !pressed)
                return;

            var xx = 0;
            foreach (var i in Items)
            {
                if (x >= xx && x < xx + i.Caption.Length + 2)
                {
                    i.Click();

                    if (i.Items.Count > 0)
                    {
                        selected = i;
                        drop.Show(xx, 1, i.Items);
                        drop.Focus();
                    }
                }

                xx += i.Caption.Length + 2;
            }
        }
    }

    class MenuDrop : Widget
    {
        private List<MenuItem> items;

        private MenuDrop drop;
        private MenuItem selected;
         
        public MenuDrop()
        {
            Visible = false;
        }

        public void Show(int x, int y, List<MenuItem> items)
        {
            Left = x;
            Top = y;
            Width = (uint)Math.Max(items.Max(i => i.Caption.Length), 18) + 5;
            Height = (uint)items.Count + 2;
            Visible = true;

            if (drop == null)
            {
                drop = new MenuDrop();
                Parent.Add(drop);
            }

            selected = null;
            this.items = items;
        }

        public override void Draw(TextRenderer renderer)
        {
            if (drop == null || !drop.Visible)
                selected = null;
            
            if (!Focussed && selected == null)
                Visible = false;

            TexterBox.Single.Draw(renderer, 0, 0, Width, Height, Character.Create(0, 9, 8));

            var y = 1;
            foreach (var i in items)
            {
                var highlight = selected != null && selected == i;
                i.Draw(renderer.Region(1, y, Width - 2, 1), highlight, true);
                y++;
            }
        }

        public override void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (button != Mouse.Button.Left || !pressed)
                return;

            var yy = 1;
            foreach (var i in items)
            {
                if ((x >= 1 && x < Width - 1) && yy == y)
                {
                    i.Click();

                    if (i.Items.Count > 0)
                    {
                        selected = i;
                        drop.Show(Left + 1, Top + yy + 1, i.Items);
                        drop.Focus();
                    }
                    else
                    {
                        Visible = false;
                    }
                }

                yy++;
            }
        }
    }

    public class MenuItem
    {
        public string Caption;
        public List<MenuItem> Items { get; private set; }
        public event Action Clicked;

        public MenuItem(string caption)
        {
            Items = new List<MenuItem>();
            Caption = caption;
        }

        public void Draw(TextRenderer renderer, bool highlight, bool extended)
        {
            var col = Character.Create(0, 1, highlight ? 9 : 8);
            var caption = string.Format(" {0} ", Caption);

            renderer.Clear(col);
            renderer.DrawString(0, 0, caption, col);

            if (extended && Items.Count > 0)
                renderer.Set((int)renderer.Width - 1, 0, Character.Create(16));
        }

        // INTERNAL
        public void Click()
        {
            if (Clicked != null)
                Clicked();
        }
    }
}
