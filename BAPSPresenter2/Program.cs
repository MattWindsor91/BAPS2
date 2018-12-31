using System;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var main = new Main();
            Application.Run(main);

            bool crashed = main.HasCrashed;
            Application.Exit();
            if (crashed)
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath);
            }
        }
    }
}
