﻿using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Window;
using Texter;

namespace Programe.Gui
{
    class Container : Widget, IContainer
    {
        private LinkedList<Widget> children;
        private Widget focus;

        public uint SurfaceWidth { get; private set; }
        public uint SurfaceHeight { get; private set; }

        public Container(uint w, uint h)
        {
            SurfaceWidth = w;
            SurfaceHeight = h;

            children = new LinkedList<Widget>();
            focus = null;
        }

        public void Add(Widget widget)
        {
            widget.Parent = this;
            children.AddFirst(widget);
        }

        public void Remove(Widget widget)
        {
            widget.Parent = null;
            children.Remove(widget);

            if (focus == widget)
                focus = null;
        }

        // INTERNAL ONLY
        public void Focus(Widget widget)
        {
            if (!children.Contains(widget))
                return;

            BringToFront(widget);

            if (focus != null)
                focus.Focussed = false;

            focus = widget;

            if (Parent != null)
                Parent.Focus(this);

            widget.Focussed = true;
        }

        // INTERNAL ONLY
        public void BringToFront(Widget widget)
        {
            var node = children.Find(widget);
            if (node == null)
                return;

            children.Remove(node);
            children.AddFirst(widget);

            if (Parent != null)
                Parent.BringToFront(this);
        }

        public void RemoveFocus()
        {
            if (focus == null)
                return;

            focus.Focussed = false;

            var focusContainer = focus as IContainer;
            if (focusContainer != null)
            {
                focusContainer.RemoveFocus();
            }
        }

        public override void Draw(TextRenderer renderer)
        {
            var node = children.Last;
            while (node != null)
            {
                var w = node.Value;

                if (w.Visible)
                {
                    var region = renderer.Region(w.Left, w.Top, w.Width, w.Height);
                    node.Value.Draw(region);
                }

                node = node.Previous;
            }
        }

        public override void KeyPressed(Keyboard.Key key, string text)
        {
            if (focus == null)
                return;

            focus.KeyPressed(key, text);
        }

        public override void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            if (pressed)
            {
                if (focus != null)
                {
                    focus.Focussed = false;
                    focus = null;
                }

                foreach (var widget in children.Where(w => w.Visible && ContainsPoint(w, x, y)))
                {
                    widget.Focus();
                    widget.MousePressed(x - widget.Left, y - widget.Top, button, true);
                    return;
                }
            }
            else
            {
                // broadcast release
                foreach (var widget in children)
                {
                    widget.MousePressed(x - widget.Left, y - widget.Top, button, false);
                }
            }
        }

        public override void MouseMoved(int x, int y)
        {
            foreach (var widget in children)
            {
                widget.MouseMoved(x - widget.Left, y - widget.Top);
            }
        }

        private static bool ContainsPoint(Widget widget, int x, int y)
        {
            return x >= widget.Left && y >= widget.Top && x <= (widget.Left + widget.Width - 1) && y <= (widget.Top + widget.Height - 1);
        }
    }
}
