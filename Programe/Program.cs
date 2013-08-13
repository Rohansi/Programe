using System;

namespace Programe
{
    class Program
    {
        static void Main(string[] args)
        {
            Client.Start();

            var gameWindow = new GameWindow();
            gameWindow.Run();
        }
    }
}
