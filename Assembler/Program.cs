using System;
using System.IO;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Console.WriteLine("Big endian CPUs are currently not supported.");
                return;
            }

            if (args.Length < 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  assembler input.pga output.pge [output.pgd]");
                return;
            }

            var index = 0;

            try
            {
                var input = args[index++];
                var output = args[index++];
                var debug = args.Length >= 3 ? args[index] : null;

                var assembler = new Assembler(File.ReadAllText(input));
                assembler.Assemble();
                File.WriteAllBytes(output, assembler.Binary);

                if (debug != null)
                {
                    assembler.Debug.Save(debug);
                }

                Console.WriteLine("Assembled to {0} words", assembler.Binary.Length / 2);
            }
            catch (AssemblerException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
