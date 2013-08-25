using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Programe.Network;

namespace Programe.Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            Game.Start(32, 32, 8);
            Server.Start();
            
            var watch = Stopwatch.StartNew();
            var accumulator = 0.0;

            while (true)
            {
                var elapsed = watch.Elapsed.TotalSeconds;
                accumulator += elapsed;
                watch.Restart();

                if (accumulator >= 1)
                {
                    accumulator = 0;
                }

                Server.Update();
                while (accumulator >= Constants.SecondsPerUpdate)
                {
                    Game.Update();
                    accumulator -= Constants.SecondsPerUpdate;
                }

                Thread.Sleep(4);
            }
        }
    }
}
