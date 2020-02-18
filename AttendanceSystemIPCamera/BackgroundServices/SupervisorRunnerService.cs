using AttendanceSystemIPCamera.Services.NetworkService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.BackgroundServices
{
    public class SupervisorRunnerService : BackgroundService
    {
        private IServiceScopeFactory serviceScopeFactory; //use to resolve injected service

        public SupervisorRunnerService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Thread myThread = new Thread(() =>
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var networkService = scope.ServiceProvider.GetService<SupervisorNetworkService>();
                    networkService.Start();
                }
            });
            myThread.SetApartmentState(ApartmentState.STA);
            myThread.Start();
            await Task.CompletedTask;
        }

    }
}
