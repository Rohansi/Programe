using System;
using System.Windows.Forms;

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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Interface.MainForm);
        }
    }
}
