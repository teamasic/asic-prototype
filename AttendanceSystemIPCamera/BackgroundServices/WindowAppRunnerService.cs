using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupervisorApp;
//using SupervisorApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.BackgroundServices
{
    public class WindowAppRunnerService
    {
        public WindowAppRunnerService()
        {
        }
        public void Run()
        {
            Thread myThread = new Thread(() =>
            {
                var application = new App();
                application.Run(new MainWindow());  // add Window if you want a window.
            });
            myThread.SetApartmentState(ApartmentState.STA);
            myThread.Start();
        }
     
    }
}
