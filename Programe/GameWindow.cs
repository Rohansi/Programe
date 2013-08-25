using System;
using Programe.Gui;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;
using Texter;

namespace Programe
{
    class GameWindow
    {
        private const uint Width = 140;
        private const uint Height = 50;
        private const uint CharWidth = 8;
        private const uint CharHeight = 12;

        public RenderWindow Window;
        public TextDisplay Overlay;
        public View GameView;
        private View overlayView;
        private GuiSystem gui;

        public void Run()
        {
            Window = new RenderWindow(new VideoMode(Width * CharWidth, Height * CharHeight), "", Styles.Close);
            Window.SetFramerateLimit(60);
            Window.Closed += (sender, args) => Window.Close();
                
            GameView = new View(Window.DefaultView);
            GameView.Zoom(2f);

            overlayView = new View(Window.DefaultView);

            TextDisplay.Initialize();
            Overlay = new TextDisplay(Width, Height);

            gui = new GuiSystem(Window, Width, Height);
            Interface.Start(gui);

            while (Window.IsOpen())
            {
                Client.Update();

                Window.SetView(overlayView);
                Window.DispatchEvents();

                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                    GameView.Center += new Vector2f(-15, 0);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                    GameView.Center += new Vector2f(15, 0);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    GameView.Center += new Vector2f(0, -15);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    GameView.Center += new Vector2f(0, 15);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
                    GameView.Center = new Vector2f(16 * Constants.PixelsPerMeter, 16 * Constants.PixelsPerMeter);

                Window.Clear(Color.White);
                Window.SetView(GameView);

                // TODO: get map dimensions from server
                var border = new RectangleShape(new Vector2f(32 * Constants.PixelsPerMeter, 32 * Constants.PixelsPerMeter));
                border.FillColor = Color.Black;
                Window.Draw(border);

                if (Client.Objects != null)
                {
                    foreach (var o in Client.Objects)
                    {
                        Window.Draw(o);
                    }
                }

                Window.SetView(overlayView);
                Overlay.Clear(Character.Blank);

                gui.Draw(Overlay);

                Overlay.Draw(Window, new Vector2f(0, 0));
                Window.Display();
            }
        }
    }
}
