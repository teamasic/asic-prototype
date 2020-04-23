using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.NetworkService;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            RegisterNetworkTask(stoppingToken);

            RegisterSyncTask(stoppingToken);

            RegisterUpdateTaskStatus(stoppingToken);

            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("SupervisorRunnerService stopping");
            FileUtils.UpdateShutDownTime(DateTime.Now);
            await Task.CompletedTask;
        }

        private void RegisterNetworkTask(CancellationToken stoppingToken)
        {
            logger.LogInformation("SupervisorRunnerService: Network service starting");
            var networkTask = Task.Factory.StartNew(() =>
            {
                HandleNetworkOperation();
            }, stoppingToken);
        }

        private void RegisterSyncTask(CancellationToken stoppingToken)
        {
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
                    await HandleSyncOperation();

                    waitingTime = TimeSpan.FromMilliseconds(Constant.DEFAULT_SYNC_MILISECONDS);
                }
            }, stoppingToken);
        }

        private void RegisterUpdateTaskStatus(CancellationToken stoppingToken)
        {
            var activateScheduleTask = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(new TimeSpan(0, 0, 30), stoppingToken);
                logger.LogInformation("Activate schedule task executing - {0}", DateTime.Now);
                await HandleSessionStatusChange();

                var waitingTime = new TimeSpan(0, Constant.DEFAULT_CHECK_SCHEDULED_SESSION, 0);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(waitingTime, stoppingToken);
                    logger.LogInformation("Activate schedule task executing - {0}", DateTime.Now);
                    await HandleSessionStatusChange();
                }
            }, stoppingToken);

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
        private async Task HandleSyncOperation()
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var recordService = scope.ServiceProvider.GetRequiredService<IRecordService>();
                    recordService.SyncAttendanceData();

                    var attendeeService = scope.ServiceProvider.GetRequiredService<IAttendeeService>();
                    await attendeeService.AutoDownloadImage();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Sync Error: " + ex);
            }
        }
        private async Task HandleSessionStatusChange()
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                await sessionService.ActivateScheduledSession(); // scheduled -> in-progress
                sessionService.ChangeSessionsToEditable(); // in-progress -> editable
                sessionService.FinishSessions(); // editable -> finished
            }
            catch (Exception ex)
            {
                logger.LogError("Activate schedule error: " + ex);
            }
        }
    }

}
