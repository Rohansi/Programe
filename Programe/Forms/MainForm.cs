using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Programe.NetObjects;
using SFML.Graphics;
using SFML.Window;
using View = SFML.Graphics.View;

namespace Programe.Forms
{
    public partial class MainForm : Form
    {
        private List<string> shipNames;
        private Font font = new Font("Data/SourceSansPro-Regular.otf");

        public RenderWindow Window;
        public Camera Camera;
        public Camera HudCamera;
        public string Following;

        public MainForm()
        {
            InitializeComponent();

            shipNames = new List<string>();
            Client.Start();

            Window = new RenderWindow(Display.Handle, new ContextSettings(0, 0, 32));
            Window.SetFramerateLimit(60);

            Camera = new Camera(Window.DefaultView);
            HudCamera = new Camera(Window.DefaultView);

            Window.Resized += (sender, args) =>
            {
                var view = new View(Camera.Position, new Vector2f(args.Width, args.Height));
                Camera = new Camera(view);

                view = new View(new Vector2f(args.Width / 2f, args.Height / 2f), new Vector2f(args.Width, args.Height));
                HudCamera = new Camera(view);
            };
        }

        public void UpdateShips()
        {
            var newShipNames = Client.Objects.OfType<NetShip>().Select(s => s.Name).OrderBy(s => s).ToList();
            if (shipNames.SequenceEqual(newShipNames))
                return;

            Invoke(() =>
            {
                PlayerList.Items.Clear();
                foreach (var n in newShipNames)
                {
                    PlayerList.Items.Add(n);
                }

                shipNames = newShipNames;

                PlayerList.SelectedIndex = newShipNames.IndexOf(Following);
                if (PlayerList.SelectedIndex == -1)
                    Following = null;
            });
        }

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            Client.Update();

            Camera.Zoom = (float)ZoomBar.Value / 100;

            if (Client.Objects != null)
            {
                var followShip = Client.Objects.OfType<NetShip>().FirstOrDefault(s => s.Name == Following);

                if (followShip != null)
                    Camera.Position = new Vector2f(followShip.X, followShip.Y);
                else
                    Camera.Position = new Vector2f(Client.Width / 2, Client.Height / 2);
            }

            Camera.Apply(Window);
            Window.DispatchEvents();

            Window.Clear(Color.Black);

            var border = new RectangleShape(new Vector2f(Client.Width, Client.Height));
            border.FillColor = new Color(21, 15, 24);
            Window.Draw(border);

            if (Client.StaticObjects != null)
            {
                foreach (var o in Client.StaticObjects)
                {
                    Window.Draw(o);
                }
            }

            if (Client.Objects != null)
            {
                foreach (var o in Client.Objects)
                {
                    Window.Draw(o);
                }
            }

            #region HUD
            HudCamera.Apply(Window);
            if (Client.Objects != null && Keyboard.IsKeyPressed(Keyboard.Key.LControl))
            {
                foreach (var ship in Client.Objects.OfType<NetShip>())
                {
                    const float healthHeight = 24;
                    const float healthWidth = 124;
                    
                    var position = ToHudPosition(new Vector2f(ship.X, ship.Y));

                    var background = new RectangleShape(new Vector2f(healthWidth, healthHeight));
                    background.Origin = background.Size / 2;
                    background.FillColor = new Color(0, 0, 0, 128);
                    background.Position = position;
                    Window.Draw(background);

                    var health = new RectangleShape(new Vector2f((healthWidth - 6) * ship.Health, healthHeight - 6));
                    health.Origin = background.Size / 2;
                    health.FillColor = new Color(0, 120, 0);
                    health.Position = position + new Vector2f(3, 3);
                    Window.Draw(health);

                    var text = new Text(ship.Name, font, (uint)healthHeight - 8);
                    var bounds = text.GetLocalBounds();
                    text.Origin = new Vector2f(bounds.Width / 2, (bounds.Height / 2) + bounds.Top);
                    text.Color = Color.White;
                    text.Position = position;
                    Window.Draw(text);
                }
            }
            #endregion

            Window.Display();
        }

        private void LoginMenuClick(object sender, EventArgs e)
        {
            var login = new LoginForm();
            var result = login.ShowDialog();

            if (result == DialogResult.OK)
                Client.Login(login.Username, login.Password);
        }

        private void RegisterMenuClick(object sender, EventArgs e)
        {
            var register = new RegisterForm();
            var result = register.ShowDialog();

            if (result == DialogResult.OK)
                Client.Register(register.Username, register.Password);
        }

        private void UploadMenuClick(object sender, EventArgs e)
        {
            if (UploadDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var bytes = File.ReadAllBytes(UploadDialog.FileName);
                    if (bytes.Length == 0 || bytes.Length > (short.MaxValue * 2) || bytes.Length % 2 != 0)
                    {
                        Interface.ShowMessage("Upload", "Invalid file size.");
                        return;
                    }

                    var shorts = new short[bytes.Length / 2];
                    Buffer.BlockCopy(bytes, 0, shorts, 0, bytes.Length);

                    var uploadPacket = new Network.Packets.Upload();
                    uploadPacket.Program = shorts;
                    Client.Send(uploadPacket);
                }
                catch (Exception er)
                {
                    Interface.ShowMessage("Upload", string.Format("Failed to upload program: {0}", er));
                }
            }
        }

        private void PlayerListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (PlayerList.SelectedIndex >= 0)
            {
                Following = shipNames[PlayerList.SelectedIndex];
            }
        }

        public void Invoke(Action action)
        {
            Invoke((Delegate)action);
        }

        private Vector2f ToHudPosition(Vector2f position)
        {
            return HudCamera.Position + (position - Camera.Position) / Camera.Zoom;
        }
    }
}
