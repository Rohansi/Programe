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
            var bytes = File.ReadAllBytes("out.pge");
            var shorts = new short[bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, shorts, 0, bytes.Length);

            Game.Start(17.5f, 9.375f);
            Server.Start();

            for (var i = 0; i < 4; i++)
            {
                string message;
                Game.SpawnQueue.Enqueue(new Ship(i.ToString("G"), shorts), out message);
            }
            
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
