using System;
using Texter;

namespace Programe.Gui
{
    // TODO: merge this into texter

    /// <summary>
    /// Draws boxes to a TextRenderer
    /// 
    /// Characters must contain 9 values describing the following:
    /// 012
    /// 345
    /// 678
    /// 
    /// 0268 are corners, 1357 are edges, 4 is the center
    /// String should have no line breaks.
    /// </summary>
    class TexterBox
    {
        public static readonly TexterBox Single = new TexterBox("\xDA\xC4\xBF\xB3 \xB3\xC0\xC4\xD9");
        public static readonly TexterBox Double = new TexterBox("\xC9\xCD\xBB\xBA \xBA\xC8\xCD\xBC");

        private string chars;

        public TexterBox(string characters)
        {
            if (characters.Length != 9)
                throw new ArgumentException("Characters must contain 9 values");

            chars = characters;
        }

        public void Draw(TextRenderer renderer, int x, int y, uint w, uint h, Character color)
        {
            for (var yy = 0; yy < h; yy++)
            {
                for (var xx = 0; xx < w; xx++)
                {
                    char c;
                    if (yy == 0)
                    {
                        if (xx == 0)
                            c = chars[0];
                        else if (xx == w - 1)
                            c = chars[2];
                        else
                            c = chars[1];
                    }
                    else if (yy == h - 1)
                    {
                        if (xx == 0)
                            c = chars[6];
                        else if (xx == w - 1)
                            c = chars[8];
                        else
                            c = chars[7];
                    }
                    else
                    {
                        if (xx == 0)
                            c = chars[3];
                        else if (xx == w - 1)
                            c = chars[5];
                        else
                            c = chars[4];
                    }

                    renderer.Set(x + xx, y + yy, Character.Create(c, color.Foreground, color.Background));
                }
            }
        }
    }
}
