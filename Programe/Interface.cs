using System;
using System.Windows.Forms;
using Programe.Forms;

namespace Programe
{
    public static class Interface
    {
        public static readonly MainForm MainForm;

        static Interface()
        {
            MainForm = new MainForm();
        }

        public static void Connected()
        {
            
        }

        public static void Disconnected()
        {
            
        }

        public static void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title);
        }
    }
}
