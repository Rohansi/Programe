using System;

namespace Programe
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
                Client.Server = args[0];

            Console.WriteLine("Using server: {0}", Client.Server);

            Client.Start();

            var gameWindow = new GameWindow();
            gameWindow.Run();
        }
    }
}
