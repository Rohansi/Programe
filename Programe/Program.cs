using System;

namespace Programe
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Client.Start();

            var gameWindow = new GameWindow();
            gameWindow.Run();
        }
    }
}
