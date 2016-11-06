using OpenTK;

using System;

namespace SPICA.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (GameWindow Window = new FrmMain())
            {
                Window.Run(60);
            }
        }
    }
}
