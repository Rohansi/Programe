using System;
using System.Globalization;
using System.IO;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || (args[0] == "locate" && args.Length < 3))
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  assembler input.asm output.pge");
                Console.WriteLine("  assembler locate input.asm address");
                return;
            }

            var input = args[0];
            var output = args[1];

            try
            {
                if (args[0] == "locate")
                {
                    input = args[1];
                    output = "";
                }

                var a = new Assembler(File.ReadAllText(input));

                if (args[0] == "locate")
                {
                    short address;
                    if (!short.TryParse(args[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address))
                    {
                        Console.WriteLine("Address not valid (must be hexadecimal without a prefix)");
                        return;
                    }

                    try
                    {
                        a.Locate(address);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    File.WriteAllBytes(output, a.Binary);
                    Console.WriteLine("Assembled to {0} bytes", a.Binary.Length);
                }
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
