using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Programe.NetObjects;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;
using View = SFML.Graphics.View;

namespace Programe.Forms
{
    public partial class MainForm : Form
    {
        private List<string> shipNames;

        public RenderWindow Window;
        public Camera Camera;
        public string Following;

        public MainForm()
        {
            InitializeComponent();

            shipNames = new List<string>();
            Client.Start();

            Window = new RenderWindow(Display.Handle);
            Window.SetFramerateLimit(60);

            Camera = new Camera(Window.DefaultView);

            Window.Resized += (sender, args) =>
            {
                var view = new View(Camera.Position, new Vector2f(args.Width, args.Height));
                Camera = new Camera(view);
            };

            Window.MouseWheelMoved += (sender, args) => Console.WriteLine("asf");
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
                    Camera.Position = new Vector2f(16 * Constants.PixelsPerMeter, 16 * Constants.PixelsPerMeter);
            }

            Camera.Apply(Window);
            Window.DispatchEvents();

            Window.Clear(Color.White);

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
    }
}
