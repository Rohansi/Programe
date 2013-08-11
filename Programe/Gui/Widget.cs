﻿using System;
using SFML.Window;
using Texter;

namespace Programe.Gui
{
    abstract class Widget
    {
        public IContainer Parent { get; private set; }

        public bool Visible = true;
        public bool Focussed = false;

        public int Left, Top;
        public uint Width { get; protected set; }
        public uint Height { get; protected set; }

        public virtual void Initialize(IContainer parent)
        {
            Parent = parent;
        }

        public virtual void Draw(TextRenderer renderer)
        {
            
        }

        public virtual void KeyPressed(Keyboard.Key key, string text)
        {
            
        }

        public virtual void MousePressed(int x, int y, Mouse.Button button, bool pressed)
        {
            
        }

        public virtual void MouseMoved(int x, int y)
        {
            
        }

        public void Focus()
        {
            if (Parent != null)
                Parent.Focus(this);
        }

        public void BringToFront()
        {
            if (Parent != null)
                Parent.BringToFront(this);
        }
    }
}
