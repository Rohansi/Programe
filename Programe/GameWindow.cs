using System;
using System.Linq;
using Programe.Gui;
using Programe.Gui.Widgets;
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

            var desktop = new Container(Width, Height - 1);
            desktop.Left = 0;
            desktop.Top = 1;
            gui.Add(desktop);

            var listbox = new ListBox(1, 3, 56, 15);
            listbox.Items.AddRange("hi hi this is a test how are you test".Split(' ').Select(i => new ListBoxItem(i)));

            var textbox = new TextBox(1, 1, 49);

            var button = new Button(51, 1, 6, "Add");
            button.Clicked += () =>
            {
                if (string.IsNullOrWhiteSpace(textbox.Value))
                    return;
                listbox.Items.Add(new ListBoxItem(textbox.Value));
                textbox.Value = "";
            };

            var window = new Gui.Widgets.Window(10, 15, 60, 20, "Sup");
            window.Closing = () => false;
            window.Add(button);
            window.Add(textbox);
            window.Add(listbox);

            var window3 = new Gui.Widgets.Window(1, 1, 20, 10, "hi hi");
            window3.Add(new Label(1, 1, 100, 1, "sup sup"));

            var window2 = new Gui.Widgets.Window(10, 10, 30, 15, "hi");
            window2.Add(window3);
            desktop.Add(window2);

            var menu = new MenuBar();
            gui.Add(menu);

            var hello = new MenuItem("hello");
            var world = new MenuItem("world");
            var test = new MenuItem("test");
            var nest = new MenuItem("nest");
            var help = new MenuItem("help");
            var dying = new MenuItem("i'm dying");

            nest.Items.Add(help);
            nest.Items.Add(dying);
            world.Items.Add(nest);
            hello.Items.Add(test);
            menu.Items.Add(hello);
            menu.Items.Add(world);

            desktop.Add(window);

            while (Window.IsOpen())
            {
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
