using AttendanceSystemIPCamera.Services.NetworkService;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static AttendanceSystemIPCamera.Framework.Constants;

namespace AttendanceSystemIPCamera.BackgroundServices
{
    public class SupervisorRunnerService : BackgroundService
    {
        private IServiceScopeFactory serviceScopeFactory; //use to resolve injected service
        private readonly ILogger logger;

        public SupervisorRunnerService(IServiceScopeFactory serviceScopeFactory,
            ILogger<SupervisorRunnerService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("SupervisorRunnerService: Network worker starting");
            var networkTask = Task.Factory.StartNew(() =>
            {
                HandleNetworkOperation();
            }, stoppingToken);
            //
            logger.LogInformation("SupervisorRunnerService: Sync timer starting");
            var syncTask = Task.Factory.StartNew(async () =>
            {
                //thời gian sync cuối cùng + 24h - thời gian shutdown = thời gian cần phải chờ thêm trước khi sync
                DateTime latestSync = FileUtils.GetLatestSyncTime();
                DateTime shutdownTime = FileUtils.GetShutDownTime();
                TimeSpan waitingTime = latestSync.AddMilliseconds(Constant.DEFAULT_SYNC_MILISECONDS)
                                            .Subtract(shutdownTime);
                if (NetworkUtils.IsInternetAvailable())
                {
                    waitingTime = TimeSpan.FromMilliseconds(60 * 1000); //sync in 60s
                }
                logger.LogInformation("Waiting time: " + waitingTime);
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (waitingTime.TotalMilliseconds > 0)
                    {
                        await Task.Delay(waitingTime, stoppingToken);
                    }
                    logger.LogInformation("Sync attendance data executing - {0}", DateTime.Now);
                    HandleSyncOperation();

                    waitingTime = TimeSpan.FromMilliseconds(Constant.DEFAULT_SYNC_MILISECONDS);
                }
            }, stoppingToken);

            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("SupervisorRunnerService stopping");
            FileUtils.UpdateShutDownTime(DateTime.Now);
            await Task.CompletedTask;
        }
        private void HandleNetworkOperation()
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var networkService = scope.ServiceProvider.GetService<SupervisorNetworkService>();
                    networkService.Start();
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Exception {e}");
            }
        }
        private void HandleSyncOperation()
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var recordService = scope.ServiceProvider.GetRequiredService<IRecordService>();
                    recordService.SyncAttendanceData();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Sync Error: " + ex);
            }
        }
    }

}
