using System;
using Programe.Gui;
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
                
            GameView = Window.DefaultView;
            overlayView = Window.DefaultView;

            TextDisplay.Initialize();
            Overlay = new TextDisplay(Width, Height);

            gui = new GuiSystem(Window, Width, Height);
            Interface.Start(gui);

            while (Window.IsOpen())
            {
                Client.Update();

                Window.SetView(overlayView);
                Window.DispatchEvents();

                Window.Clear(new Color(100, 149, 237));
                Window.SetView(GameView);

                // draw stuff

                Window.SetView(overlayView);
                Overlay.Clear(Character.Blank);

                gui.Draw(Overlay);

                Overlay.Draw(Window, new Vector2f(0, 0));
                Window.Display();
            }
        }
    }
}
