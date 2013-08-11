using System;
using Texter;

namespace Programe.Gui
{
    static class Util
    {
        public static void DrawString(this TextRenderer renderer, int x, int y, string str, Character color)
        {
            foreach (var ch in str)
            {
                renderer.Set(x, y, Character.Create(ch, color.Foreground, color.Background));
                x++;
            }
        }
    }
}
