using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Programe.Network;

namespace Programe.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes("out.pge");
            var shorts = new short[bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, shorts, 0, bytes.Length);

            var ships = new List<Ship>();

            for (var i = 0; i < 1; i++)
            {
                ships.Add(new Ship(i.ToString("G"), shorts));
            }

            Server.Start();

            var watch = Stopwatch.StartNew();
            var accumulator = 0.0;

            while (true)
            {
                accumulator += watch.Elapsed.TotalSeconds;

                // TODO: auto adjust VM speed if too laggy

                if (accumulator >= (Constants.SecondsPerUpdate * 4))
                {
                    // TODO: way too laggy
                    Console.WriteLine("Cant keep up etc");
                }

                Server.Update();
                while (accumulator >= Constants.SecondsPerUpdate)
                {
                    foreach (var ship in ships)
                    {
                        ship.Update();
                    }

                    accumulator -= Constants.SecondsPerUpdate;
                }

                watch.Restart();
                Thread.Sleep(1);
            }
        }
    }
}
