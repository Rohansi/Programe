using System;
using System.Diagnostics;
using System.IO;
using Programe.Machine;

namespace Programe
{
    class Program
    {
        static void Main(string[] args)
        {
            var machine = new VirtualMachine();

            var bytes = File.ReadAllBytes("out.pge");
            var shorts = new short[bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, shorts, 0, bytes.Length);

            for (var i = 0; i < shorts.Length; i++)
            {
                machine.Memory[i] = shorts[i];
            }

            const double seconds = 1 * 60;
            ulong steps = 0;
            var watch = Stopwatch.StartNew();

            while (watch.Elapsed.TotalSeconds < seconds)
            {
                for (var i = 0; i < 200; i++, steps++)
                {
                    machine.Step();
                }
            }

            watch.Stop();
            Console.WriteLine("Completed {0} instructions over {1}", steps, watch.Elapsed);
            Console.ReadLine();

            /*var gameWindow = new GameWindow();
            gameWindow.Run();*/
        }
    }
}
