using SupervisorApp;
using System;

namespace ConsoleApp1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var application = new App();
            application.Run(new MainWindow());  // add Window if you want a window.
        }
    }
}
